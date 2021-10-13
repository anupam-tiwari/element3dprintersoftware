namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public abstract class GCodeStreamProxy : GCodeStream
	{
		protected GCodeStream internalStream;

		public GCodeStreamProxy(GCodeStream internalStream)
		{
			this.internalStream = internalStream;
		}

		public override void Dispose()
		{
			internalStream.Dispose();
		}

		public override string ReadLine()
		{
			if (internalStream != null)
			{
				return internalStream.ReadLine();
			}
			return null;
		}

		public override void SetPrinterPosition(PrinterMove position)
		{
			internalStream.SetPrinterPosition(position);
		}
	}
}
