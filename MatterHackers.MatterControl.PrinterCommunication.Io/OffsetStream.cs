using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class OffsetStream : GCodeStreamProxy
	{
		private Vector3 offset;

		protected PrinterMove lastDestination;

		public PrinterMove LastDestination => lastDestination;

		public Vector3 Offset
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return offset;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				offset = value;
			}
		}

		public OffsetStream(GCodeStream internalStream, Vector3 offset)
			: base(internalStream)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			this.offset = offset;
		}

		public override void SetPrinterPosition(PrinterMove position)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			lastDestination = position;
			ref Vector3 position2 = ref lastDestination.position;
			position2 -= offset;
			internalStream.SetPrinterPosition(lastDestination);
		}

		public override string ReadLine()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			string text = base.ReadLine();
			if (text != null && GCodeStream.LineIsMovement(text))
			{
				PrinterMove position = GCodeStream.GetPosition(text, lastDestination);
				PrinterMove destination = position;
				ref Vector3 position2 = ref destination.position;
				position2 += offset;
				text = CreateMovementLine(destination, lastDestination);
				lastDestination = position;
				return text;
			}
			return text;
		}
	}
}
