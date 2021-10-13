using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SettingsManagement;
using MatterHackers.PolygonMesh;
using MatterHackers.PolygonMesh.Processors;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public class LibraryProviderSQLite : LibraryProvider
	{
		public static bool PreloadingCalibrationFiles = false;

		protected PrintItemCollection baseLibraryCollection;

		protected List<PrintItemCollection> childCollections = new List<PrintItemCollection>();

		private bool ignoreNextKeywordFilter;

		private string keywordFilter = string.Empty;

		private List<PrintItem> printItems = new List<PrintItem>();

		public static RootedObjectEventHandler ItemAdded = new RootedObjectEventHandler();

		private object initializingLock = new object();

		private EventHandler unregisterEvents;

		private Stopwatch timeSinceLastChange = new Stopwatch();

		public static string StaticProviderKey => "LibraryProviderSqliteKey";

		public override bool CanShare => false;

		public override int CollectionCount => childCollections.Count;

		public override int ItemCount => printItems.Count;

		public override string KeywordFilter
		{
			get
			{
				return keywordFilter;
			}
			set
			{
				if (ignoreNextKeywordFilter)
				{
					ignoreNextKeywordFilter = false;
					return;
				}
				PrintItemCollection rootLibraryCollection = GetRootLibraryCollection();
				if (value != "" && baseLibraryCollection.Id != rootLibraryCollection.Id)
				{
					LibraryProviderSQLite currentProvider = base.ParentLibraryProvider as LibraryProviderSQLite;
					while (currentProvider.ParentLibraryProvider != null && currentProvider.baseLibraryCollection.Id != rootLibraryCollection.Id)
					{
						currentProvider = currentProvider.ParentLibraryProvider as LibraryProviderSQLite;
					}
					if (currentProvider != null)
					{
						currentProvider.KeywordFilter = value;
						currentProvider.ignoreNextKeywordFilter = true;
						UiThread.RunOnIdle((Action)delegate
						{
							base.SetCurrentLibraryProvider(currentProvider);
						});
					}
				}
				else if (keywordFilter != value)
				{
					keywordFilter = value;
					LoadLibraryItems();
				}
			}
		}

		public override string ProviderKey => StaticProviderKey;

		public LibraryProviderSQLite(PrintItemCollection callerSuppliedCollection, Action<LibraryProvider> setCurrentLibraryProvider, LibraryProvider parentLibraryProvider, string visibleName)
			: base(parentLibraryProvider, setCurrentLibraryProvider)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			base.Name = visibleName;
			lock (initializingLock)
			{
				baseLibraryCollection = callerSuppliedCollection ?? GetRootLibraryCollection();
			}
			LoadLibraryItems();
			ItemAdded.RegisterEvent((EventHandler)DatabaseFileChange, ref unregisterEvents);
		}

		public override void Dispose()
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			ItemAdded.UnregisterEvent((EventHandler)DatabaseFileChange, ref unregisterEvents);
		}

		private async void DatabaseFileChange(object sender, EventArgs e)
		{
			if (timeSinceLastChange.get_IsRunning())
			{
				timeSinceLastChange.Restart();
				return;
			}
			timeSinceLastChange.Restart();
			await Task.Run(delegate
			{
				while (timeSinceLastChange.get_Elapsed().TotalSeconds < 0.5)
				{
					Thread.Sleep(10);
				}
			});
			UiThread.RunOnIdle((Action)delegate
			{
				if (!Datastore.Instance.WasExited())
				{
					LoadLibraryItems();
				}
			});
			timeSinceLastChange.Stop();
		}

		public static IEnumerable<PrintItem> GetAllPrintItemsRecursive()
		{
			return Datastore.Instance.dbSQLite.Query<PrintItem>("SELECT * FROM PrintItem WHERE PrintItemCollectionID != 0;", Array.Empty<object>());
		}

		public override void AddCollectionToLibrary(string collectionName)
		{
			PrintItemCollection printItemCollection = new PrintItemCollection(collectionName, "");
			printItemCollection.ParentCollectionID = baseLibraryCollection.Id;
			printItemCollection.Commit();
			LoadLibraryItems();
		}

		public override void AddItem(PrintItemWrapper itemToAdd)
		{
			AddItem(itemToAdd.Name, itemToAdd.FileLocation);
		}

		private async void AddItem(string fileName, string fileLocation)
		{
			await Task.Run(delegate
			{
				if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(fileLocation))
				{
					using FileStream stream = File.OpenRead(fileLocation);
					AddItem(stream, Path.GetExtension(fileLocation)!.ToUpper(), fileName);
				}
				UiThread.RunOnIdle((Action)delegate
				{
					LoadLibraryItems();
					ItemAdded.CallEvents((object)this, (EventArgs)null);
				});
			});
		}

		public void EnsureSamplePartsExist(IEnumerable<string> filenamesToValidate)
		{
			PreloadingCalibrationFiles = true;
			IEnumerable<string> existingLibaryItems = Enumerable.Select<PrintItem, string>(GetLibraryItems(), (Func<PrintItem, string>)((PrintItem i) => i.Name));
			foreach (string item in Enumerable.Where<string>(filenamesToValidate, (Func<string, bool>)((string fileName) => !Enumerable.Contains<string>(existingLibaryItems, Path.GetFileNameWithoutExtension(fileName), (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase))))
			{
				using Stream stream = StaticData.get_Instance().OpenSteam(Path.Combine("OEMSettings", "SampleParts", item));
				AddItem(stream, Path.GetExtension(item), Path.GetFileNameWithoutExtension(item), forceAMF: false);
			}
			PrintItem printItem = Enumerable.FirstOrDefault<PrintItem>(GetLibraryItems());
			if (printItem != null)
			{
				PreLoadItemToQueue(printItem);
			}
			PreloadingCalibrationFiles = false;
		}

		private void PreLoadItemToQueue(PrintItem printItem)
		{
			string fileLocation = printItem.FileLocation;
			if (string.IsNullOrEmpty(fileLocation) || !File.Exists(fileLocation))
			{
				return;
			}
			PrintItemWrapper item = new PrintItemWrapper(printItem);
			string path = printItem.Name + ".png";
			string text = Path.Combine(StaticData.get_Instance().MapPath(Path.Combine("OEMSettings", "SampleParts")), path);
			if (File.Exists(text))
			{
				string imageFileName = PartThumbnailWidget.GetImageFileName(item);
				try
				{
					Directory.CreateDirectory(Path.GetDirectoryName(imageFileName));
					File.Copy(text, imageFileName, true);
				}
				catch
				{
				}
			}
			QueueData.Instance.AddItem(item);
		}

		public override PrintItemCollection GetCollectionItem(int collectionIndex)
		{
			return childCollections[collectionIndex];
		}

		public IEnumerable<PrintItem> GetLibraryItems(string keyphrase = null)
		{
			string query = ((!string.IsNullOrEmpty(keyphrase)) ? $"SELECT * FROM PrintItem WHERE PrintItemCollectionID = {baseLibraryCollection.Id} AND Name LIKE '%{keyphrase}%' ORDER BY Name ASC;" : $"SELECT * FROM PrintItem WHERE PrintItemCollectionID = {baseLibraryCollection.Id} ORDER BY Name ASC;");
			return Datastore.Instance.dbSQLite.Query<PrintItem>(query, Array.Empty<object>());
		}

		public override string GetPrintItemName(int itemIndex)
		{
			return printItems[itemIndex].Name;
		}

		public override Task<PrintItemWrapper> GetPrintItemWrapperAsync(int index)
		{
			if (index >= 0 && index < printItems.Count)
			{
				return Task.FromResult(new PrintItemWrapper(printItems[index], GetProviderLocator()));
			}
			return null;
		}

		public override LibraryProvider GetProviderForCollection(PrintItemCollection collection)
		{
			return new LibraryProviderSQLite(collection, base.SetCurrentLibraryProvider, this, collection.Name);
		}

		public override void RemoveCollection(int collectionIndexToRemove)
		{
			childCollections[collectionIndexToRemove].Delete();
			LoadLibraryItems();
		}

		public override void RemoveItem(int itemToRemoveIndex)
		{
			if (itemToRemoveIndex >= 0)
			{
				printItems[itemToRemoveIndex].Delete();
				printItems.RemoveAt(itemToRemoveIndex);
				OnDataReloaded(null);
			}
		}

		public override void RenameCollection(int collectionIndexToRename, string newName)
		{
			childCollections[collectionIndexToRename].Name = newName;
			childCollections[collectionIndexToRename].Commit();
			LoadLibraryItems();
		}

		public override void RenameItem(int itemIndexToRename, string newName)
		{
			printItems[itemIndexToRename].Name = newName;
			printItems[itemIndexToRename].Commit();
			LoadLibraryItems();
		}

		public override void ShareItem(int itemIndexToShare)
		{
		}

		private void AddItem(Stream stream, string extension, string displayName, bool forceAMF = true)
		{
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			PrintItem printItem = new PrintItem();
			printItem.Name = displayName;
			printItem.PrintItemCollectionID = baseLibraryCollection.Id;
			printItem.Commit();
			if (forceAMF && extension != "" && MeshFileIo.ValidFileExtensions().Contains(extension.ToUpper()))
			{
				try
				{
					List<MeshGroup> list = MeshFileIo.Load(stream, extension, (ReportProgressRatio)null);
					if (!printItem.FileLocation.Contains(ApplicationDataStorage.Instance.ApplicationLibraryDataPath))
					{
						string[] array = new string[2]
						{
							"Created By",
							"Element"
						};
						printItem.FileLocation = CreateLibraryPath(".amf");
						MeshOutputSettings val = new MeshOutputSettings((OutputType)1, array, (ReportProgressRatio)null);
						MeshFileIo.Save(list, printItem.FileLocation, val, (ReportProgressRatio)null);
						printItem.Commit();
					}
				}
				catch (UnauthorizedAccessException)
				{
					UiThread.RunOnIdle((Action)delegate
					{
						StyledMessageBox.ShowMessageBox(null, "Oops! Unable to save changes, unauthorized access", "Unable to save");
					});
				}
				catch
				{
					UiThread.RunOnIdle((Action)delegate
					{
						StyledMessageBox.ShowMessageBox(null, "Oops! Unable to save changes.", "Unable to save");
					});
				}
			}
			else
			{
				printItem.FileLocation = CreateLibraryPath(extension);
				using (FileStream destination = File.Create(printItem.FileLocation))
				{
					stream.CopyTo(destination);
				}
				printItem.Commit();
			}
		}

		protected IEnumerable<PrintItemCollection> GetChildCollections()
		{
			string query = $"SELECT * FROM PrintItemCollection WHERE ParentCollectionID = {baseLibraryCollection.Id} ORDER BY Name ASC;";
			return Datastore.Instance.dbSQLite.Query<PrintItemCollection>(query, Array.Empty<object>());
		}

		private PrintItemCollection GetRootLibraryCollection()
		{
			ITableQuery<PrintItemCollection> tableQuery = Datastore.Instance.dbSQLite.Table<PrintItemCollection>();
			ParameterExpression val = Expression.Parameter(typeof(PrintItemCollection), "v");
			PrintItemCollection printItemCollection = tableQuery.Where(Expression.Lambda<Func<PrintItemCollection, bool>>((Expression)(object)Expression.Equal((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (Expression)(object)Expression.Constant((object)"_library", typeof(string))), (ParameterExpression[])(object)new ParameterExpression[1]
			{
				val
			})).Take(1).FirstOrDefault();
			if (printItemCollection == null)
			{
				printItemCollection = new PrintItemCollection();
				printItemCollection.Name = "_library";
				printItemCollection.Commit();
				baseLibraryCollection = printItemCollection;
				EnsureSamplePartsExist(OemSettings.Instance.PreloadedLibraryFiles);
			}
			return printItemCollection;
		}

		private void LoadLibraryItems()
		{
			IEnumerable<PrintItem> enumerable = null;
			IEnumerable<PrintItemCollection> enumerable2 = null;
			enumerable = GetLibraryItems(KeywordFilter);
			enumerable2 = GetChildCollections();
			printItems.Clear();
			if (enumerable != null)
			{
				printItems.AddRange(enumerable);
			}
			childCollections.Clear();
			if (enumerable2 != null)
			{
				childCollections.AddRange(enumerable2);
			}
			OnDataReloaded(null);
		}

		private static string CreateLibraryPath(string extension)
		{
			string path = Path.ChangeExtension(Path.GetRandomFileName(), string.IsNullOrEmpty(extension) ? ".amf" : extension);
			return Path.Combine(ApplicationDataStorage.Instance.ApplicationLibraryDataPath, path);
		}
	}
}
