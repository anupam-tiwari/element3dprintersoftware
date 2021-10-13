using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class PrinterActionLink : ActionLink
	{
		public PrinterInfo LinkedPrinter;

		public PrinterActionLink(string text, PrinterInfo printer, int fontSize = 10)
			: base(text, fontSize)
		{
			LinkedPrinter = printer;
		}
	}
}
