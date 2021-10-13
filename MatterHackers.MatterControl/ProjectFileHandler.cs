using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.PolygonMesh.Processors;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl
{
	internal class ProjectFileHandler
	{
		private Project project;

		private Dictionary<string, ManifestItem> sourceFiles = new Dictionary<string, ManifestItem>();

		private HashSet<string> addedFileNames = new HashSet<string>();

		private static string applicationDataPath = ApplicationDataStorage.ApplicationUserDataPath;

		private static string archiveStagingFolder = Path.Combine(applicationDataPath, "data", "temp", "project-assembly");

		private static string defaultManifestPathAndFileName = Path.Combine(archiveStagingFolder, "manifest.json");

		private static string defaultProjectPathAndFileName = Path.Combine(applicationDataPath, "data", "default.zip");

		public ProjectFileHandler(List<PrintItem> projectFiles)
		{
			if (projectFiles == null)
			{
				return;
			}
			project = new Project();
			foreach (PrintItem projectFile in projectFiles)
			{
				if (sourceFiles.ContainsKey(projectFile.FileLocation))
				{
					sourceFiles[projectFile.FileLocation].ItemQuantity = sourceFiles[projectFile.FileLocation].ItemQuantity + 1;
					continue;
				}
				string fileName = Path.GetFileName(projectFile.FileLocation);
				if (addedFileNames.Contains(fileName))
				{
					StyledMessageBox.ShowMessageBox(null, $"Duplicate file name found but in a different folder '{fileName}'. This part will not be added to the collection.\n\n{projectFile.FileLocation}", "Duplicate File");
					continue;
				}
				addedFileNames.Add(fileName);
				ManifestItem value = new ManifestItem
				{
					ItemQuantity = 1,
					Name = projectFile.Name,
					FileName = Path.GetFileName(projectFile.FileLocation)
				};
				sourceFiles.Add(projectFile.FileLocation, value);
			}
			List<ManifestItem> projectFiles2 = Enumerable.ToList<ManifestItem>((IEnumerable<ManifestItem>)sourceFiles.Values);
			project.ProjectFiles = projectFiles2;
		}

		public void SaveAs()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			FileDialog.SaveFileDialog(new SaveFileDialogParams("Save Project|*.zip", "", "", ""), (Action<SaveFileDialogParams>)onSaveFileSelected);
		}

		private void onSaveFileSelected(SaveFileDialogParams saveParams)
		{
			if (!string.IsNullOrEmpty(((FileDialogParams)saveParams).get_FileName()))
			{
				ExportToProjectArchive(((FileDialogParams)saveParams).get_FileName());
			}
		}

		public static void EmptyFolder(DirectoryInfo directory)
		{
			FileInfo[] files = directory.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				((FileSystemInfo)files[i]).Delete();
			}
			DirectoryInfo[] directories = directory.GetDirectories();
			for (int i = 0; i < directories.Length; i++)
			{
				directories[i].Delete(true);
			}
		}

		public void ExportToProjectArchive(string savedFileName = null)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			if (savedFileName == null)
			{
				savedFileName = defaultProjectPathAndFileName;
			}
			if (!Directory.Exists(archiveStagingFolder))
			{
				Directory.CreateDirectory(archiveStagingFolder);
			}
			else
			{
				EmptyFolder(new DirectoryInfo(archiveStagingFolder));
			}
			File.WriteAllText(defaultManifestPathAndFileName, JsonConvert.SerializeObject((object)project, (Formatting)1));
			foreach (KeyValuePair<string, ManifestItem> sourceFile in sourceFiles)
			{
				CopyFileToTempFolder(sourceFile.Key, sourceFile.Value.FileName);
			}
			if (File.Exists(savedFileName))
			{
				try
				{
					File.Delete(savedFileName);
				}
				catch (Exception)
				{
					string directoryName = Path.GetDirectoryName(savedFileName);
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(savedFileName);
					string extension = Path.GetExtension(savedFileName);
					for (int i = 1; i < 20; i++)
					{
						string text = Path.Combine(directoryName, $"{fileNameWithoutExtension}({i}){extension}");
						if (!File.Exists(text))
						{
							File.Move(savedFileName, text);
							break;
						}
					}
				}
			}
			ZipFile.CreateFromDirectory(archiveStagingFolder, savedFileName, CompressionLevel.Optimal, true);
		}

		private static void CopyFileToTempFolder(string sourceFile, string fileName)
		{
			if (File.Exists(sourceFile))
			{
				try
				{
					File.Copy(sourceFile, Path.Combine(archiveStagingFolder, fileName));
				}
				catch (IOException ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		public List<PrintItem> ImportFromProjectArchive(string loadedFileName = null)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Expected O, but got Unknown
			if (loadedFileName == null)
			{
				loadedFileName = defaultProjectPathAndFileName;
			}
			if (!File.Exists(loadedFileName))
			{
				return null;
			}
			try
			{
				using FileStream stream = File.OpenRead(loadedFileName);
				using ZipArchive zipArchive = new ZipArchive(stream);
				int hashCode = zipArchive.GetHashCode();
				string text = Path.Combine(applicationDataPath, "data", "temp", "project-extract", hashCode.ToString());
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				else
				{
					EmptyFolder(new DirectoryInfo(text));
				}
				List<PrintItem> list = new List<PrintItem>();
				Project project = null;
				foreach (ZipArchiveEntry entry in zipArchive.Entries)
				{
					string text2 = Path.GetExtension(entry.Name)!.ToUpper();
					if (string.IsNullOrWhiteSpace(entry.Name) || (!(entry.Name == "manifest.json") && !MeshFileIo.ValidFileExtensions().Contains(text2) && !(text2 == ".GCODE")))
					{
						continue;
					}
					string text3 = Path.Combine(text, entry.Name);
					string directoryName = Path.GetDirectoryName(text3);
					if (!Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
					using (Stream stream2 = entry.Open())
					{
						using FileStream destination = File.Create(text3);
						stream2.CopyTo(destination);
					}
					if (entry.Name == "manifest.json")
					{
						using StreamReader streamReader = new StreamReader(text3);
						project = (Project)JsonConvert.DeserializeObject(streamReader.ReadToEnd(), typeof(Project));
					}
				}
				if (project != null)
				{
					foreach (ManifestItem projectFile in project.ProjectFiles)
					{
						for (int i = 1; i <= projectFile.ItemQuantity; i++)
						{
							list.Add(GetPrintItemFromFile(Path.Combine(text, projectFile.FileName), projectFile.Name));
						}
					}
				}
				else
				{
					string[] files = Directory.GetFiles(text, "*.*", (SearchOption)1);
					foreach (string text4 in files)
					{
						list.Add(GetPrintItemFromFile(text4, Path.GetFileNameWithoutExtension(text4)));
					}
				}
				return list;
			}
			catch
			{
				return null;
			}
		}

		private PrintItem GetPrintItemFromFile(string fileName, string displayName)
		{
			return new PrintItem
			{
				FileLocation = fileName,
				Name = displayName
			};
		}
	}
}
