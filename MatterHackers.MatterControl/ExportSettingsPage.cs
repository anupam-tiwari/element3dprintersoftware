using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class ExportSettingsPage : WizardPage
	{
		private RadioButton matterControlButton;

		private RadioButton slic3rButton;

		private RadioButton curaButton;

		public ExportSettingsPage()
			: base("Cancel", "Export As")
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val2 = val;
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)val2, -1);
			matterControlButton = new RadioButton("MatterControl".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12);
			matterControlButton.set_Checked(true);
			((GuiWidget)val2).AddChild((GuiWidget)(object)matterControlButton, -1);
			slic3rButton = new RadioButton("Slic3r".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12);
			((GuiWidget)val2).AddChild((GuiWidget)(object)slic3rButton, -1);
			Button val3 = textImageButtonFactory.Generate("Export Settings".Localize());
			((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)exportButton_Click);
			});
			((GuiWidget)val3).set_Visible(true);
			((GuiWidget)cancelButton).set_Visible(true);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)val3, -1);
		}

		private GuiWidget CreateDetailInfo(string detailText)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			WrappedTextWidget val = new WrappedTextWidget(detailText, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), true);
			val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			WrappedTextWidget val2 = val;
			GuiWidget val3 = new GuiWidget();
			val3.set_HAnchor((HAnchor)5);
			val3.set_VAnchor((VAnchor)8);
			val3.set_Margin(new BorderDouble(25.0, 15.0, 5.0, 5.0));
			val3.AddChild((GuiWidget)(object)val2, -1);
			return val3;
		}

		private void exportButton_Click()
		{
			((GuiWidget)WizardWindow).Close();
			if (matterControlButton.get_Checked())
			{
				ActiveSliceSettings.Instance.Helpers.ExportAsMatterControlConfig();
			}
			else if (slic3rButton.get_Checked())
			{
				ActiveSliceSettings.Instance.Helpers.ExportAsSlic3rConfig();
			}
			else if (curaButton.get_Checked())
			{
				ActiveSliceSettings.Instance.Helpers.ExportAsCuraConfig();
			}
		}
	}
}
