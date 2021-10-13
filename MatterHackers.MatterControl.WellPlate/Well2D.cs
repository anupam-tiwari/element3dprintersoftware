using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.WellPlate
{
	public class Well2D : GuiWidget, IWell
	{
		private WellPlate2D wellPlate2D;

		private int row;

		private int col;

		public WellShape WellShape;

		private RGBA_Bytes wellColor;

		private WellPlatePart part;

		private ImageWidget partImage;

		private bool hovering;

		private bool selected;

		public bool Highlighted
		{
			get;
			set;
		}

		public bool Selected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
				wellPlate2D.SetWellSelectionStatus(row, col, selected);
				((GuiWidget)this).Invalidate();
			}
		}

		public bool PartSet => part != null;

		public Well2D(WellPlate2D wellPlate2D, int row, int col, WellShape wellShape, double wellWidth, RGBA_Bytes wellColor)
			: this(wellWidth, wellWidth, (SizeLimitsToSet)1)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			this.wellPlate2D = wellPlate2D;
			this.row = row;
			this.col = col;
			WellShape = wellShape;
			this.wellColor = wellColor;
			((GuiWidget)this).add_BoundsChanged((EventHandler)delegate
			{
				SetPart(part);
			});
			partImage = new ImageWidget((int)((GuiWidget)this).get_Height(), (int)((GuiWidget)this).get_Width());
			((GuiWidget)partImage).AnchorCenter();
			((GuiWidget)partImage).add_Click((EventHandler<MouseEventArgs>)delegate(object sender, MouseEventArgs e)
			{
				((GuiWidget)this).OnClick(e);
			});
			((GuiWidget)this).AddChild((GuiWidget)(object)partImage, -1);
		}

		public void SetPart(WellPlatePart wellPlatePart)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			part = wellPlatePart;
			if (part != null)
			{
				partImage.set_Image(ImageBuffer.CreateScaledImage(part.OriginalImage, (int)((GuiWidget)this).get_Height(), (int)((GuiWidget)this).get_Width()));
			}
			else
			{
				partImage.set_Image(new ImageBuffer((int)((GuiWidget)this).get_Height(), (int)((GuiWidget)this).get_Width()));
			}
		}

		public void ClearSelection()
		{
			selected = false;
		}

		public override void OnClick(MouseEventArgs mouseEvent)
		{
			((GuiWidget)this).OnClick(mouseEvent);
			selected = !selected;
			wellPlate2D.SetWellSelectionStatus(row, col, selected);
			((GuiWidget)this).Invalidate();
		}

		public override void OnMouseEnterBounds(MouseEventArgs mouseEvent)
		{
			((GuiWidget)this).OnMouseEnterBounds(mouseEvent);
			if (wellPlate2D.DoHighlighting)
			{
				hovering = true;
				wellPlate2D.HighlightCross(row, col, highlight: true);
				((GuiWidget)this).Invalidate();
			}
		}

		public override void OnMouseLeaveBounds(MouseEventArgs mouseEvent)
		{
			((GuiWidget)this).OnMouseLeaveBounds(mouseEvent);
			if (wellPlate2D.DoHighlighting)
			{
				hovering = false;
				wellPlate2D.HighlightCross(row, col, highlight: false);
				((GuiWidget)this).Invalidate();
			}
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			RGBA_Bytes val = wellColor;
			RGBA_Floats val2;
			if (selected)
			{
				val2 = ((RGBA_Bytes)(ref wellColor)).GetAsRGBA_Floats();
				double num = default(double);
				double num2 = default(double);
				double num3 = default(double);
				((RGBA_Floats)(ref val2)).GetHSL(ref num, ref num2, ref num3);
				num2 = Math.Min(1.0, num2 * 2.0);
				num3 = Math.Min(1.0, num3 * 1.2);
				val2 = RGBA_Floats.FromHSL(num, num2, 0.85, 1.0);
				val = ((RGBA_Floats)(ref val2)).GetAsRGBA_Bytes();
			}
			else if (Highlighted)
			{
				val2 = ((RGBA_Bytes)(ref wellColor)).GetAsRGBA_Floats();
				double num4 = default(double);
				double num5 = default(double);
				double num6 = default(double);
				((RGBA_Floats)(ref val2)).GetHSL(ref num4, ref num5, ref num6);
				val2 = RGBA_Floats.FromHSL(num4, num5, 0.3, 1.0);
				val = ((RGBA_Floats)(ref val2)).GetAsRGBA_Bytes();
			}
			if (hovering && wellPlate2D.DoHighlighting)
			{
				val = RGBA_Bytes.White;
			}
			switch (WellShape)
			{
			case WellShape.CIRCLE:
				((GuiWidget)this).set_BackgroundColor(default(RGBA_Bytes));
				graphics2D.Circle(((GuiWidget)this).get_Width() / 2.0, ((GuiWidget)this).get_Height() / 2.0, Math.Min(((GuiWidget)this).get_Width(), ((GuiWidget)this).get_Height()) / 2.0, val);
				break;
			case WellShape.SQUARE:
				((GuiWidget)this).set_BackgroundColor(val);
				break;
			}
			((GuiWidget)this).OnDraw(graphics2D);
		}
	}
}
