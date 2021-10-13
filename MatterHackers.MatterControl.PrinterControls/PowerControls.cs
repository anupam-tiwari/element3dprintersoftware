using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class PowerControls : ControlWidgetBase
	{
		private EventHandler unregisterEvents;

		private CheckBox atxPowertoggleSwitch;

		public PowerControls()
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
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Expected O, but got Unknown
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			AltGroupBox altGroupBox = new AltGroupBox((GuiWidget)new TextWidget("ATX Power Control".Localize(), 0.0, 0.0, 18.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null));
			((GuiWidget)altGroupBox).set_Margin(new BorderDouble(0.0));
			altGroupBox.BorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)altGroupBox).set_HAnchor((HAnchor)(((GuiWidget)altGroupBox).get_HAnchor() | 5));
			((GuiWidget)this).AddChild((GuiWidget)(object)altGroupBox, -1);
			atxPowertoggleSwitch = ImageButtonFactory.CreateToggleSwitch(initialState: false);
			((GuiWidget)atxPowertoggleSwitch).set_Margin(new BorderDouble(6.0, 0.0, 6.0, 6.0));
			atxPowertoggleSwitch.add_CheckedStateChanged((EventHandler)delegate
			{
				PrinterConnectionAndCommunication.Instance.AtxPowerEnabled = atxPowertoggleSwitch.get_Checked();
			});
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Padding(new BorderDouble(3.0, 5.0, 3.0, 0.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)atxPowertoggleSwitch, -1);
			((GuiWidget)altGroupBox).AddChild((GuiWidget)(object)val, -1);
			UpdateControlVisibility(null, null);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)UpdateControlVisibility, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.AtxPowerStateChanged.RegisterEvent((EventHandler)UpdatePowerSwitch, ref unregisterEvents);
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_VAnchor((VAnchor)5);
		}

		private void UpdateControlVisibility(object sender, EventArgs args)
		{
			((GuiWidget)this).set_Visible(ActiveSliceSettings.Instance.GetValue<bool>("has_power_control"));
			SetEnableLevel(PrinterConnectionAndCommunication.Instance.PrinterIsConnected ? EnableLevel.Enabled : EnableLevel.Disabled);
		}

		private void UpdatePowerSwitch(object sender, EventArgs args)
		{
			atxPowertoggleSwitch.set_Checked(PrinterConnectionAndCommunication.Instance.AtxPowerEnabled);
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
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}
	}
}
