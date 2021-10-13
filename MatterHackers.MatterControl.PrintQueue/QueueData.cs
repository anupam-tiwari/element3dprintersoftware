using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.PolygonMesh.Processors;

namespace MatterHackers.MatterControl.PrintQueue
{
	public class QueueData
	{
		public enum ValidateSizeOn32BitSystems
		{
			Required,
			Skip
		}

		public static readonly string SdCardFileName = "SD_CARD";

		private List<PrintItemWrapper> printItems = new List<PrintItemWrapper>();

		private List<int> selectedIndices = new List<int>();

		public RootedObjectEventHandler ItemAdded = new RootedObjectEventHandler();

		public RootedObjectEventHandler ItemRemoved = new RootedObjectEventHandler();

		public RootedObjectEventHandler OrderChanged = new RootedObjectEventHandler();

		public RootedObjectEventHandler SelectedIndexChanged = new RootedObjectEventHandler();

		private static QueueData instance;

		private bool gotBeginFileList;

		private EventHandler unregisterEvents;

		private PrintItemWrapper partUnderConsideration;

		public List<PrintItemWrapper> PrintItems => printItems;

		public IEnumerable<int> SelectedIndexes => selectedIndices.ToArray();

		public int SelectedIndex
		{
			get
			{
				if (ItemCount > 0)
				{
					if (selectedIndices.Count == 0)
					{
						selectedIndices.Add(0);
					}
					return selectedIndices[0];
				}
				return -1;
			}
			set
			{
				if (!selectedIndices.Contains(value) || selectedIndices.Count > 1)
				{
					selectedIndices.Clear();
					selectedIndices.Add(value);
					OnSelectedIndexChanged(null);
				}
			}
		}

		public static QueueData Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new QueueData();
					instance.LoadDefaultQueue();
				}
				return instance;
			}
		}

		public PrintItemWrapper SelectedPrintItem
		{
			get
			{
				if (SelectedIndex >= 0)
				{
					return GetPrintItemWrapper(SelectedIndex);
				}
				return null;
			}
			set
			{
				if (SelectedPrintItem == value)
				{
					return;
				}
				for (int i = 0; i < PrintItems.Count; i++)
				{
					if (PrintItems[i] == value)
					{
						SelectedIndex = i;
						return;
					}
				}
				throw new Exception("Item not in queue.");
			}
		}

		public int ItemCount => PrintItems.Count;

		public int SelectedCount
		{
			get
			{
				if (ItemCount > 0)
				{
					if (selectedIndices.Count > 0)
					{
						return selectedIndices.Count;
					}
					return 1;
				}
				return 0;
			}
		}

		public void MoveToNext()
		{
			if (SelectedIndex >= 0 && SelectedIndex < ItemCount)
			{
				if (SelectedIndex == ItemCount - 1)
				{
					SelectedIndex = 0;
				}
				else
				{
					SelectedIndex++;
				}
			}
		}

		public void SwapItemsOnIdle(int indexA, int indexB)
		{
			UiThread.RunOnIdle((Action<object>)SwapItems, (object)new SwapIndexArgs(indexA, indexB), 0.0);
		}

		private void SwapItems(object state)
		{
			int indexA = ((SwapIndexArgs)state).indexA;
			int indexB = ((SwapIndexArgs)state).indexB;
			if (indexA >= 0 && indexA < ItemCount && indexB >= 0 && indexB < ItemCount && indexA != indexB)
			{
				PrintItemWrapper value = PrintItems[indexA];
				PrintItems[indexA] = PrintItems[indexB];
				PrintItems[indexB] = value;
				OnOrderChanged(null);
				OnSelectedIndexChanged(null);
				SaveDefaultQueue();
			}
		}

		public void OnOrderChanged(EventArgs e)
		{
			OrderChanged.CallEvents((object)this, e);
		}

		public void RemoveIndexOnIdle(int index)
		{
			UiThread.RunOnIdle((Action<object>)RemoveIndex, (object)new IndexArgs(index), 0.0);
		}

		private void RemoveIndex(object state)
		{
			IndexArgs indexArgs = state as IndexArgs;
			if (indexArgs != null)
			{
				RemoveAt(indexArgs.index);
			}
		}

		public void RemoveAt(int index)
		{
			if (index >= 0 && index < ItemCount && ((!PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && !PrinterConnectionAndCommunication.Instance.PrinterIsPaused) || PrintItems[index] != PrinterConnectionAndCommunication.Instance.ActivePrintItem))
			{
				PrintItems.RemoveAt(index);
				OnItemRemoved(new IndexArgs(index));
				OnSelectedIndexChanged(null);
				SaveDefaultQueue();
			}
		}

		public void OnItemRemoved(EventArgs e)
		{
			ItemRemoved.CallEvents((object)this, e);
		}

		public void OnSelectedIndexChanged(EventArgs e)
		{
			SelectedIndexChanged.CallEvents((object)this, e);
		}

		public PrintItemWrapper GetPrintItemWrapper(int index)
		{
			if (index >= 0 && index < PrintItems.Count)
			{
				return PrintItems[index];
			}
			return null;
		}

		public int GetIndex(PrintItemWrapper printItem)
		{
			return PrintItems.IndexOf(printItem);
		}

		public string[] GetItemNames()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < PrintItems.Count; i++)
			{
				list.Add(PrintItems[i].Name);
			}
			return list.ToArray();
		}

		public string GetItemName(int itemIndex)
		{
			return PrintItems[itemIndex].Name;
		}

		public void LoadFilesFromSD()
		{
			if (PrinterConnectionAndCommunication.Instance.PrinterIsConnected && !PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && !PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
			{
				gotBeginFileList = false;
				PrinterConnectionAndCommunication.Instance.ReadLine.RegisterEvent((EventHandler)GetSdCardList, ref unregisterEvents);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("M21");
				stringBuilder.AppendLine("M20");
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(stringBuilder.ToString());
			}
		}

		private void GetSdCardList(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (val == null || val.get_Data().StartsWith("echo:"))
			{
				return;
			}
			string data = val.get_Data();
			if (!(data == "Begin file list"))
			{
				if (!(data == "End file list"))
				{
					if (!gotBeginFileList)
					{
						return;
					}
					bool flag = false;
					bool flag2 = false;
					foreach (PrintItem item in CreateReadOnlyPartList(includeProtectedItems: false))
					{
						if (item.FileLocation == SdCardFileName && item.Name == val.get_Data())
						{
							flag = true;
							break;
						}
					}
					string text = val.get_Data().ToUpper();
					if (text.Contains(".GCO") || text.Contains(".GCODE"))
					{
						flag2 = true;
					}
					if (!flag && flag2)
					{
						AddItem(new PrintItemWrapper(new PrintItem(val.get_Data(), SdCardFileName)));
					}
				}
				else
				{
					PrinterConnectionAndCommunication.Instance.ReadLine.UnregisterEvent((EventHandler)GetSdCardList, ref unregisterEvents);
				}
			}
			else
			{
				gotBeginFileList = true;
			}
		}

		public List<PrintItem> CreateReadOnlyPartList(bool includeProtectedItems)
		{
			List<PrintItem> list = new List<PrintItem>();
			for (int i = 0; i < ItemCount; i++)
			{
				PrintItem printItem = GetPrintItemWrapper(i).PrintItem;
				if (includeProtectedItems || !printItem.Protected)
				{
					list.Add(printItem);
				}
			}
			return list;
		}

		private static bool Is32Bit()
		{
			if (IntPtr.Size == 4)
			{
				return true;
			}
			return false;
		}

		public void AddItem(PrintItemWrapper item, int indexToInsert = -1, ValidateSizeOn32BitSystems checkSize = ValidateSizeOn32BitSystems.Required)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Invalid comparison between Unknown and I4
			if (Is32Bit())
			{
				bool flag = false;
				long num = 0L;
				if (File.Exists(item.FileLocation) && checkSize == ValidateSizeOn32BitSystems.Required)
				{
					num = MeshFileIo.GetEstimatedMemoryUse(item.FileLocation);
					if ((int)OsInformation.get_OperatingSystem() == 5)
					{
						if (num > 100000000)
						{
							flag = true;
						}
					}
					else if (num > 500000000)
					{
						flag = true;
					}
				}
				if (flag)
				{
					partUnderConsideration = item;
					UiThread.RunOnIdle((Action)delegate
					{
						string message = StringHelper.FormatWith("Are you sure you want to add this part ({0}) to the Queue?\nThe 3D part you are trying to load may be too complicated and cause performance or stability problems.\n\nConsider reducing the geometry before proceeding.".Localize(), new object[1]
						{
							item.Name
						});
						StyledMessageBox.ShowMessageBox(UserSaidToAllowAddToQueue, message, "File May Cause Problems".Localize(), StyledMessageBox.MessageType.YES_NO, "Add To Queue", "Do Not Add");
					});
				}
				else
				{
					DoAddItem(item, indexToInsert);
				}
			}
			else
			{
				DoAddItem(item, indexToInsert);
			}
		}

		private void UserSaidToAllowAddToQueue(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				DoAddItem(partUnderConsideration, -1);
			}
		}

		private void DoAddItem(PrintItemWrapper item, int indexToInsert)
		{
			if (indexToInsert == -1)
			{
				indexToInsert = PrintItems.Count;
			}
			PrintItems.Insert(indexToInsert, item);
			OnItemAdded(new IndexArgs(indexToInsert));
			OnSelectedIndexChanged(null);
			SaveDefaultQueue();
		}

		public void LoadDefaultQueue()
		{
			RemoveAll();
			List<PrintItem> list = new ManifestFileHandler(null).ImportFromJson();
			if (list != null)
			{
				foreach (PrintItem item in list)
				{
					AddItem(new PrintItemWrapper(item), -1, ValidateSizeOn32BitSystems.Skip);
				}
			}
			RemoveAllSdCardFiles();
		}

		public void RemoveAllSdCardFiles()
		{
			for (int num = ItemCount - 1; num >= 0; num--)
			{
				if (PrintItems[num].PrintItem.FileLocation == SdCardFileName)
				{
					RemoveAt(num);
				}
			}
		}

		public void OnItemAdded(EventArgs e)
		{
			ItemAdded.CallEvents((object)this, e);
		}

		public void SaveDefaultQueue()
		{
			new ManifestFileHandler(CreateReadOnlyPartList(includeProtectedItems: true)).ExportToJson();
		}

		public void RemoveAll()
		{
			for (int num = PrintItems.Count - 1; num >= 0; num--)
			{
				RemoveAt(num);
			}
		}

		public void RemoveSelected()
		{
			if (ItemCount <= 0 || SelectedCount <= 0)
			{
				return;
			}
			foreach (int item in (IEnumerable<int>)Enumerable.OrderByDescending<int, int>(SelectedIndexes, (Func<int, int>)((int rowItem) => rowItem)))
			{
				RemoveAt(item);
			}
			selectedIndices.Clear();
			OnSelectedIndexChanged(null);
		}

		public void ToggleSelect(int index)
		{
			if (selectedIndices.Contains(index))
			{
				Unselect(index);
			}
			else
			{
				Select(index);
			}
		}

		public void MakeSingleSelection()
		{
			if (ItemCount > 0 && SelectedCount > 1)
			{
				SelectedIndex = selectedIndices[selectedIndices.Count - 1];
			}
			if (ItemCount > 0 && SelectedIndex < 0)
			{
				SelectedIndex = 0;
			}
			else if (ItemCount > 0 && SelectedIndex >= ItemCount)
			{
				SelectedIndex = ItemCount - 1;
			}
		}

		public void Select(int index)
		{
			if (!selectedIndices.Contains(index) && index >= 0 && index < ItemCount)
			{
				selectedIndices.Add(index);
				OnSelectedIndexChanged(null);
			}
		}

		public void Unselect(int index)
		{
			if (selectedIndices.Contains(index) && index >= 0 && index < ItemCount)
			{
				selectedIndices.Remove(index);
				OnSelectedIndexChanged(null);
			}
		}
	}
}
