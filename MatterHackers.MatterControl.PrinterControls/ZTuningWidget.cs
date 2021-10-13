using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class ZTuningWidget : GuiWidget
	{
		private TextWidget zOffsetStreamDisplay;

		private Button clearZOffsetButton;

		private FlowLayoutWidget zOffsetStreamContainer;

		private EventHandler unregisterEvents;

		private bool allowRemoveButton;

		public ZTuningWidget(bool allowRemoveButton = true)
			: this()
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Expected O, but got Unknown
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Expected O, but got Unknown
			//IL_0194: Expected O, but got Unknown
			//IL_0194: Expected O, but got Unknown
			//IL_0194: Expected O, but got Unknown
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Expected O, but got Unknown
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Expected O, but got Unknown
			this.allowRemoveButton = allowRemoveButton;
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_VAnchor((VAnchor)10);
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)delegate(object s, EventArgs e)
			{
				object obj = (object)(e as StringEventArgs);
				if (((obj != null) ? ((StringEventArgs)obj).get_Data() : null) == "baby_step_z_offset")
				{
					OffsetStreamChanged(null, null);
				}
			}, ref unregisterEvents);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Margin(new BorderDouble(3.0, 0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(3.0));
			((GuiWidget)val).set_HAnchor((HAnchor)8);
			((GuiWidget)val).set_VAnchor((VAnchor)2);
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)val).set_Height(20.0);
			zOffsetStreamContainer = val;
			((GuiWidget)this).AddChild((GuiWidget)(object)zOffsetStreamContainer, -1);
			double value = ActiveSliceSettings.Instance.GetValue<double>("baby_step_z_offset");
			TextWidget val2 = new TextWidget(value.ToString("0.##"), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_Margin(new BorderDouble(5.0, 0.0, 8.0, 0.0));
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			zOffsetStreamDisplay = val2;
			((GuiWidget)zOffsetStreamContainer).AddChild((GuiWidget)(object)zOffsetStreamDisplay, -1);
			Button val3 = new Button((GuiWidget)new ButtonViewStates((GuiWidget)new ImageWidget(SliceSettingsWidget.restoreNormal), (GuiWidget)new ImageWidget(SliceSettingsWidget.restoreHover), (GuiWidget)new ImageWidget(SliceSettingsWidget.restorePressed), (GuiWidget)new ImageWidget(SliceSettingsWidget.restoreNormal)));
			((GuiWidget)val3).set_Name("Clear ZOffset button");
			((GuiWidget)val3).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
			((GuiWidget)val3).set_ToolTipText("Clear ZOffset".Localize());
			((GuiWidget)val3).set_Visible(allowRemoveButton && value != 0.0);
			clearZOffsetButton = val3;
			((GuiWidget)clearZOffsetButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				ActiveSliceSettings.Instance.SetValue("baby_step_z_offset", "0");
			});
			((GuiWidget)zOffsetStreamContainer).AddChild((GuiWidget)(object)clearZOffsetButton, -1);
		}

		internal void OffsetStreamChanged(object sender, EventArgs e)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			double value = ActiveSliceSettings.Instance.GetValue<double>("baby_step_z_offset");
			bool flag = value != 0.0;
			((GuiWidget)zOffsetStreamContainer).set_BackgroundColor((allowRemoveButton && flag) ? SliceSettingsWidget.userSettingBackgroundColor : ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)clearZOffsetButton).set_Visible(allowRemoveButton && flag);
			((GuiWidget)zOffsetStreamDisplay).set_Text(value.ToString("0.##"));
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(null, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
