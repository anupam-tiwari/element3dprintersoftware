using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class NoSettingsWidget : FlowLayoutWidget
	{
		public NoSettingsWidget()
			: this((FlowDirection)3)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Expected O, but got Unknown
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).AnchorAll();
			((GuiWidget)this).set_Padding(new BorderDouble(3.0, 0.0));
			AltGroupBox altGroupBox = new AltGroupBox((GuiWidget)new TextWidget("No Printer Selected".Localize(), 0.0, 0.0, 18.0, (Justification)0, ActiveTheme.get_Instance().get_SecondaryAccentColor(), true, false, default(RGBA_Bytes), (TypeFace)null));
			((GuiWidget)altGroupBox).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 10.0));
			altGroupBox.BorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)altGroupBox).set_HAnchor((HAnchor)5);
			((GuiWidget)altGroupBox).set_Height(90.0);
			TextWidget val = new TextWidget("No printer is currently selected. Please select a printer to edit slice settings.".Localize() + "\n\n" + "NOTE: You need to select a printer, but do not need to connect to it.".Localize(), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).set_Margin(new BorderDouble(5.0));
			val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val).set_VAnchor((VAnchor)2);
			((GuiWidget)altGroupBox).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)altGroupBox, -1);
		}
	}
}
