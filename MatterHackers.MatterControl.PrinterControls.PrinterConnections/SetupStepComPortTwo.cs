using System;
using System.Collections.Generic;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.FrostedSerial;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class SetupStepComPortTwo : ConnectionWizardPage
	{
		private string[] startingPortNames;

		private string[] currentPortNames;

		private Button nextButton;

		private Button connectButton;

		private TextWidget printerErrorMessage;

		private EventHandler unregisterEvents;

		public SetupStepComPortTwo()
		{
			startingPortNames = FrostedSerialPort.GetPortNames();
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)createPrinterConnectionMessageContainer(), -1);
			nextButton = textImageButtonFactory.Generate("Done".Localize());
			((GuiWidget)nextButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)((GuiWidget)this).get_Parent().Close);
			});
			((GuiWidget)nextButton).set_Visible(false);
			connectButton = textImageButtonFactory.Generate("Connect".Localize());
			((GuiWidget)connectButton).add_Click((EventHandler<MouseEventArgs>)ConnectButton_Click);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)nextButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)connectButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		public FlowLayoutWidget createPrinterConnectionMessageContainer()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Expected O, but got Unknown
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Expected O, but got Unknown
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Expected O, but got Unknown
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(5.0));
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(0.0, 0.0, 0.0, 5.0);
			TextWidget val2 = new TextWidget("Element will now attempt to auto-detect printer.".Localize(), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 10.0, 0.0, 5.0));
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Margin(margin);
			string arg = "Connect printer and power on".Localize();
			TextWidget val3 = new TextWidget($"1.) {arg}.", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Margin(margin);
			string arg2 = "Press".Localize();
			string arg3 = "Connect".Localize();
			TextWidget val4 = new TextWidget($"2.) {arg2} '{arg3}'.", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).set_Margin(margin);
			GuiWidget val5 = new GuiWidget();
			val5.set_VAnchor((VAnchor)5);
			Button val6 = linkButtonFactory.Generate("Manual Configuration".Localize());
			((GuiWidget)val6).set_Margin(new BorderDouble(0.0, 5.0));
			((GuiWidget)val6).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)WizardWindow.ChangeToPage<SetupStepComPortManual>);
			});
			printerErrorMessage = new TextWidget("", 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			printerErrorMessage.set_AutoExpandBoundsToText(true);
			printerErrorMessage.set_TextColor(RGBA_Bytes.Red);
			((GuiWidget)printerErrorMessage).set_HAnchor((HAnchor)5);
			((GuiWidget)printerErrorMessage).set_Margin(margin);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)printerErrorMessage, -1);
			((GuiWidget)val).AddChild(val5, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val6, -1);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			return val;
		}

		private void ConnectButton_Click(object sender, EventArgs mouseEvent)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			string text = Enumerable.FirstOrDefault<string>(Enumerable.Except<string>((IEnumerable<string>)FrostedSerialPort.GetPortNames(), (IEnumerable<string>)startingPortNames));
			if (text == null)
			{
				printerErrorMessage.set_TextColor(RGBA_Bytes.Red);
				((GuiWidget)printerErrorMessage).set_Text("Oops! Printer could not be detected ".Localize());
				return;
			}
			printerErrorMessage.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)printerErrorMessage).set_Text("Attempting to connect".Localize() + "...");
			ActiveSliceSettings.Instance.Helpers.SetComPort(text);
			PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter();
			((GuiWidget)connectButton).set_Visible(false);
		}

		private void onPrinterStatusChanged(object sender, EventArgs e)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			if (PrinterConnectionAndCommunication.Instance.PrinterIsConnected)
			{
				printerErrorMessage.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)printerErrorMessage).set_Text("Connection succeeded".Localize() + "!");
				((GuiWidget)nextButton).set_Visible(true);
				((GuiWidget)connectButton).set_Visible(false);
				UiThread.RunOnIdle((Action)delegate
				{
					if (this != null)
					{
						GuiWidget parent = ((GuiWidget)this).get_Parent();
						if (parent != null)
						{
							parent.Close();
						}
					}
				});
			}
			else if (PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect)
			{
				printerErrorMessage.set_TextColor(RGBA_Bytes.Red);
				((GuiWidget)printerErrorMessage).set_Text("Uh-oh! Could not connect to printer.".Localize());
				((GuiWidget)connectButton).set_Visible(true);
				((GuiWidget)nextButton).set_Visible(false);
			}
		}
	}
}
