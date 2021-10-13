using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl
{
	public class SetupPrinterView : SetupViewBase
	{
		private Button disconnectButton;

		private TextWidget connectionStatus;

		private EventHandler unregisterEvents;

		internal WizardPage WizardPage
		{
			get;
			set;
		}

		public SetupPrinterView(TextImageButtonFactory textImageButtonFactory)
			: base("Printer Profile")
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Expected O, but got Unknown
			base.textImageButtonFactory = textImageButtonFactory;
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 14.0));
			FlowLayoutWidget val2 = val;
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)val2, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val4 = val3;
			((GuiWidget)val2).AddChild((GuiWidget)(object)val4, -1);
			PrinterSelector printerSelector = new PrinterSelector();
			printerSelector.AddPrinter += delegate
			{
				WizardPage.WizardWindow.ChangeToSetupPrinterForm(userRequestedNewPrinter: true);
			};
			((GuiWidget)val4).AddChild((GuiWidget)(object)printerSelector, -1);
			Button themedEditButton = TextImageButtonFactory.GetThemedEditButton();
			((GuiWidget)themedEditButton).set_ToolTipText("Edit Selected Setting".Localize());
			((GuiWidget)themedEditButton).set_VAnchor((VAnchor)2);
			((GuiWidget)themedEditButton).add_Click((EventHandler<MouseEventArgs>)UiNavigation.OpenEditPrinterWizard_Click);
			((GuiWidget)val4).AddChild((GuiWidget)(object)themedEditButton, -1);
			disconnectButton = textImageButtonFactory.Generate("Disconnect");
			((GuiWidget)disconnectButton).set_Margin(new BorderDouble(12.0, 0.0, 0.0, 0.0));
			((GuiWidget)disconnectButton).set_VAnchor((VAnchor)2);
			((GuiWidget)disconnectButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				PrinterConnectionAndCommunication.Instance.Disable();
				UiThread.RunOnIdle((Action)WizardPage.WizardWindow.ChangeToPage<SetupOptionsPage>);
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)disconnectButton, -1);
			TextWidget val5 = new TextWidget("Status:", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val5).set_HAnchor((HAnchor)5);
			connectionStatus = val5;
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)connectionStatus, -1);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)updateConnectedState, ref unregisterEvents);
			updateConnectedState(null, null);
		}

		private void updateConnectedState(object sender, EventArgs e)
		{
			if (disconnectButton != null)
			{
				((GuiWidget)disconnectButton).set_Visible(PrinterConnectionAndCommunication.Instance.PrinterIsConnected);
			}
			if (connectionStatus != null)
			{
				((GuiWidget)connectionStatus).set_Text(string.Format("{0}: {1}", "Status".Localize().ToUpper(), PrinterConnectionAndCommunication.Instance.PrinterConnectionStatusVerbose));
			}
			((GuiWidget)this).Invalidate();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
