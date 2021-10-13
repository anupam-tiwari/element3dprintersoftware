using System.IO;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.MatterControl.DataStorage;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class MatterSliceInfo : SliceEngineInfo
	{
		public static string DisplayName = "MatterSlice";

		public MatterSliceInfo()
			: base(DisplayName)
		{
		}

		public override SlicingEngineTypes GetSliceEngineType()
		{
			return SlicingEngineTypes.MatterSlice;
		}

		public override bool Exists()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			if ((int)OsInformation.get_OperatingSystem() == 5 || (int)OsInformation.get_OperatingSystem() == 2 || SlicingQueue.runInProcess)
			{
				return true;
			}
			if (GetEnginePath() == null)
			{
				return false;
			}
			return File.Exists(GetEnginePath());
		}

		protected override string getWindowsPath()
		{
			return Path.GetFullPath(Path.Combine(".", "MatterSlice.exe"));
		}

		protected override string getMacPath()
		{
			return Path.Combine(ApplicationDataStorage.Instance.ApplicationPath, "MatterSlice");
		}

		protected override string getLinuxPath()
		{
			return Path.GetFullPath(Path.Combine(".", "MatterSlice.exe"));
		}
	}
}
