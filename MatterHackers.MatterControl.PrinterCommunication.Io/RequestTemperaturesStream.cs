using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class RequestTemperaturesStream : GCodeStreamProxy
	{
		private long nextReadTimeMs;

		public RequestTemperaturesStream(GCodeStream internalStream)
			: base(internalStream)
		{
			nextReadTimeMs = UiThread.get_CurrentTimerMs() + 1000;
		}

		public override string ReadLine()
		{
			if (!PrinterConnectionAndCommunication.Instance.WatingForPositionRead && nextReadTimeMs < UiThread.get_CurrentTimerMs() && PrinterConnectionAndCommunication.Instance.PrinterIsConnected)
			{
				nextReadTimeMs = UiThread.get_CurrentTimerMs() + 1000;
				return "M105";
			}
			return base.ReadLine();
		}
	}
}
