using System.Threading;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class NotPrintingStream : GCodeStream
	{
		public override void Dispose()
		{
		}

		public override string ReadLine()
		{
			Thread.Sleep(100);
			return "";
		}

		public override void SetPrinterPosition(PrinterMove position)
		{
		}
	}
}
