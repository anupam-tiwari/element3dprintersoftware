using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.MatterControl.Utilities;
using MatterHackers.PolygonMesh;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.RayTracer;
using MatterHackers.RayTracer.Traceable;
using MatterHackers.RenderOpenGl;
using MatterHackers.RenderOpenGl.OpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.MeshVisualizer
{
	public class MeshViewerWidget : GuiWidget
	{
		public enum CenterPartAfterLoad
		{
			DO,
			DONT
		}

		public class PartProcessingInfo : FlowLayoutWidget
		{
			internal TextWidget centeredInfoDescription;

			internal TextWidget centeredInfoText;

			internal ProgressControl progressControl;

			internal PartProcessingInfo(string startingTextMessage)
				: this((FlowDirection)3)
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Expected O, but got Unknown
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Expected O, but got Unknown
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Expected O, but got Unknown
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_013a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				progressControl = new ProgressControl("", RGBA_Bytes.Black, RGBA_Bytes.Black, 80, 15, 5);
				((GuiWidget)progressControl).set_HAnchor((HAnchor)2);
				((GuiWidget)this).AddChild((GuiWidget)(object)progressControl, -1);
				((GuiWidget)progressControl).set_Visible(false);
				progressControl.add_ProgressChanged((EventHandler)delegate
				{
					((GuiWidget)progressControl).set_Visible(true);
				});
				centeredInfoText = new TextWidget(startingTextMessage, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)centeredInfoText).set_HAnchor((HAnchor)2);
				centeredInfoText.set_AutoExpandBoundsToText(true);
				((GuiWidget)this).AddChild((GuiWidget)(object)centeredInfoText, -1);
				centeredInfoDescription = new TextWidget("", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				((GuiWidget)centeredInfoDescription).set_HAnchor((HAnchor)2);
				centeredInfoDescription.set_AutoExpandBoundsToText(true);
				((GuiWidget)this).AddChild((GuiWidget)(object)centeredInfoDescription, -1);
				((GuiWidget)this).set_VAnchor((VAnchor)(((GuiWidget)this).get_VAnchor() | 2));
				((GuiWidget)this).set_HAnchor((HAnchor)(((GuiWidget)this).get_HAnchor() | 2));
			}
		}

		public static ImageBuffer BedImage = null;

		public List<InteractionVolume> interactionVolumes = new List<InteractionVolume>();

		public InteractionVolume SelectedInteractionVolume;

		public PartProcessingInfo partProcessingInfo;

		private static ImageBuffer lastCreatedBedImage = new ImageBuffer();

		private static Dictionary<int, RGBA_Bytes> materialColors = new Dictionary<int, RGBA_Bytes>();

		private BackgroundWorker backgroundWorker;

		private RGBA_Bytes bedBaseColor = new RGBA_Bytes(245, 245, 255);

		private RGBA_Bytes bedMarkingsColor = RGBA_Bytes.Black;

		private static BedShape bedShape = BedShape.Rectangular;

		private static Mesh buildVolume = null;

		private static Vector3 displayVolume;

		private List<MeshGroup> meshesToRender = new List<MeshGroup>();

		private List<Matrix4X4> meshTransforms = new List<Matrix4X4>();

		private static Mesh printerBed = null;

		private static Mesh printerGrid = null;

		private RenderTypes renderType = (RenderTypes)1;

		private EventList<int> selectedMeshGroupIndices = new EventList<int>();

		private TrackballTumbleWidget trackballTumbleWidget;

		private int volumeIndexWithMouseDown = -1;

		private static RGBA_Bytes[] presetMaterialColors = (RGBA_Bytes[])(object)new RGBA_Bytes[9]
		{
			new RGBA_Bytes(143, 132, 255),
			new RGBA_Bytes(255, 112, 255),
			new RGBA_Bytes(164, 255, 240),
			new RGBA_Bytes(54, 34, 255),
			new RGBA_Bytes(196, 0, 196),
			new RGBA_Bytes(0, 184, 165),
			new RGBA_Bytes(34, 17, 201),
			new RGBA_Bytes(108, 0, 108),
			new RGBA_Bytes(0, 106, 88)
		};

		public bool MouseDownOnInteractionVolume => SelectedInteractionVolume != null;

		public static Vector2 BedCenter
		{
			get;
			private set;
		}

		public double SnapGridDistance
		{
			get;
			set;
		} = 1.0;


		public bool AllowBedRenderingWhenEmpty
		{
			get;
			set;
		}

		public RGBA_Bytes BedColor
		{
			get;
			set;
		}

		public RGBA_Bytes BuildVolumeColor
		{
			get;
			set;
		}

		public Vector3 DisplayVolume => displayVolume;

		public bool HaveSelection
		{
			get
			{
				if (MeshGroups.Count > 0)
				{
					return SelectedMeshGroupIndices.Count > 0;
				}
				return false;
			}
		}

		public List<MeshGroup> MeshGroups => meshesToRender;

		public List<Matrix4X4> MeshGroupTransforms => meshTransforms;

		public Mesh PrinterBed => printerBed;

		public bool RenderBed
		{
			get;
			set;
		}

		public static bool RenderGrid
		{
			get;
			set;
		}

		public bool RenderBuildVolume
		{
			get;
			set;
		}

		public RenderTypes RenderType
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return renderType;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				if (renderType == value)
				{
					return;
				}
				renderType = value;
				foreach (MeshGroup meshGroup in MeshGroups)
				{
					foreach (Mesh mesh in meshGroup.get_Meshes())
					{
						mesh.MarkAsChanged();
					}
				}
			}
		}

		public EventHandler SelectionChanged
		{
			get
			{
				return selectedMeshGroupIndices.ListChanged;
			}
			set
			{
				selectedMeshGroupIndices.ListChanged = value;
			}
		}

		public List<MeshGroup> SelectedMeshGroups
		{
			get
			{
				if (HaveSelection)
				{
					List<MeshGroup> list = new List<MeshGroup>();
					{
						foreach (int selectedMeshGroupIndex in SelectedMeshGroupIndices)
						{
							list.Add(MeshGroups[selectedMeshGroupIndex]);
						}
						return list;
					}
				}
				return null;
			}
		}

		public EventList<int> SelectedMeshGroupIndices => selectedMeshGroupIndices;

		public List<Matrix4X4> SelectedMeshGroupTransforms
		{
			get
			{
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				if (HaveSelection)
				{
					List<Matrix4X4> list = new List<Matrix4X4>();
					{
						foreach (int selectedMeshGroupIndex in SelectedMeshGroupIndices)
						{
							list.Add(MeshGroupTransforms[selectedMeshGroupIndex]);
						}
						return list;
					}
				}
				return null;
			}
		}

		public MeshGroup SelectedMeshGroup
		{
			get
			{
				if (HaveSelection)
				{
					return MeshGroups[SelectedMeshGroupIndex];
				}
				return null;
			}
		}

		public int SelectedMeshGroupIndex
		{
			get
			{
				if (SelectedMeshGroupIndices.Count > 0)
				{
					if (SelectedMeshGroupIndices[0] >= MeshGroups.Count)
					{
						SelectedMeshGroupIndices[0] = MeshGroups.Count - 1;
					}
					return SelectedMeshGroupIndices[0];
				}
				return -1;
			}
			set
			{
				SelectedMeshGroupIndices.Clear();
				if (value >= 0)
				{
					SelectedMeshGroupIndices.Add(value);
				}
			}
		}

		public Matrix4X4 SelectedMeshGroupTransform
		{
			get
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (HaveSelection)
				{
					return MeshGroupTransforms[SelectedMeshGroupIndex];
				}
				return Matrix4X4.Identity;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				MeshGroupTransforms[SelectedMeshGroupIndex] = value;
			}
		}

		public TrackballTumbleWidget TrackballTumbleWidget => trackballTumbleWidget;

		public event EventHandler LoadDone;

		public MeshViewerWidget(Vector3 displayVolume, Vector2 bedCenter, BedShape bedShape, string startingTextMessage = "")
			: this()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Expected O, but got Unknown
			RenderType = (RenderTypes)1;
			RenderBed = true;
			RenderBuildVolume = false;
			RGBA_Floats val = new RGBA_Floats(0.8, 0.8, 0.8, 1.0);
			BedColor = ((RGBA_Floats)(ref val)).GetAsRGBA_Bytes();
			val = new RGBA_Floats(0.2, 0.8, 0.3, 0.2);
			BuildVolumeColor = ((RGBA_Floats)(ref val)).GetAsRGBA_Bytes();
			trackballTumbleWidget = new TrackballTumbleWidget();
			trackballTumbleWidget.DrawRotationHelperCircle = false;
			trackballTumbleWidget.DrawGlContent += trackballTumbleWidget_DrawGlContent;
			trackballTumbleWidget.TransformState = (MouseDownType)2;
			((GuiWidget)this).AddChild((GuiWidget)(object)trackballTumbleWidget, -1);
			CreatePrintBed(displayVolume, bedCenter, bedShape);
			((GuiWidget)trackballTumbleWidget).AnchorAll();
			partProcessingInfo = new PartProcessingInfo(startingTextMessage);
			GuiWidget val2 = new GuiWidget();
			val2.AnchorAll();
			val2.AddChild((GuiWidget)(object)partProcessingInfo, -1);
			val2.set_Selectable(false);
			((GuiWidget)this).AddChild(val2, -1);
		}

		public static AxisAlignedBoundingBox GetAxisAlignedBoundingBox(List<MeshGroup> meshGroups)
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

		public static void AssertDebugNotDefined()
		{
		}

		public static RGBA_Bytes GetOGMaterialColor(int materialIndexBase1)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return presetMaterialColors[(materialIndexBase1 - 1) % presetMaterialColors.Length];
		}

		public static RGBA_Bytes GetMaterialColor(int materialIndexBase1)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			lock (materialColors)
			{
				string text = UserSettings.Instance.get(StringHelper.FormatWith("MaterialColor{0}", new object[1]
				{
					materialIndexBase1
				}));
				if (text != null)
				{
					string[] array = text.Split(new char[1]
					{
						','
					});
					RGBA_Bytes val = default(RGBA_Bytes);
					((RGBA_Bytes)(ref val))._002Ector(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]));
					SetMaterialColor(materialIndexBase1, val);
					return val;
				}
				if (materialColors.ContainsKey(materialIndexBase1))
				{
					return materialColors[materialIndexBase1];
				}
			}
			return GetOGMaterialColor(materialIndexBase1);
		}

		public static RGBA_Bytes GetSelectedMaterialColor(int materialIndexBase1)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			RGBA_Bytes materialColor = GetMaterialColor(materialIndexBase1);
			RGBA_Floats val = ((RGBA_Bytes)(ref materialColor)).GetAsRGBA_Floats();
			double num = default(double);
			double num2 = default(double);
			double num3 = default(double);
			((RGBA_Floats)(ref val)).GetHSL(ref num, ref num2, ref num3);
			num2 = Math.Min(1.0, num2 * 2.0);
			num3 = Math.Min(1.0, num3 * 1.2);
			val = RGBA_Floats.FromHSL(num, num2, num3, 1.0);
			return ((RGBA_Floats)(ref val)).GetAsRGBA_Bytes();
		}

		public static void SetMaterialColor(int materialIndexBase1, RGBA_Bytes color)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			lock (materialColors)
			{
				if (!materialColors.ContainsKey(materialIndexBase1))
				{
					materialColors.Add(materialIndexBase1, color);
				}
				else
				{
					materialColors[materialIndexBase1] = color;
				}
				UserSettings.Instance.set(StringHelper.FormatWith("MaterialColor{0}", new object[1]
				{
					materialIndexBase1
				}), StringHelper.FormatWith("{0},{1},{2},{3}", new object[4]
				{
					((RGBA_Bytes)(ref color)).get_Red0To255(),
					((RGBA_Bytes)(ref color)).get_Green0To255(),
					((RGBA_Bytes)(ref color)).get_Blue0To255(),
					((RGBA_Bytes)(ref color)).get_Alpha0To255()
				}));
			}
		}

		public void CreateGlDataForMeshes(List<MeshGroup> meshGroupsToPrepare)
		{
			for (int i = 0; i < meshGroupsToPrepare.Count; i++)
			{
				foreach (Mesh mesh in meshGroupsToPrepare[i].get_Meshes())
				{
					GLMeshTrianglePlugin.Get(mesh);
				}
			}
		}

		public void CreatePrintBed(Vector3 displayVolume, Vector2 bedCenter, BedShape bedShape)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Expected O, but got Unknown
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			if (BedCenter == bedCenter && MeshViewerWidget.bedShape == bedShape && MeshViewerWidget.displayVolume == displayVolume)
			{
				return;
			}
			BedCenter = bedCenter;
			MeshViewerWidget.bedShape = bedShape;
			MeshViewerWidget.displayVolume = displayVolume;
			Vector3 val = Vector3.ComponentMax(displayVolume, new Vector3(1.0, 1.0, 1.0));
			double num = Math.Max(val.x, val.y);
			double divisor = 10.0;
			int num2 = 1;
			if (num > 1000.0)
			{
				divisor = 100.0;
				num2 = 10;
			}
			else if (num > 300.0)
			{
				divisor = 50.0;
				num2 = 5;
			}
			if (val.z > 0.0)
			{
				buildVolume = PlatonicSolids.CreateCube(val);
				foreach (Vertex vertex in buildVolume.get_Vertices())
				{
					vertex.set_Position(vertex.get_Position() + new Vector3(0.0, 0.0, val.z / 2.0));
				}
			}
			CreateRectangularBedGridImage(val, bedCenter, divisor, num2);
			printerGrid = PlatonicSolids.CreateCube(val.x, val.y, 4.0);
			CommonShapes.PlaceTextureOnFace(printerGrid.get_Faces()[0], BedImage);
			ImageBuffer val3 = new ImageBuffer();
			StaticData.get_Instance().LoadImage("stage-color.jpg", val3);
			printerBed = MeshFileIo.Load(StaticData.get_Instance().MapPath("stage.stl"), (ReportProgressRatio)null)[0].get_Meshes()[0];
			foreach (Face face in printerBed.get_Faces())
			{
				CommonShapes.PlaceTextureOnFace(face, val3);
			}
			foreach (Vertex vertex2 in printerBed.get_Vertices())
			{
				vertex2.set_Position(vertex2.get_Position() - new Vector3(-bedCenter, 2.2));
			}
			foreach (Vertex vertex3 in printerGrid.get_Vertices())
			{
				vertex3.set_Position(vertex3.get_Position() - new Vector3(-bedCenter, 2.0));
			}
			if (buildVolume != null)
			{
				foreach (Vertex vertex4 in buildVolume.get_Vertices())
				{
					vertex4.set_Position(vertex4.get_Position() - new Vector3(-bedCenter, 2.2));
				}
			}
			((GuiWidget)this).Invalidate();
		}

		public AxisAlignedBoundingBox GetBoundsForSelection()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			AxisAlignedBoundingBox val = AxisAlignedBoundingBox.get_Empty();
			foreach (int selectedMeshGroupIndex in SelectedMeshGroupIndices)
			{
				Matrix4X4 val2 = MeshGroupTransforms[selectedMeshGroupIndex];
				MeshGroup val3 = MeshGroups[selectedMeshGroupIndex];
				val += val3.GetAxisAlignedBoundingBox(val2);
			}
			return val;
		}

		public void LoadMesh(string meshPathAndFileName, CenterPartAfterLoad centerPart, Vector2 bedCenter = default(Vector2))
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			if (File.Exists(meshPathAndFileName))
			{
				((GuiWidget)partProcessingInfo).set_Visible(true);
				partProcessingInfo.progressControl.set_PercentComplete(0);
				backgroundWorker = new BackgroundWorker();
				backgroundWorker.set_WorkerSupportsCancellation(true);
				backgroundWorker.add_RunWorkerCompleted(new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted));
				backgroundWorker.add_DoWork((DoWorkEventHandler)delegate(object sender, DoWorkEventArgs e)
				{
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_001c: Expected O, but got Unknown
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					List<MeshGroup> list = MeshFileIo.Load(meshPathAndFileName, new ReportProgressRatio(reportProgress0to100));
					SetMeshAfterLoad(list, centerPart, bedCenter);
					e.set_Result((object)list);
				});
				backgroundWorker.RunWorkerAsync();
				((GuiWidget)partProcessingInfo.centeredInfoText).set_Text("Loading Mesh...");
			}
			else
			{
				((GuiWidget)partProcessingInfo.centeredInfoText).set_Text(string.Format("{0}\n'{1}'", "File not found on disk.", Path.GetFileName(meshPathAndFileName)));
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (backgroundWorker != null)
			{
				backgroundWorker.CancelAsync();
			}
			((GuiWidget)this).OnClosed(e);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			((GuiWidget)this).OnDraw(graphics2D);
			foreach (InteractionVolume interactionVolume in interactionVolumes)
			{
				interactionVolume.Draw2DContent(graphics2D);
			}
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Invalid comparison between Unknown and I4
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Invalid comparison between Unknown and I4
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnMouseDown(mouseEvent);
			if (((GuiWidget)trackballTumbleWidget).get_MouseCaptured() && ((int)trackballTumbleWidget.TransformState == 2 || (int)mouseEvent.get_Button() == 2097152))
			{
				trackballTumbleWidget.DrawRotationHelperCircle = true;
			}
			Ray rayFromScreen = trackballTumbleWidget.GetRayFromScreen(mouseEvent.get_Position());
			if (FindInteractionVolumeHit(rayFromScreen, out var interactionVolumeHitIndex, out var info))
			{
				MouseEvent3DArgs mouseEvent3D = new MouseEvent3DArgs(mouseEvent, rayFromScreen, info);
				volumeIndexWithMouseDown = interactionVolumeHitIndex;
				interactionVolumes[interactionVolumeHitIndex].OnMouseDown(mouseEvent3D);
				SelectedInteractionVolume = interactionVolumes[interactionVolumeHitIndex];
			}
			else
			{
				SelectedInteractionVolume = null;
			}
		}

		public override void OnMouseMove(MouseEventArgs mouseEvent)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnMouseMove(mouseEvent);
			Ray rayFromScreen = trackballTumbleWidget.GetRayFromScreen(mouseEvent.get_Position());
			IntersectInfo info = null;
			if (MouseDownOnInteractionVolume && volumeIndexWithMouseDown != -1)
			{
				MouseEvent3DArgs mouseEvent3D = new MouseEvent3DArgs(mouseEvent, rayFromScreen, info);
				interactionVolumes[volumeIndexWithMouseDown].OnMouseMove(mouseEvent3D);
				return;
			}
			if (FindInteractionVolumeHit(rayFromScreen, out var interactionVolumeHitIndex, out info) && volumeIndexWithMouseDown == interactionVolumeHitIndex)
			{
				MouseEvent3DArgs mouseEvent3D2 = new MouseEvent3DArgs(mouseEvent, rayFromScreen, info);
				interactionVolumes[interactionVolumeHitIndex].OnMouseMove(mouseEvent3D2);
			}
			for (int i = 0; i < interactionVolumes.Count; i++)
			{
				if (i == interactionVolumeHitIndex)
				{
					interactionVolumes[i].MouseOver = true;
					interactionVolumes[i].MouseMoveInfo = info;
				}
				else
				{
					interactionVolumes[i].MouseOver = false;
					interactionVolumes[i].MouseMoveInfo = null;
				}
			}
		}

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			trackballTumbleWidget.DrawRotationHelperCircle = false;
			((GuiWidget)this).Invalidate();
			Ray rayFromScreen = trackballTumbleWidget.GetRayFromScreen(mouseEvent.get_Position());
			int interactionVolumeHitIndex;
			IntersectInfo info;
			bool flag = FindInteractionVolumeHit(rayFromScreen, out interactionVolumeHitIndex, out info);
			MouseEvent3DArgs mouseEvent3D = new MouseEvent3DArgs(mouseEvent, rayFromScreen, info);
			if (MouseDownOnInteractionVolume && volumeIndexWithMouseDown != -1)
			{
				interactionVolumes[volumeIndexWithMouseDown].OnMouseUp(mouseEvent3D);
				SelectedInteractionVolume = null;
				volumeIndexWithMouseDown = -1;
			}
			else
			{
				volumeIndexWithMouseDown = -1;
				if (flag)
				{
					interactionVolumes[interactionVolumeHitIndex].OnMouseUp(mouseEvent3D);
				}
				SelectedInteractionVolume = null;
			}
			((GuiWidget)this).OnMouseUp(mouseEvent);
		}

		public void SetMeshAfterLoad(List<MeshGroup> loadedMeshGroups, CenterPartAfterLoad centerPart, Vector2 bedCenter)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Expected O, but got Unknown
			MeshGroups.Clear();
			if (loadedMeshGroups == null)
			{
				((GuiWidget)partProcessingInfo.centeredInfoText).set_Text($"Sorry! No 3D view available\nfor this file.");
				return;
			}
			CreateGlDataForMeshes(loadedMeshGroups);
			AxisAlignedBoundingBox val = new AxisAlignedBoundingBox(Vector3.Zero, Vector3.Zero);
			bool flag = true;
			foreach (MeshGroup loadedMeshGroup in loadedMeshGroups)
			{
				if (flag)
				{
					val = loadedMeshGroup.GetAxisAlignedBoundingBox();
					flag = false;
				}
				else
				{
					val = AxisAlignedBoundingBox.Union(val, loadedMeshGroup.GetAxisAlignedBoundingBox());
				}
			}
			foreach (MeshGroup loadedMeshGroup2 in loadedMeshGroups)
			{
				meshTransforms.Add(Matrix4X4.Identity);
				MeshGroups.Add(loadedMeshGroup2);
			}
			if (centerPart == CenterPartAfterLoad.DO)
			{
				Vector3 val2 = (val.maxXYZ + val.minXYZ) / 2.0;
				for (int i = 0; i < MeshGroups.Count; i++)
				{
					List<Matrix4X4> list = meshTransforms;
					int index = i;
					list[index] *= Matrix4X4.CreateTranslation(-val2 + new Vector3(0.0, 0.0, val.get_ZSize() / 2.0) + new Vector3(bedCenter, 0.0));
				}
			}
			trackballTumbleWidget.TrackBallController = new TrackBallController();
			((GuiWidget)trackballTumbleWidget).OnBoundsChanged((EventArgs)null);
			ResetView();
		}

		public void ResetView()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			trackballTumbleWidget.ZeroVelocity();
			trackballTumbleWidget.TrackBallController.Reset();
			trackballTumbleWidget.TrackBallController.set_Scale(0.03);
			trackballTumbleWidget.TrackBallController.Translate(-new Vector3(BedCenter, 0.0));
			trackballTumbleWidget.TrackBallController.Rotate(Quaternion.FromEulerAngles(new Vector3(0.0, 0.0, 0.39269909262657166)));
			trackballTumbleWidget.TrackBallController.Rotate(Quaternion.FromEulerAngles(new Vector3(-1.1938052415847777, 0.0, 0.0)));
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			((GuiWidget)partProcessingInfo).set_Visible(false);
			if (this.LoadDone != null)
			{
				this.LoadDone(this, null);
			}
		}

		private void CreateCircularBedGridImage(int linesInX, int linesInY, int increment = 1)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Expected O, but got Unknown
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Expected O, but got Unknown
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			new Vector2((double)linesInX, (double)linesInY);
			BedImage = new ImageBuffer(1024, 1024);
			Graphics2D val = BedImage.NewGraphics2D();
			val.Clear((IColorType)(object)bedBaseColor);
			double num = (double)BedImage.get_Width() / (double)linesInX;
			int num2 = 1;
			int num3 = 16;
			val.DrawString(num2.ToString(), 4.0, 4.0, (double)num3, (Justification)0, (Baseline)3, bedMarkingsColor, false, default(RGBA_Bytes));
			double num4 = num;
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector((double)(BedImage.get_Width() / 2), (double)(BedImage.get_Height() / 2));
			for (double num5 = num + (double)(BedImage.get_Width() / 2); num5 < (double)BedImage.get_Width(); num5 += num)
			{
				val.DrawString((num2 * increment).ToString(), num5 + 2.0, (double)(BedImage.get_Height() / 2), (double)num3, (Justification)0, (Baseline)3, bedMarkingsColor, false, default(RGBA_Bytes));
				Stroke val3 = new Stroke((IVertexSource)new Ellipse(val2, num4), 1.0);
				val.Render((IVertexSource)(object)val3, (IColorType)(object)bedMarkingsColor);
				num4 += num;
				num2++;
			}
			val.Line(0.0, (double)(BedImage.get_Height() / 2), (double)BedImage.get_Width(), (double)(BedImage.get_Height() / 2), bedMarkingsColor, 1.0);
			val.Line((double)(BedImage.get_Width() / 2), 0.0, (double)(BedImage.get_Width() / 2), (double)BedImage.get_Height(), bedMarkingsColor, 1.0);
		}

		private void CreateRectangularBedGridImage(Vector3 displayVolumeToBuild, Vector2 bedCenter, double divisor, double skip)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			lock (lastCreatedBedImage)
			{
				BedImage = new ImageBuffer(1024, 1024);
				Graphics2D val = BedImage.NewGraphics2D();
				val.Clear((IColorType)(object)new RGBA_Bytes(255, 255, 255, 255));
				double num = (double)BedImage.get_Width() / (displayVolumeToBuild.x / divisor);
				double num2 = (0.0 - displayVolume.x / 2.0 + bedCenter.x) / divisor;
				int num3 = (int)Math.Round(num2);
				double num4 = num2 - (double)num3;
				int num5 = 20;
				val.DrawString(((double)num3 * skip).ToString(), 4.0, 4.0, (double)num5, (Justification)0, (Baseline)3, bedMarkingsColor, false, default(RGBA_Bytes));
				for (double num6 = num * (1.0 - num4); num6 < (double)BedImage.get_Width(); num6 += num)
				{
					num3++;
					int num7 = (int)num6;
					int num8 = 1;
					if (num3 == 0)
					{
						num8 = 2;
					}
					val.Line((double)num7, 0.0, (double)num7, (double)BedImage.get_Height(), bedMarkingsColor, (double)num8);
					val.DrawString(((double)num3 * skip).ToString(), num6 + 4.0, 4.0, (double)num5, (Justification)0, (Baseline)3, bedMarkingsColor, false, default(RGBA_Bytes));
				}
				double num9 = (double)BedImage.get_Height() / (displayVolumeToBuild.y / divisor);
				double num10 = (0.0 - displayVolume.y / 2.0 + bedCenter.y) / divisor;
				int num11 = (int)Math.Round(num10);
				double num12 = num10 - (double)num11;
				int num13 = 20;
				for (double num14 = num9 * (1.0 - num12); num14 < (double)BedImage.get_Height(); num14 += num9)
				{
					num11++;
					int num15 = (int)num14;
					int num16 = 1;
					if (num11 == 0)
					{
						num16 = 2;
					}
					val.Line(0.0, (double)num15, (double)BedImage.get_Height(), (double)num15, bedMarkingsColor, (double)num16);
					val.DrawString(((double)num11 * skip).ToString(), 4.0, num14 + 4.0, (double)num13, (Justification)0, (Baseline)3, bedMarkingsColor, false, default(RGBA_Bytes));
				}
				lastCreatedBedImage = BedImage;
			}
		}

		private bool FindInteractionVolumeHit(Ray ray, out int interactionVolumeHitIndex, out IntersectInfo info)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			interactionVolumeHitIndex = -1;
			if (interactionVolumes.Count == 0 || interactionVolumes[0].CollisionVolume == null)
			{
				info = null;
				return false;
			}
			List<IPrimitive> list = new List<IPrimitive>();
			foreach (InteractionVolume interactionVolume in interactionVolumes)
			{
				if (interactionVolume.CollisionVolume != null)
				{
					IPrimitive collisionVolume = interactionVolume.CollisionVolume;
					list.Add((IPrimitive)new Transform(collisionVolume, interactionVolume.TotalTransform));
				}
			}
			IPrimitive val = BoundingVolumeHierarchy.CreateNewHierachy(list, int.MaxValue, 0, (SortingAccelerator)null);
			info = val.GetClosestIntersection(ray);
			if (info != null)
			{
				for (int i = 0; i < interactionVolumes.Count; i++)
				{
					List<IPrimitive> list2 = new List<IPrimitive>();
					if (interactionVolumes[i].CollisionVolume != null)
					{
						interactionVolumes[i].CollisionVolume.GetContained(list2, info.closestHitObject.GetAxisAlignedBoundingBox());
						if (list2.Contains(info.closestHitObject))
						{
							interactionVolumeHitIndex = i;
							return true;
						}
					}
				}
			}
			return false;
		}

		private void reportProgress0to100(double progress0To1, string processingState, out bool continueProcessing)
		{
			if (((GuiWidget)this).get_HasBeenClosed())
			{
				continueProcessing = false;
			}
			else
			{
				continueProcessing = true;
			}
			UiThread.RunOnIdle((Action)delegate
			{
				int num = (int)(progress0To1 * 100.0);
				((GuiWidget)partProcessingInfo.centeredInfoText).set_Text(StringHelper.FormatWith("Loading Mesh {0}%...", new object[1]
				{
					num
				}));
				partProcessingInfo.progressControl.set_PercentComplete(num);
				((GuiWidget)partProcessingInfo.centeredInfoDescription).set_Text(processingState);
			});
		}

		private void trackballTumbleWidget_DrawGlContent(object sender, EventArgs e)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < MeshGroups.Count; i++)
			{
				MeshGroup val = MeshGroups[i];
				int num = 0;
				foreach (Mesh mesh in val.get_Meshes())
				{
					MeshMaterialData val2 = MeshMaterialData.Get(mesh);
					RGBA_Bytes val3 = GetMaterialColor(val2.MaterialIndex);
					if (HaveSelection && SelectedMeshGroups.Contains(val))
					{
						val3 = GetSelectedMaterialColor(val2.MaterialIndex);
					}
					RenderMeshToGl.Render(mesh, (IColorType)(object)val3, MeshGroupTransforms[i], RenderType);
					num++;
				}
			}
			if (MeshGroups.Count > 0 || AllowBedRenderingWhenEmpty)
			{
				if (RenderBed)
				{
					RenderMeshToGl.Render(printerBed, (IColorType)(object)BedColor, (RenderTypes)1);
				}
				if (RenderGrid)
				{
					RenderMeshToGl.Render(printerGrid, (IColorType)(object)BedColor, (RenderTypes)1);
				}
				if (buildVolume != null && RenderBuildVolume)
				{
					RenderMeshToGl.Render(buildVolume, (IColorType)(object)BuildVolumeColor, (RenderTypes)1);
				}
			}
			DrawInteractionVolumes(e);
		}

		private void DrawInteractionVolumes(EventArgs e)
		{
			foreach (InteractionVolume interactionVolume in interactionVolumes)
			{
				if (interactionVolume.DrawOnTop)
				{
					GL.Disable((EnableCap)2929);
					interactionVolume.DrawGlContent(new DrawGlContentEventArgs(zBuffered: false));
					GL.Enable((EnableCap)2929);
				}
			}
			foreach (InteractionVolume interactionVolume2 in interactionVolumes)
			{
				interactionVolume2.DrawGlContent(new DrawGlContentEventArgs(zBuffered: true));
			}
		}
	}
}
