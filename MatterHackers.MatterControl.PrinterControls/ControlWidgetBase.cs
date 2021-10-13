using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.CustomWidgets;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class ControlWidgetBase : DisableableWidget
	{
		protected TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private readonly double TallButtonHeight = 25.0 * GuiWidget.get_DeviceScale();

		public ControlWidgetBase()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			textImageButtonFactory.normalFillColor = RGBA_Bytes.White;
			textImageButtonFactory.disabledFillColor = RGBA_Bytes.White;
			textImageButtonFactory.FixedHeight = TallButtonHeight;
			textImageButtonFactory.fontSize = 11.0;
			textImageButtonFactory.disabledTextColor = RGBA_Bytes.DarkGray;
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.normalTextColor = RGBA_Bytes.Black;
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
		}

		protected static GuiWidget CreateSeparatorLine()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			GuiWidget val = new GuiWidget(10.0 * GuiWidget.get_DeviceScale(), 1.0 * GuiWidget.get_DeviceScale(), (SizeLimitsToSet)1);
			val.set_Margin(new BorderDouble(0.0, 5.0));
			val.set_HAnchor((HAnchor)5);
			val.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			return val;
		}
	}
}
