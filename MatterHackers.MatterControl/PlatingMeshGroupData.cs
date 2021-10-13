using System.Collections.Generic;
using MatterHackers.RayTracer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class PlatingMeshGroupData
	{
		public Vector2 spacing;

		public List<IPrimitive> meshTraceableData = new List<IPrimitive>();
	}
}
