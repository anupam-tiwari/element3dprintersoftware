using System.Collections.Generic;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.Slicing
{
	public class SliceLayer
	{
		public struct Segment
		{
			internal Vector2 start;

			internal Vector2 end;

			public Vector2 Start => start;

			public Vector2 End => end;

			internal Segment(Vector2 start, Vector2 end)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				this.start = start;
				this.end = end;
			}
		}

		private double zHeight;

		private List<Segment> unorderedSegments = new List<Segment>();

		private List<PathStorage> perimeters;

		public double ZHeight => zHeight;

		public List<Segment> UnorderedSegments => unorderedSegments;

		public List<PathStorage> Perimeters => perimeters;

		public SliceLayer(double zHeight)
		{
			this.zHeight = zHeight;
		}
	}
}
