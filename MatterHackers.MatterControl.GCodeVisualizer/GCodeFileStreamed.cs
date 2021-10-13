using System;
using System.IO;
using System.Text;
using MatterHackers.Agg;
using MatterHackers.MatterSlice;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class GCodeFileStreamed : GCodeFile
	{
		private StreamReader openGcodeStream;

		private object locker = new object();

		private bool readLastLineOfFile;

		private int readLineCount;

		private const int MaxLinesToBuffer = 128;

		private PrinterMachineInstruction[] readLinesRingBuffer = new PrinterMachineInstruction[128];

		private Tools printTools;

		private double feedRateMmPerMin;

		private Vector3 lastPrinterPosition;

		private double lastEPosition;

		public override Tools PrintTools => printTools;

		public override int LineCount
		{
			get
			{
				if (openGcodeStream != null && !readLastLineOfFile)
				{
					return Math.Max(readLineCount + 1, (int)(openGcodeStream.BaseStream.Length / 14));
				}
				return readLineCount;
			}
		}

		public long ByteCount
		{
			get
			{
				if (openGcodeStream != null && !readLastLineOfFile)
				{
					return openGcodeStream.BaseStream.Length;
				}
				return 0L;
			}
		}

		public long BytePosition
		{
			get
			{
				if (openGcodeStream != null && !readLastLineOfFile)
				{
					return openGcodeStream.BaseStream.Position;
				}
				return 0L;
			}
		}

		public override double TotalSecondsInPrint => -1.0;

		public override int NumChangesInZ => 0;

		public GCodeFileStreamed(string fileName)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			openGcodeStream = new StreamReader(stream);
			Tools val = new Tools(new ConfigSettings());
			int num = 32000;
			using (Stream stream2 = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				byte[] array = new byte[num];
				stream2.Seek(Math.Max(0L, stream2.Length - num), SeekOrigin.Begin);
				stream2.Read(array, 0, num);
				stream2.Close();
				string @string = Encoding.UTF8.GetString(array);
				if (@string.Contains("NewTools"))
				{
					int startIndex = @string.IndexOf("NewTools", StringComparison.InvariantCulture);
					int num2 = @string.IndexOf('=', startIndex) + 1;
					int num3 = @string.IndexOf('\n', startIndex);
					string text = @string.Substring(num2, num3 - num2);
					val.ClearAllTools();
					val.AddNewTools(text.Trim());
				}
			}
			printTools = val;
		}

		~GCodeFileStreamed()
		{
			CloseStream();
		}

		private void CloseStream()
		{
			if (openGcodeStream != null)
			{
				openGcodeStream.Close();
				openGcodeStream = null;
			}
		}

		public override void Clear()
		{
			CloseStream();
			readLastLineOfFile = false;
			readLineCount = 0;
		}

		public override Vector2 GetWeightedCenter()
		{
			throw new NotImplementedException("A streamed GCode file should not need to do this. Please validate the code that is calling this.");
		}

		public override RectangleDouble GetBounds()
		{
			throw new NotImplementedException("A streamed GCode file should not need to do this. Please validate the code that is calling this.");
		}

		public override double GetFilamentCubicMm(double filamentDiameter)
		{
			throw new NotImplementedException("A streamed GCode file should not need to do this. Please validate the code that is calling this.");
		}

		public override bool IsExtruding(int instructionIndexToCheck)
		{
			throw new NotImplementedException();
		}

		public override double GetLayerHeight()
		{
			throw new NotImplementedException();
		}

		public override double GetFirstLayerHeight()
		{
			throw new NotImplementedException();
		}

		public override double GetFilamentUsedMm(double filamentDiameter)
		{
			throw new NotImplementedException();
		}

		public override double PercentComplete(int instructionIndex)
		{
			lock (locker)
			{
				if (openGcodeStream != null && openGcodeStream.BaseStream.Length > 0)
				{
					return (double)openGcodeStream.BaseStream.Position / (double)openGcodeStream.BaseStream.Length * 100.0;
				}
			}
			return 100.0;
		}

		public override int GetInstructionIndexAtLayer(int layerIndex)
		{
			return 0;
		}

		public override double GetFilamentDiameter()
		{
			return 0.0;
		}

		public override double GetFilamentWeightGrams(double filamentDiameterMm, double density)
		{
			return 0.0;
		}

		public override int GetLayerIndex(int instructionIndex)
		{
			return 0;
		}

		public override PrinterMachineInstruction Instruction(int index)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			lock (locker)
			{
				if (index < readLineCount - 128)
				{
					throw new Exception("You are asking for a line we no longer have bufferd");
				}
				while (index >= readLineCount)
				{
					string text = openGcodeStream.ReadLine();
					if (text == null)
					{
						readLastLineOfFile = true;
						text = "";
					}
					int num = readLineCount % 128;
					readLinesRingBuffer[num] = new PrinterMachineInstruction(text);
					PrinterMachineInstruction printerMachineInstruction = readLinesRingBuffer[num];
					Vector3 deltaPositionThisLine = default(Vector3);
					double deltaEPositionThisLine = 0.0;
					string text2 = text.ToUpper().Trim();
					if (text2.StartsWith("G0") || text2.StartsWith("G1"))
					{
						double readValue = 0.0;
						if (GCodeFile.GetFirstNumberAfter("F", text2, ref readValue))
						{
							feedRateMmPerMin = readValue;
						}
						Vector3 val = lastPrinterPosition;
						GCodeFile.GetFirstNumberAfter("X", text2, ref val.x);
						GCodeFile.GetFirstNumberAfter("Y", text2, ref val.y);
						GCodeFile.GetFirstNumberAfter("Z", text2, ref val.z);
						double readValue2 = lastEPosition;
						GCodeFile.GetFirstNumberAfter("E", text2, ref readValue2);
						deltaPositionThisLine = val - lastPrinterPosition;
						deltaEPositionThisLine = Math.Abs(readValue2 - lastEPosition);
						lastPrinterPosition = val;
						lastEPosition = readValue2;
					}
					else if (text2.StartsWith("G92"))
					{
						double readValue3 = 0.0;
						if (GCodeFile.GetFirstNumberAfter("E", text2, ref readValue3))
						{
							lastEPosition = readValue3;
						}
					}
					if (feedRateMmPerMin > 0.0)
					{
						printerMachineInstruction.secondsThisLine = (float)GCodeFile.GetSecondsThisLine(deltaPositionThisLine, deltaEPositionThisLine, feedRateMmPerMin);
					}
					readLineCount++;
				}
			}
			return readLinesRingBuffer[index % 128];
		}

		public override double Ratio0to1IntoContainedLayer(int instructionIndex)
		{
			if (ByteCount != 0L)
			{
				return (double)BytePosition / (double)ByteCount;
			}
			return 1.0;
		}
	}
}
