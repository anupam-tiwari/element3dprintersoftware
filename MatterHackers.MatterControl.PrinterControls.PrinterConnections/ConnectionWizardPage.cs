using System;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class ConnectionWizardPage : WizardPage
	{
		public ConnectionWizardPage()
		{
			((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				PrinterConnectionAndCommunication.Instance.HaltConnectionThread();
			});
		}
	}
}
