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
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public class LibraryProviderSelector : LibraryProvider
	{
		private List<ILibraryCreator> libraryCreators = new List<ILibraryCreator>();

		private Dictionary<int, LibraryProvider> libraryProviders = new Dictionary<int, LibraryProvider>();

		private EventHandler unregisterEvents;

		private List<ImageBuffer> folderImagesForChildren = new List<ImageBuffer>();

		private int firstAddedDirectoryIndex;

		private PluginFinder<LibraryProviderPlugin> libraryProviderPlugins;

		private bool includeQueueLibraryProvider;

		public static readonly string ProviderKeyName = "ProviderSelectorKey";

		public static RootedObjectEventHandler LibraryRootNotice = new RootedObjectEventHandler();

		private static LibraryProviderSelector currentInstance;

		private string keywordFilter = "";

		public ILibraryCreator PurchasedLibraryCreator
		{
			get;
			private set;
		}

		public ILibraryCreator SharedLibraryCreator
		{
			get;
			private set;
		}

		public override bool CanShare => false;

		public override int CollectionCount
		{
			get
			{
				if (string.IsNullOrEmpty(KeywordFilter))
				{
					return libraryCreators.Count;
				}
				return 0;
			}
		}

		public override int ItemCount
		{
			get
			{
				if (string.IsNullOrEmpty(KeywordFilter))
				{
					return 0;
				}
				return 1;
			}
		}

		public override string KeywordFilter
		{
			get
			{
				return keywordFilter;
			}
			set
			{
				if (keywordFilter != value)
				{
					keywordFilter = value;
					OnDataReloaded(null);
				}
			}
		}

		public override string ProviderKey => ProviderKeyName;

		public LibraryProviderSelector(Action<LibraryProvider> setCurrentLibraryProvider, bool includeQueueLibraryProvider)
			: base(null, setCurrentLibraryProvider)
		{
			currentInstance = this;
			this.includeQueueLibraryProvider = includeQueueLibraryProvider;
			base.Name = "Home".Localize();
			LibraryRootNotice.RegisterEvent((EventHandler)delegate
			{
				ReloadData();
			}, ref unregisterEvents);
			ApplicationController.Instance.CloudSyncStatusChanged.RegisterEvent((EventHandler)CloudSyncStatusChanged, ref unregisterEvents);
			libraryProviderPlugins = new PluginFinder<LibraryProviderPlugin>((string)null, (IComparer<LibraryProviderPlugin>)null);
			ReloadData();
		}

		public static void Reload()
		{
			currentInstance.ReloadData();
		}

		private void ReloadData()
		{
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			libraryCreators.Clear();
			folderImagesForChildren.Clear();
			if (includeQueueLibraryProvider)
			{
				libraryCreators.Add(new LibraryProviderQueueCreator());
				AddFolderImage("queue_folder.png");
			}
			libraryCreators.Add(new LibraryProviderSQLiteCreator());
			AddFolderImage("library_folder.png");
			foreach (LibraryProviderPlugin plugin in libraryProviderPlugins.Plugins)
			{
				if (plugin.ProviderKey == "LibraryProviderPurchasedKey")
				{
					PurchasedLibraryCreator = plugin;
				}
				if (plugin.ProviderKey == "LibraryProviderSharedKey")
				{
					SharedLibraryCreator = plugin;
				}
				if (plugin.ShouldBeShown())
				{
					libraryCreators.Add(plugin);
					folderImagesForChildren.Add(plugin.GetFolderImage());
				}
			}
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
			if (Directory.Exists(text))
			{
				libraryCreators.Add(new LibraryProviderFileSystemCreator(text, "Downloads".Localize(), useIncrementedNameDuringTypeChange: true));
				AddFolderImage("download_folder.png");
			}
			string text2 = Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "LibraryFolders.conf");
			if (File.Exists(text2))
			{
				foreach (string item in File.ReadLines(text2))
				{
					if (Directory.Exists(item))
					{
						libraryCreators.Add(new LibraryProviderFileSystemCreator(item, ((FileSystemInfo)new DirectoryInfo(item)).get_Name(), useIncrementedNameDuringTypeChange: true));
						AddFolderImage("download_folder.png");
					}
				}
			}
			firstAddedDirectoryIndex = libraryCreators.Count;
			OnDataReloaded(null);
		}

		public override bool IsProtected()
		{
			return true;
		}

		private void AddFolderImage(string iconFileName)
		{
			string text = Path.Combine("FileDialog", iconFileName);
			ImageBuffer item = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon(text));
			folderImagesForChildren.Add(item);
		}

		public override ImageBuffer GetCollectionFolderImage(int collectionIndex)
		{
			return folderImagesForChildren[collectionIndex];
		}

		public override void RenameCollection(int collectionIndexToRename, string newName)
		{
			if (collectionIndexToRename < firstAddedDirectoryIndex)
			{
				return;
			}
			LibraryProviderFileSystemCreator libraryProviderFileSystemCreator = libraryCreators[collectionIndexToRename] as LibraryProviderFileSystemCreator;
			if (libraryProviderFileSystemCreator != null && libraryProviderFileSystemCreator.Description != newName)
			{
				libraryProviderFileSystemCreator.Description = newName;
				UiThread.RunOnIdle((Action)delegate
				{
					OnDataReloaded(null);
				});
			}
		}

		public override void RenameItem(int itemIndexToRename, string newName)
		{
			throw new NotImplementedException();
		}

		public override void ShareItem(int itemIndexToShare)
		{
		}

		public void CloudSyncStatusChanged(object sender, EventArgs eventArgs)
		{
			ApplicationController.CloudSyncEventArgs obj = eventArgs as ApplicationController.CloudSyncEventArgs;
			if (obj != null && !obj.IsAuthenticated && base.SetCurrentLibraryProvider != null)
			{
				base.SetCurrentLibraryProvider(this);
			}
		}

		public override void AddCollectionToLibrary(string collectionName)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Expected O, but got Unknown
				FileDialog.SelectFolderDialog(new SelectFolderDialogParams("Select Folder", (RootFolderTypes)0, true, "", ""), (Action<SelectFolderDialogParams>)delegate(SelectFolderDialogParams folderParams)
				{
					libraryCreators.Add(new LibraryProviderFileSystemCreator(folderParams.get_FolderPath(), collectionName));
					AddFolderImage("folder.png");
					UiThread.RunOnIdle((Action)delegate
					{
						OnDataReloaded(null);
					});
				});
			});
		}

		public override void AddItem(PrintItemWrapper itemToAdd)
		{
			if (Directory.Exists(itemToAdd.FileLocation))
			{
				libraryCreators.Add(new LibraryProviderFileSystemCreator(itemToAdd.FileLocation, Path.GetFileName(itemToAdd.FileLocation)));
				AddFolderImage("folder.png");
				UiThread.RunOnIdle((Action)delegate
				{
					OnDataReloaded(null);
				});
			}
		}

		public override GuiWidget GetItemThumbnail(int printItemIndex)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			ImageWidget val = new ImageWidget(ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon(Path.Combine("FileDialog", "file.png"), 48, 48)));
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			return (GuiWidget)val;
		}

		public override void Dispose()
		{
			foreach (KeyValuePair<int, LibraryProvider> libraryProvider in libraryProviders)
			{
				libraryProvider.Value.Dispose();
			}
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
		}

		public override PrintItemCollection GetCollectionItem(int collectionIndex)
		{
			if (libraryProviders.ContainsKey(collectionIndex))
			{
				libraryProviders[collectionIndex].Dispose();
				libraryProviders.Remove(collectionIndex);
			}
			LibraryProvider libraryProvider = libraryCreators[collectionIndex].CreateLibraryProvider(this, base.SetCurrentLibraryProvider);
			libraryProviders.Add(collectionIndex, libraryProvider);
			return new PrintItemCollection(libraryProvider.Name, libraryProvider.ProviderKey);
		}

		public override string GetPrintItemName(int itemIndex)
		{
			return LibraryRowItem.SearchResultsNotAvailableToken;
		}

		public override Task<PrintItemWrapper> GetPrintItemWrapperAsync(int itemIndex)
		{
			return Task.FromResult(new PrintItemWrapper(new PrintItem(LibraryRowItem.SearchResultsNotAvailableToken, LibraryRowItem.SearchResultsNotAvailableToken), GetProviderLocator())
			{
				UseIncrementedNameDuringTypeChange = true
			});
		}

		public override LibraryProvider GetProviderForCollection(PrintItemCollection collection)
		{
			LibraryProvider libraryProvider = Enumerable.FirstOrDefault<LibraryProvider>(Enumerable.Where<LibraryProvider>((IEnumerable<LibraryProvider>)libraryProviders.Values, (Func<LibraryProvider, bool>)((LibraryProvider p) => p.ProviderKey == collection.Key)));
			if (libraryProvider != null)
			{
				return libraryProvider;
			}
			foreach (ILibraryCreator libraryCreator in libraryCreators)
			{
				if (collection.Key == libraryCreator.ProviderKey)
				{
					return libraryCreator.CreateLibraryProvider(this, base.SetCurrentLibraryProvider);
				}
			}
			throw new NotImplementedException();
		}

		public override void RemoveCollection(int collectionIndexToRemove)
		{
			libraryCreators.RemoveAt(collectionIndexToRemove);
			UiThread.RunOnIdle((Action)delegate
			{
				OnDataReloaded(null);
			});
		}

		public override void RemoveItem(int itemToRemoveIndex)
		{
			throw new NotImplementedException();
		}

		public LibraryProvider GetPurchasedLibrary()
		{
			((LibraryProviderPlugin)PurchasedLibraryCreator).ForceVisible();
			return PurchasedLibraryCreator.CreateLibraryProvider(this, base.SetCurrentLibraryProvider);
		}

		public LibraryProvider GetSharedLibrary()
		{
			((LibraryProviderPlugin)SharedLibraryCreator).ForceVisible();
			return SharedLibraryCreator.CreateLibraryProvider(this, base.SetCurrentLibraryProvider);
		}
	}
}
