using System;

namespace MatterHackers.MatterControl.FrostedSerial
{
	public class SerialErrorReceivedEventArgs : EventArgs
	{
		private SerialError eventType;

		public SerialError EventType => eventType;

		internal SerialErrorReceivedEventArgs(SerialError eventType)
		{
			this.eventType = eventType;
		}
	}
}
