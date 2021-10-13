using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class LabelContainer : GuiWidget
	{
		private RGBA_Bytes backgroundColor = new RGBA_Bytes(0, 140, 158);

		public LabelContainer()
			: this()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Margin(new BorderDouble(2.0, -2.0, 2.0, 5.0));
			((GuiWidget)this).set_Padding(new BorderDouble(3.0));
			((GuiWidget)this).set_BackgroundColor(backgroundColor);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
