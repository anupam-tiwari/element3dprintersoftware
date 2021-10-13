using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class TextImageWidget : GuiWidget
	{
		private ImageBuffer image;

		protected RGBA_Bytes fillColor = new RGBA_Bytes(0, 0, 0, 0);

		protected RGBA_Bytes borderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);

		protected double borderWidth = 1.0;

		protected double borderRadius;

		public TextImageWidget(string label, RGBA_Bytes fillColor, RGBA_Bytes borderColor, RGBA_Bytes textColor, double borderWidth, BorderDouble margin, ImageBuffer image = null, double fontSize = 12.0, FlowDirection flowDirection = 0, double height = 40.0, double width = 0.0, bool centerText = false, double imageSpacing = 0.0)
			: this()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected O, but got Unknown
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Expected O, but got Unknown
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Expected O, but got Unknown
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Expected O, but got Unknown
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Expected O, but got Unknown
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			this.image = image;
			this.fillColor = fillColor;
			this.borderColor = borderColor;
			this.borderWidth = borderWidth;
			((GuiWidget)this).set_Margin(new BorderDouble(0.0));
			((GuiWidget)this).set_Padding(new BorderDouble(0.0));
			TextWidget val = new TextWidget(label, 0.0, 0.0, fontSize, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			FlowLayoutWidget val2 = new FlowLayoutWidget(flowDirection);
			if (centerText)
			{
				GuiWidget val3 = new GuiWidget(0.0, 1.0, (SizeLimitsToSet)1);
				val3.set_HAnchor((HAnchor)5);
				((GuiWidget)val2).AddChild(val3, -1);
			}
			if (image != (ImageBuffer)null && image.get_Width() > 0)
			{
				ImageWidget val4 = new ImageWidget(image);
				((GuiWidget)val4).set_VAnchor((VAnchor)2);
				((GuiWidget)val4).set_Margin(new BorderDouble(0.0, 0.0, imageSpacing, 0.0));
				((GuiWidget)val2).AddChild((GuiWidget)(object)val4, -1);
			}
			if (label != "")
			{
				((GuiWidget)val).set_VAnchor((VAnchor)2);
				val.set_TextColor(textColor);
				((GuiWidget)val).set_Padding(new BorderDouble(3.0, 0.0));
				((GuiWidget)val2).AddChild((GuiWidget)(object)val, -1);
			}
			if (centerText)
			{
				GuiWidget val5 = new GuiWidget(0.0, 1.0, (SizeLimitsToSet)1);
				val5.set_HAnchor((HAnchor)5);
				((GuiWidget)val2).AddChild(val5, -1);
				((GuiWidget)val2).set_HAnchor((HAnchor)13);
			}
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			((GuiWidget)val2).set_MinimumSize(new Vector2(width, height));
			((GuiWidget)val2).set_Margin(margin);
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).set_HAnchor((HAnchor)13);
			((GuiWidget)this).set_VAnchor((VAnchor)10);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Expected O, but got Unknown
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			if (((RGBA_Bytes)(ref borderColor)).get_Alpha0To255() > 0)
			{
				RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
				if (borderWidth > 0.0)
				{
					if (borderWidth == 1.0)
					{
						graphics2D.Rectangle(localBounds, borderColor, 1.0);
					}
					else
					{
						RoundedRect val = new RoundedRect(localBounds, borderRadius);
						graphics2D.Render((IVertexSource)new Stroke((IVertexSource)(object)val, borderWidth), (IColorType)(object)borderColor);
					}
				}
			}
			if (((RGBA_Bytes)(ref fillColor)).get_Alpha0To255() > 0)
			{
				RectangleDouble localBounds2 = ((GuiWidget)this).get_LocalBounds();
				((RectangleDouble)(ref localBounds2)).Inflate(0.0 - borderWidth);
				RoundedRect val2 = new RoundedRect(localBounds2, Math.Max(borderRadius - borderWidth, 0.0));
				graphics2D.Render((IVertexSource)(object)val2, (IColorType)(object)fillColor);
			}
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
