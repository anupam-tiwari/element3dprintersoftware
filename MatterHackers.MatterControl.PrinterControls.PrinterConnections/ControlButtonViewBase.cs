using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class ControlButtonViewBase : GuiWidget
	{
		protected RGBA_Bytes fillColor;

		protected RGBA_Bytes borderColor;

		protected double borderWidth;

		protected double borderRadius;

		protected double padding;

		public ControlButtonViewBase(string label, double width, double height, double textHeight, double borderWidth, double borderRadius, double padding, RGBA_Bytes textColor, RGBA_Bytes fillColor, RGBA_Bytes borderColor)
			: this(width, height, (SizeLimitsToSet)1)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			this.borderRadius = borderRadius;
			this.borderWidth = borderWidth;
			this.fillColor = fillColor;
			this.borderColor = borderColor;
			this.padding = padding;
			TextWidget val = new TextWidget(label, textHeight, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).set_VAnchor((VAnchor)2);
			((GuiWidget)val).set_HAnchor((HAnchor)2);
			val.set_TextColor(textColor);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			RoundedRect val = new RoundedRect(localBounds, borderRadius);
			graphics2D.Render((IVertexSource)(object)val, (IColorType)(object)borderColor);
			RectangleDouble val2 = localBounds;
			((RectangleDouble)(ref val2)).Inflate(0.0 - borderWidth);
			RoundedRect val3 = new RoundedRect(val2, Math.Max(borderRadius - borderWidth, 0.0));
			graphics2D.Render((IVertexSource)(object)val3, (IColorType)(object)fillColor);
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
