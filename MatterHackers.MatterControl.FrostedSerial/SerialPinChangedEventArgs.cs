using System;

namespace MatterHackers.MatterControl.FrostedSerial
{
	public class SerialPinChangedEventArgs : EventArgs
	{
		private SerialPinChange eventType;

		public SerialPinChange EventType => eventType;

		internal SerialPinChangedEventArgs(SerialPinChange eventType)
		{
			this.eventType = eventType;
		}
	}
}
