using MatterHackers.RayTracer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class MeshSelectInfo
	{
		public HitQuadrant HitQuadrant;

		public bool DownOnPart;

		public PlaneShape HitPlane;

		public Vector3 LastMoveDelta;

		public Vector3 PlaneDownHitPos;
	}
}
