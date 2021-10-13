using MatterHackers.Agg.PlatformAbstract;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl
{
	public sealed class VersionInfo
	{
		public static readonly VersionInfo Instance = DeserializeFromDisk();

		public string ReleaseVersion
		{
			get;
			set;
		}

		public string BuildVersion
		{
			get;
			set;
		}

		public string BuildToken
		{
			get;
			set;
		}

		public string ProjectToken
		{
			get;
			set;
		}

		private VersionInfo()
		{
		}

		private static VersionInfo DeserializeFromDisk()
		{
			return JsonConvert.DeserializeObject<VersionInfo>(StaticData.get_Instance().ReadAllText("BuildInfo.txt"));
		}
	}
}
