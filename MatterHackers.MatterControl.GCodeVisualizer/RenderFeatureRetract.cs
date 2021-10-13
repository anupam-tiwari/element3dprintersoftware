using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.VertexSource;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.RenderOpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class RenderFeatureRetract : RenderFeatureBase
	{
		public static double RetractionDrawRadius = 1.0;

		private float extrusionAmount;

		private float mmPerSecond;

		private Vector3Float position;

		public RenderFeatureRetract(Vector3 position, double extrusionAmount, int extruderIndex, double mmPerSecond)
			: base(extruderIndex)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			this.extrusionAmount = (float)extrusionAmount;
			this.mmPerSecond = (float)mmPerSecond;
			this.position = new Vector3Float(position);
		}

		private double Radius(double layerScale)
		{
			double num = RetractionDrawRadius * layerScale;
			return Math.Sqrt(Math.PI * num * num * (double)Math.Abs(extrusionAmount) / Math.PI);
		}

		public override void CreateRender3DData(VectorPOD<ColorVertexData> colorVertexData, VectorPOD<int> indexData, GCodeRenderInfo renderInfo)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			if ((renderInfo.CurrentRenderType & RenderType.Retractions) == RenderType.Retractions)
			{
				Vector3 val = default(Vector3);
				((Vector3)(ref val))._002Ector(position);
				if (renderInfo.CurrentRenderType.HasFlag(RenderType.HideExtruderOffsets))
				{
					Vector2 extruderOffset = renderInfo.GetExtruderOffset(extruderIndex);
					val += new Vector3(extruderOffset, 0.0);
				}
				RGBA_Bytes color = MeshViewerWidget.GetMaterialColor(extruderIndex + 1);
				if (extruderIndex == 0)
				{
					color = ((!(extrusionAmount > 0f)) ? RGBA_Bytes.Red : RGBA_Bytes.Blue);
				}
				if (extrusionAmount > 0f)
				{
					RenderFeatureBase.CreatePointer(colorVertexData, indexData, val + new Vector3(0.0, 0.0, 1.3), val + new Vector3(0.0, 0.0, 0.3), Radius(1.0), 5, color);
				}
				else
				{
					RenderFeatureBase.CreatePointer(colorVertexData, indexData, val + new Vector3(0.0, 0.0, 0.3), val + new Vector3(0.0, 0.0, 1.3), Radius(1.0), 5, color);
				}
			}
		}

		public override void Render(Graphics2D graphics2D, GCodeRenderInfo renderInfo)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			if ((renderInfo.CurrentRenderType & RenderType.Retractions) == RenderType.Retractions)
			{
				double num = Radius(renderInfo.LayerScale);
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector((double)position.x, (double)position.y);
				if (renderInfo.CurrentRenderType.HasFlag(RenderType.HideExtruderOffsets))
				{
					Vector2 extruderOffset = renderInfo.GetExtruderOffset(extruderIndex);
					val += extruderOffset;
				}
				Affine transform = renderInfo.Transform;
				((Affine)(ref transform)).transform(ref val);
				RGBA_Bytes val2 = default(RGBA_Bytes);
				((RGBA_Bytes)(ref val2))._002Ector(RGBA_Bytes.Red, 200);
				if (extrusionAmount > 0f)
				{
					((RGBA_Bytes)(ref val2))._002Ector(RGBA_Bytes.Blue, 200);
				}
				Graphics2DOpenGL val3 = graphics2D as Graphics2DOpenGL;
				if (val3 != null)
				{
					val3.DrawAACircle(val, num, (IColorType)(object)val2);
					return;
				}
				Ellipse val4 = new Ellipse(val, num);
				graphics2D.Render((IVertexSource)(object)val4, (IColorType)(object)val2);
			}
		}
	}
}
