using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class TextScrollBar : GuiWidget
	{
		private TextScrollWidget textScrollWidget;

		private bool downOnBar;

		public TextScrollBar(TextScrollWidget textScrollWidget, int width)
			: this((double)width, ScrollBar.ScrollBarWidth, (SizeLimitsToSet)1)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			this.textScrollWidget = textScrollWidget;
			((GuiWidget)this).set_Margin(new BorderDouble(0.0, 5.0));
			((GuiWidget)this).set_VAnchor((VAnchor)5);
			((GuiWidget)this).set_BackgroundColor(RGBA_Bytes.LightGray);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			int num = 10;
			double num2 = textScrollWidget.Position0To1 * (((GuiWidget)this).get_Height() - (double)num);
			RectangleDouble val = default(RectangleDouble);
			((RectangleDouble)(ref val))._002Ector(0.0, num2, ((GuiWidget)this).get_Width(), num2 + (double)num);
			graphics2D.FillRectangle(val, (IColorType)(object)RGBA_Bytes.DarkGray);
			((GuiWidget)this).OnDraw(graphics2D);
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			downOnBar = true;
			textScrollWidget.Position0To1 = mouseEvent.get_Y() / ((GuiWidget)this).get_Height();
			((GuiWidget)this).OnMouseDown(mouseEvent);
		}

		public override void OnMouseMove(MouseEventArgs mouseEvent)
		{
			if (downOnBar)
			{
				textScrollWidget.Position0To1 = mouseEvent.get_Y() / ((GuiWidget)this).get_Height();
			}
			((GuiWidget)this).OnMouseMove(mouseEvent);
		}

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			downOnBar = false;
			((GuiWidget)this).OnMouseUp(mouseEvent);
		}

		public override void OnMouseWheel(MouseEventArgs mouseEvent)
		{
			double num = (double)mouseEvent.get_WheelDelta() / ((GuiWidget)this).get_Height() + textScrollWidget.Position0To1;
			if (num > 1.0)
			{
				num = 1.0;
			}
			else if (num < 0.0)
			{
				num = 0.0;
			}
			textScrollWidget.Position0To1 = num;
			((GuiWidget)this).OnMouseWheel(mouseEvent);
		}
	}
}
