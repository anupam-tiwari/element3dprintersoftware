using System.IO;
using MatterHackers.MatterControl.DataStorage;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class CuraEngineInfo : SliceEngineInfo
	{
		public CuraEngineInfo()
			: base("CuraEngine")
		{
		}

		public override SlicingEngineTypes GetSliceEngineType()
		{
			return SlicingEngineTypes.CuraEngine;
		}

		protected override string getWindowsPath()
		{
			string text = Path.Combine("..", "CuraEngine.exe");
			if (!File.Exists(text))
			{
				text = Path.Combine(".", "CuraEngine.exe");
			}
			return Path.GetFullPath(text);
		}

		protected override string getMacPath()
		{
			return Path.Combine(ApplicationDataStorage.Instance.ApplicationPath, "CuraEngine");
		}

		protected override string getLinuxPath()
		{
			string text = Path.Combine("..", "CuraEngine.exe");
			if (!File.Exists(text))
			{
				text = Path.Combine(".", "CuraEngine.exe");
			}
			return Path.GetFullPath(text);
		}
	}
}
