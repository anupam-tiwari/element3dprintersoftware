using System;

namespace MatterHackers.MatterControl.VersionManagement
{
	public class ResponseSuccessEventArgs<ResponseType> : EventArgs
	{
		public ResponseType ResponseItem
		{
			get;
			set;
		}
	}
}
