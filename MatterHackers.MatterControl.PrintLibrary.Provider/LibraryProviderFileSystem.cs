using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public class LibraryProviderFileSystem : LibraryProvider
	{
		private string currentDirectory = ".";

		private List<string> currentDirectoryDirectories = new List<string>();

		private List<string> currentDirectoryFiles = new List<string>();

		private FileSystemWatcher directoryWatcher;

		private string keywordFilter = string.Empty;

		private string rootPath;

		private bool useIncrementedNameDuringTypeChange;

		public override int CollectionCount => currentDirectoryDirectories.Count;

		public override bool CanShare => false;

		public override int ItemCount => currentDirectoryFiles.Count;

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
					GetFilesAndCollectionsInCurrentDirectory(keywordFilter.Trim() != "");
				}
			}
		}

		public override string ProviderKey => "FileSystem_" + rootPath + "_Key";

		public LibraryProviderFileSystem(string rootPath, string name, LibraryProvider parentLibraryProvider, Action<LibraryProvider> setCurrentLibraryProvider, bool useIncrementedNameDuringTypeChange = false)
			: base(parentLibraryProvider, setCurrentLibraryProvider)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Invalid comparison between Unknown and I4
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Expected O, but got Unknown
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Expected O, but got Unknown
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected O, but got Unknown
			base.Name = name;
			this.rootPath = rootPath;
			this.useIncrementedNameDuringTypeChange = useIncrementedNameDuringTypeChange;
			if ((int)OsInformation.get_OperatingSystem() == 1)
			{
				directoryWatcher = new FileSystemWatcher();
				directoryWatcher.set_Path(rootPath);
				directoryWatcher.set_NotifyFilter((NotifyFilters)51);
				directoryWatcher.add_Changed(new FileSystemEventHandler(DiretoryContentsChanged));
				directoryWatcher.add_Created(new FileSystemEventHandler(DiretoryContentsChanged));
				directoryWatcher.add_Deleted(new FileSystemEventHandler(DiretoryContentsChanged));
				directoryWatcher.add_Renamed(new RenamedEventHandler(DiretoryContentsChanged));
				directoryWatcher.set_EnableRaisingEvents(true);
			}
			GetFilesAndCollectionsInCurrentDirectory();
		}

		public override void RenameItem(int itemIndexToRename, string newName)
		{
			string text = Path.Combine(rootPath, currentDirectoryFiles[itemIndexToRename]);
			if (File.Exists(text))
			{
				string extension = Path.GetExtension(text);
				string path = Path.Combine(Path.GetDirectoryName(text), newName);
				path = Path.ChangeExtension(path, extension);
				File.Move(text, path);
				Stopwatch val = Stopwatch.StartNew();
				while (File.Exists(path) && val.get_ElapsedMilliseconds() < 100)
				{
					Thread.Sleep(1);
				}
				GetFilesAndCollectionsInCurrentDirectory();
			}
		}

		public override void ShareItem(int itemIndexToShare)
		{
		}

		public void ChangeName(string newName)
		{
			base.Name = newName;
		}

		public override void AddCollectionToLibrary(string collectionName)
		{
			string text = Path.Combine(rootPath, currentDirectory, collectionName);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
				GetFilesAndCollectionsInCurrentDirectory();
			}
		}

		public override void AddItem(PrintItemWrapper itemToAdd)
		{
			string destPath = rootPath;
			itemToAdd.FileLocation = CopyFile(itemToAdd.FileLocation, itemToAdd.Name, destPath);
			GetFilesAndCollectionsInCurrentDirectory();
		}

		public override void Dispose()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Expected O, but got Unknown
			if (directoryWatcher != null)
			{
				directoryWatcher.set_EnableRaisingEvents(false);
				directoryWatcher.remove_Changed(new FileSystemEventHandler(DiretoryContentsChanged));
				directoryWatcher.remove_Created(new FileSystemEventHandler(DiretoryContentsChanged));
				directoryWatcher.remove_Deleted(new FileSystemEventHandler(DiretoryContentsChanged));
				directoryWatcher.remove_Renamed(new RenamedEventHandler(DiretoryContentsChanged));
			}
		}

		public override PrintItemCollection GetCollectionItem(int collectionIndex)
		{
			string text = currentDirectoryDirectories[collectionIndex];
			return new PrintItemCollection(Path.GetFileNameWithoutExtension(text), Path.Combine(rootPath, text));
		}

		public override string GetPrintItemName(int itemIndex)
		{
			return Path.GetFileNameWithoutExtension(currentDirectoryFiles[itemIndex]);
		}

		public override Task<PrintItemWrapper> GetPrintItemWrapperAsync(int itemIndex)
		{
			string fileLocation = currentDirectoryFiles[itemIndex];
			return Task.FromResult(new PrintItemWrapper(new PrintItem(GetPrintItemName(itemIndex), fileLocation), GetProviderLocator())
			{
				UseIncrementedNameDuringTypeChange = true
			});
		}

		public override LibraryProvider GetProviderForCollection(PrintItemCollection collection)
		{
			return new LibraryProviderFileSystem(Path.Combine(rootPath, collection.Key), collection.Name, this, base.SetCurrentLibraryProvider);
		}

		public override void RenameCollection(int collectionIndexToRename, string newName)
		{
			string text = Path.Combine(rootPath, currentDirectoryDirectories[collectionIndexToRename]);
			if (Directory.Exists(text))
			{
				string text2 = Path.Combine(Path.GetDirectoryName(text), newName);
				Directory.Move(text, text2);
				Stopwatch val = Stopwatch.StartNew();
				while (Directory.Exists(text2) && val.get_ElapsedMilliseconds() < 100)
				{
					Thread.Sleep(1);
				}
				GetFilesAndCollectionsInCurrentDirectory();
			}
		}

		public override void RemoveCollection(int collectionIndexToRemove)
		{
			string text = Path.Combine(rootPath, currentDirectoryDirectories[collectionIndexToRemove]);
			if (Directory.Exists(text))
			{
				Directory.Delete(text, true);
				Stopwatch val = Stopwatch.StartNew();
				while (Directory.Exists(text) && val.get_ElapsedMilliseconds() < 100)
				{
					Thread.Sleep(1);
				}
				GetFilesAndCollectionsInCurrentDirectory();
			}
		}

		public override void RemoveItem(int itemToRemoveIndex)
		{
			File.Delete(currentDirectoryFiles[itemToRemoveIndex]);
			GetFilesAndCollectionsInCurrentDirectory();
		}

		private static string CopyFile(string sourceFile, string destFileName, string destPath)
		{
			try
			{
				Directory.CreateDirectory(destPath);
			}
			catch (Exception)
			{
			}
			string path = Path.Combine(destPath, destFileName);
			path = Path.ChangeExtension(path, Path.GetExtension(sourceFile));
			try
			{
				if (!File.Exists(path))
				{
					File.Copy(sourceFile, path);
					return path;
				}
				string directoryName = Path.GetDirectoryName(path);
				string text = Path.GetFileNameWithoutExtension(path);
				string extension = Path.GetExtension(path);
				int num = text.LastIndexOf(' ');
				if (num != -1 && int.TryParse(text.Substring(num), out var _))
				{
					text = text.Substring(0, num);
				}
				int num2 = 2;
				string text2 = Path.Combine(directoryName, text + " " + num2 + extension);
				while (File.Exists(text2))
				{
					num2++;
					text2 = Path.Combine(directoryName, text + " " + num2 + extension);
				}
				File.Copy(sourceFile, text2);
				return path;
			}
			catch (Exception)
			{
				return path;
			}
		}

		private void DiretoryContentsChanged(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				GetFilesAndCollectionsInCurrentDirectory();
			});
		}

		private async void GetFilesAndCollectionsInCurrentDirectory(bool recursive = false)
		{
			List<string> newReadDirectoryDirectories = new List<string>();
			List<string> newReadDirectoryFiles = new List<string>();
			await Task.Run(delegate
			{
				try
				{
					string[] array = null;
					array = ((!recursive) ? Directory.GetDirectories(Path.Combine(rootPath, currentDirectory)) : Directory.GetDirectories(Path.Combine(rootPath, currentDirectory), "*.*", (SearchOption)1));
					foreach (string item2 in (IEnumerable<string>)Enumerable.OrderBy<string, string>((IEnumerable<string>)array, (Func<string, string>)((string f) => f)))
					{
						string item = item2.Substring(rootPath.Length + 1);
						newReadDirectoryDirectories.Add(item);
					}
				}
				catch (Exception)
				{
				}
				try
				{
					string text = keywordFilter.ToUpper();
					foreach (string item3 in (IEnumerable<string>)Enumerable.OrderBy<string, string>((IEnumerable<string>)Directory.GetFiles(Path.Combine(rootPath, currentDirectory)), (Func<string, string>)((string f) => f)))
					{
						string value = Path.GetExtension(item3)!.ToLower();
						if (!string.IsNullOrEmpty(value) && ApplicationSettings.LibraryFilterFileExtensions.Contains(value) && (text.Trim() == string.Empty || FileNameContainsFilter(item3, text)))
						{
							newReadDirectoryFiles.Add(item3);
						}
					}
					if (recursive)
					{
						foreach (string item4 in newReadDirectoryDirectories)
						{
							foreach (string item5 in (IEnumerable<string>)Enumerable.OrderBy<string, string>((IEnumerable<string>)Directory.GetFiles(Path.Combine(rootPath, item4)), (Func<string, string>)((string f) => f)))
							{
								if (ApplicationSettings.LibraryFilterFileExtensions.Contains(Path.GetExtension(item5)!.ToLower()) && (keywordFilter.Trim() == string.Empty || FileNameContainsFilter(item5, text)))
								{
									newReadDirectoryFiles.Add(item5);
								}
							}
						}
					}
				}
				catch (Exception)
				{
				}
			});
			if (recursive)
			{
				currentDirectoryDirectories.Clear();
			}
			else
			{
				currentDirectoryDirectories = newReadDirectoryDirectories;
			}
			currentDirectoryFiles = newReadDirectoryFiles;
			OnDataReloaded(null);
		}

		private bool FileNameContainsFilter(string filename, string upperFilter)
		{
			string[] array = upperFilter.Split(new char[1]
			{
				' '
			});
			foreach (string value in array)
			{
				if (!Path.GetFileNameWithoutExtension(filename.ToUpper().Replace('_', ' '))!.Contains(value))
				{
					return false;
				}
			}
			return true;
		}

		private string GetPathFromLocator(List<ProviderLocatorNode> providerLocator)
		{
			string text = Path.Combine(rootPath, providerLocator[providerLocator.Count - 1].Key);
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			return text.Replace("." + directorySeparatorChar, "");
		}
	}
}
