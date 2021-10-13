using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.PolygonMesh.Processors;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public abstract class LibraryProvider : IDisposable
	{
		public class ProgressPlug
		{
			public ReportProgressRatio ProgressOutput;

			public void ProgressInput(double progress0To1, string processingState, out bool continueProcessing)
			{
				continueProcessing = true;
				if (ProgressOutput != null)
				{
					ProgressOutput.Invoke(progress0To1, processingState, ref continueProcessing);
				}
			}
		}

		protected Dictionary<int, ProgressPlug> itemReportProgressHandlers = new Dictionary<int, ProgressPlug>();

		private LibraryProvider parentLibraryProvider;

		protected Action<LibraryProvider> SetCurrentLibraryProvider
		{
			get;
			private set;
		}

		public LibraryProvider ParentLibraryProvider => parentLibraryProvider;

		public static ImageBuffer NormalFolderImage
		{
			get;
		} = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadImage(Path.Combine("Icons", "FileDialog", "folder.png")));


		public static ImageBuffer UpFolderImage
		{
			get;
		} = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadImage(Path.Combine("Icons", "FileDialog", "up_folder.png")));


		public bool HasParent => ParentLibraryProvider != null;

		public abstract int CollectionCount
		{
			get;
		}

		public abstract int ItemCount
		{
			get;
		}

		public abstract bool CanShare
		{
			get;
		}

		public abstract string ProviderKey
		{
			get;
		}

		public virtual string KeywordFilter
		{
			get;
			set;
		}

		public virtual string StatusMessage
		{
			get;
		} = "";


		public string Name
		{
			get;
			protected set;
		}

		public event EventHandler DataReloaded;

		public LibraryProvider(LibraryProvider parentLibraryProvider, Action<LibraryProvider> setCurrentLibraryProvider)
		{
			SetCurrentLibraryProvider = setCurrentLibraryProvider;
			this.parentLibraryProvider = parentLibraryProvider;
		}

		public void AddFilesToLibrary(IList<string> files, ReportProgressRatio reportProgress = null)
		{
			foreach (string file in files)
			{
				string text = Path.GetExtension(file)!.ToUpper();
				if ((!(text != "") || !MeshFileIo.ValidFileExtensions().Contains(text)) && !(text == ".GCODE") && !(text == ".ZIP"))
				{
					continue;
				}
				if (text == ".ZIP")
				{
					List<PrintItem> list = new ProjectFileHandler(null).ImportFromProjectArchive(file);
					if (list == null)
					{
						continue;
					}
					foreach (PrintItem item in list)
					{
						AddItem(new PrintItemWrapper(item, GetProviderLocator()));
					}
				}
				else
				{
					AddItem(new PrintItemWrapper(new PrintItem(Path.GetFileNameWithoutExtension(file), file), GetProviderLocator()));
				}
			}
		}

		public List<ProviderLocatorNode> GetProviderLocator()
		{
			List<ProviderLocatorNode> list = new List<ProviderLocatorNode>();
			if (ParentLibraryProvider != null)
			{
				list.AddRange(ParentLibraryProvider.GetProviderLocator());
			}
			list.Add(new ProviderLocatorNode(ProviderKey, Name));
			return list;
		}

		public abstract void AddCollectionToLibrary(string collectionName);

		public abstract void AddItem(PrintItemWrapper itemToAdd);

		public abstract PrintItemCollection GetCollectionItem(int collectionIndex);

		public abstract Task<PrintItemWrapper> GetPrintItemWrapperAsync(int itemIndex);

		public abstract LibraryProvider GetProviderForCollection(PrintItemCollection collection);

		public abstract void RemoveCollection(int collectionIndexToRemove);

		public abstract void RemoveItem(int itemIndexToRemove);

		public virtual void RemoveItems(int[] indexes)
		{
			foreach (int item in (IEnumerable<int>)Enumerable.OrderByDescending<int, int>((IEnumerable<int>)indexes, (Func<int, int>)((int i) => i)))
			{
				RemoveItem(item);
			}
		}

		public virtual void MoveItems(int[] indexes)
		{
		}

		public abstract void RenameCollection(int collectionIndexToRename, string newName);

		public abstract void RenameItem(int itemIndexToRename, string newName);

		public abstract void ShareItem(int itemIndexToShare);

		public void OnDataReloaded(EventArgs eventArgs)
		{
			this.DataReloaded?.Invoke(this, eventArgs);
		}

		public virtual void Dispose()
		{
		}

		public virtual int GetCollectionChildCollectionCount(int collectionIndex)
		{
			return GetProviderForCollection(GetCollectionItem(collectionIndex)).CollectionCount;
		}

		public virtual ImageBuffer GetCollectionFolderImage(int collectionIndex)
		{
			return NormalFolderImage;
		}

		public virtual int GetCollectionItemCount(int collectionIndex)
		{
			return GetProviderForCollection(GetCollectionItem(collectionIndex)).ItemCount;
		}

		public virtual GuiWidget GetItemThumbnail(int printItemIndex)
		{
			return (GuiWidget)(object)new PartThumbnailWidget(GetPrintItemWrapperAsync(printItemIndex).Result, "part_icon_transparent_40x40.png", "building_thumbnail_40x40.png", PartThumbnailWidget.ImageSizes.Size50x50);
		}

		public virtual string GetPrintItemName(int itemIndex)
		{
			return "";
		}

		public LibraryProvider GetRootProvider()
		{
			LibraryProvider libraryProvider = this;
			while (libraryProvider != null && libraryProvider.ParentLibraryProvider != null)
			{
				libraryProvider = libraryProvider.ParentLibraryProvider;
			}
			return libraryProvider;
		}

		public virtual bool IsItemProtected(int itemIndex)
		{
			return false;
		}

		public virtual bool IsItemReadOnly(int itemIndex)
		{
			return false;
		}

		public void RegisterForProgress(int itemIndex, ReportProgressRatio reportProgress)
		{
			if (!itemReportProgressHandlers.ContainsKey(itemIndex))
			{
				itemReportProgressHandlers.Add(itemIndex, new ProgressPlug
				{
					ProgressOutput = reportProgress
				});
			}
			else
			{
				itemReportProgressHandlers[itemIndex].ProgressOutput = reportProgress;
			}
		}

		protected ProgressPlug GetItemProgressPlug(int itemIndex)
		{
			if (!itemReportProgressHandlers.ContainsKey(itemIndex))
			{
				itemReportProgressHandlers.Add(itemIndex, new ProgressPlug());
			}
			return itemReportProgressHandlers[itemIndex];
		}

		public virtual bool IsProtected()
		{
			return false;
		}
	}
}
