using MatterHackers.Agg;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.VertexSource;
using MatterHackers.RenderOpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class RenderFeatureTravel : RenderFeatureBase
	{
		protected Vector3Float start;

		protected Vector3Float end;

		protected float travelSpeed;

		protected Vector3Float GetStart(GCodeRenderInfo renderInfo)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			if (renderInfo.CurrentRenderType.HasFlag(RenderType.HideExtruderOffsets))
			{
				Vector3Float result = start;
				Vector2 extruderOffset = renderInfo.GetExtruderOffset(extruderIndex);
				result.x += (float)extruderOffset.x;
				result.y += (float)extruderOffset.y;
				return result;
			}
			return start;
		}

		protected Vector3Float GetEnd(GCodeRenderInfo renderInfo)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			if (renderInfo.CurrentRenderType.HasFlag(RenderType.HideExtruderOffsets))
			{
				Vector3Float result = end;
				Vector2 extruderOffset = renderInfo.GetExtruderOffset(extruderIndex);
				result.x += (float)extruderOffset.x;
				result.y += (float)extruderOffset.y;
				return result;
			}
			return end;
		}

		public RenderFeatureTravel(Vector3 start, Vector3 end, int extruderIndex, double travelSpeed)
			: base(extruderIndex)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			base.extruderIndex = extruderIndex;
			this.start = new Vector3Float(start);
			this.end = new Vector3Float(end);
			this.travelSpeed = (float)travelSpeed;
		}

		public override void CreateRender3DData(VectorPOD<ColorVertexData> colorVertexData, VectorPOD<int> indexData, GCodeRenderInfo renderInfo)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if ((renderInfo.CurrentRenderType & RenderType.Moves) == RenderType.Moves)
			{
				Vector3Float val = GetStart(renderInfo);
				Vector3Float val2 = GetEnd(renderInfo);
				RenderFeatureBase.CreateCylinder(colorVertexData, indexData, new Vector3(val), new Vector3(val2), 0.1, 6, GCodeRenderer.TravelColor, 0.2);
			}
		}

		public override void Render(Graphics2D graphics2D, GCodeRenderInfo renderInfo)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Expected O, but got Unknown
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Expected O, but got Unknown
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Expected O, but got Unknown
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			if ((renderInfo.CurrentRenderType & RenderType.Moves) != RenderType.Moves)
			{
				return;
			}
			double num = 0.35 * renderInfo.LayerScale;
			RGBA_Bytes val = default(RGBA_Bytes);
			((RGBA_Bytes)(ref val))._002Ector(10, 190, 15);
			Graphics2DOpenGL val2 = graphics2D as Graphics2DOpenGL;
			if (val2 != null)
			{
				Vector3Float val3 = GetStart(renderInfo);
				Vector3Float val4 = GetEnd(renderInfo);
				Vector2 val5 = default(Vector2);
				((Vector2)(ref val5))._002Ector((double)val3.x, (double)val3.y);
				Affine transform = renderInfo.Transform;
				((Affine)(ref transform)).transform(ref val5);
				Vector2 val6 = default(Vector2);
				((Vector2)(ref val6))._002Ector((double)val4.x, (double)val4.y);
				transform = renderInfo.Transform;
				((Affine)(ref transform)).transform(ref val6);
				val2.DrawAALineRounded(val5, val6, num, (IColorType)(object)val);
				return;
			}
			PathStorage val7 = new PathStorage();
			Stroke val8 = new Stroke((IVertexSource)new VertexSourceApplyTransform((IVertexSource)(object)val7, (ITransform)(object)renderInfo.Transform), num);
			val8.line_cap((LineCap)2);
			val8.line_join((LineJoin)2);
			Vector3Float val9 = GetStart(renderInfo);
			Vector3Float val10 = GetEnd(renderInfo);
			val7.Add((double)val9.x, (double)val9.y, (FlagsAndCommand)1);
			if (val10.x != val9.x || val10.y != val9.y)
			{
				val7.Add((double)val10.x, (double)val10.y, (FlagsAndCommand)2);
			}
			else
			{
				val7.Add((double)val10.x + 0.01, (double)val10.y, (FlagsAndCommand)2);
			}
			graphics2D.Render((IVertexSource)(object)val8, 0, (IColorType)(object)val);
		}
	}
}
