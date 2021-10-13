using System;
using System.Collections;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.PolygonMesh;
using MatterHackers.RayTracer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class MeshFaceTraceable : IPrimitive
	{
		private Face face;

		public MaterialAbstract Material
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public MeshFaceTraceable(Face face)
		{
			this.face = face;
		}

		public RGBA_Floats GetColor(IntersectInfo info)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return RGBA_Floats.Red;
		}

		public bool GetContained(List<IPrimitive> results, AxisAlignedBoundingBox subRegion)
		{
			if (GetAxisAlignedBoundingBox().Contains(subRegion))
			{
				results.Add((IPrimitive)(object)this);
				return true;
			}
			return false;
		}

		public IntersectInfo GetClosestIntersection(Ray ray)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Expected O, but got Unknown
			Vector3[] array = (Vector3[])(object)new Vector3[3];
			int num = 0;
			foreach (FaceEdge item in face.FaceEdges())
			{
				array[num++] = item.firstVertex.get_Position();
				if (num == 3)
				{
					break;
				}
			}
			Plane val = default(Plane);
			((Plane)(ref val))._002Ector(array[0], array[1], array[2]);
			double num2 = default(double);
			bool flag = default(bool);
			if (((Plane)(ref val)).RayHitPlane(ray, ref num2, ref flag))
			{
				Vector3 val2 = ray.origin + ray.directionNormal * num2;
				if (face.PointInPoly(val2))
				{
					IntersectInfo val3 = new IntersectInfo();
					((IntersectInfo)(nint)val3).closestHitObject = (IPrimitive)(object)this;
					((IntersectInfo)(nint)val3).distanceToHit = num2;
					((IntersectInfo)(nint)val3).hitPosition = val2;
					((IntersectInfo)(nint)val3).normalAtHit = face.normal;
					((IntersectInfo)(nint)val3).hitType = (IntersectionType)1;
					return val3;
				}
			}
			return null;
		}

		public int FindFirstRay(RayBundle rayBundle, int rayIndexToStartCheckingFrom)
		{
			throw new NotImplementedException();
		}

		public void GetClosestIntersections(RayBundle rayBundle, int rayIndexToStartCheckingFrom, IntersectInfo[] intersectionsForBundle)
		{
			throw new NotImplementedException();
		}

		public IEnumerable IntersectionIterator(Ray ray)
		{
			throw new NotImplementedException();
		}

		public double GetSurfaceArea()
		{
			AxisAlignedBoundingBox axisAlignedBoundingBox = GetAxisAlignedBoundingBox();
			double num = Math.Min(axisAlignedBoundingBox.get_XSize(), Math.Min(axisAlignedBoundingBox.get_YSize(), axisAlignedBoundingBox.get_ZSize()));
			if (num == axisAlignedBoundingBox.get_XSize())
			{
				return axisAlignedBoundingBox.get_YSize() * axisAlignedBoundingBox.get_ZSize();
			}
			if (num == axisAlignedBoundingBox.get_YSize())
			{
				return axisAlignedBoundingBox.get_XSize() * axisAlignedBoundingBox.get_ZSize();
			}
			return axisAlignedBoundingBox.get_XSize() * axisAlignedBoundingBox.get_YSize();
		}

		public AxisAlignedBoundingBox GetAxisAlignedBoundingBox()
		{
			return face.GetAxisAlignedBoundingBox();
		}

		public Vector3 GetCenter()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return face.GetCenter();
		}

		public double GetIntersectCost()
		{
			return 700.0;
		}
	}
}
