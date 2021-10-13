using System;
using MatterHackers.Agg.UI;
using MatterHackers.GuiAutomation;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public static class UiNavigation
	{
		public static void OpenEditPrinterWizard_Click(object sender, EventArgs e)
		{
			Button editButton = sender as Button;
			((GuiWidget)editButton).set_ToolTipText("Edit Current Printer Settings".Localize());
			if (editButton != null)
			{
				((GuiWidget)editButton).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					((GuiWidget)editButton).remove_Click((EventHandler<MouseEventArgs>)OpenEditPrinterWizard_Click);
				});
				OpenEditPrinterWizard("baud_rate Edit Field,auto_connect Edit Field,com_port Edit Field");
			}
		}

		public static void OpenEditPrinterWizard(string widgetNameToHighlight)
		{
			if (PrinterConnectionAndCommunication.Instance?.ActivePrinter?.ID != null && ActiveSliceSettings.Instance.PrinterSelected && !WizardWindow.IsOpen("PrinterSetup"))
			{
				UiThread.RunOnIdle((Action)delegate
				{
					WizardWindow.Show<EditPrinterSettingsPage>("EditSettings", "Edit Printer Settings");
				});
			}
		}

		private static void HighlightWidget(AutomationRunner testRunner, string widgetNameToHighlight)
		{
			SystemWindow val = default(SystemWindow);
			GuiWidget widgetByName = testRunner.GetWidgetByName(widgetNameToHighlight, ref val, 0.2, (SearchRegion)null);
			if (widgetByName != null)
			{
				AttentionGetter.GetAttention(widgetByName);
			}
		}
	}
}
