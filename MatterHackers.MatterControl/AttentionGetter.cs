using System;
using System.Collections.Generic;
using System.Diagnostics;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class AttentionGetter
	{
		private static HashSet<GuiWidget> runningAttentions = new HashSet<GuiWidget>();

		private double animationDelay = 0.05;

		private int cycles = 1;

		private double lightnessChange = 1.0;

		private double pulseTime = 1.38;

		private RGBA_Bytes startColor;

		private Stopwatch timeSinceStart;

		private GuiWidget widgetToHighlight;

		private AttentionGetter(GuiWidget widgetToHighlight)
		{
			this.widgetToHighlight = widgetToHighlight;
			widgetToHighlight.add_AfterDraw((EventHandler<DrawEventArgs>)ConnectToWidget);
		}

		public static AttentionGetter GetAttention(GuiWidget widgetToHighlight)
		{
			if (!runningAttentions.Contains(widgetToHighlight))
			{
				runningAttentions.Add(widgetToHighlight);
				return new AttentionGetter(widgetToHighlight);
			}
			return null;
		}

		private void ChangeBackgroundColor()
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			if (widgetToHighlight != null)
			{
				double num;
				for (num = timeSinceStart.get_Elapsed().TotalSeconds; num > pulseTime; num -= pulseTime)
				{
				}
				num = num * 2.0 / pulseTime;
				if (num > 1.0)
				{
					num = 1.0 - (num - 1.0);
				}
				double num2 = EaseInOutQuad(num);
				widgetToHighlight.set_BackgroundColor(ColorExtensionMethods.AdjustLightness((IColorType)(object)startColor, 1.0 + lightnessChange * num2).GetAsRGBA_Bytes());
				if (widgetToHighlight.get_HasBeenClosed() || timeSinceStart.get_Elapsed().TotalSeconds > (double)cycles * pulseTime)
				{
					widgetToHighlight.set_BackgroundColor(startColor);
					widgetToHighlight.remove_AfterDraw((EventHandler<DrawEventArgs>)ConnectToWidget);
					runningAttentions.Remove(widgetToHighlight);
					widgetToHighlight = null;
				}
				else
				{
					UiThread.RunOnIdle((Action)ChangeBackgroundColor, animationDelay);
				}
			}
		}

		private double EaseInOutQuad(double t)
		{
			if (t <= 0.5)
			{
				return 2.0 * (t * t);
			}
			t -= 0.5;
			return 2.0 * t * (1.0 - t) + 0.5;
		}

		private void ConnectToWidget(object drawingWidget, DrawEventArgs e)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			GuiWidget val = drawingWidget as GuiWidget;
			while (true)
			{
				RGBA_Bytes backgroundColor = val.get_BackgroundColor();
				if (((RGBA_Bytes)(ref backgroundColor)).get_Alpha0To255() != 0)
				{
					break;
				}
				val = val.get_Parent();
			}
			startColor = val.get_BackgroundColor();
			timeSinceStart = Stopwatch.StartNew();
			widgetToHighlight.remove_AfterDraw((EventHandler<DrawEventArgs>)ConnectToWidget);
			UiThread.RunOnIdle((Action)ChangeBackgroundColor, animationDelay);
		}
	}
}
