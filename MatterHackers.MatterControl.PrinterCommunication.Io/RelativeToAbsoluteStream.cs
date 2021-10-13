namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class RelativeToAbsoluteStream : GCodeStreamProxy
	{
		protected PrinterMove lastDestination;

		private bool xyzAbsoluteMode = true;

		private bool eAbsoluteMode = true;

		public PrinterMove LastDestination => lastDestination;

		public RelativeToAbsoluteStream(GCodeStream internalStream)
			: base(internalStream)
		{
		}

		public override void SetPrinterPosition(PrinterMove position)
		{
			lastDestination = position;
			internalStream.SetPrinterPosition(lastDestination);
		}

		public string ProcessLine(string lineToProcess)
		{
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			if (lineToProcess != null && lineToProcess.StartsWith("G9"))
			{
				if (lineToProcess.StartsWith("G91"))
				{
					xyzAbsoluteMode = false;
					eAbsoluteMode = false;
					return "";
				}
				if (lineToProcess.StartsWith("G90"))
				{
					xyzAbsoluteMode = true;
					eAbsoluteMode = true;
				}
				if (lineToProcess.StartsWith("M83"))
				{
					eAbsoluteMode = false;
				}
				else if (lineToProcess.StartsWith("82"))
				{
					eAbsoluteMode = true;
				}
			}
			if (lineToProcess != null && GCodeStream.LineIsMovement(lineToProcess))
			{
				PrinterMove position = default(PrinterMove);
				if (xyzAbsoluteMode && eAbsoluteMode)
				{
					position = GCodeStream.GetPosition(lineToProcess, lastDestination);
				}
				else
				{
					PrinterMove position2 = GCodeStream.GetPosition(lineToProcess, lastDestination);
					double feedRate = position2.feedRate;
					if (!xyzAbsoluteMode)
					{
						position2 = GCodeStream.GetPosition(lineToProcess, PrinterMove.Zero);
						position2 += lastDestination;
					}
					PrinterMove position3 = GCodeStream.GetPosition(lineToProcess, lastDestination);
					if (!eAbsoluteMode)
					{
						position3 = GCodeStream.GetPosition(lineToProcess, PrinterMove.Zero);
						position3 += lastDestination;
					}
					position.extrusion = position3.extrusion;
					position.feedRate = feedRate;
					position.position = position2.position;
					lineToProcess = CreateMovementLine(position, lastDestination);
				}
				lastDestination = position;
			}
			return lineToProcess;
		}

		public override string ReadLine()
		{
			string lineToProcess = base.ReadLine();
			return ProcessLine(lineToProcess);
		}
	}
}
