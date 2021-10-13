using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public struct PrinterMove
	{
		public static readonly PrinterMove Nowhere = new PrinterMove
		{
			position = new Vector3(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity),
			extrusion = double.PositiveInfinity,
			feedRate = double.PositiveInfinity
		};

		public static readonly PrinterMove Zero;

		public double extrusion;

		public double feedRate;

		public Vector3 position;

		public double LengthSquared => ((Vector3)(ref position)).get_LengthSquared();

		public PrinterMove(Vector3 absoluteDestination, double currentExtruderDestination, double currentFeedRate)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			this = default(PrinterMove);
			position = absoluteDestination;
			extrusion = currentExtruderDestination;
			feedRate = currentFeedRate;
		}

		public static PrinterMove operator -(PrinterMove left, PrinterMove right)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			ref Vector3 reference = ref left.position;
			reference -= right.position;
			left.extrusion -= right.extrusion;
			left.feedRate -= right.feedRate;
			return left;
		}

		public static PrinterMove operator /(PrinterMove left, double scale)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			ref Vector3 reference = ref left.position;
			reference /= scale;
			left.extrusion /= scale;
			left.feedRate /= scale;
			return left;
		}

		public static PrinterMove operator +(PrinterMove left, PrinterMove right)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			ref Vector3 reference = ref left.position;
			reference += right.position;
			left.extrusion += right.extrusion;
			left.feedRate += right.feedRate;
			return left;
		}
	}
}
