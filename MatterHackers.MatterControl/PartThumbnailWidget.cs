using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PartPreviewWindow;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.PolygonMesh;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.PolygonMesh.Rendering;
using MatterHackers.RayTracer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class PartThumbnailWidget : ClickWidget
	{
		public enum ImageSizes
		{
			Size50x50,
			Size115x115
		}

		private enum RenderType
		{
			NONE,
			ORTHOGROPHIC,
			PERSPECTIVE,
			RAY_TRACE
		}

		private object locker = new object();

		public new double BorderWidth;

		public RGBA_Bytes FillColor = new RGBA_Bytes(255, 255, 255);

		public RGBA_Bytes HoverBackgroundColor = new RGBA_Bytes(0, 0, 0, 50);

		protected double borderRadius;

		protected RGBA_Bytes HoverBorderColor;

		private const int renderOrthoAndroid = 20000000;

		private const int renderOrthoDesktop = 100000000;

		private const int tooBigAndroid = 50000000;

		private const int tooBigDesktop = 250000000;

		private static string partExtension = ".png";

		private static bool processingThumbnail = false;

		private ImageBuffer buildingThumbnailImage = new ImageBuffer();

		private RGBA_Bytes normalBackgroundColor = ActiveTheme.get_Instance().get_PrimaryAccentColor();

		private ImageBuffer noThumbnailImage = new ImageBuffer();

		private PartPreviewMainWindow partPreviewWindow;

		private PrintItemWrapper printItemWrapper;

		private bool thumbNailHasBeenCreated;

		private ImageBuffer thumbnailImage = new ImageBuffer();

		private EventHandler unregisterEvents;

		public PrintItemWrapper ItemWrapper
		{
			get
			{
				return printItemWrapper;
			}
			set
			{
				if (ItemWrapper != null)
				{
					PrintItemWrapper.FileHasChanged.UnregisterEvent((EventHandler)item_FileHasChanged, ref unregisterEvents);
				}
				printItemWrapper = value;
				thumbNailHasBeenCreated = false;
				if (ItemWrapper != null)
				{
					PrintItemWrapper.FileHasChanged.RegisterEvent((EventHandler)item_FileHasChanged, ref unregisterEvents);
				}
			}
		}

		public ImageSizes Size
		{
			get;
			set;
		}

		private static Point2D BigRenderSize => new Point2D(460, 460);

		public event EventHandler<StringEventArgs> DoneRendering;

		public PartThumbnailWidget(PrintItemWrapper item, string noThumbnailFileName, string buildingThumbnailFileName, ImageSizes size)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Expected O, but got Unknown
			((GuiWidget)this).set_ToolTipText("Click to show in 3D View".Localize());
			ItemWrapper = item;
			((GuiWidget)this).set_Margin(new BorderDouble(0.0));
			((GuiWidget)this).set_Padding(new BorderDouble(5.0));
			Size = size;
			switch (size)
			{
			case ImageSizes.Size50x50:
				((GuiWidget)this).set_Width(50.0 * GuiWidget.get_DeviceScale());
				((GuiWidget)this).set_Height(50.0 * GuiWidget.get_DeviceScale());
				break;
			case ImageSizes.Size115x115:
				((GuiWidget)this).set_Width(115.0 * GuiWidget.get_DeviceScale());
				((GuiWidget)this).set_Height(115.0 * GuiWidget.get_DeviceScale());
				break;
			default:
				throw new NotImplementedException();
			}
			((GuiWidget)this).set_MinimumSize(new Vector2(((GuiWidget)this).get_Width(), ((GuiWidget)this).get_Height()));
			((GuiWidget)this).set_BackgroundColor(normalBackgroundColor);
			((GuiWidget)this).set_Cursor((Cursors)3);
			((GuiWidget)this).set_ToolTipText("Click to show in 3D View".Localize());
			if (noThumbnailImage.get_Width() == 0)
			{
				StaticData.get_Instance().LoadIcon(noThumbnailFileName, noThumbnailImage);
				ExtensionMethods.InvertLightness(noThumbnailImage);
				StaticData.get_Instance().LoadIcon(buildingThumbnailFileName, buildingThumbnailImage);
				ExtensionMethods.InvertLightness(buildingThumbnailImage);
			}
			thumbnailImage = new ImageBuffer(buildingThumbnailImage);
			base.Click += DoOnMouseClick;
			((GuiWidget)this).add_MouseEnterBounds((EventHandler)onEnter);
			((GuiWidget)this).add_MouseLeaveBounds((EventHandler)onExit);
		}

		private void DoOnMouseClick(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				if (printItemWrapper != null)
				{
					if (File.Exists(printItemWrapper.FileLocation))
					{
						if (Keyboard.IsKeyDown((Keys)16))
						{
							OpenPartPreviewWindow(View3DWidget.AutoRotate.Disabled);
						}
						else
						{
							OpenPartPreviewWindow(View3DWidget.AutoRotate.Enabled);
						}
					}
					else
					{
						QueueRowItem.ShowCantFindFileMessage(printItemWrapper);
					}
				}
			});
		}

		public static string GetImageFileName(PrintItemWrapper item)
		{
			return GetImageFileName(item.FileHashCode.ToString());
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		private void LoadOrCreateThumbnail()
		{
			lock (locker)
			{
				if (!thumbNailHasBeenCreated && !processingThumbnail)
				{
					thumbNailHasBeenCreated = true;
					processingThumbnail = true;
					CreateThumbnail();
					processingThumbnail = false;
				}
			}
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Expected O, but got Unknown
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Expected O, but got Unknown
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			if (!thumbNailHasBeenCreated && !processingThumbnail)
			{
				if (SetImageFast())
				{
					thumbNailHasBeenCreated = true;
					OnDoneRendering();
				}
				else
				{
					Task.Run(delegate
					{
						LoadOrCreateThumbnail();
					});
				}
			}
			if (((GuiWidget)this).get_FirstWidgetUnderMouse())
			{
				new RoundedRect(((GuiWidget)this).get_LocalBounds(), 0.0);
			}
			graphics2D.Render((IImageByte)(object)thumbnailImage, ((GuiWidget)this).get_Width() / 2.0 - (double)(thumbnailImage.get_Width() / 2), ((GuiWidget)this).get_Height() / 2.0 - (double)(thumbnailImage.get_Height() / 2));
			base.OnDraw(graphics2D);
			if (((RGBA_Bytes)(ref HoverBorderColor)).get_Alpha0To255() > 0)
			{
				((GuiWidget)this).get_LocalBounds();
				Stroke val = new Stroke((IVertexSource)new RoundedRect(((GuiWidget)this).get_LocalBounds(), borderRadius), BorderWidth);
				graphics2D.Render((IVertexSource)(object)val, (IColorType)(object)HoverBorderColor);
			}
		}

		private static ImageBuffer BuildImageFromMeshGroups(List<MeshGroup> loadedMeshGroups, string stlHashCode, Point2D size)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Expected O, but got Unknown
			if (loadedMeshGroups != null && loadedMeshGroups.Count > 0 && loadedMeshGroups[0].get_Meshes() != null && loadedMeshGroups[0].get_Meshes()[0] != null)
			{
				ImageBuffer val = new ImageBuffer(size.x, size.y);
				Graphics2D val2 = val.NewGraphics2D();
				val2.Clear((IColorType)(object)default(RGBA_Bytes));
				AxisAlignedBoundingBox val3 = loadedMeshGroups[0].GetAxisAlignedBoundingBox();
				for (int i = 1; i < loadedMeshGroups.Count; i++)
				{
					val3 = AxisAlignedBoundingBox.Union(val3, loadedMeshGroups[i].GetAxisAlignedBoundingBox());
				}
				double num = Math.Max(val3.get_XSize(), val3.get_YSize());
				double num2 = (double)size.x / (num * 1.2);
				RectangleDouble val4 = default(RectangleDouble);
				((RectangleDouble)(ref val4))._002Ector(val3.minXYZ.x, val3.minXYZ.y, val3.maxXYZ.x, val3.maxXYZ.y);
				foreach (MeshGroup loadedMeshGroup in loadedMeshGroups)
				{
					foreach (Mesh mesh in loadedMeshGroup.get_Meshes())
					{
						OrthographicZProjection.DrawTo(val2, mesh, new Vector2(((double)size.x / num2 - ((RectangleDouble)(ref val4)).get_Width()) / 2.0 - val4.Left, ((double)size.y / num2 - ((RectangleDouble)(ref val4)).get_Height()) / 2.0 - val4.Bottom), num2, RGBA_Bytes.White);
					}
				}
				if (File.Exists("RunUnitTests.txt"))
				{
					foreach (Mesh mesh2 in loadedMeshGroups[0].get_Meshes())
					{
						if (mesh2.GetNonManifoldEdges().Count > 0)
						{
							val2.Circle((double)(size.x / 4), (double)(size.x / 4), (double)(size.x / 8), RGBA_Bytes.Red);
						}
					}
				}
				val.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				AllWhite.DoAllWhite(val);
				return val;
			}
			return null;
		}

		private static string GetImageFileName(string stlHashCode)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			string text = Path.Combine(ThumbnailPath(), Path.ChangeExtension(StringHelper.FormatWith("{0}_{1}x{2}", new object[3]
			{
				stlHashCode,
				BigRenderSize.x,
				BigRenderSize.y
			}), partExtension));
			string directoryName = Path.GetDirectoryName(text);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			return text;
		}

		private static RenderType GetRenderType(string fileLocation)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Invalid comparison between Unknown and I4
			if (UserSettings.Instance.get("ThumbnailRenderingMode") == "raytraced")
			{
				if (Is32Bit())
				{
					long num = 0L;
					if (File.Exists(fileLocation))
					{
						num = MeshFileIo.GetEstimatedMemoryUse(fileLocation);
						if ((int)OsInformation.get_OperatingSystem() == 5)
						{
							if (num > 20000000)
							{
								return RenderType.ORTHOGROPHIC;
							}
						}
						else if (num > 100000000)
						{
							return RenderType.ORTHOGROPHIC;
						}
					}
				}
				return RenderType.RAY_TRACE;
			}
			return RenderType.ORTHOGROPHIC;
		}

		private static bool Is32Bit()
		{
			if (IntPtr.Size == 4)
			{
				return true;
			}
			return false;
		}

		public static ImageBuffer LoadImageFromDisk(string stlHashCode)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			try
			{
				ImageBuffer val = new ImageBuffer(BigRenderSize.x, BigRenderSize.y);
				string imageFileName = GetImageFileName(stlHashCode);
				if (File.Exists(imageFileName))
				{
					if (partExtension == ".png")
					{
						if (ImageIO.LoadImageData(imageFileName, val))
						{
							return val;
						}
					}
					else if (ImageTgaIO.LoadImageData(val, imageFileName))
					{
						return val;
					}
				}
			}
			catch
			{
			}
			return null;
		}

		public static string ThumbnailPath()
		{
			return Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "data", "temp", "thumbnails");
		}

		private void CreateThumbnail()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Expected O, but got Unknown
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Expected O, but got Unknown
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Expected O, but got Unknown
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Expected O, but got Unknown
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Expected O, but got Unknown
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Expected O, but got Unknown
			string stlHashCode = ItemWrapper.FileHashCode.ToString();
			ImageBuffer val = new ImageBuffer();
			if (!File.Exists(ItemWrapper.FileLocation))
			{
				return;
			}
			List<MeshGroup> list = MeshFileIo.Load(ItemWrapper.FileLocation, (ReportProgressRatio)null);
			switch (GetRenderType(ItemWrapper.FileLocation))
			{
			case RenderType.RAY_TRACE:
			{
				ThumbnailTracer thumbnailTracer = new ThumbnailTracer(list, BigRenderSize.x, BigRenderSize.y);
				thumbnailTracer.DoTrace();
				val = thumbnailTracer.destImage;
				break;
			}
			case RenderType.PERSPECTIVE:
			{
				ThumbnailTracer thumbnailTracer2 = new ThumbnailTracer(list, BigRenderSize.x, BigRenderSize.y);
				thumbnailImage = new ImageBuffer(buildingThumbnailImage);
				thumbnailImage.NewGraphics2D().Clear((IColorType)(object)new RGBA_Bytes(255, 255, 255, 0));
				val = new ImageBuffer(BigRenderSize.x, BigRenderSize.y);
				foreach (MeshGroup item in list)
				{
					double minZ = double.MaxValue;
					double maxZ = double.MinValue;
					foreach (Mesh mesh in item.get_Meshes())
					{
						thumbnailTracer2.GetMinMaxZ(mesh, ref minZ, ref maxZ);
					}
					foreach (Mesh mesh2 in item.get_Meshes())
					{
						thumbnailTracer2.DrawTo(val.NewGraphics2D(), mesh2, RGBA_Bytes.White, minZ, maxZ);
					}
				}
				if (val == (ImageBuffer)null)
				{
					val = new ImageBuffer(noThumbnailImage);
				}
				break;
			}
			case RenderType.NONE:
			case RenderType.ORTHOGROPHIC:
				thumbnailImage = new ImageBuffer(buildingThumbnailImage);
				thumbnailImage.NewGraphics2D().Clear((IColorType)(object)new RGBA_Bytes(255, 255, 255, 0));
				val = BuildImageFromMeshGroups(list, stlHashCode, BigRenderSize);
				if (val == (ImageBuffer)null)
				{
					val = new ImageBuffer(noThumbnailImage);
				}
				break;
			}
			string imageFileName = GetImageFileName(stlHashCode);
			if (partExtension == ".png")
			{
				ImageIO.SaveImageData(imageFileName, (IImageByte)(object)val);
			}
			else
			{
				ImageTgaIO.SaveImageData(imageFileName, val);
			}
			val.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
			thumbnailImage = ImageBuffer.CreateScaledImage(val, (int)((GuiWidget)this).get_Width(), (int)((GuiWidget)this).get_Height());
			UiThread.RunOnIdle((Action)EnsureImageUpdated);
			OnDoneRendering();
		}

		private void OnDoneRendering()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			if (ItemWrapper != null)
			{
				string imageFileName = GetImageFileName(ItemWrapper.FileHashCode.ToString());
				if (this.DoneRendering != null)
				{
					this.DoneRendering(this, new StringEventArgs(imageFileName));
				}
			}
		}

		private void EnsureImageUpdated()
		{
			thumbnailImage.MarkImageChanged();
			((GuiWidget)this).Invalidate();
		}

		private void item_FileHasChanged(object sender, EventArgs e)
		{
			PrintItemWrapper printItemWrapper = sender as PrintItemWrapper;
			if (printItemWrapper != null && printItemWrapper.FileLocation == this.printItemWrapper.FileLocation)
			{
				thumbNailHasBeenCreated = false;
				((GuiWidget)this).Invalidate();
			}
		}

		private static bool MeshIsTooBigToLoad(string fileLocation)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			if (Is32Bit())
			{
				long num = 0L;
				if (File.Exists(fileLocation))
				{
					num = MeshFileIo.GetEstimatedMemoryUse(fileLocation);
					if ((int)OsInformation.get_OperatingSystem() == 5)
					{
						if (num > 50000000)
						{
							return true;
						}
					}
					else if (num > 250000000)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void onEnter(object sender, EventArgs e)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			HoverBorderColor = new RGBA_Bytes(255, 255, 255);
			((GuiWidget)this).Invalidate();
		}

		private void onExit(object sender, EventArgs e)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			HoverBorderColor = default(RGBA_Bytes);
			((GuiWidget)this).Invalidate();
		}

		private void OpenPartPreviewWindow(View3DWidget.AutoRotate autoRotate)
		{
			if (partPreviewWindow == null)
			{
				partPreviewWindow = new PartPreviewMainWindow(ItemWrapper, autoRotate);
				((GuiWidget)partPreviewWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					partPreviewWindow = null;
				});
			}
			else
			{
				((GuiWidget)partPreviewWindow).BringToFront();
			}
		}

		public static ImageBuffer GetImageForItem(PrintItemWrapper itemWrapper, int width, int height)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Expected O, but got Unknown
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Expected O, but got Unknown
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Expected O, but got Unknown
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Expected O, but got Unknown
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Expected O, but got Unknown
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Expected O, but got Unknown
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Expected O, but got Unknown
			if (itemWrapper == null)
			{
				return StaticData.get_Instance().LoadIcon("part_icon_transparent_100x100.png");
			}
			ImageBuffer val = new ImageBuffer(width, height);
			if (itemWrapper.FileLocation == QueueData.SdCardFileName)
			{
				ImageBuffer val2 = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon(Path.ChangeExtension("icon_sd_card_115x115", partExtension)));
				val.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				Graphics2D obj = val.NewGraphics2D();
				obj.Render((IImageByte)(object)val2, (double)(width / 2 - val2.get_Width() / 2), (double)(height / 2 - val2.get_Height() / 2));
				Ellipse val3 = new Ellipse(new Vector2((double)width / 2.0, (double)height / 2.0), (double)(width / 2 - width / 12));
				obj.Render((IVertexSource)new Stroke((IVertexSource)(object)val3, (double)(width / 12)), (IColorType)(object)RGBA_Bytes.White);
				return val;
			}
			if (Path.GetExtension(itemWrapper.FileLocation)!.ToUpper() == ".GCODE")
			{
				val.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				Graphics2D obj2 = val.NewGraphics2D();
				Vector2 val4 = default(Vector2);
				((Vector2)(ref val4))._002Ector((double)width / 2.0, (double)height / 2.0);
				Ellipse val5 = new Ellipse(val4, (double)(width / 2 - width / 12));
				obj2.Render((IVertexSource)new Stroke((IVertexSource)(object)val5, (double)(width / 12)), (IColorType)(object)RGBA_Bytes.White);
				obj2.DrawString("GCode", val4.x, val4.y, (double)(8 * width / 50), (Justification)1, (Baseline)1, RGBA_Bytes.White, false, default(RGBA_Bytes));
				return val;
			}
			if (!File.Exists(itemWrapper.FileLocation))
			{
				val.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				Graphics2D obj3 = val.NewGraphics2D();
				Vector2 val6 = default(Vector2);
				((Vector2)(ref val6))._002Ector((double)width / 2.0, (double)height / 2.0);
				obj3.DrawString("Missing", val6.x, val6.y, (double)(8 * width / 50), (Justification)1, (Baseline)1, RGBA_Bytes.White, false, default(RGBA_Bytes));
				return val;
			}
			if (MeshIsTooBigToLoad(itemWrapper.FileLocation))
			{
				val.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				Graphics2D obj4 = val.NewGraphics2D();
				Vector2 val7 = default(Vector2);
				((Vector2)(ref val7))._002Ector((double)width / 2.0, (double)height / 2.0);
				double num = (double)(8 * width / 50) * GuiWidget.get_DeviceScale() * 2.0;
				obj4.DrawString("Reduce\nPolygons\nto\nRender", val7.x, val7.y + num, (double)(8 * width / 50), (Justification)1, (Baseline)1, RGBA_Bytes.White, false, default(RGBA_Bytes));
				return val;
			}
			string text = itemWrapper.FileHashCode.ToString();
			if (text != "0")
			{
				ImageBuffer val8 = LoadImageFromDisk(text);
				if (val8 != (ImageBuffer)null)
				{
					val = val8;
				}
				val8.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				val = ImageBuffer.CreateScaledImage(val8, width, height);
			}
			val.MarkImageChanged();
			return val;
		}

		private bool SetImageFast()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Expected O, but got Unknown
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Expected O, but got Unknown
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Expected O, but got Unknown
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Expected O, but got Unknown
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Expected O, but got Unknown
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Expected O, but got Unknown
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Expected O, but got Unknown
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Expected O, but got Unknown
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Expected O, but got Unknown
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Expected O, but got Unknown
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Expected O, but got Unknown
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Expected O, but got Unknown
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Expected O, but got Unknown
			if (ItemWrapper == null)
			{
				thumbnailImage = new ImageBuffer(noThumbnailImage);
				((GuiWidget)this).Invalidate();
				return true;
			}
			if (ItemWrapper.FileLocation == QueueData.SdCardFileName)
			{
				switch (Size)
				{
				case ImageSizes.Size115x115:
					thumbnailImage = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon(Path.ChangeExtension("icon_sd_card_115x115", partExtension)));
					break;
				case ImageSizes.Size50x50:
					thumbnailImage = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon(Path.ChangeExtension("icon_sd_card_50x50", partExtension)));
					break;
				default:
					throw new NotImplementedException();
				}
				thumbnailImage.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				Graphics2D obj = thumbnailImage.NewGraphics2D();
				Ellipse val = new Ellipse(new Vector2(((GuiWidget)this).get_Width() / 2.0, ((GuiWidget)this).get_Height() / 2.0), ((GuiWidget)this).get_Width() / 2.0 - ((GuiWidget)this).get_Width() / 12.0);
				obj.Render((IVertexSource)new Stroke((IVertexSource)(object)val, ((GuiWidget)this).get_Width() / 12.0), (IColorType)(object)RGBA_Bytes.White);
				UiThread.RunOnIdle((Action)EnsureImageUpdated);
				return true;
			}
			if (Path.GetExtension(ItemWrapper.FileLocation)!.ToUpper() == ".GCODE")
			{
				thumbnailImage = new ImageBuffer((int)((GuiWidget)this).get_Width(), (int)((GuiWidget)this).get_Height());
				thumbnailImage.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				Graphics2D obj2 = thumbnailImage.NewGraphics2D();
				Vector2 val2 = default(Vector2);
				((Vector2)(ref val2))._002Ector(((GuiWidget)this).get_Width() / 2.0, ((GuiWidget)this).get_Height() / 2.0);
				Ellipse val3 = new Ellipse(val2, ((GuiWidget)this).get_Width() / 2.0 - ((GuiWidget)this).get_Width() / 12.0);
				obj2.Render((IVertexSource)new Stroke((IVertexSource)(object)val3, ((GuiWidget)this).get_Width() / 12.0), (IColorType)(object)RGBA_Bytes.White);
				obj2.DrawString("GCode", val2.x, val2.y, 8.0 * ((GuiWidget)this).get_Width() / 50.0, (Justification)1, (Baseline)1, RGBA_Bytes.White, false, default(RGBA_Bytes));
				UiThread.RunOnIdle((Action)EnsureImageUpdated);
				return true;
			}
			if (!File.Exists(ItemWrapper.FileLocation))
			{
				thumbnailImage = new ImageBuffer((int)((GuiWidget)this).get_Width(), (int)((GuiWidget)this).get_Height());
				thumbnailImage.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				Graphics2D obj3 = thumbnailImage.NewGraphics2D();
				Vector2 val4 = default(Vector2);
				((Vector2)(ref val4))._002Ector(((GuiWidget)this).get_Width() / 2.0, ((GuiWidget)this).get_Height() / 2.0);
				obj3.DrawString("Missing", val4.x, val4.y, 8.0 * ((GuiWidget)this).get_Width() / 50.0, (Justification)1, (Baseline)1, RGBA_Bytes.White, false, default(RGBA_Bytes));
				UiThread.RunOnIdle((Action)EnsureImageUpdated);
				return true;
			}
			if (MeshIsTooBigToLoad(ItemWrapper.FileLocation))
			{
				thumbnailImage = new ImageBuffer((int)((GuiWidget)this).get_Width(), (int)((GuiWidget)this).get_Height());
				thumbnailImage.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				Graphics2D obj4 = thumbnailImage.NewGraphics2D();
				Vector2 val5 = default(Vector2);
				((Vector2)(ref val5))._002Ector(((GuiWidget)this).get_Width() / 2.0, ((GuiWidget)this).get_Height() / 2.0);
				double num = 8.0 * ((GuiWidget)this).get_Width() / 50.0 * GuiWidget.get_DeviceScale() * 2.0;
				obj4.DrawString("Reduce\nPolygons\nto\nRender", val5.x, val5.y + num, 8.0 * ((GuiWidget)this).get_Width() / 50.0, (Justification)1, (Baseline)1, RGBA_Bytes.White, false, default(RGBA_Bytes));
				UiThread.RunOnIdle((Action)EnsureImageUpdated);
				return true;
			}
			string text = ItemWrapper.FileHashCode.ToString();
			if (text != "0")
			{
				ImageBuffer val6 = LoadImageFromDisk(text);
				if (val6 == (ImageBuffer)null)
				{
					thumbnailImage = new ImageBuffer(buildingThumbnailImage);
					return false;
				}
				val6.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
				thumbnailImage = ImageBuffer.CreateScaledImage(val6, (int)((GuiWidget)this).get_Width(), (int)((GuiWidget)this).get_Height());
				UiThread.RunOnIdle((Action)EnsureImageUpdated);
				return true;
			}
			return false;
		}
	}
}
