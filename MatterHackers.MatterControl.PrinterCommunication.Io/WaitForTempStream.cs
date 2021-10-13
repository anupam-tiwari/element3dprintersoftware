using System.Diagnostics;
using System.Threading;
using MatterHackers.MatterControl.GCodeVisualizer;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class WaitForTempStream : GCodeStreamProxy
	{
		private enum State
		{
			passthrough,
			waitingForExtruderTemp,
			waitingForBedTemp
		}

		private double extruderIndex;

		private double ignoreRequestIfBelowTemp = 20.0;

		private double sameTempRange = 1.0;

		private State state;

		private double targetTemp;

		private Stopwatch timeHaveBeenAtTemp = new Stopwatch();

		private double waitAfterReachTempTime = 3.0;

		private bool waitWhenCooling;

		public bool HeatingBed => state == State.waitingForBedTemp;

		public bool HeatingExtruder => state == State.waitingForExtruderTemp;

		public WaitForTempStream(GCodeStream internalStream)
			: base(internalStream)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			state = State.passthrough;
		}

		public void Cancel()
		{
			state = State.passthrough;
		}

		public override string ReadLine()
		{
			switch (state)
			{
			case State.passthrough:
			{
				string text = base.ReadLine();
				if (text != null && text.StartsWith("M"))
				{
					if (text.StartsWith("M109"))
					{
						if (text.Contains("F") || !text.Contains("S"))
						{
							return text;
						}
						waitWhenCooling = false;
						text = "M104" + text.Substring(4);
						GCodeFile.GetFirstNumberAfter("S", text, ref targetTemp);
						extruderIndex = 0.0;
						GCodeFile.GetFirstNumberAfter("T", text, ref extruderIndex);
						if (!(targetTemp > ignoreRequestIfBelowTemp))
						{
							Thread.Sleep(100);
							return "";
						}
						state = State.waitingForExtruderTemp;
						timeHaveBeenAtTemp.Reset();
					}
					else if (text.StartsWith("M190"))
					{
						bool firstNumberAfter = GCodeFile.GetFirstNumberAfter("R", text, ref targetTemp);
						bool firstNumberAfter2 = GCodeFile.GetFirstNumberAfter("S", text, ref targetTemp);
						if (!(firstNumberAfter || firstNumberAfter2))
						{
							Thread.Sleep(100);
							return "";
						}
						if (!(targetTemp > ignoreRequestIfBelowTemp))
						{
							Thread.Sleep(100);
							return "";
						}
						waitWhenCooling = firstNumberAfter;
						text = "M140 S" + targetTemp;
						state = State.waitingForBedTemp;
						timeHaveBeenAtTemp.Reset();
					}
				}
				return text;
			}
			case State.waitingForExtruderTemp:
			{
				double actualExtruderTemperature = PrinterConnectionAndCommunication.Instance.GetActualExtruderTemperature((int)extruderIndex);
				if (actualExtruderTemperature >= targetTemp - sameTempRange && actualExtruderTemperature <= targetTemp + sameTempRange && !timeHaveBeenAtTemp.get_IsRunning())
				{
					timeHaveBeenAtTemp.Start();
				}
				if (timeHaveBeenAtTemp.get_Elapsed().TotalSeconds > waitAfterReachTempTime || PrinterConnectionAndCommunication.Instance.PrintWasCanceled)
				{
					state = State.passthrough;
					return "";
				}
				Thread.Sleep(100);
				return "";
			}
			case State.waitingForBedTemp:
			{
				double actualBedTemperature = PrinterConnectionAndCommunication.Instance.ActualBedTemperature;
				bool flag = ((!waitWhenCooling) ? (actualBedTemperature >= targetTemp - sameTempRange) : (actualBedTemperature >= targetTemp - sameTempRange && actualBedTemperature <= targetTemp + sameTempRange));
				if (flag && !timeHaveBeenAtTemp.get_IsRunning())
				{
					timeHaveBeenAtTemp.Start();
				}
				if (timeHaveBeenAtTemp.get_Elapsed().TotalSeconds > waitAfterReachTempTime || PrinterConnectionAndCommunication.Instance.PrintWasCanceled)
				{
					state = State.passthrough;
					return "";
				}
				Thread.Sleep(100);
				return "";
			}
			default:
				return null;
			}
		}
	}
}
