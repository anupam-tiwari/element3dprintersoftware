using System.Collections.Generic;
using System.IO;
using MatterHackers.MatterControl.DataStorage;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl
{
	internal class ManifestFileHandler
	{
		private ManifestFile project;

		private static string applicationDataPath = ApplicationDataStorage.ApplicationUserDataPath;

		private static string defaultPathAndFileName = Path.Combine(applicationDataPath, "data", "default.mcp");

		public ManifestFileHandler(List<PrintItem> projectFiles)
		{
			if (projectFiles != null)
			{
				project = new ManifestFile();
				project.ProjectFiles = projectFiles;
			}
		}

		public void ExportToJson(string savedFileName = null)
		{
			if (savedFileName == null)
			{
				savedFileName = defaultPathAndFileName;
			}
			string text = JsonConvert.SerializeObject((object)project, (Formatting)1);
			string text2 = Path.Combine(applicationDataPath, "data");
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			File.WriteAllText(savedFileName, text);
		}

		public List<PrintItem> ImportFromJson(string filePath = null)
		{
			if (filePath == null)
			{
				filePath = defaultPathAndFileName;
			}
			if (!File.Exists(filePath))
			{
				return null;
			}
			ManifestFile manifestFile = JsonConvert.DeserializeObject<ManifestFile>(File.ReadAllText(filePath));
			if (manifestFile == null)
			{
				return new List<PrintItem>();
			}
			return manifestFile.ProjectFiles;
		}
	}
}
