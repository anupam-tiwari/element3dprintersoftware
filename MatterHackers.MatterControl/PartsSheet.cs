using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.PolygonMesh;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.PolygonMesh.Rendering;
using MatterHackers.VectorMath;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace MatterHackers.MatterControl
{
	public class PartsSheet
	{
		internal class PartImage
		{
			internal double xOffset;

			internal bool wasDrawn;

			internal ImageBuffer image;

			public PartImage(ImageBuffer imageOfPart)
			{
				image = imageOfPart;
			}
		}

		public class FileNameAndPresentationName
		{
			public string fileName;

			public string presentationName;

			public FileNameAndPresentationName(string fileName, string presentationName)
			{
				this.fileName = fileName;
				this.presentationName = presentationName;
			}
		}

		private string pathAndFileToSaveTo;

		private List<FileNameAndPresentationName> queuPartFilesToAdd;

		private List<PartImage> partImagesToPrint = new List<PartImage>();

		private const double inchesPerMm = 0.0393701;

		private int countThatHaveBeenSaved;

		private Vector2 sheetSizeMM;

		private static bool currentlySaving;

		public int CountThatHaveBeenSaved => countThatHaveBeenSaved;

		public int CountOfParts => queuPartFilesToAdd.Count;

		public Vector2 SheetSizeMM
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return sheetSizeMM;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				sheetSizeMM = value;
			}
		}

		public Vector2 SheetSizeInches
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				return SheetSizeMM * 0.0393701;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				SheetSizeMM = value / 0.0393701;
			}
		}

		public double PixelPerMM => 0.0393701 * (double)SheetDpi;

		public BorderDouble PageMarginMM => new BorderDouble(10.0, 5.0);

		public BorderDouble PageMarginPixels => PageMarginMM * PixelPerMM;

		public double PartMarginMM => 2.0;

		public double PartMarginPixels => PartMarginMM * PixelPerMM;

		public double PartPaddingMM => 2.0;

		public double PartPaddingPixels => PartPaddingMM * PixelPerMM;

		public int SheetDpi
		{
			get;
			set;
		}

		public event EventHandler DoneSaving;

		public event EventHandler UpdateRemainingItems;

		public PartsSheet(List<PrintItem> dataSource, string pathAndFileToSaveTo)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			this.pathAndFileToSaveTo = pathAndFileToSaveTo;
			SheetDpi = 300;
			SheetSizeInches = new Vector2(8.5, 11.0);
			queuPartFilesToAdd = new List<FileNameAndPresentationName>();
			foreach (PrintItem item in dataSource)
			{
				queuPartFilesToAdd.Add(new FileNameAndPresentationName(item.FileLocation, item.Name));
			}
		}

		private void OnDoneSaving()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			if (this.DoneSaving != null)
			{
				this.DoneSaving(this, (EventArgs)new StringEventArgs(Path.GetFileName("Saving to PDF")));
			}
		}

		public void SaveSheets()
		{
			Thread thread = new Thread(new ThreadStart(SavingFunction));
			thread.Name = "Save Part Sheet";
			thread.IsBackground = true;
			thread.Start();
		}

		public void SavingFunction()
		{
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected O, but got Unknown
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Expected O, but got Unknown
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Expected O, but got Unknown
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Expected O, but got Unknown
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Expected O, but got Unknown
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Expected O, but got Unknown
			currentlySaving = true;
			countThatHaveBeenSaved = 0;
			RectangleDouble val2 = default(RectangleDouble);
			Vector2 val5 = default(Vector2);
			RectangleDouble val8 = default(RectangleDouble);
			foreach (FileNameAndPresentationName item in queuPartFilesToAdd)
			{
				List<MeshGroup> list = null;
				if (File.Exists(item.fileName))
				{
					list = MeshFileIo.Load(item.fileName, (ReportProgressRatio)null);
				}
				if (list != null)
				{
					bool flag = true;
					AxisAlignedBoundingBox val = null;
					foreach (MeshGroup item2 in list)
					{
						if (flag)
						{
							val = item2.GetAxisAlignedBoundingBox();
							flag = false;
						}
						else
						{
							val = AxisAlignedBoundingBox.Union(val, item2.GetAxisAlignedBoundingBox());
						}
					}
					((RectangleDouble)(ref val2))._002Ector(val.minXYZ.x, val.minXYZ.y, val.maxXYZ.x, val.maxXYZ.y);
					double num = ((RectangleDouble)(ref val2)).get_Width() + PartMarginMM * 2.0;
					double num2 = 5.0;
					double num3 = num2 + ((RectangleDouble)(ref val2)).get_Height() + PartMarginMM * 2.0;
					TypeFacePrinter val3 = new TypeFacePrinter(item.presentationName, 28.0, Vector2.Zero, (Justification)1, (Baseline)1);
					double val4 = val3.GetSize((string)null).x + PartMarginPixels * 2.0;
					((Vector2)(ref val5))._002Ector(num * PixelPerMM, num3 * PixelPerMM);
					ImageBuffer val6 = new ImageBuffer((int)Math.Max(val4, val5.x), (int)val5.y);
					val3.set_Origin(new Vector2((double)(val6.get_Width() / 2), num2 / 2.0 * PixelPerMM));
					Graphics2D val7 = val6.NewGraphics2D();
					((RectangleDouble)(ref val8))._002Ector(0.0, 0.0, (double)val6.get_Width(), (double)val6.get_Height());
					double num4 = 0.5 * PixelPerMM;
					((RectangleDouble)(ref val8)).Inflate((0.0 - num4) / 2.0);
					RoundedRect val9 = new RoundedRect(val8, PartMarginMM * PixelPerMM);
					val7.Render((IVertexSource)(object)val9, (IColorType)(object)RGBA_Bytes.LightGray);
					Stroke val10 = new Stroke((IVertexSource)(object)val9, num4);
					val7.Render((IVertexSource)(object)val10, (IColorType)(object)RGBA_Bytes.DarkGray);
					foreach (MeshGroup item3 in list)
					{
						foreach (Mesh mesh in item3.get_Meshes())
						{
							OrthographicZProjection.DrawTo(val7, mesh, new Vector2(0.0 - val2.Left + PartMarginMM, 0.0 - val2.Bottom + num2 + PartMarginMM), PixelPerMM, RGBA_Bytes.Black);
						}
					}
					val7.Render((IVertexSource)(object)val3, (IColorType)(object)RGBA_Bytes.Black);
					partImagesToPrint.Add(new PartImage(val6));
					countThatHaveBeenSaved++;
				}
				if (this.UpdateRemainingItems != null)
				{
					this.UpdateRemainingItems(this, (EventArgs)new StringEventArgs(Path.GetFileName(item.presentationName)));
				}
			}
			partImagesToPrint.Sort(new Comparison<PartImage>(BiggestToLittlestImages));
			PdfDocument val11 = new PdfDocument();
			val11.get_Info().set_Title("Aether Parts Sheet");
			val11.get_Info().set_Author("Aether Innovations Inc.");
			val11.get_Info().set_Subject("This is a list of the parts that are in a queue from Element.");
			val11.get_Info().set_Keywords("Element, STL, 3D Printing");
			int nextPartToPrintIndex = 0;
			int num5 = 1;
			bool flag2 = false;
			while (!flag2 && nextPartToPrintIndex < partImagesToPrint.Count)
			{
				PdfPage pdfPage = val11.AddPage();
				CreateOnePage(num5++, ref nextPartToPrintIndex, pdfPage);
			}
			try
			{
				val11.Save(pathAndFileToSaveTo);
				Process.Start(pathAndFileToSaveTo);
			}
			catch (Exception)
			{
			}
			OnDoneSaving();
			currentlySaving = false;
		}

		private static int BiggestToLittlestImages(PartImage one, PartImage two)
		{
			return two.image.get_Height().CompareTo(one.image.get_Height());
		}

		private void CreateOnePage(int plateNumber, ref int nextPartToPrintIndex, PdfPage pdfPage)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Expected O, but got Unknown
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Expected O, but got Unknown
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			ImageBuffer val = new ImageBuffer(2550, 3300);
			Graphics2D val2 = val.NewGraphics2D();
			double num = PrintTopOfPage(val, val2);
			Vector2 val3 = default(Vector2);
			((Vector2)(ref val3))._002Ector(PageMarginPixels.Left, num);
			double num2 = 0.0;
			List<PartImage> list = new List<PartImage>();
			while (nextPartToPrintIndex < partImagesToPrint.Count)
			{
				ImageBuffer image = partImagesToPrint[nextPartToPrintIndex].image;
				num2 = Math.Max(num2, image.get_Height());
				if (list.Count > 0 && val3.x + (double)image.get_Width() > (double)val.get_Width() - PageMarginPixels.Right)
				{
					if (list.Count == 1)
					{
						val2.Render((IImageByte)(object)list[0].image, (double)(val.get_Width() / 2 - list[0].image.get_Width() / 2), val3.y - num2);
					}
					else
					{
						foreach (PartImage item in list)
						{
							val2.Render((IImageByte)(object)item.image, item.xOffset, val3.y - num2);
						}
					}
					val3.x = PageMarginPixels.Left;
					val3.y -= num2 + PartPaddingPixels * 2.0;
					num2 = 0.0;
					list.Clear();
					if (val3.y - (double)image.get_Height() < PageMarginPixels.Bottom)
					{
						break;
					}
				}
				else
				{
					partImagesToPrint[nextPartToPrintIndex].xOffset = val3.x;
					list.Add(partImagesToPrint[nextPartToPrintIndex]);
					val3.x += (double)image.get_Width() + PartPaddingPixels * 2.0;
					nextPartToPrintIndex++;
				}
			}
			foreach (PartImage item2 in list)
			{
				val2.Render((IImageByte)(object)item2.image, item2.xOffset, val3.y - num2);
			}
			TypeFacePrinter val4 = new TypeFacePrinter($"{Path.GetFileNameWithoutExtension(pathAndFileToSaveTo)}", 32.0, default(Vector2), (Justification)1, (Baseline)3);
			val4.set_Origin(new Vector2((double)(((IImage)val2.get_DestImage()).get_Width() / 2), 110.0));
			val2.Render((IVertexSource)(object)val4, (IColorType)(object)RGBA_Bytes.Black);
			val4 = new TypeFacePrinter($"Page {plateNumber}", 28.0, default(Vector2), (Justification)1, (Baseline)3);
			val4.set_Origin(new Vector2((double)(((IImage)val2.get_DestImage()).get_Width() / 2), 60.0));
			val2.Render((IVertexSource)(object)val4, (IColorType)(object)RGBA_Bytes.Black);
			string text = Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "data", "temp", "plateImages");
			string text2 = Path.Combine(text, plateNumber + ".jpeg");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			ImageIO.SaveImageData(text2, (IImageByte)(object)val);
			XGraphics obj = XGraphics.FromPdfPage(pdfPage);
			XImage val5 = XImage.FromFile(text2);
			obj.DrawImage(val5, 0.0, 0.0, XUnit.op_Implicit(pdfPage.get_Width()), XUnit.op_Implicit(pdfPage.get_Height()));
		}

		private double PrintTopOfPage(ImageBuffer plateInventoryImage, Graphics2D plateGraphics)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			plateGraphics.Clear((IColorType)(object)RGBA_Bytes.White);
			double num = (double)plateInventoryImage.get_Height() - PageMarginMM.Top * PixelPerMM;
			string text = "PartSheetLogo.png";
			if (StaticData.get_Instance().FileExists(text))
			{
				ImageBuffer val = StaticData.get_Instance().LoadImage(text);
				num -= (double)val.get_Height();
				plateGraphics.Render((IImageByte)(object)val, (double)((plateInventoryImage.get_Width() - val.get_Width()) / 2), num);
			}
			num -= PartPaddingPixels;
			double num2 = 1.0;
			RectangleDouble val2 = default(RectangleDouble);
			((RectangleDouble)(ref val2))._002Ector(0.0, 0.0, (double)plateInventoryImage.get_Width() - PageMarginPixels.Left * 2.0, num2 * PixelPerMM);
			((RectangleDouble)(ref val2)).Offset(PageMarginPixels.Left, num - ((RectangleDouble)(ref val2)).get_Height());
			plateGraphics.FillRectangle(val2, (IColorType)(object)RGBA_Bytes.Black);
			return num - (((RectangleDouble)(ref val2)).get_Height() + PartPaddingPixels);
		}

		public static bool IsSaving()
		{
			return currentlySaving;
		}
	}
}
