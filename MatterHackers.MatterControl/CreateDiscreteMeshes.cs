using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.VertexSource;
using MatterHackers.MarchingSquares;
using MatterHackers.PolygonMesh;
using MatterHackers.PolygonMesh.Rendering;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public static class CreateDiscreteMeshes
	{
		public static List<Mesh> SplitConnectedIntoMeshes(MeshGroup meshGroupToSplit, ReportProgressRatio reportProgress)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_0072: Expected O, but got Unknown
			List<Mesh> list = new List<Mesh>();
			double ratioPerDiscreetMesh = 1.0 / (double)meshGroupToSplit.get_Meshes().Count;
			double currentRatioDone = 0.0;
			ReportProgressRatio val = default(ReportProgressRatio);
			foreach (Mesh mesh in meshGroupToSplit.get_Meshes())
			{
				ReportProgressRatio obj = val;
				if (obj == null)
				{
					ReportProgressRatio val2 = delegate(double progress0To1, string processingState, out bool continueProcessing)
					{
						if (reportProgress != null)
						{
							double num = currentRatioDone + ratioPerDiscreetMesh * progress0To1;
							reportProgress.Invoke(num, "Split Into Meshes", ref continueProcessing);
						}
						else
						{
							continueProcessing = true;
						}
					};
					ReportProgressRatio val3 = val2;
					val = val2;
					obj = val3;
				}
				List<Mesh> collection = SplitVolumesIntoMeshes(mesh, obj);
				list.AddRange(collection);
				currentRatioDone += ratioPerDiscreetMesh;
			}
			return list;
		}

		public static List<Mesh> SplitVolumesIntoMeshes(Mesh meshToSplit, ReportProgressRatio reportProgress)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			List<Mesh> list = new List<Mesh>();
			HashSet<Face> val = new HashSet<Face>();
			Mesh val2 = null;
			Stack<Face> val3 = new Stack<Face>();
			bool flag = default(bool);
			for (int i = 0; i < meshToSplit.get_Faces().Count; i++)
			{
				Face val4 = meshToSplit.get_Faces()[i];
				if (!val.Contains(val4))
				{
					val3.Push(val4);
					val2 = new Mesh();
					MeshMaterialData val5 = MeshMaterialData.Get(meshToSplit);
					MeshMaterialData.Get(val2).MaterialIndex = val5.MaterialIndex;
					while (val3.get_Count() > 0)
					{
						foreach (Vertex item2 in val3.Pop().Vertices())
						{
							foreach (Face item3 in item2.ConnectedFaces())
							{
								if (val.Contains(item3))
								{
									continue;
								}
								val.Add(item3);
								val3.Push(item3);
								List<Vertex> list2 = new List<Vertex>();
								foreach (FaceEdge item4 in item3.FaceEdges())
								{
									Vertex item = val2.CreateVertex(item4.firstVertex.get_Position(), (CreateOption)0, (SortOption)1, 0.0);
									list2.Add(item);
								}
								val2.CreateFace(list2.ToArray(), (CreateOption)0);
							}
						}
					}
					val2.CleanAndMergMesh(0.0, (ReportProgressRatio)null);
					list.Add(val2);
					val2 = null;
				}
				if (reportProgress != null)
				{
					double num = (double)i / (double)meshToSplit.get_Faces().Count;
					reportProgress.Invoke(num, "Split Into Meshes", ref flag);
				}
			}
			return list;
		}

		public static Mesh[] SplitIntoMeshesOnOrthographicZ(Mesh meshToSplit, Vector3 buildVolume, ReportProgressRatio reportProgress)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Expected O, but got Unknown
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			AxisAlignedBoundingBox axisAlignedBoundingBox = meshToSplit.GetAxisAlignedBoundingBox();
			buildVolume.x = Math.Max(buildVolume.x, axisAlignedBoundingBox.get_XSize() + 2.0);
			buildVolume.y = Math.Max(buildVolume.y, axisAlignedBoundingBox.get_YSize() + 2.0);
			buildVolume.z = Math.Max(buildVolume.z, axisAlignedBoundingBox.get_ZSize() + 2.0);
			double num = 5.0;
			ImageBuffer val = new ImageBuffer((int)(buildVolume.x * num), (int)(buildVolume.y * num));
			Vector2 val2 = new Vector2(buildVolume.x / 2.0, buildVolume.y / 2.0) - new Vector2(axisAlignedBoundingBox.get_Center().x, axisAlignedBoundingBox.get_Center().y);
			OrthographicZProjection.DrawTo(val.NewGraphics2D(), meshToSplit, val2, num, RGBA_Bytes.White);
			bool flag = true;
			if (reportProgress != null)
			{
				reportProgress.Invoke(0.2, "", ref flag);
			}
			Dilate.DoDilate3x3Binary(val, 1);
			PolyTree val3 = FindDistictObjectBounds(val);
			if (val3 == null)
			{
				return (Mesh[])(object)new Mesh[1]
				{
					meshToSplit
				};
			}
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			GetAreasRecursive((PolyNode)(object)val3, list);
			if (list.Count == 0)
			{
				return null;
			}
			if (list.Count == 1)
			{
				return (Mesh[])(object)new Mesh[1]
				{
					meshToSplit
				};
			}
			Graphics2D val4 = val.NewGraphics2D();
			val4.Clear((IColorType)(object)RGBA_Bytes.Black);
			Random random = new Random();
			foreach (List<IntPoint> item2 in list)
			{
				val4.Render((IVertexSource)(object)PlatingHelper.PolygonToPathStorage(item2), (IColorType)(object)new RGBA_Bytes(random.Next(128, 255), random.Next(128, 255), random.Next(128, 255)));
			}
			if (reportProgress != null)
			{
				reportProgress.Invoke(0.5, "", ref flag);
			}
			Mesh[] array = (Mesh[])(object)new Mesh[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = new Mesh();
			}
			Vector2 val5 = default(Vector2);
			foreach (Face face in meshToSplit.get_Faces())
			{
				bool flag2 = false;
				foreach (FaceEdge item3 in face.FaceEdges())
				{
					((Vector2)(ref val5))._002Ector(item3.firstVertex.get_Position().x, item3.firstVertex.get_Position().y);
					val5 += val2;
					val5 *= num;
					for (int num2 = list.Count - 1; num2 >= 0; num2--)
					{
						if (PointInPolygon(list[num2], new IntPoint((long)(int)val5.x, (long)(int)val5.y)))
						{
							List<Vertex> list2 = new List<Vertex>();
							foreach (FaceEdge item4 in face.FaceEdges())
							{
								Vertex item = array[num2].CreateVertex(item4.firstVertex.get_Position(), (CreateOption)1, (SortOption)0, 0.0);
								list2.Add(item);
							}
							array[num2].CreateFace(list2.ToArray(), (CreateOption)1);
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						break;
					}
				}
			}
			if (reportProgress != null)
			{
				reportProgress.Invoke(0.8, "", ref flag);
			}
			for (int j = 0; j < Enumerable.Count<Mesh>((IEnumerable<Mesh>)array); j++)
			{
				_ = array[j];
			}
			return array;
		}

		public static bool PointInPolygon(List<IntPoint> polygon, IntPoint testPosition)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			int count = polygon.Count;
			bool flag = false;
			for (int i = 0; i < count; i++)
			{
				int num = i - 1;
				if (num < 0)
				{
					num += count;
				}
				if (((polygon[i].Y <= testPosition.Y && testPosition.Y < polygon[num].Y) || (polygon[num].Y <= testPosition.Y && testPosition.Y < polygon[i].Y)) && testPosition.X - polygon[i].X < (polygon[num].X - polygon[i].X) * (testPosition.Y - polygon[i].Y) / (polygon[num].Y - polygon[i].Y))
				{
					flag = !flag;
				}
			}
			return flag;
		}

		private static void GetAreasRecursive(PolyNode polyTreeForPlate, List<List<IntPoint>> discreteAreas)
		{
			if (!polyTreeForPlate.get_IsHole())
			{
				discreteAreas.Add(polyTreeForPlate.get_Contour());
			}
			foreach (PolyNode child in polyTreeForPlate.get_Childs())
			{
				GetAreasRecursive(child, discreteAreas);
			}
		}

		public static PolyTree FindDistictObjectBounds(ImageBuffer image)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Expected O, but got Unknown
			MarchingSquaresByte val = new MarchingSquaresByte(image, 5, 0);
			val.CreateLineSegments();
			List<List<IntPoint>> list = val.CreateLineLoops(1, int.MaxValue);
			if (list.Count == 1)
			{
				return null;
			}
			IntPoint val2 = default(IntPoint);
			((IntPoint)(ref val2))._002Ector(long.MaxValue, long.MaxValue);
			IntPoint val3 = default(IntPoint);
			((IntPoint)(ref val3))._002Ector(long.MinValue, long.MinValue);
			foreach (List<IntPoint> item in list)
			{
				foreach (IntPoint item2 in item)
				{
					val2.X = Math.Min(item2.X - 10, val2.X);
					val2.Y = Math.Min(item2.Y - 10, val2.Y);
					val3.X = Math.Max(item2.X + 10, val3.X);
					val3.Y = Math.Max(item2.Y + 10, val3.Y);
				}
			}
			List<IntPoint> list2 = new List<IntPoint>();
			list2.Add(val2);
			list2.Add(new IntPoint(val2.X, val3.Y));
			list2.Add(val3);
			list2.Add(new IntPoint(val3.X, val2.Y));
			Clipper val4 = new Clipper(0);
			((ClipperBase)val4).AddPaths(list, (PolyType)0, true);
			((ClipperBase)val4).AddPath(list2, (PolyType)1, true);
			PolyTree val5 = new PolyTree();
			val4.Execute((ClipType)0, val5);
			return val5;
		}
	}
}
