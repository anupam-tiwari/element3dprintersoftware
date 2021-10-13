using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class HomePrinterPage : InstructionsPage
	{
		public HomePrinterPage(string pageDescription, string instructionsText)
			: base(pageDescription, instructionsText)
		{
		}

		public override void PageIsBecomingActive()
		{
			PrinterConnectionAndCommunication.Instance.HomeAxis(PrinterConnectionAndCommunication.Axis.XYZ);
			base.PageIsBecomingActive();
		}
	}
}
