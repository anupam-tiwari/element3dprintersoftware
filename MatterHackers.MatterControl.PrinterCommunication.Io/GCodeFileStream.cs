using MatterHackers.MatterControl.GCodeVisualizer;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class GCodeFileStream : GCodeStream
	{
		private int printerCommandQueueLineIndex = -1;

		public GCodeFile FileStreaming
		{
			get;
			private set;
		}

		public int LineIndex => printerCommandQueueLineIndex;

		public GCodeFileStream(GCodeFile fileStreaming, int startLine = 0)
		{
			FileStreaming = fileStreaming;
			printerCommandQueueLineIndex = startLine;
		}

		public override void Dispose()
		{
		}

		public override string ReadLine()
		{
			if (printerCommandQueueLineIndex < FileStreaming.LineCount)
			{
				return FileStreaming.Instruction(printerCommandQueueLineIndex++).Line;
			}
			return null;
		}

		public override void SetPrinterPosition(PrinterMove position)
		{
		}
	}
}
