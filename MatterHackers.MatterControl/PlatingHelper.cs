using System;
using System.Collections.Generic;
using ClipperLib;
using MatterHackers.Agg;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Localizations;
using MatterHackers.PolygonMesh;
using MatterHackers.RayTracer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public static class PlatingHelper
	{
		public static PathStorage PolygonToPathStorage(List<IntPoint> polygon)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			PathStorage val = new PathStorage();
			bool flag = true;
			foreach (IntPoint item in polygon)
			{
				if (flag)
				{
					val.Add((double)item.X, (double)item.Y, (FlagsAndCommand)1);
					flag = false;
				}
				else
				{
					val.Add((double)item.X, (double)item.Y, (FlagsAndCommand)2);
				}
			}
			val.ClosePolygon();
			val.Add(0.0, 0.0, (FlagsAndCommand)0);
			return val;
		}

		public static PathStorage PolygonToPathStorage(List<List<IntPoint>> polygons)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			PathStorage val = new PathStorage();
			foreach (List<IntPoint> polygon in polygons)
			{
				bool flag = true;
				foreach (IntPoint item in polygon)
				{
					if (flag)
					{
						val.Add((double)item.X, (double)item.Y, (FlagsAndCommand)1);
						flag = false;
					}
					else
					{
						val.Add((double)item.X, (double)item.Y, (FlagsAndCommand)2);
					}
				}
				val.ClosePolygon();
			}
			val.Add(0.0, 0.0, (FlagsAndCommand)0);
			return val;
		}

		public static void ArrangeMeshGroups(List<MeshGroup> asyncMeshGroups, List<Matrix4X4> asyncMeshGroupTransforms, List<PlatingMeshGroupData> asyncPlatingDatas, Action<double, string> reportProgressChanged)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < asyncMeshGroups.Count; i++)
			{
				List<Matrix4X4> list = asyncMeshGroupTransforms;
				int index = i;
				list[index] *= Matrix4X4.CreateTranslation(10000.0, 10000.0, 0.0);
			}
			for (int j = 0; j < asyncMeshGroups.Count; j++)
			{
				AxisAlignedBoundingBox val = asyncMeshGroups[j].GetAxisAlignedBoundingBox(asyncMeshGroupTransforms[j]);
				for (int k = j + 1; k < asyncMeshGroups.Count; k++)
				{
					AxisAlignedBoundingBox axisAlignedBoundingBox = asyncMeshGroups[k].GetAxisAlignedBoundingBox(asyncMeshGroupTransforms[k]);
					if (Math.Max(val.get_XSize(), val.get_YSize()) < Math.Max(axisAlignedBoundingBox.get_XSize(), axisAlignedBoundingBox.get_YSize()))
					{
						PlatingMeshGroupData value = asyncPlatingDatas[j];
						asyncPlatingDatas[j] = asyncPlatingDatas[k];
						asyncPlatingDatas[k] = value;
						MeshGroup value2 = asyncMeshGroups[j];
						asyncMeshGroups[j] = asyncMeshGroups[k];
						asyncMeshGroups[k] = value2;
						Matrix4X4 val2 = asyncMeshGroupTransforms[j];
						Matrix4X4 val3 = asyncMeshGroupTransforms[k];
						Matrix4X4 val4 = val2;
						val2 = val3;
						val3 = (asyncMeshGroupTransforms[j] = val4);
						asyncMeshGroupTransforms[k] = val2;
						val = axisAlignedBoundingBox;
					}
				}
			}
			double num = 1.0 / (double)asyncMeshGroups.Count;
			double num2 = 0.0;
			for (int l = 0; l < asyncMeshGroups.Count; l++)
			{
				reportProgressChanged(num2, "Calculating Positions...".Localize());
				Vector3 minXYZ = asyncMeshGroups[l].GetAxisAlignedBoundingBox(asyncMeshGroupTransforms[l]).minXYZ;
				List<Matrix4X4> list = asyncMeshGroupTransforms;
				int index = l;
				list[index] *= Matrix4X4.CreateTranslation(-minXYZ);
				MoveMeshGroupToOpenPosition(l, asyncPlatingDatas, asyncMeshGroups, asyncMeshGroupTransforms);
				if (asyncPlatingDatas[l].meshTraceableData.Count == 0)
				{
					CreateITraceableForMeshGroup(asyncPlatingDatas, asyncMeshGroups, l, null);
				}
				num2 += num;
				PlaceMeshGroupOnBed(asyncMeshGroups, asyncMeshGroupTransforms, l);
			}
			AxisAlignedBoundingBox val6 = asyncMeshGroups[0].GetAxisAlignedBoundingBox(asyncMeshGroupTransforms[0]);
			for (int m = 1; m < asyncMeshGroups.Count; m++)
			{
				val6 = AxisAlignedBoundingBox.Union(val6, asyncMeshGroups[m].GetAxisAlignedBoundingBox(asyncMeshGroupTransforms[m]));
			}
			Vector3 val7 = (val6.maxXYZ + val6.minXYZ) / 2.0;
			for (int n = 0; n < asyncMeshGroups.Count; n++)
			{
				List<Matrix4X4> list = asyncMeshGroupTransforms;
				int index = n;
				list[index] *= Matrix4X4.CreateTranslation(-val7 + new Vector3(0.0, 0.0, val6.get_ZSize() / 2.0));
			}
		}

		public static void PlaceMeshGroupOnBed(List<MeshGroup> meshesGroupList, List<Matrix4X4> meshTransforms, int index)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			AxisAlignedBoundingBox axisAlignedBoundingBox = GetAxisAlignedBoundingBox(meshesGroupList[index], meshTransforms[index]);
			Vector3 val = (axisAlignedBoundingBox.maxXYZ + axisAlignedBoundingBox.minXYZ) / 2.0;
			meshTransforms[index] *= Matrix4X4.CreateTranslation(new Vector3(0.0, 0.0, 0.0 - val.z + axisAlignedBoundingBox.get_ZSize() / 2.0));
		}

		public static void PlaceMeshAtHeight(List<MeshGroup> meshesGroupList, List<Matrix4X4> meshTransforms, int index, double zHeight)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			AxisAlignedBoundingBox axisAlignedBoundingBox = GetAxisAlignedBoundingBox(meshesGroupList[index], meshTransforms[index]);
			Vector3 val = (axisAlignedBoundingBox.maxXYZ + axisAlignedBoundingBox.minXYZ) / 2.0;
			meshTransforms[index] *= Matrix4X4.CreateTranslation(new Vector3(0.0, 0.0, zHeight - val.z + axisAlignedBoundingBox.get_ZSize() / 2.0));
		}

		public static void CenterMeshGroupXY(List<MeshGroup> meshesGroupList, List<Matrix4X4> meshTransforms, int index)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			AxisAlignedBoundingBox axisAlignedBoundingBox = GetAxisAlignedBoundingBox(meshesGroupList[index], meshTransforms[index]);
			Vector3 val = (axisAlignedBoundingBox.maxXYZ + axisAlignedBoundingBox.minXYZ) / 2.0;
			meshTransforms[index] *= Matrix4X4.CreateTranslation(new Vector3(0.0 - val.x + axisAlignedBoundingBox.get_XSize() / 2.0, 0.0 - val.y + axisAlignedBoundingBox.get_YSize() / 2.0, 0.0));
		}

		public static void FindPositionForGroupAndAddToPlate(MeshGroup meshGroupToAdd, Matrix4X4 meshTransform, List<PlatingMeshGroupData> perMeshInfo, List<MeshGroup> meshesGroupsToAvoid, List<Matrix4X4> meshTransforms)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			if (meshGroupToAdd != null && meshGroupToAdd.get_Meshes().Count >= 1)
			{
				AxisAlignedBoundingBox val = GetAxisAlignedBoundingBox(meshesGroupsToAvoid[0], meshTransforms[0]);
				for (int i = 1; i < meshesGroupsToAvoid.Count; i++)
				{
					AxisAlignedBoundingBox axisAlignedBoundingBox = GetAxisAlignedBoundingBox(meshesGroupsToAvoid[i], meshTransforms[i]);
					val = AxisAlignedBoundingBox.Union(val, axisAlignedBoundingBox);
				}
				meshesGroupsToAvoid.Add(meshGroupToAdd);
				PlatingMeshGroupData item = new PlatingMeshGroupData();
				perMeshInfo.Add(item);
				meshTransforms.Add(meshTransform);
				int num = meshesGroupsToAvoid.Count - 1;
				Vector3 minXYZ = GetAxisAlignedBoundingBox(meshesGroupsToAvoid[num], meshTransforms[num]).minXYZ;
				int index = num;
				meshTransforms[index] *= Matrix4X4.CreateTranslation(-minXYZ + val.minXYZ);
				MoveMeshGroupToOpenPosition(num, perMeshInfo, meshesGroupsToAvoid, meshTransforms);
				PlaceMeshGroupOnBed(meshesGroupsToAvoid, meshTransforms, num);
			}
		}

		private static AxisAlignedBoundingBox GetAxisAlignedBoundingBox(MeshGroup meshGroup, Matrix4X4 transform)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return meshGroup.GetAxisAlignedBoundingBox(transform);
		}

		public static void MoveMeshGroupToOpenPosition(int meshGroupToMoveIndex, List<PlatingMeshGroupData> perMeshInfo, List<MeshGroup> allMeshGroups, List<Matrix4X4> meshTransforms)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			AxisAlignedBoundingBox val = GetAxisAlignedBoundingBox(allMeshGroups[0], meshTransforms[0]);
			for (int i = 1; i < meshGroupToMoveIndex; i++)
			{
				AxisAlignedBoundingBox axisAlignedBoundingBox = GetAxisAlignedBoundingBox(allMeshGroups[i], meshTransforms[i]);
				val = AxisAlignedBoundingBox.Union(val, axisAlignedBoundingBox);
			}
			_ = val.minXYZ;
			_ = val.minXYZ;
			MeshGroup val2 = allMeshGroups[meshGroupToMoveIndex];
			AxisAlignedBoundingBox axisAlignedBoundingBox2 = GetAxisAlignedBoundingBox(val2, meshTransforms[meshGroupToMoveIndex]);
			axisAlignedBoundingBox2.minXYZ -= new Vector3(2.0, 2.0, 0.0);
			axisAlignedBoundingBox2.maxXYZ += new Vector3(2.0, 2.0, 0.0);
			Matrix4X4 transform = Matrix4X4.Identity;
			int num = 1;
			bool flag = false;
			while (!flag && meshGroupToMoveIndex > 0)
			{
				int num2 = 0;
				int xStep = num;
				for (num2 = 0; num2 < num; num2++)
				{
					flag = CheckPosition(meshGroupToMoveIndex, allMeshGroups, meshTransforms, val2, axisAlignedBoundingBox2, num2, xStep, ref transform);
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					num2 = num;
					for (xStep = 0; xStep < num; xStep++)
					{
						flag = CheckPosition(meshGroupToMoveIndex, allMeshGroups, meshTransforms, val2, axisAlignedBoundingBox2, num2, xStep, ref transform);
						if (flag)
						{
							break;
						}
					}
					if (!flag)
					{
						xStep = num;
						flag = CheckPosition(meshGroupToMoveIndex, allMeshGroups, meshTransforms, val2, axisAlignedBoundingBox2, num2, xStep, ref transform);
					}
				}
				num++;
			}
			meshTransforms[meshGroupToMoveIndex] *= transform;
		}

		private static bool CheckPosition(int meshGroupToMoveIndex, List<MeshGroup> allMeshGroups, List<Matrix4X4> meshTransforms, MeshGroup meshGroupToMove, AxisAlignedBoundingBox meshToMoveBounds, int yStep, int xStep, ref Matrix4X4 transform)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			double num = 5.0;
			double num2 = 5.0;
			Matrix4X4 val = Matrix4X4.CreateTranslation((double)xStep * num, (double)yStep * num2, 0.0);
			Vector3 val2 = Vector3.Transform(Vector3.Zero, val);
			transform = Matrix4X4.CreateTranslation(val2);
			AxisAlignedBoundingBox val3 = meshToMoveBounds.NewTransformed(transform);
			bool flag = false;
			for (int i = 0; i < meshGroupToMoveIndex; i++)
			{
				MeshGroup val4 = allMeshGroups[i];
				if (val4 != meshGroupToMove)
				{
					AxisAlignedBoundingBox axisAlignedBoundingBox = GetAxisAlignedBoundingBox(val4, meshTransforms[i]);
					AxisAlignedBoundingBox val5 = AxisAlignedBoundingBox.Intersection(val3, axisAlignedBoundingBox);
					if (val5.get_XSize() > 0.0 && val5.get_YSize() > 0.0)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return true;
			}
			return false;
		}

		public static void CreateITraceableForMeshGroup(List<PlatingMeshGroupData> perMeshGroupInfo, List<MeshGroup> meshGroups, int meshGroupIndex, ReportProgressRatio reportProgress)
		{
			if (meshGroups == null)
			{
				return;
			}
			MeshGroup val = meshGroups[meshGroupIndex];
			perMeshGroupInfo[meshGroupIndex].meshTraceableData.Clear();
			int num = 0;
			foreach (Mesh mesh in val.get_Meshes())
			{
				num += mesh.get_Faces().Count;
			}
			int currentAction = 0;
			bool needToUpdateProgressReport = true;
			bool flag = default(bool);
			for (int i = 0; i < val.get_Meshes().Count; i++)
			{
				List<IPrimitive> list = AddTraceDataForMesh(val.get_Meshes()[i], num, ref currentAction, ref needToUpdateProgressReport, reportProgress);
				needToUpdateProgressReport = true;
				if (reportProgress != null)
				{
					reportProgress.Invoke((double)currentAction / (double)num, "Creating Trace Group", ref flag);
				}
				IPrimitive item = BoundingVolumeHierarchy.CreateNewHierachy(list, 0, 0, (SortingAccelerator)null);
				perMeshGroupInfo[meshGroupIndex].meshTraceableData.Add(item);
			}
		}

		public static IPrimitive CreateTraceDataForMesh(Mesh mesh)
		{
			int currentAction = 0;
			bool needToUpdateProgressReport = false;
			return BoundingVolumeHierarchy.CreateNewHierachy(AddTraceDataForMesh(mesh, 0, ref currentAction, ref needToUpdateProgressReport, null), int.MaxValue, 0, (SortingAccelerator)null);
		}

		private static List<IPrimitive> AddTraceDataForMesh(Mesh mesh, int totalActionCount, ref int currentAction, ref bool needToUpdateProgressReport, ReportProgressRatio reportProgress)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			List<IPrimitive> list = new List<IPrimitive>();
			List<Vector3> list2 = new List<Vector3>();
			bool flag = default(bool);
			foreach (Face face in mesh.get_Faces())
			{
				list2.Clear();
				foreach (Vertex item2 in face.Vertices())
				{
					list2.Add(item2.get_Position());
				}
				Vector3 val = list2[1];
				for (int i = 2; i < list2.Count; i++)
				{
					TriangleShape item = new TriangleShape(list2[0], val, list2[i], (MaterialAbstract)null);
					list.Add((IPrimitive)(object)item);
					val = list2[i];
				}
				if (reportProgress != null)
				{
					if ((currentAction % 256 == 0) | needToUpdateProgressReport)
					{
						reportProgress.Invoke((double)currentAction / (double)totalActionCount, "Creating Trace Polygons", ref flag);
						needToUpdateProgressReport = false;
					}
					currentAction++;
				}
			}
			return list;
		}

		public static Matrix4X4 ApplyAtCenter(IHasAABB meshToApplyTo, Matrix4X4 currentTransform, Matrix4X4 transformToApply)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			return ApplyAtCenter(meshToApplyTo.GetAxisAlignedBoundingBox(currentTransform), currentTransform, transformToApply);
		}

		public static Matrix4X4 ApplyAtCenter(AxisAlignedBoundingBox boundsToApplyTo, Matrix4X4 currentTransform, Matrix4X4 transformToApply)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return ApplyAtPosition(currentTransform, transformToApply, boundsToApplyTo.get_Center());
		}

		public static Matrix4X4 ApplyAtPosition(Matrix4X4 currentTransform, Matrix4X4 transformToApply, Vector3 postionToApplyAt)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			currentTransform *= Matrix4X4.CreateTranslation(-postionToApplyAt);
			currentTransform *= transformToApply;
			currentTransform *= Matrix4X4.CreateTranslation(postionToApplyAt);
			return currentTransform;
		}
	}
}
