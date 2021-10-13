using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class UpdateNotificationMark : GuiWidget
	{
		public UpdateNotificationMark()
			: this(12.0, 12.0, (SizeLimitsToSet)1)
		{
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			graphics2D.Circle(((GuiWidget)this).get_Width() / 2.0, ((GuiWidget)this).get_Height() / 2.0, ((GuiWidget)this).get_Width() / 2.0, RGBA_Bytes.White);
			graphics2D.Circle(((GuiWidget)this).get_Width() / 2.0, ((GuiWidget)this).get_Height() / 2.0, ((GuiWidget)this).get_Width() / 2.0 - 1.0, RGBA_Bytes.Red);
			graphics2D.FillRectangle(((GuiWidget)this).get_Width() / 2.0 - 1.0, ((GuiWidget)this).get_Height() / 2.0 - 3.0, ((GuiWidget)this).get_Width() / 2.0 + 1.0, ((GuiWidget)this).get_Height() / 2.0 + 3.0, (IColorType)(object)RGBA_Bytes.White);
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
