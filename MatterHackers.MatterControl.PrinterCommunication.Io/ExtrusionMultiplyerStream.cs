using MatterHackers.MatterControl.GCodeVisualizer;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class ExtrusionMultiplyerStream : GCodeStreamProxy
	{
		private double currentActualExtrusionPosition;

		private double previousGcodeRequestedExtrusionPosition;

		public static double ExtrusionRatio
		{
			get;
			set;
		} = 1.0;


		public ExtrusionMultiplyerStream(GCodeStream internalStream)
			: base(internalStream)
		{
		}

		public override string ReadLine()
		{
			return ApplyExtrusionMultiplier(internalStream.ReadLine());
		}

		private string ApplyExtrusionMultiplier(string lineBeingSent)
		{
			if (lineBeingSent != null)
			{
				if (GCodeStream.LineIsMovement(lineBeingSent))
				{
					double readValue = 0.0;
					if (GCodeFile.GetFirstNumberAfter("E", lineBeingSent, ref readValue))
					{
						double num = readValue - previousGcodeRequestedExtrusionPosition;
						double numberToPutIn = currentActualExtrusionPosition + num * ExtrusionRatio;
						lineBeingSent = GCodeFile.ReplaceNumberAfter('E', lineBeingSent, numberToPutIn);
						previousGcodeRequestedExtrusionPosition = readValue;
						currentActualExtrusionPosition = numberToPutIn;
					}
				}
				else if (lineBeingSent.StartsWith("G92"))
				{
					double readValue2 = 0.0;
					if (GCodeFile.GetFirstNumberAfter("E", lineBeingSent, ref readValue2))
					{
						previousGcodeRequestedExtrusionPosition = readValue2;
						currentActualExtrusionPosition = readValue2;
					}
				}
			}
			return lineBeingSent;
		}
	}
}
