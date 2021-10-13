namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class FeedRateMultiplyerStream : GCodeStreamProxy
	{
		public PrinterMove LastDestination
		{
			get;
			private set;
		}

		public static double FeedRateRatio
		{
			get;
			set;
		} = 1.0;


		public FeedRateMultiplyerStream(GCodeStream internalStream)
			: base(internalStream)
		{
		}

		public override void SetPrinterPosition(PrinterMove position)
		{
			LastDestination = position;
			internalStream.SetPrinterPosition(LastDestination);
		}

		public override string ReadLine()
		{
			string text = internalStream.ReadLine();
			if (text != null && GCodeStream.LineIsMovement(text))
			{
				PrinterMove position = GCodeStream.GetPosition(text, LastDestination);
				PrinterMove destination = position;
				destination.feedRate *= FeedRateRatio;
				text = CreateMovementLine(destination, LastDestination);
				LastDestination = position;
				return text;
			}
			return text;
		}
	}
}
