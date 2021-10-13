using System;
using System.Diagnostics;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class SlideWidget : GuiWidget
	{
		private Stopwatch timeHasBeenChanging = new Stopwatch();

		private double OriginalWidth
		{
			get;
			set;
		}

		private double TargetWidth
		{
			get;
			set;
		}

		public SlideWidget()
			: this(0.0, 0.0, (SizeLimitsToSet)1)
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown


		public override void OnDraw(Graphics2D graphics2D)
		{
			((GuiWidget)this).OnDraw(graphics2D);
			if (((GuiWidget)this).get_Width() != TargetWidth)
			{
				SetSlidePosition();
				((GuiWidget)this).Invalidate();
			}
		}

		public void SlideIn()
		{
			((GuiWidget)this).set_Visible(true);
			if (OriginalWidth == 0.0)
			{
				OriginalWidth = ((GuiWidget)this).get_Width();
			}
			TargetWidth = OriginalWidth;
			((GuiWidget)this).set_Width(0.1);
			timeHasBeenChanging.Restart();
			SetSlidePosition();
			((GuiWidget)this).Invalidate();
		}

		public void SlideOut()
		{
			((GuiWidget)this).set_Visible(true);
			if (OriginalWidth == 0.0)
			{
				OriginalWidth = ((GuiWidget)this).get_Width();
			}
			TargetWidth = 0.0;
			timeHasBeenChanging.Restart();
			SetSlidePosition();
			((GuiWidget)this).Invalidate();
		}

		private void SetSlidePosition()
		{
			if (TargetWidth == 0.0 && ((GuiWidget)this).get_Width() == 0.0)
			{
				((GuiWidget)this).set_Visible(false);
				((GuiWidget)this).set_Width(OriginalWidth);
			}
			else if (TargetWidth != ((GuiWidget)this).get_Width())
			{
				double num = timeHasBeenChanging.get_ElapsedMilliseconds();
				double width = ((GuiWidget)this).get_Width();
				double num2 = TargetWidth - width;
				num2 = ((!(num2 < 0.0)) ? Math.Min(num, num2) : Math.Max(0.0 - num, num2));
				double width2 = width + num2;
				((GuiWidget)this).set_Width(width2);
			}
		}
	}
}
