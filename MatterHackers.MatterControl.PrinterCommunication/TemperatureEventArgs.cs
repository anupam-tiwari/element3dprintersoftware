using System;

namespace MatterHackers.MatterControl.PrinterCommunication
{
	public class TemperatureEventArgs : EventArgs
	{
		public int Index0Based
		{
			get;
		}

		public double Temperature
		{
			get;
		}

		public TemperatureEventArgs(int index0Based, double temperature)
		{
			Index0Based = index0Based;
			Temperature = temperature;
		}
	}
}
