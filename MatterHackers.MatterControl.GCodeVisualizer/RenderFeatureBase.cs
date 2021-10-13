using MatterHackers.Agg;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public abstract class RenderFeatureBase
	{
		protected int extruderIndex;

		public abstract void Render(Graphics2D graphics2D, GCodeRenderInfo renderInfo);

		public abstract void CreateRender3DData(VectorPOD<ColorVertexData> colorVertexData, VectorPOD<int> indexData, GCodeRenderInfo renderInfo);

		public RenderFeatureBase(int extruderIndex)
		{
			this.extruderIndex = extruderIndex;
		}

		public static void CreateCylinder(VectorPOD<ColorVertexData> colorVertexData, VectorPOD<int> indexData, Vector3 startPos, Vector3 endPos, double radius, int steps, RGBA_Bytes color, double layerHeight)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = endPos - startPos;
			Vector3 normal = ((Vector3)(ref val)).GetNormal();
			Vector3 val2 = Vector3.GetPerpendicular(startPos, endPos);
			Vector3 normal2 = ((Vector3)(ref val2)).GetNormal();
			int[] array = new int[steps];
			int[] array2 = new int[steps];
			int[] array3 = new int[steps];
			int[] array4 = new int[steps];
			double num = (layerHeight / 2.0 + layerHeight * 0.1) / radius;
			double num2 = radius / radius;
			Vector3 val3 = default(Vector3);
			((Vector3)(ref val3))._002Ector(num2, num2, num);
			for (int i = 0; i < steps; i++)
			{
				Vector3 normal3 = Vector3.Transform(normal2, Matrix4X4.CreateRotation(val, 6.2831854820251465 / (double)(steps * 2) + 6.2831854820251465 / (double)steps * (double)i));
				Vector3 val4 = Vector3.Transform(normal2 * radius, Matrix4X4.CreateRotation(val, 6.2831854820251465 / (double)(steps * 2) + 6.2831854820251465 / (double)steps * (double)i));
				val4 *= val3;
				Vector3 position = startPos + val4;
				array[i] = colorVertexData.get_Count();
				colorVertexData.Add(new ColorVertexData(position, normal3, color));
				Vector3 position2 = endPos + val4;
				array2[i] = colorVertexData.get_Count();
				colorVertexData.Add(new ColorVertexData(position2, normal3, color));
				Vector3 val5 = Vector3.Cross(val, normal2);
				Vector3 val6 = Vector3.Transform(normal2, Matrix4X4.CreateRotation(val5, 0.7853981852531433));
				val6 = Vector3.Transform(val6, Matrix4X4.CreateRotation(val, 6.2831854820251465 / (double)(steps * 2) + 6.2831854820251465 / (double)steps * (double)i));
				val2 = val6 * val3;
				val6 = ((Vector3)(ref val2)).GetNormal();
				Vector3 val7 = val6 * radius;
				val7 *= val3;
				Vector3 position3 = startPos + val7;
				array3[i] = colorVertexData.get_Count();
				colorVertexData.Add(new ColorVertexData(position3, val6, color));
				Vector3 val8 = Vector3.Transform(normal2, Matrix4X4.CreateRotation(-val5, 0.7853981852531433));
				val8 = Vector3.Transform(val8, Matrix4X4.CreateRotation(val, 6.2831854820251465 / (double)(steps * 2) + 6.2831854820251465 / (double)steps * (double)i));
				val2 = val8 * val3;
				val8 = ((Vector3)(ref val2)).GetNormal();
				Vector3 val9 = val8 * radius;
				val9 *= val3;
				Vector3 position4 = endPos + val9;
				array4[i] = colorVertexData.get_Count();
				colorVertexData.Add(new ColorVertexData(position4, val8, color));
			}
			int count = colorVertexData.get_Count();
			Vector3 val10 = normal * radius;
			val10 *= val3;
			colorVertexData.Add(new ColorVertexData(startPos - val10, -normal, color));
			int count2 = colorVertexData.get_Count();
			colorVertexData.Add(new ColorVertexData(endPos + val10, normal, color));
			for (int j = 0; j < steps; j++)
			{
				indexData.Add(array[j]);
				indexData.Add(array2[j]);
				indexData.Add(array2[(j + 1) % steps]);
				indexData.Add(array[j]);
				indexData.Add(array2[(j + 1) % steps]);
				indexData.Add(array[(j + 1) % steps]);
				indexData.Add(array[j]);
				indexData.Add(array3[j]);
				indexData.Add(array3[(j + 1) % steps]);
				indexData.Add(array[j]);
				indexData.Add(array3[(j + 1) % steps]);
				indexData.Add(array[(j + 1) % steps]);
				indexData.Add(array2[j]);
				indexData.Add(array4[j]);
				indexData.Add(array4[(j + 1) % steps]);
				indexData.Add(array2[j]);
				indexData.Add(array4[(j + 1) % steps]);
				indexData.Add(array2[(j + 1) % steps]);
				indexData.Add(count);
				indexData.Add(array3[j]);
				indexData.Add(array3[(j + 1) % steps]);
				indexData.Add(count2);
				indexData.Add(array4[j]);
				indexData.Add(array4[(j + 1) % steps]);
			}
		}

		public static void CreatePointer(VectorPOD<ColorVertexData> colorVertexData, VectorPOD<int> indexData, Vector3 startPos, Vector3 endPos, double radius, int steps, RGBA_Bytes color)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = endPos - startPos;
			Vector3 normal = ((Vector3)(ref val)).GetNormal();
			Vector3 perpendicular = Vector3.GetPerpendicular(startPos, endPos);
			Vector3 normal2 = ((Vector3)(ref perpendicular)).GetNormal();
			int[] array = new int[steps];
			for (int i = 0; i < steps; i++)
			{
				Vector3 normal3 = Vector3.Transform(normal2, Matrix4X4.CreateRotation(val, 6.2831854820251465 / (double)(steps * 2) + 6.2831854820251465 / (double)steps * (double)i));
				Vector3 val2 = Vector3.Transform(normal2 * radius, Matrix4X4.CreateRotation(val, 6.2831854820251465 / (double)(steps * 2) + 6.2831854820251465 / (double)steps * (double)i));
				Vector3 position = startPos + val2;
				array[i] = colorVertexData.get_Count();
				colorVertexData.Add(new ColorVertexData(position, normal3, color));
				_ = endPos + val2;
			}
			int count = colorVertexData.get_Count();
			colorVertexData.Add(new ColorVertexData(endPos, normal, color));
			for (int j = 0; j < steps; j++)
			{
				indexData.Add(array[j]);
				indexData.Add(array[(j + 1) % steps]);
				indexData.Add(count);
			}
		}
	}
}
