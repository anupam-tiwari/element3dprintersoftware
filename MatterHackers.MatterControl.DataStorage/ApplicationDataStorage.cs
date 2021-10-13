using System;
using System.IO;
using System.Reflection;

namespace MatterHackers.MatterControl.DataStorage
{
	public class ApplicationDataStorage
	{
		public bool FirstRun;

		private static ApplicationDataStorage globalInstance;

		private static readonly string applicationDataFolderName = "Element";

		private readonly string datastoreName = "Element.db";

		private string applicationPath;

		private static string applicationUserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationDataFolderName);

		public static ApplicationDataStorage Instance
		{
			get
			{
				if (globalInstance == null)
				{
					globalInstance = new ApplicationDataStorage();
				}
				return globalInstance;
			}
		}

		public string ApplicationLibraryDataPath
		{
			get
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				string text = Path.Combine(ApplicationUserDataPath, "Library");
				DirectoryInfo val = new DirectoryInfo(text);
				if (!((FileSystemInfo)val).get_Exists())
				{
					val.Create();
				}
				return text;
			}
		}

		public string ApplicationPath
		{
			get
			{
				if (applicationPath == null)
				{
					applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				}
				return applicationPath;
			}
		}

		public string ApplicationTempDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationDataFolderName, "data", "temp");

		public static string ApplicationUserDataPath => applicationUserDataPath;

		public string DatastorePath => Path.Combine(ApplicationUserDataPath, datastoreName);

		public string GCodeOutputPath
		{
			get
			{
				string text = Path.Combine(ApplicationUserDataPath, "data", "gcode");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
		}

		public ApplicationDataStorage()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			DirectoryInfo val = new DirectoryInfo(ApplicationUserDataPath);
			if (!((FileSystemInfo)val).get_Exists())
			{
				val.Create();
			}
		}

		public string GetTempFileName(string fileExtension = null)
		{
			string path = (string.IsNullOrEmpty(fileExtension) ? Path.GetRandomFileName() : Path.ChangeExtension(Path.GetRandomFileName(), "." + fileExtension.TrimStart(new char[1]
			{
				'.'
			})));
			return Path.Combine(ApplicationTempDataPath, path);
		}

		internal void OverrideAppDataLocation(string path)
		{
			Console.WriteLine("   Overriding ApplicationUserDataPath: " + path);
			Directory.CreateDirectory(path);
			applicationUserDataPath = path;
			Datastore.Instance = new Datastore();
			Datastore.Instance.Initialize();
		}
	}
}
