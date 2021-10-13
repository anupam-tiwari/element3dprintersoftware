using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;

namespace MatterHackers.MatterControl
{
	public class ClickWidget : GuiWidget
	{
		private int borderWidth;

		private RGBA_Bytes borderColor = RGBA_Bytes.Black;

		public bool GetChildClicks;

		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				borderWidth = value;
				((GuiWidget)this).Invalidate();
			}
		}

		public RGBA_Bytes BorderColor
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return borderColor;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				borderColor = value;
				((GuiWidget)this).Invalidate();
			}
		}

		public event EventHandler Click;

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			if (((GuiWidget)this).PositionWithinLocalBounds(mouseEvent.get_X(), mouseEvent.get_Y()) && this.Click != null && (GetChildClicks || ((GuiWidget)this).get_MouseCaptured()))
			{
				UiThread.RunOnIdle((Action)delegate
				{
					this.Click(this, (EventArgs)(object)mouseEvent);
				});
			}
			((GuiWidget)this).OnMouseUp(mouseEvent);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			if (BorderWidth > 0)
			{
				if (BorderWidth == 1)
				{
					graphics2D.Rectangle(localBounds, BorderColor, 1.0);
				}
				else
				{
					RoundedRect val = new RoundedRect(localBounds, 0.0);
					graphics2D.Render((IVertexSource)new Stroke((IVertexSource)(object)val, (double)BorderWidth), (IColorType)(object)BorderColor);
				}
			}
			((GuiWidget)this).OnDraw(graphics2D);
		}

		public ClickWidget()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)

	}
}
