using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.RenderOpenGl.OpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.MeshVisualizer
{
	public class TrackballTumbleWidget : GuiWidget
	{
		internal class MotionQueue
		{
			internal struct TimeAndPosition
			{
				internal long timeMs;

				internal Vector2 position;

				internal TimeAndPosition(Vector2 position, long timeMs)
				{
					//IL_0008: Unknown result type (might be due to invalid IL or missing references)
					//IL_0009: Unknown result type (might be due to invalid IL or missing references)
					this.timeMs = timeMs;
					this.position = position;
				}
			}

			private List<TimeAndPosition> motionQueue = new List<TimeAndPosition>();

			internal void AddMoveToMotionQueue(Vector2 position, long timeMs)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				if (motionQueue.Count > 4)
				{
					motionQueue.RemoveAt(0);
				}
				motionQueue.Add(new TimeAndPosition(position, timeMs));
			}

			internal void Clear()
			{
				motionQueue.Clear();
			}

			internal Vector2 GetVelocityPixelsPerMs()
			{
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				if (motionQueue.Count > 1)
				{
					TimeAndPosition timeAndPosition = motionQueue[motionQueue.Count - 1];
					int num = motionQueue.Count - 1;
					while (num > 0 && motionQueue[num - 1].timeMs + 100 > timeAndPosition.timeMs)
					{
						num--;
					}
					TimeAndPosition timeAndPosition2 = motionQueue[num];
					double num2 = timeAndPosition.timeMs - timeAndPosition2.timeMs;
					if (num2 > 0.0)
					{
						return (timeAndPosition.position - timeAndPosition2.position) / num2;
					}
				}
				return Vector2.Zero;
			}
		}

		private bool doOpenGlDrawing = true;

		private float[] ambientLight = new float[4]
		{
			0.2f,
			0.2f,
			0.2f,
			1f
		};

		private float[] diffuseLight0 = new float[4]
		{
			0.7f,
			0.7f,
			0.7f,
			1f
		};

		private float[] specularLight0 = new float[4]
		{
			0.5f,
			0.5f,
			0.5f,
			1f
		};

		private float[] lightDirection0 = new float[4]
		{
			-1f,
			-1f,
			1f,
			0f
		};

		private float[] diffuseLight1 = new float[4]
		{
			0.5f,
			0.5f,
			0.5f,
			1f
		};

		private float[] specularLight1 = new float[4]
		{
			0.3f,
			0.3f,
			0.3f,
			1f
		};

		private float[] lightDirection1 = new float[4]
		{
			1f,
			1f,
			1f,
			0f
		};

		private RGBA_Bytes rotationHelperCircleColor = new RGBA_Bytes(RGBA_Bytes.Black, 200);

		private TrackBallController mainTrackBallController = new TrackBallController();

		private List<IVertexSource> insideArrows = new List<IVertexSource>();

		private List<IVertexSource> outsideArrows = new List<IVertexSource>();

		private MotionQueue motionQueue = new MotionQueue();

		private double startAngle;

		private double startDistanceBetweenPoints = 1.0;

		private double pinchStartScale = 1.0;

		private Vector2 currentVelocityPerMs;

		private int updatesPerSecond = 30;

		private Matrix4X4 projectionMatrix;

		private Matrix4X4 inverseProjectionMatrix;

		private Matrix4X4 modelviewMatrix;

		private Matrix4X4 inverseModelviewMatrix;

		public bool DoOpenGlDrawing
		{
			get
			{
				return doOpenGlDrawing;
			}
			set
			{
				doOpenGlDrawing = value;
			}
		}

		public MouseDownType TransformState
		{
			get;
			set;
		}

		public RGBA_Bytes RotationHelperCircleColor
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return rotationHelperCircleColor;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				rotationHelperCircleColor = value;
			}
		}

		public bool DrawRotationHelperCircle
		{
			get;
			set;
		}

		public TrackBallController TrackBallController
		{
			get
			{
				return mainTrackBallController;
			}
			set
			{
				mainTrackBallController.remove_TransformChanged((EventHandler)TrackBallController_TransformChanged);
				mainTrackBallController = value;
				mainTrackBallController.add_TransformChanged((EventHandler)TrackBallController_TransformChanged);
			}
		}

		public bool LockTrackBall
		{
			get;
			set;
		}

		public Matrix4X4 ProjectionMatrix => projectionMatrix;

		public Matrix4X4 InverseProjectionMatrix => inverseProjectionMatrix;

		public Matrix4X4 ModelviewMatrix => modelviewMatrix;

		public Matrix4X4 InverseModelviewMatrix => inverseModelviewMatrix;

		public event EventHandler DrawGlContent;

		public TrackballTumbleWidget()
			: this()
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Expected O, but got Unknown
			((GuiWidget)this).AnchorAll();
			DrawRotationHelperCircle = true;
			TrackBallController.add_TransformChanged((EventHandler)TrackBallController_TransformChanged);
		}

		private void TrackBallController_TransformChanged(object sender, EventArgs e)
		{
			CalculateModelviewMatrix();
		}

		public override void OnBoundsChanged(EventArgs e)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			Vector2 screenCenter = default(Vector2);
			((Vector2)(ref screenCenter))._002Ector(((GuiWidget)this).get_Width() / 2.0, ((GuiWidget)this).get_Height() / 2.0);
			double trackBallRadius = Math.Min(((GuiWidget)this).get_Width() * 0.45, ((GuiWidget)this).get_Height() * 0.45);
			TrackBallController.set_ScreenCenter(screenCenter);
			TrackBallController.set_TrackBallRadius(trackBallRadius);
			CalculateProjectionMatrix();
			MakeArrowIcons();
			((GuiWidget)this).OnBoundsChanged(e);
		}

		public Vector2 GetScreenPosition(Vector3 worldPosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.TransformPerspective(Vector3.Transform(worldPosition, ModelviewMatrix), ProjectionMatrix);
			return new Vector2(val.x * ((GuiWidget)this).get_Width() / 2.0 + ((GuiWidget)this).get_Width() / 2.0, val.y / val.z * ((GuiWidget)this).get_Height() / 2.0 + ((GuiWidget)this).get_Height() / 2.0);
		}

		public Vector3 GetScreenSpace(Vector3 worldPosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Transform(Vector3.Transform(worldPosition, ModelviewMatrix), ProjectionMatrix);
		}

		public Ray GetRayFromScreen(Vector2 screenPosition)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			Vector4 val = default(Vector4);
			val.x = 2.0 * screenPosition.x / ((GuiWidget)this).get_Width() - 1.0;
			val.y = 2.0 * screenPosition.y / ((GuiWidget)this).get_Height() - 1.0;
			val.z = -1.0;
			val.w = 1.0;
			Vector4 val2 = Vector4.Transform(val, InverseProjectionMatrix);
			val2.z = -1.0;
			val2.w = 0.0;
			Vector3 val3 = new Vector3(Vector4.Transform(val2, InverseModelviewMatrix));
			Vector3 normal = ((Vector3)(ref val3)).GetNormal();
			return new Ray(Vector3.Transform(Vector3.Zero, InverseModelviewMatrix), normal, 0.0, double.PositiveInfinity, (IntersectionType)1);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (DoOpenGlDrawing)
			{
				SetGlContext();
				OnDrawGlContent();
				UnsetGlContext();
			}
			((GuiWidget)this).get_LocalBounds();
			if (DrawRotationHelperCircle)
			{
				DrawTrackballRadius(graphics2D);
			}
			((GuiWidget)this).OnDraw(graphics2D);
		}

		public void DrawTrackballRadius(Graphics2D graphics2D)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			Vector2 screenCenter = TrackBallController.get_ScreenCenter();
			double trackBallRadius = TrackBallController.get_TrackBallRadius();
			Stroke val = new Stroke((IVertexSource)new Ellipse(screenCenter, trackBallRadius, trackBallRadius, 0, false), 3.0);
			graphics2D.Render((IVertexSource)(object)val, (IColorType)(object)RotationHelperCircleColor);
			if (insideArrows.Count == 0)
			{
				MakeArrowIcons();
			}
			if (TrackBallController.get_LastMoveInsideRadius())
			{
				foreach (IVertexSource insideArrow in insideArrows)
				{
					graphics2D.Render(insideArrow, (IColorType)(object)RotationHelperCircleColor);
				}
				return;
			}
			foreach (IVertexSource outsideArrow in outsideArrows)
			{
				graphics2D.Render(outsideArrow, (IColorType)(object)RotationHelperCircleColor);
			}
		}

		private void MakeArrowIcons()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Expected O, but got Unknown
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Expected O, but got Unknown
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Expected O, but got Unknown
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Expected O, but got Unknown
			Vector2 screenCenter = TrackBallController.get_ScreenCenter();
			double trackBallRadius = TrackBallController.get_TrackBallRadius();
			insideArrows.Clear();
			PathStorage val = new PathStorage("M560.512 0.570216 C560.512 2.05696 280.518 560.561 280.054 560 C278.498 558.116 0 0.430888 0.512416 0.22416 C0.847112 0.089136 63.9502 27.1769 140.742 60.4192 C140.742 60.4192 280.362 120.86 280.362 120.86 C280.362 120.86 419.756 60.4298 419.756 60.4298 C496.422 27.1934 559.456 0 559.831 0 C560.205 0 560.512 0.2566 560.512 0.570216 Z");
			RectangleDouble bounds = val.GetBounds();
			double num = trackBallRadius / 10.0;
			Affine val2 = Affine.NewTranslation(-((RectangleDouble)(ref bounds)).get_Center());
			Affine val3 = Affine.NewScaling(1.0 / ((RectangleDouble)(ref bounds)).get_Width());
			Affine val4 = Affine.NewScaling(num);
			Affine val5 = Affine.NewTranslation(new Vector2(0.0, trackBallRadius * 9.0 / 10.0));
			Affine val6 = Affine.NewTranslation(screenCenter);
			for (int i = 0; i < 4; i++)
			{
				Affine val7 = val2 * val3 * val4 * val5 * Affine.NewRotation(1.5707963705062866 * (double)i) * val6;
				insideArrows.Add((IVertexSource)new VertexSourceApplyTransform((IVertexSource)(object)val, (ITransform)(object)val7));
			}
			outsideArrows.Clear();
			PathStorage val8 = new PathStorage("M560.512 0.570216 C560.512 2.05696 280.518 560.561 280.054 560 C278.498 558.116 0 0.430888 0.512416 0.22416 C0.847112 0.089136 63.9502 27.1769 140.742 60.4192 C140.742 60.4192 280.362 120.86 280.362 120.86 C280.362 120.86 419.756 60.4298 419.756 60.4298 C496.422 27.1934 559.456 0 559.831 0 C560.205 0 560.512 0.2566 560.512 0.570216 Z");
			RectangleDouble bounds2 = val8.GetBounds();
			double num2 = trackBallRadius / 15.0;
			Affine val9 = Affine.NewTranslation(-((RectangleDouble)(ref bounds2)).get_Center());
			Affine val10 = Affine.NewScaling(1.0 / ((RectangleDouble)(ref bounds2)).get_Width());
			Affine val11 = Affine.NewScaling(num2);
			Affine val12 = Affine.NewTranslation(new Vector2(0.0, trackBallRadius * 16.0 / 15.0));
			Affine val13 = Affine.NewTranslation(screenCenter);
			for (int j = 0; j < 4; j++)
			{
				Affine val14 = val9 * val10 * val11 * Affine.NewRotation(1.5707963705062866) * val12 * Affine.NewRotation(0.7853981852531433 + 1.5707963705062866 * (double)j + 0.07853981852531433) * val13;
				outsideArrows.Add((IVertexSource)new VertexSourceApplyTransform((IVertexSource)(object)val8, (ITransform)(object)val14));
				Affine val15 = val9 * val10 * val11 * Affine.NewRotation(-1.5707963705062866) * val12 * Affine.NewRotation(0.7853981852531433 + 1.5707963705062866 * (double)j - 0.07853981852531433) * val13;
				outsideArrows.Add((IVertexSource)new VertexSourceApplyTransform((IVertexSource)(object)val8, (ITransform)(object)val15));
			}
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Invalid comparison between Unknown and I4
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Invalid comparison between Unknown and I4
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Invalid comparison between Unknown and I4
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Invalid comparison between Unknown and I4
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Invalid comparison between Unknown and I4
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Invalid comparison between Unknown and I4
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Expected I4, but got Unknown
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Invalid comparison between Unknown and I4
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Invalid comparison between Unknown and I4
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnMouseDown(mouseEvent);
			if (LockTrackBall || !((GuiWidget)this).get_MouseCaptured())
			{
				return;
			}
			Vector2 val = default(Vector2);
			if (mouseEvent.get_NumPositions() == 1)
			{
				val.x = mouseEvent.get_X();
				val.y = mouseEvent.get_Y();
			}
			else
			{
				val = (mouseEvent.GetPosition(1) + mouseEvent.GetPosition(0)) / 2.0;
			}
			currentVelocityPerMs = Vector2.Zero;
			motionQueue.Clear();
			motionQueue.AddMoveToMotionQueue(val, UiThread.get_CurrentTimerMs());
			if (mouseEvent.get_NumPositions() > 1)
			{
				Vector2 position = mouseEvent.GetPosition(0);
				Vector2 position2 = mouseEvent.GetPosition(1);
				Vector2 val2 = position2 - position;
				startDistanceBetweenPoints = ((Vector2)(ref val2)).get_Length();
				pinchStartScale = TrackBallController.get_Scale();
				startAngle = Math.Atan2(position2.y - position.y, position2.x - position.x);
				if ((int)TransformState != 0)
				{
					if (!LockTrackBall && (int)TrackBallController.get_CurrentTrackingType() != 0)
					{
						TrackBallController.OnMouseUp();
					}
					TrackBallController.OnMouseDown(val, Matrix4X4.Identity, (MouseDownType)1);
				}
			}
			if ((int)mouseEvent.get_Button() == 1048576)
			{
				if ((int)TrackBallController.get_CurrentTrackingType() != 0)
				{
					return;
				}
				Keys modifierKeys = ((GuiWidget)this).get_ModifierKeys();
				if ((int)modifierKeys == 65536)
				{
					TrackBallController.OnMouseDown(val, Matrix4X4.Identity, (MouseDownType)1);
					return;
				}
				if ((int)modifierKeys == 131072 && (int)OsInformation.get_OperatingSystem() != 1)
				{
					TrackBallController.OnMouseDown(val, Matrix4X4.Identity, (MouseDownType)3);
					return;
				}
				if ((int)modifierKeys == 262144 && (int)OsInformation.get_OperatingSystem() != 2)
				{
					TrackBallController.OnMouseDown(val, Matrix4X4.Identity, (MouseDownType)2);
					return;
				}
				MouseDownType transformState = TransformState;
				switch (transformState - 1)
				{
				case 1:
					TrackBallController.OnMouseDown(val, Matrix4X4.Identity, (MouseDownType)2);
					break;
				case 0:
					TrackBallController.OnMouseDown(val, Matrix4X4.Identity, (MouseDownType)1);
					break;
				case 2:
					TrackBallController.OnMouseDown(val, Matrix4X4.Identity, (MouseDownType)3);
					break;
				}
			}
			else if ((int)mouseEvent.get_Button() == 4194304)
			{
				if ((int)TrackBallController.get_CurrentTrackingType() == 0)
				{
					TrackBallController.OnMouseDown(val, Matrix4X4.Identity, (MouseDownType)1);
				}
			}
			else if ((int)mouseEvent.get_Button() == 2097152 && (int)TrackBallController.get_CurrentTrackingType() == 0)
			{
				TrackBallController.OnMouseDown(val, Matrix4X4.Identity, (MouseDownType)2);
			}
		}

		public override void OnMouseMove(MouseEventArgs mouseEvent)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Invalid comparison between Unknown and I4
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnMouseMove(mouseEvent);
			Vector2 val = default(Vector2);
			if (mouseEvent.get_NumPositions() == 1)
			{
				val.x = mouseEvent.get_X();
				val.y = mouseEvent.get_Y();
				if (((GuiWidget)this).get_MouseCaptured() && (int)TransformState == 2)
				{
					DrawRotationHelperCircle = true;
				}
				else
				{
					DrawRotationHelperCircle = false;
				}
			}
			else
			{
				val = (mouseEvent.GetPosition(1) + mouseEvent.GetPosition(0)) / 2.0;
				DrawRotationHelperCircle = false;
			}
			motionQueue.AddMoveToMotionQueue(val, UiThread.get_CurrentTimerMs());
			if (!LockTrackBall && (int)TrackBallController.get_CurrentTrackingType() != 0)
			{
				TrackBallController.OnMouseMove(val);
				((GuiWidget)this).Invalidate();
			}
			if ((int)TransformState != 0 && mouseEvent.get_NumPositions() > 1 && startDistanceBetweenPoints > 0.0)
			{
				Vector2 position = mouseEvent.GetPosition(0);
				Vector2 position2 = mouseEvent.GetPosition(1);
				Vector2 val2 = position2 - position;
				double length = ((Vector2)(ref val2)).get_Length();
				double scale = pinchStartScale * length / startDistanceBetweenPoints;
				TrackBallController.set_Scale(scale);
				Math.Atan2(position2.y - position.y, position2.x - position.x);
			}
		}

		public void ZeroVelocity()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			currentVelocityPerMs = Vector2.Zero;
		}

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I4
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			if (!LockTrackBall && (int)TrackBallController.get_CurrentTrackingType() != 0)
			{
				if ((int)TrackBallController.get_CurrentTrackingType() == 2 && TrackBallController.get_LastMoveInsideRadius())
				{
					motionQueue.AddMoveToMotionQueue(mouseEvent.get_Position(), UiThread.get_CurrentTimerMs());
					currentVelocityPerMs = motionQueue.GetVelocityPixelsPerMs();
					if (((Vector2)(ref currentVelocityPerMs)).get_LengthSquared() > 0.0)
					{
						UiThread.RunOnIdle((Action)ApplyVelocity);
					}
				}
				TrackBallController.OnMouseUp();
			}
			((GuiWidget)this).OnMouseUp(mouseEvent);
		}

		private void ApplyVelocity()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			double num = 1000.0 / (double)updatesPerSecond;
			if (((Vector2)(ref currentVelocityPerMs)).get_LengthSquared() > 0.0 && (int)TrackBallController.get_CurrentTrackingType() == 0)
			{
				RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
				Vector2 center = ((RectangleDouble)(ref localBounds)).get_Center();
				TrackBallController.OnMouseDown(center, Matrix4X4.Identity, (MouseDownType)2);
				TrackBallController.OnMouseMove(center + currentVelocityPerMs * num);
				TrackBallController.OnMouseUp();
				((GuiWidget)this).Invalidate();
				currentVelocityPerMs *= 0.85;
				if (((Vector2)(ref currentVelocityPerMs)).get_LengthSquared() < 0.01 / num)
				{
					currentVelocityPerMs = Vector2.Zero;
				}
				if (((Vector2)(ref currentVelocityPerMs)).get_LengthSquared() > 0.0)
				{
					UiThread.RunOnIdle((Action)ApplyVelocity, 1.0 / (double)updatesPerSecond);
				}
			}
		}

		public override void OnMouseWheel(MouseEventArgs mouseEvent)
		{
			if (!LockTrackBall)
			{
				TrackBallController.OnMouseWheel(mouseEvent.get_WheelDelta());
				((GuiWidget)this).Invalidate();
			}
			((GuiWidget)this).OnMouseWheel(mouseEvent);
		}

		private void OnDrawGlContent()
		{
			this.DrawGlContent?.Invoke(this, null);
		}

		private void GradientBand(double startHeight, double endHeight, int startColor, int endColor)
		{
			GL.Color4(startColor - 5, startColor - 5, startColor, 255);
			GL.Vertex2(-1.0, startHeight);
			GL.Color4(endColor - 5, endColor - 5, endColor, 255);
			GL.Vertex2(1.0, endHeight);
			GL.Vertex2(-1.0, endHeight);
			GL.Color4(startColor - 5, startColor - 5, startColor, 255);
			GL.Vertex2(1.0, startHeight);
			GL.Vertex2(-1.0, startHeight);
			GL.Color4(endColor - 5, endColor - 5, endColor, 255);
			GL.Vertex2(1.0, endHeight);
		}

		private void ClearToGradient()
		{
			GL.MatrixMode((MatrixMode)5889);
			GL.LoadIdentity();
			GL.MatrixMode((MatrixMode)5888);
			GL.LoadIdentity();
			GL.Begin((BeginMode)4);
			GradientBand(1.0, 0.0, 255, 245);
			GradientBand(0.0, -1.0, 245, 220);
			GL.End();
		}

		private void SetGlContext()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			GL.ClearDepth(1.0);
			GL.Clear((ClearBufferMask)256);
			GL.PushAttrib((AttribMask)2048);
			RectangleDouble val = ((GuiWidget)this).TransformToScreenSpace(((GuiWidget)this).get_LocalBounds());
			GL.Viewport((int)val.Left, (int)val.Bottom, (int)((RectangleDouble)(ref val)).get_Width(), (int)((RectangleDouble)(ref val)).get_Height());
			GL.ShadeModel((ShadingModel)7425);
			GL.FrontFace((FrontFaceDirection)2305);
			GL.CullFace((CullFaceMode)1029);
			GL.DepthFunc((DepthFunction)515);
			GL.Disable((EnableCap)2929);
			GL.Light((LightName)16384, (LightParameter)4608, ambientLight);
			GL.Light((LightName)16384, (LightParameter)4609, diffuseLight0);
			GL.Light((LightName)16384, (LightParameter)4610, specularLight0);
			GL.Light((LightName)16384, (LightParameter)4608, new float[4]);
			GL.Light((LightName)16385, (LightParameter)4609, diffuseLight1);
			GL.Light((LightName)16385, (LightParameter)4610, specularLight1);
			GL.ColorMaterial((MaterialFace)1032, (ColorMaterialParameter)5634);
			GL.Enable((EnableCap)16384);
			GL.Enable((EnableCap)16385);
			GL.Enable((EnableCap)2929);
			GL.Enable((EnableCap)3042);
			GL.Enable((EnableCap)2977);
			GL.Enable((EnableCap)2896);
			GL.Enable((EnableCap)2903);
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector((double)lightDirection0[0], (double)lightDirection0[1], (double)lightDirection0[2]);
			((Vector3)(ref val2)).Normalize();
			lightDirection0[0] = (float)val2.x;
			lightDirection0[1] = (float)val2.y;
			lightDirection0[2] = (float)val2.z;
			GL.Light((LightName)16384, (LightParameter)4611, lightDirection0);
			GL.Light((LightName)16385, (LightParameter)4611, lightDirection1);
			GL.MatrixMode((MatrixMode)5889);
			GL.PushMatrix();
			Matrix4X4 val3 = ProjectionMatrix;
			GL.LoadMatrix(((Matrix4X4)(ref val3)).GetAsDoubleArray());
			GL.MatrixMode((MatrixMode)5888);
			GL.PushMatrix();
			val3 = ModelviewMatrix;
			GL.LoadMatrix(((Matrix4X4)(ref val3)).GetAsDoubleArray());
		}

		private void UnsetGlContext()
		{
			GL.MatrixMode((MatrixMode)5889);
			GL.PopMatrix();
			GL.MatrixMode((MatrixMode)5888);
			GL.PopMatrix();
			GL.Disable((EnableCap)2903);
			GL.Disable((EnableCap)2896);
			GL.Disable((EnableCap)16384);
			GL.Disable((EnableCap)16385);
			GL.Disable((EnableCap)2977);
			GL.Disable((EnableCap)3042);
			GL.Disable((EnableCap)2929);
			GL.PopAttrib();
		}

		public void CalculateProjectionMatrix()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			projectionMatrix = Matrix4X4.Identity;
			if (((GuiWidget)this).get_Width() > 0.0 && ((GuiWidget)this).get_Height() > 0.0)
			{
				Matrix4X4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0), ((GuiWidget)this).get_Width() / ((GuiWidget)this).get_Height(), 0.10000000149011612, 100.0, ref projectionMatrix);
				inverseProjectionMatrix = Matrix4X4.Invert(ProjectionMatrix);
			}
		}

		public void CalculateModelviewMatrix()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			modelviewMatrix = Matrix4X4.CreateTranslation(0.0, 0.0, -7.0);
			modelviewMatrix = TrackBallController.GetTransform4X4() * modelviewMatrix;
			inverseModelviewMatrix = Matrix4X4.Invert(modelviewMatrix);
		}

		public double GetWorldUnitsPerScreenPixelAtPosition(Vector3 worldPosition, double maxRatio = 5.0)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			Vector2 screenPosition = GetScreenPosition(worldPosition);
			Ray rayFromScreen = GetRayFromScreen(screenPosition);
			Vector3 val = worldPosition - rayFromScreen.origin;
			double length = ((Vector3)(ref val)).get_Length();
			Ray rayFromScreen2 = GetRayFromScreen(new Vector2(screenPosition.x + 1.0, screenPosition.y));
			val = rayFromScreen2.origin + rayFromScreen2.directionNormal * length - worldPosition;
			double length2 = ((Vector3)(ref val)).get_Length();
			if (length2 > maxRatio)
			{
				return maxRatio;
			}
			return length2;
		}
	}
}
