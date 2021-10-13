using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class PrinterSelectRadioButton : RadioButton
	{
		public PrinterInfo printer;

		public PrinterSelectRadioButton(PrinterInfo printer)
			: this(printer.Name, 12)
		{
			this.printer = printer;
		}
	}
}
