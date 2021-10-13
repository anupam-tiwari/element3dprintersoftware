using System;
using System.Collections.Generic;
using System.Diagnostics;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class SlidePanel : GuiWidget
	{
		public List<GuiWidget> panels = new List<GuiWidget>();

		private int currentPanelIndex = -1;

		private int desiredPanelIndex;

		private Stopwatch timeHasBeenChanging = new Stopwatch();

		public int PanelIndex
		{
			get
			{
				return currentPanelIndex;
			}
			set
			{
				if (currentPanelIndex != value)
				{
					desiredPanelIndex = value;
					timeHasBeenChanging.Restart();
					SetSlidePosition();
				}
			}
		}

		public SlidePanel(int count)
			: this()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			((GuiWidget)this).AnchorAll();
			for (int i = 0; i < count; i++)
			{
				GuiWidget val = (GuiWidget)new FlowLayoutWidget((FlowDirection)3);
				panels.Add(val);
				((GuiWidget)this).AddChild(val, -1);
			}
		}

		public void SetPanelIndexImmediate(int index)
		{
			desiredPanelIndex = index;
			SetSlidePosition();
		}

		public GuiWidget GetPanel(int index)
		{
			return panels[index];
		}

		public override void OnBoundsChanged(EventArgs e)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < panels.Count; i++)
			{
				panels[i].set_LocalBounds(((GuiWidget)this).get_LocalBounds());
			}
			SetSlidePosition();
			((GuiWidget)this).OnBoundsChanged(e);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			((GuiWidget)this).OnDraw(graphics2D);
			if (currentPanelIndex != desiredPanelIndex)
			{
				SetSlidePosition();
				((GuiWidget)this).Invalidate();
			}
		}

		private void SetSlidePosition()
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			if (currentPanelIndex != desiredPanelIndex)
			{
				double num = timeHasBeenChanging.get_ElapsedMilliseconds();
				double num2 = (double)desiredPanelIndex * (0.0 - ((GuiWidget)this).get_Width());
				double x = panels[0].get_OriginRelativeParent().x;
				double num3 = num2 - x;
				num3 = ((!(num3 < 0.0)) ? Math.Min(num, num3) : Math.Max(0.0 - num, num3));
				double num4 = x + num3;
				for (int i = 0; i < panels.Count; i++)
				{
					panels[i].set_OriginRelativeParent(new Vector2(num4, 0.0));
					num4 += ((GuiWidget)this).get_Width();
				}
				if (x + num3 == num2)
				{
					currentPanelIndex = desiredPanelIndex;
				}
			}
			else
			{
				double num5 = (double)desiredPanelIndex * (0.0 - ((GuiWidget)this).get_Width());
				for (int j = 0; j < panels.Count; j++)
				{
					panels[j].set_OriginRelativeParent(new Vector2(num5, 0.0));
					num5 += ((GuiWidget)this).get_Width();
				}
			}
		}
	}
}
