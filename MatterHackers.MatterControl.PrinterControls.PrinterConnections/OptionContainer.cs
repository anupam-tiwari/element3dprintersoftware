using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class OptionContainer : GuiWidget
	{
		private RGBA_Bytes borderColor = new RGBA_Bytes(63, 63, 70);

		private RGBA_Bytes backgroundColor = ActiveTheme.get_Instance().get_PrimaryBackgroundColor();

		public OptionContainer()
			: this()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Margin(new BorderDouble(2.0, 5.0, 2.0, 0.0));
			((GuiWidget)this).set_BackgroundColor(backgroundColor);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnDraw(graphics2D);
			graphics2D.Rectangle(((GuiWidget)this).get_LocalBounds(), borderColor, 1.0);
		}
	}
}
