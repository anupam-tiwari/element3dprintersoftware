using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.PolygonMesh;
using MatterHackers.RayTracer;
using MatterHackers.VectorMath;

namespace MatterHackers.WellPlate
{
	public static class MeshToBVH
	{
		public static IPrimitive Convert(Mesh simpleMesh, MaterialAbstract partMaterial = null)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected O, but got Unknown
			List<IPrimitive> list = new List<IPrimitive>();
			if (partMaterial == null)
			{
				partMaterial = (MaterialAbstract)new SolidMaterial(new RGBA_Floats(0.9, 0.2, 0.1), 0.01, 0.0, 2.0);
			}
			int num = 0;
			Vector3[] array = (Vector3[])(object)new Vector3[3];
			foreach (Face face in simpleMesh.get_Faces())
			{
				foreach (Vertex item in face.Vertices())
				{
					array[num++] = item.get_Position();
					if (num == 3)
					{
						num = 0;
						list.Add((IPrimitive)new TriangleShape(array[0], array[1], array[2], partMaterial));
					}
				}
			}
			return BoundingVolumeHierarchy.CreateNewHierachy(list, int.MaxValue, 0, (SortingAccelerator)null);
		}

		public static IPrimitive Convert(List<MeshGroup> meshGroups, MaterialAbstract partMaterial = null)
		{
			List<IPrimitive> list = new List<IPrimitive>();
			foreach (MeshGroup meshGroup in meshGroups)
			{
				list.Add(Convert(meshGroup, partMaterial));
			}
			return BoundingVolumeHierarchy.CreateNewHierachy(list, int.MaxValue, 0, (SortingAccelerator)null);
		}

		public static IPrimitive Convert(MeshGroup meshGroup, MaterialAbstract partMaterial = null)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Expected O, but got Unknown
			List<IPrimitive> list = new List<IPrimitive>();
			int num = 0;
			Vector3[] array = (Vector3[])(object)new Vector3[3];
			foreach (Mesh mesh in meshGroup.get_Meshes())
			{
				RGBA_Bytes materialColor = MeshViewerWidget.GetMaterialColor(MeshMaterialData.Get(mesh).MaterialIndex);
				SolidMaterial val = new SolidMaterial(((RGBA_Bytes)(ref materialColor)).GetAsRGBA_Floats(), 0.01, 0.0, 2.0);
				foreach (Face face in mesh.get_Faces())
				{
					foreach (Vertex item in face.Vertices())
					{
						array[num++] = item.get_Position();
						if (num == 3)
						{
							num = 0;
							list.Add((IPrimitive)new TriangleShape(array[0], array[1], array[2], (MaterialAbstract)(object)val));
						}
					}
				}
			}
			return BoundingVolumeHierarchy.CreateNewHierachy(list, int.MaxValue, 0, (SortingAccelerator)null);
		}

		public static IPrimitive ConvertUnoptomized(Mesh simpleMesh)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Expected O, but got Unknown
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			List<IPrimitive> list = new List<IPrimitive>();
			SolidMaterial val = new SolidMaterial(new RGBA_Floats(0.0, 0.32, 0.58), 0.01, 0.0, 2.0);
			int num = 0;
			Vector3[] array = (Vector3[])(object)new Vector3[3];
			foreach (Face face in simpleMesh.get_Faces())
			{
				foreach (Vertex item in face.Vertices())
				{
					array[num++] = item.get_Position();
					if (num == 3)
					{
						num = 0;
						list.Add((IPrimitive)new TriangleShape(array[0], array[1], array[2], (MaterialAbstract)(object)val));
					}
				}
			}
			return (IPrimitive)new UnboundCollection((IList<IPrimitive>)list);
		}
	}
}
