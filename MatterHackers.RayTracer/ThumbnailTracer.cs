using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.OpenGlGui;
using MatterHackers.Agg.RasterizerScanline;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.PolygonMesh;
using MatterHackers.RayTracer.Light;
using MatterHackers.RayTracer.Traceable;
using MatterHackers.VectorMath;
using MatterHackers.WellPlate;

namespace MatterHackers.RayTracer
{
	public class ThumbnailTracer
	{
		internal class RenderPoint
		{
			internal Vector2 position;

			internal double z;

			internal RGBA_Bytes color;
		}

		public sealed class BlenderZBuffer : BlenderBase8888, IRecieveBlenderByte
		{
			public RGBA_Bytes PixelToColorRGBA_Bytes(byte[] buffer, int bufferOffset)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				return new RGBA_Bytes((int)buffer[bufferOffset + 2], (int)buffer[bufferOffset + 1], (int)buffer[bufferOffset], (int)buffer[bufferOffset + 3]);
			}

			public void CopyPixels(byte[] buffer, int bufferOffset, RGBA_Bytes sourceColor, int count)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				do
				{
					if (sourceColor.green > buffer[bufferOffset + 1])
					{
						buffer[bufferOffset + 2] = sourceColor.red;
						buffer[bufferOffset + 1] = sourceColor.green;
						buffer[bufferOffset] = sourceColor.blue;
						buffer[bufferOffset + 3] = byte.MaxValue;
					}
					else if (sourceColor.green == buffer[bufferOffset + 1] && sourceColor.blue > buffer[bufferOffset])
					{
						buffer[bufferOffset + 2] = sourceColor.red;
						buffer[bufferOffset + 1] = sourceColor.green;
						buffer[bufferOffset] = sourceColor.blue;
						buffer[bufferOffset + 3] = byte.MaxValue;
					}
					bufferOffset += 4;
				}
				while (--count != 0);
			}

			public void BlendPixel(byte[] buffer, int bufferOffset, RGBA_Bytes sourceColor)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				if (sourceColor.green > buffer[bufferOffset + 1])
				{
					buffer[bufferOffset + 2] = sourceColor.red;
					buffer[bufferOffset + 1] = sourceColor.green;
					buffer[bufferOffset] = sourceColor.blue;
					buffer[bufferOffset + 3] = byte.MaxValue;
				}
				else if (sourceColor.green == buffer[bufferOffset + 1] && sourceColor.blue > buffer[bufferOffset])
				{
					buffer[bufferOffset + 2] = sourceColor.red;
					buffer[bufferOffset + 1] = sourceColor.green;
					buffer[bufferOffset] = sourceColor.blue;
					buffer[bufferOffset + 3] = byte.MaxValue;
				}
			}

			public void BlendPixels(byte[] destBuffer, int bufferOffset, RGBA_Bytes[] sourceColors, int sourceColorsOffset, byte[] covers, int coversIndex, bool firstCoverForAll, int count)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				do
				{
					BlendPixel(destBuffer, bufferOffset, sourceColors[sourceColorsOffset]);
					bufferOffset += 4;
					sourceColorsOffset++;
				}
				while (--count != 0);
			}

			public BlenderZBuffer()
				: this()
			{
			}
		}

		private class TrackBallCamera : ICamera
		{
			private TrackballTumbleWidget trackballTumbleWidget;

			public TrackBallCamera(TrackballTumbleWidget trackballTumbleWidget)
			{
				this.trackballTumbleWidget = trackballTumbleWidget;
			}

			public Ray GetRay(double screenX, double screenY)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				return trackballTumbleWidget.GetRayFromScreen(new Vector2(screenX, screenY));
			}
		}

		public ImageBuffer destImage;

		private IPrimitive allObjects;

		private Transform allObjectsHolder;

		private List<MeshGroup> loadedMeshGroups;

		private RayTracer rayTracer;

		private List<IPrimitive> renderCollection;

		private Scene scene;

		private Point2D size;

		public TrackballTumbleWidget trackballTumbleWidget;

		private static Vector3 lightNormal;

		private static RGBA_Floats lightIllumination;

		private static RGBA_Floats ambiantIllumination;

		public ThumbnailTracer(List<MeshGroup> meshGroups, int width, int height)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			RayTracer val = new RayTracer();
			val.set_AntiAliasing((AntiAliasing)32);
			val.set_MultiThreaded(false);
			rayTracer = val;
			renderCollection = new List<IPrimitive>();
			base._002Ector();
			size = new Point2D(width, height);
			trackballTumbleWidget = new TrackballTumbleWidget();
			trackballTumbleWidget.set_DoOpenGlDrawing(false);
			((GuiWidget)trackballTumbleWidget).set_LocalBounds(new RectangleDouble(0.0, 0.0, (double)width, (double)height));
			loadedMeshGroups = meshGroups;
			SetRenderPosition(loadedMeshGroups);
			((GuiWidget)trackballTumbleWidget).AnchorCenter();
		}

		public void DoTrace()
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			CreateScene();
			RectangleInt val = default(RectangleInt);
			((RectangleInt)(ref val))._002Ector(0, 0, size.x, size.y);
			if (destImage == (ImageBuffer)null || destImage.get_Width() != ((RectangleInt)(ref val)).get_Width() || destImage.get_Height() != ((RectangleInt)(ref val)).get_Height())
			{
				destImage = new ImageBuffer(((RectangleInt)(ref val)).get_Width(), ((RectangleInt)(ref val)).get_Height());
			}
			rayTracer.set_MultiThreaded(!PrinterConnectionAndCommunication.Instance.PrinterIsPrinting);
			rayTracer.RayTraceScene(val, scene);
			rayTracer.CopyColorBufferToImage(destImage, val);
		}

		public void SetRenderPosition(List<MeshGroup> loadedMeshGroups)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			trackballTumbleWidget.get_TrackBallController().Reset();
			trackballTumbleWidget.get_TrackBallController().set_Scale(0.03);
			trackballTumbleWidget.get_TrackBallController().Rotate(Quaternion.FromEulerAngles(new Vector3(0.0, 0.0, 0.39269909262657166)));
			trackballTumbleWidget.get_TrackBallController().Rotate(Quaternion.FromEulerAngles(new Vector3(-1.1938052415847777, 0.0, 0.0)));
			ScaleMeshToView(loadedMeshGroups);
		}

		private void AddAFloor()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Expected O, but got Unknown
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Expected O, but got Unknown
			ImageBuffer val = new ImageBuffer(200, 200);
			Graphics2D val2 = val.NewGraphics2D();
			Random random = new Random(0);
			RGBA_Bytes val3 = default(RGBA_Bytes);
			for (int i = 0; i < 100; i++)
			{
				((RGBA_Bytes)(ref val3))._002Ector(random.NextDouble(), random.NextDouble(), random.NextDouble());
				val2.Circle(new Vector2(random.NextDouble() * (double)val.get_Width(), random.NextDouble() * (double)val.get_Height()), random.NextDouble() * 40.0 + 10.0, val3);
			}
			scene.shapes.Add((IPrimitive)new PlaneShape(new Vector3(0.0, 0.0, 1.0), 0.0, (MaterialAbstract)new TextureMaterial(val, 0.0, 0.0, 0.2, 1.0)));
		}

		internal void render_gouraud(IImageByte backBuffer, IScanlineCache sl, IRasterizer ras, RenderPoint[] points)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			ImageBuffer val = new ImageBuffer();
			val.Attach(backBuffer, (IRecieveBlenderByte)(object)new BlenderZBuffer());
			ImageClippingProxy val2 = new ImageClippingProxy((IImageByte)val);
			span_allocator val3 = new span_allocator();
			span_gouraud_rgba val4 = new span_gouraud_rgba();
			((span_gouraud)val4).colors((IColorType)(object)points[0].color, (IColorType)(object)points[1].color, (IColorType)(object)points[2].color);
			((span_gouraud)val4).triangle(points[0].position.x, points[0].position.y, points[1].position.x, points[1].position.y, points[2].position.x, points[2].position.y, 0.0);
			ras.add_path((IVertexSource)(object)val4);
			new ScanlineRenderer().GenerateAndRender(ras, sl, (IImageByte)(object)val2, val3, (ISpanGenerator)(object)val4);
		}

		public void DrawTo(Graphics2D graphics2D, Mesh meshToDraw, RGBA_Bytes partColorIn, double minZ, double maxZ)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Expected O, but got Unknown
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Expected O, but got Unknown
			RGBA_Floats asRGBA_Floats = ((RGBA_Bytes)(ref partColorIn)).GetAsRGBA_Floats();
			graphics2D.get_Rasterizer().gamma((IGammaFunction)new gamma_power(0.3));
			RenderPoint[] array = new RenderPoint[3]
			{
				new RenderPoint(),
				new RenderPoint(),
				new RenderPoint()
			};
			foreach (Face face in meshToDraw.get_Faces())
			{
				int num = 0;
				Vector3 val = Vector3.TransformVector(face.normal, trackballTumbleWidget.get_ModelviewMatrix());
				Vector3 normal = ((Vector3)(ref val)).GetNormal();
				if (!(normal.z > 0.0))
				{
					continue;
				}
				foreach (FaceEdge item in face.FaceEdges())
				{
					array[num].position = trackballTumbleWidget.GetScreenPosition(item.firstVertex.get_Position());
					Vector3 val2 = Vector3.TransformPosition(item.firstVertex.get_Position(), trackballTumbleWidget.get_ModelviewMatrix());
					array[num].z = val2.z;
					num++;
				}
				RGBA_Floats val3 = default(RGBA_Floats);
				double num2 = Vector3.Dot(lightNormal, normal);
				if (num2 > 0.0)
				{
					val3 = asRGBA_Floats * lightIllumination * num2;
				}
				val3 = RGBA_Floats.ComponentMax(val3, asRGBA_Floats * ambiantIllumination);
				for (num = 0; num < 3; num++)
				{
					int num3 = (int)((array[num].z - minZ) / (maxZ - minZ) * 65536.0);
					array[num].color = new RGBA_Bytes(((RGBA_Floats)(ref val3)).get_Red0To255(), num3 >> 8, num3 & 0xFF);
				}
				scanline_unpacked_8 sl = new scanline_unpacked_8();
				ScanlineRasterizer ras = new ScanlineRasterizer();
				render_gouraud(graphics2D.get_DestImage(), (IScanlineCache)(object)sl, (IRasterizer)(object)ras, array);
				byte[] buffer = graphics2D.get_DestImage().GetBuffer();
				int num4 = ((IImage)graphics2D.get_DestImage()).get_Width() * ((IImage)graphics2D.get_DestImage()).get_Height();
				for (int i = 0; i < num4; i++)
				{
					buffer[i * 4 + 2] = buffer[i * 4 + 2];
					buffer[i * 4 + 1] = buffer[i * 4 + 2];
					buffer[i * 4] = buffer[i * 4 + 2];
				}
			}
		}

		private AxisAlignedBoundingBox GetAxisAlignedBoundingBox(List<MeshGroup> meshGroups)
		{
			AxisAlignedBoundingBox val = AxisAlignedBoundingBox.get_Empty();
			bool flag = true;
			foreach (MeshGroup meshGroup in meshGroups)
			{
				AxisAlignedBoundingBox axisAlignedBoundingBox = meshGroup.GetAxisAlignedBoundingBox();
				if (flag)
				{
					val = axisAlignedBoundingBox;
					flag = false;
				}
				else
				{
					val = AxisAlignedBoundingBox.Union(val, axisAlignedBoundingBox);
				}
			}
			return val;
		}

		private void AddTestMesh(List<MeshGroup> meshGroups)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			if (meshGroups == null)
			{
				return;
			}
			AxisAlignedBoundingBox axisAlignedBoundingBox = GetAxisAlignedBoundingBox(meshGroups);
			loadedMeshGroups = meshGroups;
			Vector3 center = axisAlignedBoundingBox.get_Center();
			foreach (MeshGroup meshGroup in meshGroups)
			{
				meshGroup.Translate(-center);
			}
			ScaleMeshToView(loadedMeshGroups);
			RGBA_Bytes white = default(RGBA_Bytes);
			((RGBA_Bytes)(ref white))._002Ector(0, 130, 153);
			white = RGBA_Bytes.White;
			IPrimitive item = MeshToBVH.Convert(loadedMeshGroups, (MaterialAbstract)new SolidMaterial(((RGBA_Bytes)(ref white)).GetAsRGBA_Floats(), 0.01, 0.0, 2.0));
			renderCollection.Add(item);
		}

		private void CreateScene()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Expected O, but got Unknown
			scene = new Scene((ICamera)null);
			scene.camera = (ICamera)(object)new TrackBallCamera(trackballTumbleWidget);
			scene.background = new Background(new RGBA_Floats(1f, 1f, 1f, 0f), 0.6);
			AddTestMesh(loadedMeshGroups);
			allObjects = BoundingVolumeHierarchy.CreateNewHierachy(renderCollection, int.MaxValue, 0, (SortingAccelerator)null);
			allObjectsHolder = new Transform(allObjects);
			scene.shapes.Add((IPrimitive)(object)allObjectsHolder);
			scene.lights.Add((ILight)new PointLight(new Vector3(-5000.0, -5000.0, 3000.0), new RGBA_Floats(0.5, 0.5, 0.5)));
		}

		private RectangleDouble GetScreenBounds(AxisAlignedBoundingBox meshBounds)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble zeroIntersection = RectangleDouble.ZeroIntersection;
			((RectangleDouble)(ref zeroIntersection)).ExpandToInclude(trackballTumbleWidget.GetScreenPosition(new Vector3(meshBounds.minXYZ.x, meshBounds.minXYZ.y, meshBounds.minXYZ.z)));
			((RectangleDouble)(ref zeroIntersection)).ExpandToInclude(trackballTumbleWidget.GetScreenPosition(new Vector3(meshBounds.maxXYZ.x, meshBounds.minXYZ.y, meshBounds.minXYZ.z)));
			((RectangleDouble)(ref zeroIntersection)).ExpandToInclude(trackballTumbleWidget.GetScreenPosition(new Vector3(meshBounds.maxXYZ.x, meshBounds.maxXYZ.y, meshBounds.minXYZ.z)));
			((RectangleDouble)(ref zeroIntersection)).ExpandToInclude(trackballTumbleWidget.GetScreenPosition(new Vector3(meshBounds.minXYZ.x, meshBounds.maxXYZ.y, meshBounds.minXYZ.z)));
			((RectangleDouble)(ref zeroIntersection)).ExpandToInclude(trackballTumbleWidget.GetScreenPosition(new Vector3(meshBounds.minXYZ.x, meshBounds.minXYZ.y, meshBounds.maxXYZ.z)));
			((RectangleDouble)(ref zeroIntersection)).ExpandToInclude(trackballTumbleWidget.GetScreenPosition(new Vector3(meshBounds.maxXYZ.x, meshBounds.minXYZ.y, meshBounds.maxXYZ.z)));
			((RectangleDouble)(ref zeroIntersection)).ExpandToInclude(trackballTumbleWidget.GetScreenPosition(new Vector3(meshBounds.maxXYZ.x, meshBounds.maxXYZ.y, meshBounds.maxXYZ.z)));
			((RectangleDouble)(ref zeroIntersection)).ExpandToInclude(trackballTumbleWidget.GetScreenPosition(new Vector3(meshBounds.minXYZ.x, meshBounds.maxXYZ.y, meshBounds.maxXYZ.z)));
			return zeroIntersection;
		}

		public void GetMinMaxZ(Mesh mesh, ref double minZ, ref double maxZ)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			AxisAlignedBoundingBox axisAlignedBoundingBox = mesh.GetAxisAlignedBoundingBox(trackballTumbleWidget.get_ModelviewMatrix());
			minZ = Math.Min(axisAlignedBoundingBox.minXYZ.z, minZ);
			maxZ = Math.Max(axisAlignedBoundingBox.maxXYZ.z, maxZ);
		}

		private bool NeedsToBeSmaller(RectangleDouble partScreenBounds, RectangleDouble goalBounds)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			if (partScreenBounds.Bottom < goalBounds.Bottom || partScreenBounds.Top > goalBounds.Top || partScreenBounds.Left < goalBounds.Left || partScreenBounds.Right > goalBounds.Right)
			{
				return true;
			}
			return false;
		}

		private void ScaleMeshToView(List<MeshGroup> loadedMeshGroups)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			if (loadedMeshGroups == null)
			{
				return;
			}
			AxisAlignedBoundingBox axisAlignedBoundingBox = GetAxisAlignedBoundingBox(loadedMeshGroups);
			bool flag = false;
			double num = 0.1;
			RectangleDouble goalBounds = default(RectangleDouble);
			((RectangleDouble)(ref goalBounds))._002Ector(0.0, 0.0, (double)size.x, (double)size.y);
			((RectangleDouble)(ref goalBounds)).Inflate(-10);
			while (!flag)
			{
				RectangleDouble screenBounds = GetScreenBounds(axisAlignedBoundingBox);
				if (!NeedsToBeSmaller(screenBounds, goalBounds))
				{
					TrackBallController trackBallController = trackballTumbleWidget.get_TrackBallController();
					trackBallController.set_Scale(trackBallController.get_Scale() * (1.0 + num));
					screenBounds = GetScreenBounds(axisAlignedBoundingBox);
					if (NeedsToBeSmaller(screenBounds, goalBounds))
					{
						num /= 2.0;
					}
					continue;
				}
				TrackBallController trackBallController2 = trackballTumbleWidget.get_TrackBallController();
				trackBallController2.set_Scale(trackBallController2.get_Scale() * (1.0 - num));
				screenBounds = GetScreenBounds(axisAlignedBoundingBox);
				if (!NeedsToBeSmaller(screenBounds, goalBounds))
				{
					num /= 2.0;
					if (num < 0.001)
					{
						flag = true;
					}
				}
			}
		}

		static ThumbnailTracer()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = new Vector3(-1.0, 1.0, 1.0);
			lightNormal = ((Vector3)(ref val)).GetNormal();
			lightIllumination = new RGBA_Floats(1f, 1f, 1f);
			ambiantIllumination = new RGBA_Floats(0.4, 0.4, 0.4);
		}
	}
}
