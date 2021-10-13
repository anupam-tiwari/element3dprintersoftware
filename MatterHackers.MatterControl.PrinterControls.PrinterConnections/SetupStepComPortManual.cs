using System;
using System.Collections.Generic;
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
	public class SetupStepComPortManual : ConnectionWizardPage
	{
		private Button nextButton;

		private Button connectButton;

		private Button refreshButton;

		private Button printerComPortHelpLink;

		private bool printerComPortIsAvailable;

		private TextWidget printerComPortHelpMessage;

		private TextWidget printerComPortError;

		private EventHandler unregisterEvents;

		protected List<SerialPortIndexRadioButton> SerialPortButtonsList = new List<SerialPortIndexRadioButton>();

		public SetupStepComPortManual()
		{
			FlowLayoutWidget val = createComPortContainer();
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)val, -1);
			nextButton = textImageButtonFactory.Generate("Done".Localize());
			((GuiWidget)nextButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)((GuiWidget)this).get_Parent().Close);
			});
			((GuiWidget)nextButton).set_Visible(false);
			connectButton = textImageButtonFactory.Generate("Connect".Localize());
			((GuiWidget)connectButton).add_Click((EventHandler<MouseEventArgs>)ConnectButton_Click);
			refreshButton = textImageButtonFactory.Generate("Refresh".Localize());
			((GuiWidget)refreshButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)WizardWindow.ChangeToPage<SetupStepComPortManual>);
			});
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)nextButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)connectButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)refreshButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private FlowLayoutWidget createComPortContainer()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Expected O, but got Unknown
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Expected O, but got Unknown
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Expected O, but got Unknown
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Expected O, but got Unknown
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(0.0, 0.0, 0.0, 3.0);
			string arg = "Serial Port".Localize();
			TextWidget val2 = new TextWidget($"{arg}:", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 10.0));
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			CreateSerialPortControls(val3, null);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val4).set_Margin(margin);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			printerComPortError = new TextWidget("Currently available serial ports.".Localize(), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			printerComPortError.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			printerComPortError.set_AutoExpandBoundsToText(true);
			printerComPortHelpLink = linkButtonFactory.Generate("What's this?".Localize());
			((GuiWidget)printerComPortHelpLink).set_Margin(new BorderDouble(5.0, 0.0, 0.0, 0.0));
			((GuiWidget)printerComPortHelpLink).set_VAnchor((VAnchor)1);
			((GuiWidget)printerComPortHelpLink).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)printerComPortHelpMessage).set_Visible(!((GuiWidget)printerComPortHelpMessage).get_Visible());
			});
			printerComPortHelpMessage = new TextWidget("The 'Serial Port' section lists all available serial\nports on your device. Changing which USB port the printer\nis conneted to may change the associated serial port.\n\nTip: If you are uncertain, unplug/plug in your printer\nand hit refresh. The new port that appears should be\nyour printer.".Localize(), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			printerComPortHelpMessage.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)printerComPortHelpMessage).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 10.0));
			((GuiWidget)printerComPortHelpMessage).set_Visible(false);
			((GuiWidget)val4).AddChild((GuiWidget)(object)printerComPortError, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)printerComPortHelpLink, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)printerComPortHelpMessage, -1);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			return val;
		}

		private void onPrinterStatusChanged(object sender, EventArgs e)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			if (PrinterConnectionAndCommunication.Instance.PrinterIsConnected)
			{
				((GuiWidget)printerComPortHelpLink).set_Visible(false);
				printerComPortError.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)printerComPortError).set_Text("Connection succeeded".Localize() + "!");
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
				((GuiWidget)printerComPortHelpLink).set_Visible(false);
				printerComPortError.set_TextColor(RGBA_Bytes.Red);
				((GuiWidget)printerComPortError).set_Text("Uh-oh! Could not connect to printer.".Localize());
				((GuiWidget)connectButton).set_Visible(true);
				((GuiWidget)nextButton).set_Visible(false);
			}
		}

		private void MoveToNextWidget(object state)
		{
			WizardWindow.ChangeToInstallDriverOrComPortOne();
		}

		private void ConnectButton_Click(object sender, EventArgs mouseEvent)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				((GuiWidget)printerComPortHelpLink).set_Visible(false);
				printerComPortError.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)printerComPortError).set_Text("Attempting to connect".Localize() + "...");
				printerComPortError.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				ActiveSliceSettings.Instance.Helpers.SetComPort(GetSelectedSerialPort());
				PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter();
				((GuiWidget)connectButton).set_Visible(false);
				((GuiWidget)refreshButton).set_Visible(false);
			}
			catch
			{
				((GuiWidget)printerComPortHelpLink).set_Visible(false);
				printerComPortError.set_TextColor(RGBA_Bytes.Red);
				((GuiWidget)printerComPortError).set_Text("Oops! Please select a serial port.".Localize());
			}
		}

		protected void CreateSerialPortControls(FlowLayoutWidget comPortContainer, string activePrinterSerialPort)
		{
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected O, but got Unknown
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			string[] portNames = FrostedSerialPort.GetPortNames();
			foreach (string text in portNames)
			{
				SerialPortIndexRadioButton serialPortIndexRadioButton = createComPortOption(text, activePrinterSerialPort == text);
				if (((RadioButton)serialPortIndexRadioButton).get_Checked())
				{
					printerComPortIsAvailable = true;
				}
				SerialPortButtonsList.Add(serialPortIndexRadioButton);
				((GuiWidget)comPortContainer).AddChild((GuiWidget)(object)serialPortIndexRadioButton, -1);
				num++;
			}
			if (!printerComPortIsAvailable && activePrinterSerialPort != null)
			{
				SerialPortIndexRadioButton serialPortIndexRadioButton2 = createComPortOption(activePrinterSerialPort, isActivePrinterPort: true);
				((GuiWidget)serialPortIndexRadioButton2).set_Enabled(false);
				((GuiWidget)comPortContainer).AddChild((GuiWidget)(object)serialPortIndexRadioButton2, -1);
				SerialPortButtonsList.Add(serialPortIndexRadioButton2);
				num++;
			}
			if (num == 0)
			{
				TextWidget val = new TextWidget("No COM ports available".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)val).set_Margin(new BorderDouble(3.0, 6.0, 5.0, 6.0));
				val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)comPortContainer).AddChild((GuiWidget)(object)val, -1);
			}
		}

		private SerialPortIndexRadioButton createComPortOption(string portName, bool isActivePrinterPort)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			SerialPortIndexRadioButton serialPortIndexRadioButton = new SerialPortIndexRadioButton(portName, portName);
			((GuiWidget)serialPortIndexRadioButton).set_HAnchor((HAnchor)1);
			((GuiWidget)serialPortIndexRadioButton).set_Margin(new BorderDouble(3.0, 3.0, 5.0, 3.0));
			((RadioButton)serialPortIndexRadioButton).set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((RadioButton)serialPortIndexRadioButton).set_Checked(isActivePrinterPort);
			return serialPortIndexRadioButton;
		}

		private string GetSelectedSerialPort()
		{
			foreach (SerialPortIndexRadioButton serialPortButtons in SerialPortButtonsList)
			{
				if (((RadioButton)serialPortButtons).get_Checked())
				{
					return serialPortButtons.PortValue;
				}
			}
			throw new Exception("Could not find a selected button.".Localize());
		}
	}
}
