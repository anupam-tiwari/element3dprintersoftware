using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.PolygonMesh;
using MatterHackers.RayTracer;
using MatterHackers.RayTracer.Traceable;
using MatterHackers.RenderOpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class DebugBvh
	{
		private int startRenderLevel;

		private int endRenderLevel;

		private Stack<Matrix4X4> transform = new Stack<Matrix4X4>();

		private Mesh lineMesh = PlatonicSolids.CreateCube(1.0, 1.0, 1.0);

		public static void Render(IPrimitive bvhToRender, Matrix4X4 startingTransform, int startRenderLevel = 0, int endRenderLevel = int.MaxValue)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			DebugBvh debugBvh = new DebugBvh(startRenderLevel, endRenderLevel);
			debugBvh.transform.Push(startingTransform);
			debugBvh.RenderRecursive((dynamic)bvhToRender);
		}

		public DebugBvh(int startRenderLevel = 0, int endRenderLevel = int.MaxValue)
		{
			this.startRenderLevel = startRenderLevel;
			this.endRenderLevel = endRenderLevel;
		}

		private void RenderLine(Matrix4X4 transform, Vector3 start, Vector3 end, RGBA_Bytes color, bool zBuffered = true)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = (start + end) / 2.0;
			Vector3 val2 = start - end;
			Matrix4X4 val3 = Matrix4X4.CreateRotation(new Quaternion(Vector3.UnitX + new Vector3(0.0001, -1E-05, 2E-05), ((Vector3)(ref val2)).GetNormal()));
			Vector3 val4 = end - start;
			Matrix4X4 val5 = Matrix4X4.CreateScale(((Vector3)(ref val4)).get_Length(), 1.0, 1.0) * val3 * Matrix4X4.CreateTranslation(val) * transform;
			if (zBuffered)
			{
				RenderMeshToGl.Render(lineMesh, (IColorType)(object)RGBA_Bytes.Black, val5, (RenderTypes)1);
			}
			else
			{
				RenderMeshToGl.Render(lineMesh, (IColorType)(object)new RGBA_Bytes(RGBA_Bytes.Black, 5), val5, (RenderTypes)1);
			}
		}

		private void RenderBounds(AxisAlignedBoundingBox aabb)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			RGBA_Bytes red = RGBA_Bytes.Red;
			RenderLine(transform.Peek(), aabb.GetBottomCorner(0), aabb.GetBottomCorner(1), red);
			RenderLine(transform.Peek(), aabb.GetBottomCorner(1), aabb.GetBottomCorner(2), red);
			RenderLine(transform.Peek(), aabb.GetBottomCorner(2), aabb.GetBottomCorner(3), red);
			RenderLine(transform.Peek(), aabb.GetBottomCorner(3), aabb.GetBottomCorner(0), red);
			RenderLine(transform.Peek(), aabb.GetTopCorner(0), aabb.GetTopCorner(1), red);
			RenderLine(transform.Peek(), aabb.GetTopCorner(1), aabb.GetTopCorner(2), red);
			RenderLine(transform.Peek(), aabb.GetTopCorner(2), aabb.GetTopCorner(3), red);
			RenderLine(transform.Peek(), aabb.GetTopCorner(3), aabb.GetTopCorner(0), red);
			RenderLine(transform.Peek(), new Vector3(aabb.minXYZ.x, aabb.minXYZ.y, aabb.minXYZ.z), new Vector3(aabb.minXYZ.x, aabb.minXYZ.y, aabb.maxXYZ.z), red);
			RenderLine(transform.Peek(), new Vector3(aabb.maxXYZ.x, aabb.minXYZ.y, aabb.minXYZ.z), new Vector3(aabb.maxXYZ.x, aabb.minXYZ.y, aabb.maxXYZ.z), red);
			RenderLine(transform.Peek(), new Vector3(aabb.minXYZ.x, aabb.maxXYZ.y, aabb.minXYZ.z), new Vector3(aabb.minXYZ.x, aabb.maxXYZ.y, aabb.maxXYZ.z), red);
			RenderLine(transform.Peek(), new Vector3(aabb.maxXYZ.x, aabb.maxXYZ.y, aabb.minXYZ.z), new Vector3(aabb.maxXYZ.x, aabb.maxXYZ.y, aabb.maxXYZ.z), red);
		}

		public void RenderRecursive(object objectToProcess, int level = 0)
		{
			throw new Exception("You must write the specialized function for this type.");
		}

		public void RenderRecursive(UnboundCollection objectToProcess, int level = 0)
		{
			RenderBounds(objectToProcess.GetAxisAlignedBoundingBox());
			foreach (IPrimitive item in objectToProcess.get_Items())
			{
				this.RenderRecursive((dynamic)item, level + 1);
			}
		}

		public void RenderRecursive(MeshFaceTraceable objectToProcess, int level = 0)
		{
			RenderBounds(objectToProcess.GetAxisAlignedBoundingBox());
		}

		public void RenderRecursive(Transform objectToProcess, int level = 0)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			RenderBounds(objectToProcess.GetAxisAlignedBoundingBox());
			transform.Push(((Axis3D)objectToProcess).get_Transform());
			this.RenderRecursive((dynamic)objectToProcess.get_Child(), level + 1);
		}
	}
}
