using System;

namespace MatterHackers.MatterControl.FrostedSerial
{
	public class SerialDataReceivedEventArgs : EventArgs
	{
		private SerialData eventType;

		public SerialData EventType => eventType;

		internal SerialDataReceivedEventArgs(SerialData eventType)
		{
			this.eventType = eventType;
		}
	}
}
