using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.VertexSource;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.RenderOpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class RenderFeatureExtrusion : RenderFeatureTravel
	{
		private int instructionIndex;

		private double extrusionWidth;

		private float extrusionVolumeMm3;

		private float layerHeight;

		private RGBA_Bytes color;

		public RenderFeatureExtrusion(Vector3 start, Vector3 end, int extruderIndex, int instructionIndex, double extrusionWidth, double travelSpeed, double totalExtrusionMm, double filamentDiameterMm, double layerHeight, RGBA_Bytes color)
			: base(start, end, extruderIndex, travelSpeed)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			this.instructionIndex = instructionIndex;
			this.color = color;
			double num = filamentDiameterMm / 2.0;
			double num2 = num * num * Math.PI;
			this.extrusionWidth = extrusionWidth;
			extrusionVolumeMm3 = (float)(num2 * totalExtrusionMm);
			this.layerHeight = (float)layerHeight;
		}

		private double GetRadius(RenderType renderType)
		{
			return GetExtrusionWidth(renderType) / 2.0;
		}

		private double GetExtrusionWidth(RenderType renderType)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			double result = extrusionWidth;
			if ((renderType & RenderType.SimulateExtrusion) == RenderType.SimulateExtrusion && (double)extrusionVolumeMm3 > 0.0)
			{
				Vector3Float val = end - start;
				double num = ((Vector3Float)(ref val)).get_Length();
				if (num > 0.1)
				{
					result = (double)extrusionVolumeMm3 / num / (double)layerHeight;
				}
			}
			return result;
		}

		public override void CreateRender3DData(VectorPOD<ColorVertexData> colorVertexData, VectorPOD<int> indexData, GCodeRenderInfo renderInfo)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			if ((renderInfo.CurrentRenderType & RenderType.Extrusions) == RenderType.Extrusions)
			{
				Vector3Float start = GetStart(renderInfo);
				Vector3Float end = GetEnd(renderInfo);
				double radius = GetRadius(renderInfo.CurrentRenderType);
				if ((renderInfo.CurrentRenderType & RenderType.SpeedColors) == RenderType.SpeedColors)
				{
					RenderFeatureBase.CreateCylinder(colorVertexData, indexData, new Vector3(start), new Vector3(end), radius, 6, color, layerHeight);
				}
				else
				{
					RenderFeatureBase.CreateCylinder(colorVertexData, indexData, new Vector3(start), new Vector3(end), radius, 6, MeshViewerWidget.GetMaterialColor(extruderIndex + 1), layerHeight);
				}
				if (renderInfo.GCodeSelectionInfo.SelectedFeatureInstructionIndices.Contains(instructionIndex))
				{
					RenderFeatureBase.CreateCylinder(colorVertexData, indexData, new Vector3(start), new Vector3(end), radius, 6, MeshViewerWidget.GetSelectedMaterialColor(extruderIndex + 1), layerHeight);
				}
			}
		}

		public override void Render(Graphics2D graphics2D, GCodeRenderInfo renderInfo)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Expected O, but got Unknown
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Expected O, but got Unknown
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Expected O, but got Unknown
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0520: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Expected O, but got Unknown
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Expected O, but got Unknown
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Expected O, but got Unknown
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			if (!renderInfo.CurrentRenderType.HasFlag(RenderType.Extrusions))
			{
				return;
			}
			double num = GetExtrusionWidth(renderInfo.CurrentRenderType) * renderInfo.LayerScale;
			RGBA_Bytes val = MeshViewerWidget.GetMaterialColor(extruderIndex + 1);
			if (renderInfo.CurrentRenderType.HasFlag(RenderType.SpeedColors))
			{
				val = color;
			}
			if (renderInfo.GCodeSelectionInfo.SelectedFeatureInstructionIndices.Contains(instructionIndex))
			{
				val = MeshViewerWidget.GetSelectedMaterialColor(extruderIndex + 1);
			}
			if (renderInfo.CurrentRenderType.HasFlag(RenderType.TransparentExtrusion))
			{
				((RGBA_Bytes)(ref val))._002Ector(val, 200);
			}
			if (instructionIndex < renderInfo.GCodeSelectionInfo.StartLineIndex || (renderInfo.GCodeSelectionInfo.EndLineIndex != -1 && instructionIndex > renderInfo.GCodeSelectionInfo.EndLineIndex))
			{
				((RGBA_Bytes)(ref val))._002Ector(val, 96);
				val = RGBA_Bytes.Black;
			}
			bool flag = false;
			bool flag2 = false;
			if (instructionIndex == renderInfo.GCodeSelectionInfo.StartLineIndex)
			{
				flag = true;
			}
			if (instructionIndex == renderInfo.GCodeSelectionInfo.EndLineIndex)
			{
				flag2 = true;
			}
			Affine transform;
			if (flag || flag2)
			{
				Graphics2DOpenGL val2 = graphics2D as Graphics2DOpenGL;
				if (val2 != null)
				{
					Vector3Float start = GetStart(renderInfo);
					Vector3Float end = GetEnd(renderInfo);
					Vector2 val3 = default(Vector2);
					((Vector2)(ref val3))._002Ector((double)start.x, (double)start.y);
					transform = renderInfo.Transform;
					((Affine)(ref transform)).transform(ref val3);
					Vector2 val4 = default(Vector2);
					((Vector2)(ref val4))._002Ector((double)end.x, (double)end.y);
					transform = renderInfo.Transform;
					((Affine)(ref transform)).transform(ref val4);
					val2.DrawAALineRounded(val3, val4, num, (IColorType)(object)val);
					if (flag)
					{
						Vector2 startBedPosition = renderInfo.GCodeSelectionInfo.StartBedPosition;
						transform = renderInfo.Transform;
						((Affine)(ref transform)).transform(ref startBedPosition);
						startBedPosition = PutClickOnLine(startBedPosition, val3, val4);
						val2.DrawAALineRounded(val3, startBedPosition, num, (IColorType)(object)RGBA_Bytes.Black);
						val2.DrawAACircle(startBedPosition, num / 2.0, (IColorType)(object)RGBA_Bytes.Green);
					}
					if (flag2)
					{
						Vector2 endBedPosition = renderInfo.GCodeSelectionInfo.EndBedPosition;
						transform = renderInfo.Transform;
						((Affine)(ref transform)).transform(ref endBedPosition);
						endBedPosition = PutClickOnLine(endBedPosition, val3, val4);
						val2.DrawAALineRounded(val4, endBedPosition, num, (IColorType)(object)RGBA_Bytes.Black);
						val2.DrawAACircle(endBedPosition, num / 2.0, (IColorType)(object)RGBA_Bytes.Green);
					}
					return;
				}
				PathStorage val5 = new PathStorage();
				Stroke val6 = new Stroke((IVertexSource)new VertexSourceApplyTransform((IVertexSource)(object)val5, (ITransform)(object)renderInfo.Transform), num);
				val6.line_cap((LineCap)2);
				val6.line_join((LineJoin)2);
				Vector3Float start2 = GetStart(renderInfo);
				Vector3Float end2 = GetEnd(renderInfo);
				Vector2 begin = default(Vector2);
				((Vector2)(ref begin))._002Ector((double)start2.x, (double)start2.y);
				transform = renderInfo.Transform;
				((Affine)(ref transform)).transform(ref begin);
				Vector2 stop = default(Vector2);
				((Vector2)(ref stop))._002Ector((double)end2.x, (double)end2.y);
				transform = renderInfo.Transform;
				((Affine)(ref transform)).transform(ref stop);
				val5.Add((double)start2.x, (double)start2.y, (FlagsAndCommand)1);
				val5.Add((double)end2.x, (double)end2.y, (FlagsAndCommand)2);
				graphics2D.Render((IVertexSource)(object)val6, 0, (IColorType)(object)val);
				if (flag)
				{
					Vector2 val7 = PutClickOnLine(renderInfo.GCodeSelectionInfo.StartBedPosition, new Vector2((double)start2.x, (double)start2.y), new Vector2((double)end2.x, (double)end2.y));
					val5.remove_all();
					val5.Add((double)start2.x, (double)start2.y, (FlagsAndCommand)1);
					val5.Add(val7.x, val7.y, (FlagsAndCommand)2);
					Vector2 startBedPosition2 = renderInfo.GCodeSelectionInfo.StartBedPosition;
					transform = renderInfo.Transform;
					((Affine)(ref transform)).transform(ref startBedPosition2);
					graphics2D.Render((IVertexSource)(object)val6, 0, (IColorType)(object)RGBA_Bytes.Black);
					graphics2D.Circle(PutClickOnLine(startBedPosition2, begin, stop), num / 2.0, RGBA_Bytes.Green);
				}
				if (flag2)
				{
					Vector2 val8 = PutClickOnLine(renderInfo.GCodeSelectionInfo.EndBedPosition, new Vector2((double)start2.x, (double)start2.y), new Vector2((double)end2.x, (double)end2.y));
					val5.remove_all();
					val5.Add((double)end2.x, (double)end2.y, (FlagsAndCommand)1);
					val5.Add(val8.x, val8.y, (FlagsAndCommand)2);
					Vector2 endBedPosition2 = renderInfo.GCodeSelectionInfo.EndBedPosition;
					transform = renderInfo.Transform;
					((Affine)(ref transform)).transform(ref endBedPosition2);
					graphics2D.Render((IVertexSource)(object)val6, 0, (IColorType)(object)RGBA_Bytes.Black);
					graphics2D.Circle(PutClickOnLine(endBedPosition2, begin, stop), num / 2.0, RGBA_Bytes.Green);
				}
			}
			else
			{
				Graphics2DOpenGL val9 = graphics2D as Graphics2DOpenGL;
				if (val9 != null)
				{
					Vector3Float start3 = GetStart(renderInfo);
					Vector3Float end3 = GetEnd(renderInfo);
					Vector2 val10 = default(Vector2);
					((Vector2)(ref val10))._002Ector((double)start3.x, (double)start3.y);
					transform = renderInfo.Transform;
					((Affine)(ref transform)).transform(ref val10);
					Vector2 val11 = default(Vector2);
					((Vector2)(ref val11))._002Ector((double)end3.x, (double)end3.y);
					transform = renderInfo.Transform;
					((Affine)(ref transform)).transform(ref val11);
					val9.DrawAALineRounded(val10, val11, num, (IColorType)(object)val);
				}
				else
				{
					PathStorage val12 = new PathStorage();
					Stroke val13 = new Stroke((IVertexSource)new VertexSourceApplyTransform((IVertexSource)val12, (ITransform)(object)renderInfo.Transform), num);
					val13.line_cap((LineCap)2);
					val13.line_join((LineJoin)2);
					Vector3Float start4 = GetStart(renderInfo);
					Vector3Float end4 = GetEnd(renderInfo);
					val12.Add((double)start4.x, (double)start4.y, (FlagsAndCommand)1);
					val12.Add((double)end4.x, (double)end4.y, (FlagsAndCommand)2);
					graphics2D.Render((IVertexSource)(object)val13, 0, (IColorType)(object)val);
				}
			}
		}

		private Vector2 PutClickOnLine(Vector2 clickPoint, Vector2 begin, Vector2 stop)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			Vector2 zero = Vector2.Zero;
			if (Math.Abs(stop.x - begin.x) < 0.0001)
			{
				zero.x = begin.x;
				zero.y = clickPoint.y;
			}
			else if (Math.Abs(stop.y - begin.y) < 0.0001)
			{
				zero.x = clickPoint.x;
				zero.y = begin.y;
			}
			else
			{
				double num = (stop.y - begin.y) / (stop.x - begin.x);
				double num2 = -1.0 / num;
				zero.x = (num * begin.x - num2 * clickPoint.x - begin.y + clickPoint.y) / (num - num2);
				zero.y = num * (zero.x - begin.x) + begin.y;
			}
			return zero;
		}
	}
}
