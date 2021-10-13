using System;

namespace MatterHackers.MatterControl.VersionManagement
{
	public class ResponseErrorEventArgs : EventArgs
	{
		public JsonResponseDictionary ResponseValues
		{
			get;
			set;
		}
	}
}
