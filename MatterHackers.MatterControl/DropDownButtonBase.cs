using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;

namespace MatterHackers.MatterControl
{
	public class DropDownButtonBase : GuiWidget
	{
		private RGBA_Bytes fillColor;

		private RGBA_Bytes borderColor;

		private double borderWidth;

		public DropDownButtonBase(string label, RGBA_Bytes fillColor, RGBA_Bytes borderColor, RGBA_Bytes textColor, double borderWidth, BorderDouble margin, int fontSize = 12, FlowDirection flowDirection = 0, double height = 40.0)
			: this()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Expected O, but got Unknown
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			if (label != "")
			{
				TextWidget val2 = new TextWidget(label, 0.0, 0.0, (double)fontSize, (Justification)0, textColor, true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)val2).set_VAnchor((VAnchor)2);
				((GuiWidget)val2).set_Padding(new BorderDouble(0.0, 0.0));
				((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			}
			GuiWidget val3 = new GuiWidget(20.0, height, (SizeLimitsToSet)1);
			val3.set_VAnchor((VAnchor)2);
			((GuiWidget)val).AddChild(val3, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)this).set_Padding(new BorderDouble(0.0, 0.0));
			this.fillColor = fillColor;
			this.borderColor = borderColor;
			this.borderWidth = borderWidth;
			((GuiWidget)this).set_HAnchor((HAnchor)8);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			DrawBorder(graphics2D);
			DrawBackground(graphics2D);
			((GuiWidget)this).OnDraw(graphics2D);
		}

		private void DrawBorder(Graphics2D graphics2D)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			if (((RGBA_Bytes)(ref borderColor)).get_Alpha0To255() > 0)
			{
				RoundedRect val = new RoundedRect(((GuiWidget)this).get_LocalBounds(), 0.0);
				graphics2D.Render((IVertexSource)new Stroke((IVertexSource)(object)val, borderWidth), (IColorType)(object)borderColor);
			}
		}

		private void DrawBackground(Graphics2D graphics2D)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (((RGBA_Bytes)(ref fillColor)).get_Alpha0To255() > 0)
			{
				RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
				((RectangleDouble)(ref localBounds)).Inflate(0.0 - borderWidth);
				RoundedRect val = new RoundedRect(localBounds, 0.0);
				graphics2D.Render((IVertexSource)(object)val, (IColorType)(object)fillColor);
			}
		}
	}
}
