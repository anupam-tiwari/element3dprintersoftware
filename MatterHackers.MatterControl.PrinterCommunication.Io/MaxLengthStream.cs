using System;
using System.Collections.Generic;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class MaxLengthStream : GCodeStreamProxy
	{
		protected PrinterMove lastDestination;

		private List<PrinterMove> movesToSend = new List<PrinterMove>();

		private double maxSecondsPerSegment = 0.05;

		public PrinterMove LastDestination => lastDestination;

		public double MaxSegmentLength
		{
			get;
			set;
		}

		public MaxLengthStream(GCodeStream internalStream, double maxSegmentLength)
			: base(internalStream)
		{
			MaxSegmentLength = maxSegmentLength;
		}

		public override string ReadLine()
		{
			if (movesToSend.Count == 0)
			{
				string text = base.ReadLine();
				if (text != null && GCodeStream.LineIsMovement(text))
				{
					PrinterMove position = GCodeStream.GetPosition(text, lastDestination);
					PrinterMove left = position - lastDestination;
					left.feedRate = 0.0;
					double num = Math.Max(left.LengthSquared, left.extrusion * left.extrusion);
					if (num > MaxSegmentLength * MaxSegmentLength)
					{
						double num2 = Math.Sqrt(num);
						int val = (int)Math.Ceiling(num2 / MaxSegmentLength);
						double num3 = 1.0 / (position.feedRate / 60.0 * maxSecondsPerSegment / num2);
						int num4 = Math.Max(1, Math.Min(val, (int)num3));
						if (num4 > 1)
						{
							PrinterMove printerMove = left / num4;
							PrinterMove item = lastDestination + printerMove;
							item.feedRate = position.feedRate;
							for (int i = 0; i < num4; i++)
							{
								lock (movesToSend)
								{
									movesToSend.Add(item);
								}
								item += printerMove;
							}
							PrinterMove destination = movesToSend[0];
							lock (movesToSend)
							{
								movesToSend.RemoveAt(0);
							}
							string result = CreateMovementLine(destination, lastDestination);
							lastDestination = destination;
							return result;
						}
					}
					lastDestination = position;
				}
				return text;
			}
			PrinterMove destination2 = movesToSend[0];
			lock (movesToSend)
			{
				movesToSend.RemoveAt(0);
			}
			string result2 = CreateMovementLine(destination2, lastDestination);
			lastDestination = destination2;
			return result2;
		}

		public void Cancel()
		{
			lock (movesToSend)
			{
				movesToSend.Clear();
			}
		}

		public override void SetPrinterPosition(PrinterMove position)
		{
			lastDestination = position;
			internalStream.SetPrinterPosition(lastDestination);
		}
	}
}
