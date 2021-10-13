using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class SolidSlideView
	{
		private SolidSlider sliderAttachedTo;

		public RGBA_Bytes BackgroundColor
		{
			get;
			set;
		}

		public RGBA_Bytes FillColor
		{
			get;
			set;
		}

		public RGBA_Bytes TrackColor
		{
			get;
			set;
		}

		public double TrackHeight
		{
			get;
			set;
		}

		public TickPlacement TextPlacement
		{
			get;
			set;
		}

		public RGBA_Bytes TextColor
		{
			get;
			set;
		}

		public StyledTypeFace TextStyle
		{
			get;
			set;
		}

		public RGBA_Bytes ThumbColor
		{
			get;
			set;
		}

		public TickPlacement TickPlacement
		{
			get;
			set;
		}

		public RGBA_Bytes TickColor
		{
			get;
			set;
		}

		public SolidSlideView(SolidSlider sliderWidget)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			sliderAttachedTo = sliderWidget;
			TrackHeight = 10.0;
			TextColor = RGBA_Bytes.Black;
			TrackColor = new RGBA_Bytes(220, 220, 220);
			ThumbColor = ActiveTheme.get_Instance().get_SecondaryAccentColor();
		}

		private RectangleDouble GetTrackBounds()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble result = default(RectangleDouble);
			if ((int)sliderAttachedTo.Orientation == 0)
			{
				((RectangleDouble)(ref result))._002Ector(0.0, (0.0 - TrackHeight) / 2.0, sliderAttachedTo.TotalWidthInPixels, TrackHeight / 2.0);
			}
			else
			{
				((RectangleDouble)(ref result))._002Ector((0.0 - TrackHeight) / 2.0, 0.0, TrackHeight / 2.0, sliderAttachedTo.TotalWidthInPixels);
			}
			return result;
		}

		private RectangleDouble GetThumbBounds()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return sliderAttachedTo.GetThumbHitBounds();
		}

		public RectangleDouble GetTotalBounds()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble trackBounds = GetTrackBounds();
			((RectangleDouble)(ref trackBounds)).ExpandToInclude(GetThumbBounds());
			return trackBounds;
		}

		public void DoDrawBeforeChildren(Graphics2D graphics2D)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			graphics2D.FillRectangle(GetTotalBounds(), (IColorType)(object)BackgroundColor);
		}

		public void DoDrawAfterChildren(Graphics2D graphics2D)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Expected O, but got Unknown
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			RoundedRect val = new RoundedRect(GetTrackBounds(), 0.0);
			if ((int)sliderAttachedTo.Orientation == 0)
			{
				new Vector2(sliderAttachedTo.TotalWidthInPixels / 2.0, 0.0 - TrackHeight - 12.0);
			}
			else
			{
				new Vector2(0.0, 0.0 - TrackHeight - 12.0);
			}
			graphics2D.Render((IVertexSource)(object)val, (IColorType)(object)TrackColor);
			RoundedRect val2 = new RoundedRect(sliderAttachedTo.GetThumbHitBounds(), 0.0);
			RGBA_Bytes thumbColor = ThumbColor;
			RGBA_Floats asRGBA_Floats = ((RGBA_Bytes)(ref thumbColor)).GetAsRGBA_Floats();
			RGBA_Floats val3 = RGBA_Floats.Black;
			val3 = RGBA_Floats.GetTweenColor(asRGBA_Floats, ((RGBA_Floats)(ref val3)).GetAsRGBA_Floats(), 0.2);
			graphics2D.Render((IVertexSource)(object)val2, (IColorType)(object)((RGBA_Floats)(ref val3)).GetAsRGBA_Bytes());
		}
	}
}
