using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class SetupStepBaudRate : ConnectionWizardPage
	{
		private List<RadioButton> BaudRateButtonsList = new List<RadioButton>();

		private FlowLayoutWidget printerBaudRateContainer;

		private TextWidget printerBaudRateError;

		private GuiWidget baudRateWidget;

		private RadioButton otherBaudRateRadioButton;

		private MHTextEditWidget otherBaudRateInput;

		private Button nextButton;

		private Button printerBaudRateHelpLink;

		private TextWidget printerBaudRateHelpMessage;

		public SetupStepBaudRate()
		{
			printerBaudRateContainer = createPrinterBaudRateContainer();
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)printerBaudRateContainer, -1);
			nextButton = textImageButtonFactory.Generate("Continue".Localize());
			((GuiWidget)nextButton).add_Click((EventHandler<MouseEventArgs>)NextButton_Click);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)nextButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			BindBaudRateHandlers();
		}

		private FlowLayoutWidget createPrinterBaudRateContainer()
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
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Expected O, but got Unknown
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Expected O, but got Unknown
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Expected O, but got Unknown
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(0.0, 0.0, 0.0, 3.0);
			string arg = "Baud Rate".Localize();
			TextWidget val2 = new TextWidget($"{arg}:", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 10.0));
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			baudRateWidget = GetBaudRateWidget();
			baudRateWidget.set_HAnchor((HAnchor)5);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_Margin(margin);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			printerBaudRateError = new TextWidget("Select the baud rate.".Localize(), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			printerBaudRateError.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			printerBaudRateError.set_AutoExpandBoundsToText(true);
			printerBaudRateHelpLink = linkButtonFactory.Generate("What's this?".Localize());
			((GuiWidget)printerBaudRateHelpLink).set_Margin(new BorderDouble(5.0, 0.0, 0.0, 0.0));
			((GuiWidget)printerBaudRateHelpLink).set_VAnchor((VAnchor)1);
			((GuiWidget)printerBaudRateHelpLink).add_Click((EventHandler<MouseEventArgs>)printerBaudRateHelp_Click);
			printerBaudRateHelpMessage = new TextWidget("The term 'Baud Rate' roughly means the speed at which\ndata is transmitted.  Baud rates may differ from printer to\nprinter. Refer to your printer manual for more info.\n\nTip: If you are uncertain - try 250000.".Localize(), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			printerBaudRateHelpMessage.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)printerBaudRateHelpMessage).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 10.0));
			((GuiWidget)printerBaudRateHelpMessage).set_Visible(false);
			((GuiWidget)val3).AddChild((GuiWidget)(object)printerBaudRateError, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)printerBaudRateHelpLink, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild(baudRateWidget, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)printerBaudRateHelpMessage, -1);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			return val;
		}

		private void printerBaudRateHelp_Click(object sender, EventArgs mouseEvent)
		{
			((GuiWidget)printerBaudRateHelpMessage).set_Visible(!((GuiWidget)printerBaudRateHelpMessage).get_Visible());
		}

		public GuiWidget GetBaudRateWidget()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Expected O, but got Unknown
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			List<string> list = new List<string>
			{
				"115200",
				"250000"
			};
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(3.0, 3.0, 5.0, 0.0);
			foreach (string item in list)
			{
				RadioButton val2 = new RadioButton(item, 12);
				BaudRateButtonsList.Add(val2);
				((GuiWidget)val2).set_Margin(margin);
				val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				if (ActiveSliceSettings.Instance.GetValue("baud_rate") == item)
				{
					val2.set_Checked(true);
				}
				((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			}
			otherBaudRateRadioButton = new RadioButton("Other".Localize(), 12);
			((GuiWidget)otherBaudRateRadioButton).set_Margin(margin);
			otherBaudRateRadioButton.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val).AddChild((GuiWidget)(object)otherBaudRateRadioButton, -1);
			otherBaudRateInput = new MHTextEditWidget();
			((GuiWidget)otherBaudRateInput).set_Visible(false);
			((GuiWidget)otherBaudRateInput).set_HAnchor((HAnchor)5);
			string value = ActiveSliceSettings.Instance.GetValue("baud_rate");
			if (value != null && !list.Contains(value))
			{
				otherBaudRateRadioButton.set_Checked(true);
				((GuiWidget)otherBaudRateInput).set_Text(value);
				((GuiWidget)otherBaudRateInput).set_Visible(true);
			}
			((GuiWidget)val).AddChild((GuiWidget)(object)otherBaudRateInput, -1);
			return (GuiWidget)(object)val;
		}

		private void BindBaudRateHandlers()
		{
			otherBaudRateRadioButton.add_CheckedStateChanged((EventHandler)BindBaudRate_Select);
			foreach (RadioButton baudRateButtons in BaudRateButtonsList)
			{
				baudRateButtons.add_CheckedStateChanged((EventHandler)BindBaudRate_Select);
			}
			BindBaudRate_Select(null, null);
		}

		private void BindBaudRate_Select(object sender, EventArgs e)
		{
			if (otherBaudRateRadioButton.get_Checked())
			{
				((GuiWidget)otherBaudRateInput).set_Visible(true);
			}
			else
			{
				((GuiWidget)otherBaudRateInput).set_Visible(false);
			}
		}

		private void MoveToNextWidget()
		{
			WizardWindow.ChangeToInstallDriverOrComPortOne();
		}

		private void NextButton_Click(object sender, EventArgs mouseEvent)
		{
			if (OnSave())
			{
				UiThread.RunOnIdle((Action)MoveToNextWidget);
			}
		}

		private bool OnSave()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			string text = null;
			try
			{
				text = GetSelectedBaudRate();
			}
			catch
			{
				((GuiWidget)printerBaudRateHelpLink).set_Visible(false);
				printerBaudRateError.set_TextColor(RGBA_Bytes.Red);
				((GuiWidget)printerBaudRateError).set_Text("Oops! Please select a baud rate.".Localize());
			}
			if (text != null)
			{
				try
				{
					ActiveSliceSettings.Instance.Helpers.SetBaudRate(text);
					return true;
				}
				catch
				{
					((GuiWidget)printerBaudRateHelpLink).set_Visible(false);
					printerBaudRateError.set_TextColor(RGBA_Bytes.Red);
					((GuiWidget)printerBaudRateError).set_Text("Oops! Baud Rate must be an integer.".Localize());
					return false;
				}
			}
			return false;
		}

		private string GetSelectedBaudRate()
		{
			foreach (RadioButton baudRateButtons in BaudRateButtonsList)
			{
				if (baudRateButtons.get_Checked())
				{
					return ((GuiWidget)baudRateButtons).get_Text();
				}
			}
			if (otherBaudRateRadioButton.get_Checked())
			{
				return ((GuiWidget)otherBaudRateInput).get_Text();
			}
			throw new Exception("Could not find a selected button.".Localize());
		}
	}
}
