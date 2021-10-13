namespace MatterHackers.MatterControl
{
	public class PublicDevice
	{
		public string DeviceToken
		{
			get;
			set;
		}

		public string ProfileToken
		{
			get;
			set;
		}

		public string ShortProfileID
		{
			get;
			set;
		}

		public string CacheKey => ShortProfileID + ".printer";
	}
}
