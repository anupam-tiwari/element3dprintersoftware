using System;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.PrinterCommunication
{
	public class PrintItemWrapperEventArgs : EventArgs
	{
		public PrintItemWrapper PrintItemWrapper
		{
			get;
		}

		public PrintItemWrapperEventArgs(PrintItemWrapper printItemWrapper)
		{
			PrintItemWrapper = printItemWrapper;
		}
	}
}
