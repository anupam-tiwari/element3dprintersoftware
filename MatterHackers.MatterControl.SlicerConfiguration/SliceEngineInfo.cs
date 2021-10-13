using System;
using System.IO;
using MatterHackers.Agg.PlatformAbstract;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public abstract class SliceEngineInfo
	{
		public string Name
		{
			get;
			set;
		}

		protected abstract string getWindowsPath();

		protected abstract string getMacPath();

		protected abstract string getLinuxPath();

		public abstract SlicingEngineTypes GetSliceEngineType();

		public SliceEngineInfo(string name)
		{
			Name = name;
		}

		public virtual bool Exists()
		{
			if (GetEnginePath() == null)
			{
				return false;
			}
			return File.Exists(GetEnginePath());
		}

		public string GetEnginePath()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected I4, but got Unknown
			OSType operatingSystem = OsInformation.get_OperatingSystem();
			return (operatingSystem - 1) switch
			{
				0 => getWindowsPath(), 
				1 => getMacPath(), 
				2 => getLinuxPath(), 
				4 => null, 
				_ => throw new NotImplementedException(), 
			};
		}
	}
}
