using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Localizations;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class ProgressDial : GuiWidget
	{
		private RGBA_Bytes borderColor;

		private Stroke borderStroke;

		private double completedRatio = -1.0;

		private double innerRingRadius;

		private double layerCompletedRatio;

		private int layerCount = -1;

		private TextWidget layerCountWidget;

		private double outerRingRadius;

		private double outerRingStrokeWidth = 7.0 * GuiWidget.get_DeviceScale();

		private TextWidget percentCompleteWidget;

		private RGBA_Bytes PrimaryAccentColor = ActiveTheme.get_Instance().get_PrimaryAccentColor();

		private RGBA_Bytes PrimaryAccentShade = ColorExtensionMethods.AdjustLightness((IColorType)(object)ActiveTheme.get_Instance().get_PrimaryAccentColor(), 0.7).GetAsRGBA_Bytes();

		private double innerRingStrokeWidth = 10.0 * GuiWidget.get_DeviceScale();

		public double CompletedRatio
		{
			get
			{
				return completedRatio;
			}
			set
			{
				if (completedRatio != value)
				{
					completedRatio = Math.Min(value, 1.0);
					((GuiWidget)this).Invalidate();
					((GuiWidget)percentCompleteWidget).set_Text($"{CompletedRatio * 100.0:0}%");
				}
			}
		}

		public double LayerCompletedRatio
		{
			get
			{
				return layerCompletedRatio;
			}
			set
			{
				if (layerCompletedRatio != value)
				{
					layerCompletedRatio = value;
					((GuiWidget)this).Invalidate();
				}
			}
		}

		public int LayerCount
		{
			get
			{
				return layerCount;
			}
			set
			{
				if (layerCount != value)
				{
					layerCount = value;
					if (layerCount == 0)
					{
						((GuiWidget)layerCountWidget).set_Text("Printing".Localize() + "...");
					}
					else
					{
						((GuiWidget)layerCountWidget).set_Text("Layer".Localize() + " " + layerCount);
					}
				}
			}
		}

		public ProgressDial()
			: this()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Expected O, but got Unknown
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Expected O, but got Unknown
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			TextWidget val = new TextWidget("", 0.0, 0.0, 22.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_AutoExpandBoundsToText(true);
			((GuiWidget)val).set_VAnchor((VAnchor)2);
			((GuiWidget)val).set_HAnchor((HAnchor)2);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 20.0, 0.0, 0.0));
			percentCompleteWidget = val;
			CompletedRatio = 0.0;
			TextWidget val2 = new TextWidget("", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			((GuiWidget)val2).set_HAnchor((HAnchor)2);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 32.0));
			layerCountWidget = val2;
			LayerCount = 0;
			((GuiWidget)this).AddChild((GuiWidget)(object)percentCompleteWidget, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)layerCountWidget, -1);
			borderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((RGBA_Bytes)(ref borderColor)).set_Alpha0To1(0.3f);
		}

		public override void OnBoundsChanged(EventArgs e)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Expected O, but got Unknown
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			double num = ((RectangleDouble)(ref localBounds)).get_Width() / 2.0 - 20.0 * GuiWidget.get_DeviceScale();
			outerRingRadius = num - outerRingStrokeWidth / 2.0 - 6.0 * GuiWidget.get_DeviceScale();
			innerRingRadius = outerRingRadius - outerRingStrokeWidth / 2.0 - innerRingStrokeWidth / 2.0;
			borderStroke = new Stroke((IVertexSource)new Ellipse(Vector2.Zero, num, num, 0, false), 1.0);
			((GuiWidget)this).OnBoundsChanged(e);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Expected O, but got Unknown
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Expected O, but got Unknown
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected O, but got Unknown
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			graphics2D.Render(ExtensionMethods.Translate((IVertexSource)(object)borderStroke, ((RectangleDouble)(ref localBounds)).get_Center()), (IColorType)(object)borderColor);
			Stroke val = new Stroke((IVertexSource)new Arc(Vector2.Zero, new Vector2(outerRingRadius, outerRingRadius), 0.0, MathHelper.DegreesToRadians(360.0) * LayerCompletedRatio, (Direction)0, 1.0), 1.0);
			val.width(outerRingStrokeWidth);
			graphics2D.Render(ExtensionMethods.Translate(ExtensionMethods.Rotate((IVertexSource)(object)val, 90.0, (AngleType)0), ((RectangleDouble)(ref localBounds)).get_Center()), (IColorType)(object)PrimaryAccentShade);
			val = new Stroke((IVertexSource)new Arc(Vector2.Zero, new Vector2(innerRingRadius, innerRingRadius), 0.0, MathHelper.DegreesToRadians(360.0) * CompletedRatio, (Direction)0, 1.0), 1.0);
			val.width(innerRingStrokeWidth);
			graphics2D.Render(ExtensionMethods.Translate(ExtensionMethods.Rotate((IVertexSource)(object)val, 90.0, (AngleType)0), ((RectangleDouble)(ref localBounds)).get_Center()), (IColorType)(object)PrimaryAccentColor);
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
