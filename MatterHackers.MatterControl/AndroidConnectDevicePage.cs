using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl
{
	public class AndroidConnectDevicePage : WizardPage
	{
		private EventHandler unregisterEvents;

		private TextWidget generalError;

		private Button connectButton;

		private Button skipButton;

		private Button nextButton;

		private Button retryButton;

		private Button troubleshootButton;

		private TextWidget skipMessage;

		private FlowLayoutWidget retryButtonContainer;

		private FlowLayoutWidget connectButtonContainer;

		public AndroidConnectDevicePage()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Expected O, but got Unknown
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Expected O, but got Unknown
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Expected O, but got Unknown
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Expected O, but got Unknown
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Expected O, but got Unknown
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Expected O, but got Unknown
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Expected O, but got Unknown
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Expected O, but got Unknown
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Expected O, but got Unknown
			TextWidget val = new TextWidget("Connect Your Device".Localize() + ":", 0.0, 0.0, labelFontSize, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 10.0, 0.0, 0.0));
			TextWidget val2 = val;
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)new TextWidget("Instructions:".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)new TextWidget("1. Power on your 3D Printer.".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)new TextWidget("2. Attach your 3D Printer via USB.".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)new TextWidget("3. Press 'Connect'.".Localize(), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null), -1);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)communicationStateChanged, ref unregisterEvents);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0));
			connectButtonContainer = val3;
			connectButton = whiteImageButtonFactory.Generate("Connect".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)connectButton).set_Margin(new BorderDouble(0.0, 0.0, 10.0, 0.0));
			((GuiWidget)connectButton).add_Click((EventHandler<MouseEventArgs>)ConnectButton_Click);
			skipButton = whiteImageButtonFactory.Generate("Skip".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)skipButton).add_Click((EventHandler<MouseEventArgs>)NextButton_Click);
			((GuiWidget)connectButtonContainer).AddChild((GuiWidget)(object)connectButton, -1);
			((GuiWidget)connectButtonContainer).AddChild((GuiWidget)(object)skipButton, -1);
			((GuiWidget)connectButtonContainer).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)connectButtonContainer, -1);
			skipMessage = new TextWidget("(Press 'Skip' to setup connection later)".Localize(), 0.0, 0.0, 10.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)skipMessage, -1);
			TextWidget val4 = new TextWidget("", 0.0, 0.0, errorFontSize, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_TextColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).set_Visible(false);
			((GuiWidget)val4).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 20.0));
			generalError = val4;
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)generalError, -1);
			retryButton = whiteImageButtonFactory.Generate("Retry".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)retryButton).add_Click((EventHandler<MouseEventArgs>)ConnectButton_Click);
			((GuiWidget)retryButton).set_Margin(new BorderDouble(0.0, 0.0, 10.0, 0.0));
			troubleshootButton = whiteImageButtonFactory.Generate("Troubleshoot".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)troubleshootButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)WizardWindow.ChangeToPage<SetupWizardTroubleshooting>);
			});
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val5).set_HAnchor((HAnchor)5);
			((GuiWidget)val5).set_Margin(new BorderDouble(0.0, 6.0));
			((GuiWidget)val5).set_Visible(false);
			retryButtonContainer = val5;
			((GuiWidget)retryButtonContainer).AddChild((GuiWidget)(object)retryButton, -1);
			((GuiWidget)retryButtonContainer).AddChild((GuiWidget)(object)troubleshootButton, -1);
			((GuiWidget)retryButtonContainer).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)retryButtonContainer, -1);
			nextButton = textImageButtonFactory.Generate("Continue".Localize());
			((GuiWidget)nextButton).add_Click((EventHandler<MouseEventArgs>)NextButton_Click);
			((GuiWidget)nextButton).set_Visible(false);
			GuiWidget val6 = new GuiWidget();
			val6.set_HAnchor((HAnchor)5);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)nextButton, -1);
			((GuiWidget)footerRow).AddChild(val6, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			updateControls(firstLoad: true);
		}

		private void ConnectButton_Click(object sender, EventArgs mouseEvent)
		{
			PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter(showHelpIfNoPort: true);
		}

		private void NextButton_Click(object sender, EventArgs mouseEvent)
		{
			((GuiWidget)generalError).set_Text("Please wait...");
			((GuiWidget)generalError).set_Visible(true);
			((GuiWidget)nextButton).set_Visible(false);
			UiThread.RunOnIdle((Action)((GuiWidget)WizardWindow).Close);
		}

		private void communicationStateChanged(object sender, EventArgs args)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				updateControls(firstLoad: false);
			});
		}

		private void updateControls(bool firstLoad)
		{
			((GuiWidget)connectButton).set_Visible(false);
			((GuiWidget)skipMessage).set_Visible(false);
			((GuiWidget)generalError).set_Visible(false);
			((GuiWidget)nextButton).set_Visible(false);
			((GuiWidget)connectButtonContainer).set_Visible(false);
			((GuiWidget)retryButtonContainer).set_Visible(false);
			if (PrinterConnectionAndCommunication.Instance.PrinterIsConnected)
			{
				((GuiWidget)generalError).set_Text(StringHelper.FormatWith("{0}!", new object[1]
				{
					"Connection succeeded".Localize()
				}));
				((GuiWidget)generalError).set_Visible(true);
				((GuiWidget)nextButton).set_Visible(true);
			}
			else if (firstLoad || PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.Disconnected)
			{
				((GuiWidget)generalError).set_Text("");
				((GuiWidget)connectButton).set_Visible(true);
				((GuiWidget)connectButtonContainer).set_Visible(true);
			}
			else if (PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect)
			{
				((GuiWidget)generalError).set_Text(StringHelper.FormatWith("{0}...", new object[1]
				{
					"Attempting to connect".Localize()
				}));
				((GuiWidget)generalError).set_Visible(true);
			}
			else
			{
				((GuiWidget)generalError).set_Text("Uh-oh! Could not connect to printer.".Localize());
				((GuiWidget)generalError).set_Visible(true);
				((GuiWidget)nextButton).set_Visible(false);
				((GuiWidget)retryButtonContainer).set_Visible(true);
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
