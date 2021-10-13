using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.PolygonMesh;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.Slicing
{
	public class SliceLayers
	{
		private List<SliceLayer> allLayers = new List<SliceLayer>();

		public List<SliceLayer> AllLayers => allLayers;

		public void GetPerimetersForAllLayers(Mesh meshToSlice, double firstLayerHeight, double otherLayerHeights)
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			AllLayers.Clear();
			int num = (int)((meshToSlice.GetAxisAlignedBoundingBox().get_ZSize() - firstLayerHeight) / otherLayerHeights + 0.5);
			double num2 = otherLayerHeights;
			if (firstLayerHeight > 0.0)
			{
				num++;
				num2 = firstLayerHeight;
			}
			for (int i = 0; i < num; i++)
			{
				allLayers.Add(new SliceLayer(num2));
				num2 += otherLayerHeights;
			}
			Plane val = default(Plane);
			Vector3 val2 = default(Vector3);
			Vector3 val3 = default(Vector3);
			foreach (Face face in meshToSlice.get_Faces())
			{
				double num3 = double.MaxValue;
				double num4 = double.MinValue;
				foreach (FaceEdge item in face.FaceEdges())
				{
					num3 = Math.Min(num3, item.firstVertex.get_Position().z);
					num4 = Math.Max(num4, item.firstVertex.get_Position().z);
				}
				for (int j = 0; j < num; j++)
				{
					SliceLayer sliceLayer = allLayers[j];
					double zHeight = sliceLayer.ZHeight;
					if (!(zHeight < num3))
					{
						if (zHeight > num4)
						{
							break;
						}
						((Plane)(ref val))._002Ector(Vector3.UnitZ, zHeight);
						if (face.GetCutLine(val, ref val2, ref val3))
						{
							sliceLayer.UnorderedSegments.Add(new SliceLayer.Segment(new Vector2(val2.x, val2.y), new Vector2(val3.x, val3.y)));
						}
					}
				}
			}
		}

		public SliceLayer GetPerimetersAtHeight(Mesh meshToSlice, double zHeight)
		{
			throw new NotImplementedException();
		}

		public void DumpSegmentsToGcode(string filename)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			StreamWriter streamWriter = new StreamWriter(filename);
			streamWriter.Write("; some gcode to look at the layer segments");
			int num = 0;
			for (int i = 0; i < allLayers.Count; i++)
			{
				streamWriter.Write(StringHelper.FormatWith("; LAYER:{0}\n", new object[1]
				{
					i
				}));
				List<SliceLayer.Segment> unorderedSegments = allLayers[i].UnorderedSegments;
				for (int j = 0; j < unorderedSegments.Count; j++)
				{
					SliceLayer.Segment segment = unorderedSegments[j];
					streamWriter.Write("G1 X{0}Y{1}\n", segment.start.x, segment.start.y);
					streamWriter.Write("G1 X{0}Y{1}E{2}\n", segment.end.x, segment.end.y, num++);
				}
			}
			streamWriter.Close();
		}
	}
}
