using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class FanControls : ControlWidgetBase
	{
		private EventHandler unregisterEvents;

		private EditableNumberDisplay fanSpeedDisplay;

		private CheckBox toggleSwitch;

		private bool doingDisplayUpdateFromPrinter;

		public FanControls()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Expected O, but got Unknown
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Expected O, but got Unknown
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			AltGroupBox altGroupBox = new AltGroupBox((GuiWidget)new TextWidget("Fan".Localize(), 0.0, 0.0, 18.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null));
			((GuiWidget)altGroupBox).set_Margin(new BorderDouble(0.0));
			altGroupBox.BorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)altGroupBox).set_HAnchor((HAnchor)(((GuiWidget)altGroupBox).get_HAnchor() | 5));
			((GuiWidget)altGroupBox).set_VAnchor((VAnchor)8);
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).AddChild((GuiWidget)(object)altGroupBox, -1);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val2).set_Padding(new BorderDouble(3.0, 5.0, 3.0, 0.0));
			((GuiWidget)val2).AddChild(CreateFanControls(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			SetDisplayAttributes();
			fanSpeedDisplay = new EditableNumberDisplay(textImageButtonFactory, StringHelper.FormatWith("{0}%", new object[1]
			{
				PrinterConnectionAndCommunication.Instance.FanSpeed0To255.ToString()
			}), "100%");
			fanSpeedDisplay.EditComplete += delegate
			{
				PrinterConnectionAndCommunication.Instance.FanSpeed0To255 = (int)(fanSpeedDisplay.GetValue() * 255.5 / 100.0);
			};
			((GuiWidget)val).AddChild((GuiWidget)(object)fanSpeedDisplay, -1);
			((GuiWidget)altGroupBox).AddChild((GuiWidget)(object)val, -1);
		}

		private void SetDisplayAttributes()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			textImageButtonFactory.normalFillColor = RGBA_Bytes.Transparent;
			textImageButtonFactory.FixedWidth = 38.0 * GuiWidget.get_DeviceScale();
			textImageButtonFactory.FixedHeight = 20.0 * GuiWidget.get_DeviceScale();
			textImageButtonFactory.fontSize = 10.0;
			textImageButtonFactory.borderWidth = 1.0;
			textImageButtonFactory.normalBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.hoverBorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			textImageButtonFactory.disabledTextColor = RGBA_Bytes.Gray;
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_SecondaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)this).set_HAnchor((HAnchor)5);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		private GuiWidget CreateFanControls()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Expected O, but got Unknown
			PrinterConnectionAndCommunication.Instance.FanSpeedSet.RegisterEvent((EventHandler)FanSpeedChanged_Event, ref unregisterEvents);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0, 0.0, 5.0));
			bool initialState = PrinterConnectionAndCommunication.Instance.FanSpeed0To255 != 0;
			toggleSwitch = ImageButtonFactory.CreateToggleSwitch(initialState);
			((GuiWidget)toggleSwitch).set_VAnchor((VAnchor)2);
			toggleSwitch.add_CheckedStateChanged((EventHandler)ToggleSwitch_Click);
			((GuiWidget)toggleSwitch).set_Margin(new BorderDouble(5.0, 0.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)toggleSwitch, -1);
			return (GuiWidget)val;
		}

		private void FanSpeedChanged_Event(object sender, EventArgs e)
		{
			int fanSpeed0To = PrinterConnectionAndCommunication.Instance.FanSpeed0To255;
			fanSpeedDisplay.SetDisplayString(StringHelper.FormatWith("{0}%", new object[1]
			{
				(int)((double)fanSpeed0To * 100.5 / 255.0)
			}));
			doingDisplayUpdateFromPrinter = true;
			if (fanSpeed0To > 0)
			{
				toggleSwitch.set_Checked(true);
			}
			else
			{
				toggleSwitch.set_Checked(false);
			}
			doingDisplayUpdateFromPrinter = false;
		}

		private void ToggleSwitch_Click(object sender, EventArgs e)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if (!doingDisplayUpdateFromPrinter)
			{
				if (((CheckBox)sender).get_Checked())
				{
					PrinterConnectionAndCommunication.Instance.FanSpeed0To255 = 255;
				}
				else
				{
					PrinterConnectionAndCommunication.Instance.FanSpeed0To255 = 0;
				}
			}
		}
	}
}
