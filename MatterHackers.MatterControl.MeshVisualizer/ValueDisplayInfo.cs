using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.RenderOpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.MeshVisualizer
{
	public class ValueDisplayInfo
	{
		private string measureDisplayedString = "";

		private ImageBuffer measureDisplayImage;

		private string formatString;

		private string unitsString;

		public ValueDisplayInfo(string formatString = "{0:0.00}", string unitsString = "mm")
		{
			this.formatString = formatString;
			this.unitsString = unitsString;
		}

		public void DisplaySizeInfo(Graphics2D graphics2D, Vector2 widthDisplayCenter, double size)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Expected O, but got Unknown
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			string b = StringHelper.FormatWith(formatString, new object[1]
			{
				size
			});
			if (measureDisplayImage == (ImageBuffer)null || measureDisplayedString != b)
			{
				measureDisplayedString = b;
				TypeFacePrinter val = new TypeFacePrinter(measureDisplayedString, 16.0, default(Vector2), (Justification)0, (Baseline)3);
				TypeFacePrinter val2 = new TypeFacePrinter(unitsString, 10.0, default(Vector2), (Justification)0, (Baseline)3);
				double num = 1.0;
				BorderDouble val3 = default(BorderDouble);
				((BorderDouble)(ref val3))._002Ector(5.0);
				val.set_Origin(new Vector2(val3.Left, val3.Bottom));
				RectangleDouble localBounds = val.get_LocalBounds();
				val2.set_Origin(new Vector2(localBounds.Right + num, val3.Bottom));
				RectangleDouble localBounds2 = val2.get_LocalBounds();
				measureDisplayImage = new ImageBuffer((int)(((RectangleDouble)(ref localBounds)).get_Width() + ((BorderDouble)(ref val3)).get_Width() + ((RectangleDouble)(ref localBounds2)).get_Width() + num), (int)(((RectangleDouble)(ref localBounds)).get_Height() + ((BorderDouble)(ref val3)).get_Height()));
				ImageGlPlugin.GetImageGlPlugin(measureDisplayImage, true, true);
				Graphics2D val4 = measureDisplayImage.NewGraphics2D();
				val4.Clear((IColorType)(object)new RGBA_Bytes(RGBA_Bytes.White, 128));
				val.Render(val4, RGBA_Bytes.Black);
				val2.Render(val4, RGBA_Bytes.Black);
			}
			widthDisplayCenter -= new Vector2((double)(measureDisplayImage.get_Width() / 2), (double)(measureDisplayImage.get_Height() / 2));
			graphics2D.Render((IImageByte)(object)measureDisplayImage, widthDisplayCenter);
		}
	}
}
