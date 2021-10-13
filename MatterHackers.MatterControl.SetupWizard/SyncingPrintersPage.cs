using System;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.SetupWizard
{
	public class SyncingPrintersPage : WizardPage
	{
		private TextWidget syncingDetails;

		public SyncingPrintersPage()
			: base("Close")
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected O, but got Unknown
			TextWidget val = new TextWidget("Syncing Profiles...".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			syncingDetails = new TextWidget("Retrieving sync information...".Localize(), 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			syncingDetails.set_AutoExpandBoundsToText(true);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)syncingDetails, -1);
			Progress<SyncReportType> arg = new Progress<SyncReportType>(new Action<SyncReportType>(ReportProgress));
			ApplicationController.SyncPrinterProfiles("SyncingPrintersPage.ctor()", arg).ContinueWith(delegate
			{
				if (!Enumerable.Any<PrinterInfo>(ProfileManager.Instance.ActiveProfiles))
				{
					WizardWindow.ChangeToSetupPrinterForm();
				}
				else if (Enumerable.Count<PrinterInfo>(ProfileManager.Instance.ActiveProfiles) == 1)
				{
					ActiveSliceSettings.ShowComPortConnectionHelp();
					ActiveSliceSettings.SwitchToProfile(Enumerable.First<PrinterInfo>(ProfileManager.Instance.ActiveProfiles).ID);
					UiThread.RunOnIdle((Action)((GuiWidget)WizardWindow).Close);
				}
				else
				{
					UiThread.RunOnIdle((Action)((GuiWidget)WizardWindow).Close);
				}
			});
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
		}

		private void ReportProgress(SyncReportType report)
		{
			((GuiWidget)syncingDetails).set_Text(report.actionLabel);
		}
	}
}
