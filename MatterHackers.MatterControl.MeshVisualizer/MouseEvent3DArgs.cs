using System;
using MatterHackers.Agg.UI;
using MatterHackers.RayTracer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.MeshVisualizer
{
	public class MouseEvent3DArgs : EventArgs
	{
		public IntersectInfo info;

		public MouseEventArgs MouseEvent2D;

		private Ray mouseRay;

		public Ray MouseRay => mouseRay;

		public MouseEvent3DArgs(MouseEventArgs mouseEvent2D, Ray mouseRay, IntersectInfo info)
		{
			this.info = info;
			MouseEvent2D = mouseEvent2D;
			this.mouseRay = mouseRay;
		}
	}
}
