using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.RayTracer;
using MatterHackers.RenderOpenGl;
using MatterHackers.RenderOpenGl.OpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.MeshVisualizer
{
	public class InteractionVolume
	{
		[Flags]
		public enum LineArrows
		{
			None = 0x0,
			Start = 0x1,
			End = 0x2,
			Both = 0x3
		}

		public bool MouseDownOnControl;

		public Matrix4X4 TotalTransform = Matrix4X4.Identity;

		private IPrimitive collisionVolume;

		private MeshViewerWidget meshViewerToDrawWith;

		private bool mouseOver;

		public IPrimitive CollisionVolume
		{
			get
			{
				return collisionVolume;
			}
			set
			{
				collisionVolume = value;
			}
		}

		public bool DrawOnTop
		{
			get;
			protected set;
		}

		public MeshViewerWidget MeshViewerToDrawWith => meshViewerToDrawWith;

		public bool MouseOver
		{
			get
			{
				return mouseOver;
			}
			set
			{
				if (mouseOver != value)
				{
					mouseOver = value;
					Invalidate();
				}
			}
		}

		public IntersectInfo MouseMoveInfo
		{
			get;
			set;
		}

		public InteractionVolume(IPrimitive collisionVolume, MeshViewerWidget meshViewerToDrawWith)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			this.collisionVolume = collisionVolume;
			this.meshViewerToDrawWith = meshViewerToDrawWith;
		}

		public static void DrawMeasureLine(Graphics2D graphics2D, Vector2 lineStart, Vector2 lineEnd, RGBA_Bytes color, LineArrows arrows)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Expected O, but got Unknown
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Expected O, but got Unknown
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Expected O, but got Unknown
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Expected O, but got Unknown
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Expected O, but got Unknown
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			graphics2D.Line(lineStart, lineEnd, RGBA_Bytes.Black, 1.0);
			Vector2 val = lineEnd - lineStart;
			if (((Vector2)(ref val)).get_LengthSquared() > 0.0 && (arrows.HasFlag(LineArrows.Start) || arrows.HasFlag(LineArrows.End)))
			{
				PathStorage val2 = new PathStorage();
				val2.MoveTo(-3.0, -5.0);
				val2.LineTo(0.0, 0.0);
				val2.LineTo(3.0, -5.0);
				if (arrows.HasFlag(LineArrows.End))
				{
					double num = Math.Atan2(val.y, val.x);
					IVertexSource val3 = (IVertexSource)new VertexSourceApplyTransform((IVertexSource)new VertexSourceApplyTransform((IVertexSource)(object)val2, (ITransform)(object)Affine.NewRotation(num - 1.5707963705062866)), (ITransform)(object)Affine.NewTranslation(lineEnd));
					graphics2D.Render(val3, (IColorType)(object)RGBA_Bytes.Black);
				}
				if (arrows.HasFlag(LineArrows.Start))
				{
					double num2 = Math.Atan2(val.y, val.x) + 3.1415927410125732;
					IVertexSource val4 = (IVertexSource)new VertexSourceApplyTransform((IVertexSource)new VertexSourceApplyTransform((IVertexSource)(object)val2, (ITransform)(object)Affine.NewRotation(num2 - 1.5707963705062866)), (ITransform)(object)Affine.NewTranslation(lineStart));
					graphics2D.Render(val4, (IColorType)(object)RGBA_Bytes.Black);
				}
			}
		}

		public static void RenderTransformedPath(Matrix4X4 transform, IVertexSource path, RGBA_Bytes color, bool doDepthTest)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			GL.Disable((EnableCap)3553);
			GL.MatrixMode((MatrixMode)5888);
			GL.PushMatrix();
			GL.MultMatrix(((Matrix4X4)(ref transform)).GetAsFloatArray());
			GL.Enable((EnableCap)3042);
			GL.BlendFunc((BlendingFactorSrc)770, (BlendingFactorDest)771);
			GL.Disable((EnableCap)2896);
			if (doDepthTest)
			{
				GL.Enable((EnableCap)2929);
			}
			else
			{
				GL.Disable((EnableCap)2929);
			}
			new Graphics2DOpenGL().DrawAAShape(path, (IColorType)(object)color);
			GL.PopMatrix();
		}

		public virtual void Draw2DContent(Graphics2D graphics2D)
		{
		}

		public virtual void DrawGlContent(EventArgs e)
		{
		}

		public void Invalidate()
		{
			((GuiWidget)MeshViewerToDrawWith).Invalidate();
		}

		public virtual void OnMouseDown(MouseEvent3DArgs mouseEvent3D)
		{
			MouseDownOnControl = true;
			((GuiWidget)MeshViewerToDrawWith).Invalidate();
		}

		public virtual void OnMouseMove(MouseEvent3DArgs mouseEvent3D)
		{
		}

		public virtual void OnMouseUp(MouseEvent3DArgs mouseEvent3D)
		{
			MouseDownOnControl = false;
		}

		public virtual void SetPosition()
		{
		}

		public static Vector3 SetBottomControlHeight(AxisAlignedBoundingBox originalSelectedBounds, Vector3 cornerPosition)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (originalSelectedBounds.minXYZ.z < 0.0)
			{
				if (originalSelectedBounds.maxXYZ.z < 0.0)
				{
					cornerPosition.z = originalSelectedBounds.maxXYZ.z;
				}
				else
				{
					cornerPosition.z = 0.0;
				}
			}
			return cornerPosition;
		}
	}
}
