using System.IO;
using MatterHackers.MatterControl.DataStorage;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class Slic3rInfo : SliceEngineInfo
	{
		public Slic3rInfo()
			: base("Slic3r")
		{
		}

		public override SlicingEngineTypes GetSliceEngineType()
		{
			return SlicingEngineTypes.Slic3r;
		}

		protected override string getWindowsPath()
		{
			string text = Path.Combine("..", "Slic3r", "slic3r.exe");
			if (!File.Exists(text))
			{
				text = Path.Combine(".", "Slic3r", "slic3r.exe");
			}
			return Path.GetFullPath(text);
		}

		protected override string getMacPath()
		{
			return Path.Combine(ApplicationDataStorage.Instance.ApplicationPath, "Slic3r.app", "Contents", "MacOS", "slic3r");
		}

		protected override string getLinuxPath()
		{
			string text = Path.Combine("..", "Slic3r", "bin", "slic3r");
			if (!File.Exists(text))
			{
				text = Path.Combine(".", "Slic3r", "bin", "slic3r");
			}
			return Path.GetFullPath(text);
		}
	}
}
