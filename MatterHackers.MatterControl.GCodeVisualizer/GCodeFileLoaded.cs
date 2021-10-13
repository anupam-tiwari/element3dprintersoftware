using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.MatterSlice;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class GCodeFileLoaded : GCodeFile
	{
		private double amountOfAccumulatedEWhileParsing;

		private List<int> indexOfChangeInZ = new List<int>();

		private List<int> movesInLayer = new List<int>();

		private Vector2 center = Vector2.Zero;

		private bool gcodeHasExplicitLayerChangeInfo;

		private double firstLayerThickness;

		private double layerThickness;

		private double filamentUsedMmCache;

		private double diameterOfFilamentUsedMmCache;

		private List<PrinterMachineInstruction> GCodeCommandQueue = new List<PrinterMachineInstruction>();

		private Tools printTools;

		private double filamentDiameterCache;

		public override Tools PrintTools => printTools;

		public override int LineCount => GCodeCommandQueue.Count;

		public override double TotalSecondsInPrint => Instruction(0).secondsToEndFromHere;

		public Vector2 Center => center;

		private List<int> IndexOfChangeInZ => indexOfChangeInZ;

		public override int NumChangesInZ => indexOfChangeInZ.Count;

		public GCodeFileLoaded(bool gcodeHasExplicitLayerChangeInfo = false)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			this.gcodeHasExplicitLayerChangeInfo = gcodeHasExplicitLayerChangeInfo;
		}

		public GCodeFileLoaded(string pathAndFileName, bool gcodeHasExplicitLayerChangeInfo = false)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			this.gcodeHasExplicitLayerChangeInfo = gcodeHasExplicitLayerChangeInfo;
			Load(pathAndFileName);
		}

		public override PrinterMachineInstruction Instruction(int index)
		{
			return GCodeCommandQueue[index];
		}

		public override void Clear()
		{
			indexOfChangeInZ.Clear();
			GCodeCommandQueue.Clear();
		}

		public void Add(PrinterMachineInstruction printerMachineInstruction)
		{
			Insert(LineCount, printerMachineInstruction);
		}

		public void Insert(int insertIndex, PrinterMachineInstruction printerMachineInstruction)
		{
			for (int i = 0; i < indexOfChangeInZ.Count; i++)
			{
				if (insertIndex < indexOfChangeInZ[i])
				{
					indexOfChangeInZ[i]++;
				}
			}
			GCodeCommandQueue.Insert(insertIndex, printerMachineInstruction);
		}

		public static GCodeFile ParseGCodeString(string gcodeContents)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			DoWorkEventArgs val = new DoWorkEventArgs((object)gcodeContents);
			ParseFileContents(null, val);
			return (GCodeFile)val.get_Result();
		}

		public static GCodeFileLoaded Load(Stream fileStream)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			GCodeFileLoaded result = null;
			try
			{
				string text = "";
				using (StreamReader streamReader = new StreamReader(fileStream))
				{
					text = streamReader.ReadToEnd();
				}
				DoWorkEventArgs val = new DoWorkEventArgs((object)text);
				ParseFileContents(null, val);
				result = (GCodeFileLoaded)val.get_Result();
				return result;
			}
			catch (IOException)
			{
				return result;
			}
		}

		public static void LoadInBackground(BackgroundWorker backgroundWorker, string fileName)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			if (Path.GetExtension(fileName)!.ToUpper() == ".GCODE")
			{
				try
				{
					if (File.Exists(fileName))
					{
						if (GCodeFile.FileTooBigToLoad(fileName))
						{
							backgroundWorker.RunWorkerAsync((object)null);
							return;
						}
						backgroundWorker.add_DoWork(new DoWorkEventHandler(ParseFileContents));
						backgroundWorker.RunWorkerAsync((object)File.ReadAllText(fileName));
					}
					else
					{
						backgroundWorker.RunWorkerAsync((object)null);
					}
				}
				catch (IOException)
				{
				}
			}
			else
			{
				backgroundWorker.RunWorkerAsync((object)null);
			}
		}

		public new void Load(string gcodePathAndFileName)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if (GCodeFile.FileTooBigToLoad(gcodePathAndFileName))
			{
				return;
			}
			using FileStream stream = new FileStream(gcodePathAndFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using StreamReader streamReader = new StreamReader(stream);
			GCodeFileLoaded gCodeFileLoaded = Load(streamReader.BaseStream);
			indexOfChangeInZ = gCodeFileLoaded.indexOfChangeInZ;
			movesInLayer = gCodeFileLoaded.movesInLayer;
			center = gCodeFileLoaded.center;
			GCodeCommandQueue = gCodeFileLoaded.GCodeCommandQueue;
			printTools = gCodeFileLoaded.PrintTools;
		}

		private static IEnumerable<string> CustomSplit(string newtext, char splitChar)
		{
			int num = 0;
			for (int positionOfSplitChar = newtext.IndexOf(splitChar); positionOfSplitChar != -1; positionOfSplitChar = newtext.IndexOf(splitChar, num))
			{
				yield return newtext.Substring(num, positionOfSplitChar - num).Trim();
				num = positionOfSplitChar + 1;
			}
			yield return newtext.Substring(num);
		}

		private static int CountNumLines(string gCodeString)
		{
			int num = 0;
			for (int i = 0; i < gCodeString.Length; i++)
			{
				if (gCodeString[i] == '\n')
				{
					num++;
				}
			}
			return num + 1;
		}

		public static void ParseFileContents(object sender, DoWorkEventArgs doWorkEventArgs)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			string text = (string)doWorkEventArgs.get_Argument();
			if (text == null)
			{
				return;
			}
			Stopwatch val = Stopwatch.StartNew();
			BackgroundWorker val2 = sender as BackgroundWorker;
			Stopwatch val3 = new Stopwatch();
			val3.Start();
			PrinterMachineInstruction printerMachineInstruction = new PrinterMachineInstruction("None");
			bool flag = false;
			if (Regex.IsMatch(text, "\n[^;\n]*;[^;\n]*LAYER:"))
			{
				flag = true;
			}
			Tools val4 = new Tools(new ConfigSettings());
			val4.AlwaysValid = true;
			if (text.Contains("NewTools"))
			{
				int startIndex = text.IndexOf("NewTools", StringComparison.InvariantCulture);
				int num = text.IndexOf('=', startIndex) + 1;
				int num2 = text.IndexOf('\n', startIndex);
				string text2 = text.Substring(num, num2 - num);
				val4.ClearAllTools();
				val4.AddNewTools(text2.Trim());
			}
			GCodeFileLoaded gCodeFileLoaded = new GCodeFileLoaded(flag);
			gCodeFileLoaded.printTools = val4;
			int num3 = CountNumLines(text);
			int num4 = 0;
			int num5 = 0;
			foreach (string item in CustomSplit(text, '\n'))
			{
				string text3 = item.Trim();
				printerMachineInstruction = new PrinterMachineInstruction(text3, printerMachineInstruction);
				if (text3.Length > 0)
				{
					switch (text3[0])
					{
					case 'G':
						gCodeFileLoaded.ParseGLine(text3, printerMachineInstruction);
						break;
					case 'M':
						gCodeFileLoaded.ParseMLine(text3, printerMachineInstruction);
						break;
					case 'T':
					{
						double readValue = 0.0;
						if (GCodeFile.GetFirstNumberAfter("T", text3, ref readValue))
						{
							printerMachineInstruction.ExtruderIndex = (int)readValue;
						}
						break;
					}
					case ';':
						if (flag && GCodeFile.IsLayerChange(text3))
						{
							gCodeFileLoaded.movesInLayer.Add(num5);
							num5 = 0;
							gCodeFileLoaded.IndexOfChangeInZ.Add(gCodeFileLoaded.GCodeCommandQueue.Count);
						}
						if (text3.StartsWith("; layerThickness"))
						{
							gCodeFileLoaded.layerThickness = double.Parse(text3.Split(new char[1]
							{
								'='
							})[1]);
						}
						else if (text3.StartsWith("; firstLayerThickness") && gCodeFileLoaded.firstLayerThickness == 0.0)
						{
							gCodeFileLoaded.firstLayerThickness = double.Parse(text3.Split(new char[1]
							{
								'='
							})[1]);
						}
						break;
					}
				}
				gCodeFileLoaded.GCodeCommandQueue.Add(printerMachineInstruction);
				if (gCodeFileLoaded.InstructionIndexIsFeature(gCodeFileLoaded.GCodeCommandQueue.Count - 1))
				{
					num5++;
				}
				if (val2 != null)
				{
					if (val2.get_CancellationPending())
					{
						return;
					}
					if (val2.get_WorkerReportsProgress() && val3.get_ElapsedMilliseconds() > 200)
					{
						val2.ReportProgress(num4 * 100 / num3 / 2);
						val3.Restart();
					}
				}
				num4++;
			}
			gCodeFileLoaded.movesInLayer.Add(num5);
			gCodeFileLoaded.AnalyzeGCodeLines(val2);
			doWorkEventArgs.set_Result((object)gCodeFileLoaded);
			val.Stop();
			Console.WriteLine(StringHelper.FormatWith("Time To Load Seconds: {0:0.00}", new object[1]
			{
				val.get_Elapsed().TotalSeconds
			}));
		}

		private void AnalyzeGCodeLines(BackgroundWorker backgroundWorker = null)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			double num = 0.0;
			Vector3 val = default(Vector3);
			double num2 = 0.0;
			Stopwatch val2 = new Stopwatch();
			val2.Start();
			for (int i = 0; i < GCodeCommandQueue.Count; i++)
			{
				PrinterMachineInstruction printerMachineInstruction = GCodeCommandQueue[i];
				string line = printerMachineInstruction.Line;
				Vector3 deltaPositionThisLine = default(Vector3);
				double deltaEPositionThisLine = 0.0;
				string text = line.ToUpper().Trim();
				if (text.StartsWith("G0") || text.StartsWith("G1"))
				{
					double readValue = 0.0;
					if (GCodeFile.GetFirstNumberAfter("F", text, ref readValue))
					{
						num = readValue;
					}
					Vector3 val3 = val;
					GCodeFile.GetFirstNumberAfter("X", text, ref val3.x);
					GCodeFile.GetFirstNumberAfter("Y", text, ref val3.y);
					GCodeFile.GetFirstNumberAfter("Z", text, ref val3.z);
					double readValue2 = num2;
					GCodeFile.GetFirstNumberAfter("E", text, ref readValue2);
					deltaPositionThisLine = val3 - val;
					deltaEPositionThisLine = Math.Abs(readValue2 - num2);
					val = val3;
					num2 = readValue2;
				}
				else if (text.StartsWith("G92"))
				{
					double readValue3 = 0.0;
					if (GCodeFile.GetFirstNumberAfter("E", text, ref readValue3))
					{
						num2 = readValue3;
					}
				}
				if (num > 0.0)
				{
					printerMachineInstruction.secondsThisLine = (float)GCodeFile.GetSecondsThisLine(deltaPositionThisLine, deltaEPositionThisLine, num);
				}
				if (backgroundWorker != null)
				{
					if (backgroundWorker.get_CancellationPending())
					{
						return;
					}
					if (backgroundWorker.get_WorkerReportsProgress() && val2.get_ElapsedMilliseconds() > 200)
					{
						backgroundWorker.ReportProgress(i * 100 / GCodeCommandQueue.Count / 2 + 50);
						val2.Restart();
					}
				}
			}
			double num3 = 0.0;
			for (int num4 = GCodeCommandQueue.Count - 1; num4 >= 0; num4--)
			{
				PrinterMachineInstruction printerMachineInstruction2 = GCodeCommandQueue[num4];
				num3 += (double)printerMachineInstruction2.secondsThisLine;
				printerMachineInstruction2.secondsToEndFromHere = (float)num3;
			}
		}

		public override double PercentComplete(int instructionIndex)
		{
			if (GCodeCommandQueue.Count > 0)
			{
				return Math.Min(99.9, (double)instructionIndex / (double)GCodeCommandQueue.Count * 100.0);
			}
			return 100.0;
		}

		public override int GetInstructionIndexAtLayer(int layerIndex)
		{
			return IndexOfChangeInZ[layerIndex];
		}

		private void ParseMLine(string lineString, PrinterMachineInstruction processingMachineState)
		{
			int num = lineString.IndexOf(';');
			if (num != -1)
			{
				lineString = lineString.Substring(0, num);
			}
			string text = lineString.Split(new char[1]
			{
				' '
			})[0].Substring(1).Trim();
			switch (_003CPrivateImplementationDetails_003E.ComputeStringHash(text))
			{
			case 552431802u:
				_ = text == "01";
				break;
			case 856466825u:
				_ = text == "6";
				break;
			case 1748227631u:
				_ = text == "101";
				break;
			case 334175660u:
				_ = text == "18";
				break;
			case 2279835011u:
				_ = text == "42";
				break;
			case 2380647820u:
				_ = text == "72";
				break;
			case 2397425439u:
				_ = text == "73";
				break;
			case 2416027439u:
				_ = text == "82";
				break;
			case 2399249820u:
				_ = text == "83";
				break;
			case 2382472201u:
				_ = text == "84";
				break;
			case 234789874u:
				_ = text == "92";
				break;
			case 1765005250u:
				_ = text == "102";
				break;
			case 1781782869u:
				_ = text == "103";
				break;
			case 1664339536u:
				_ = text == "104";
				break;
			case 1681117155u:
				_ = text == "105";
				break;
			case 1697894774u:
				_ = text == "106";
				break;
			case 1714672393u:
				_ = text == "107";
				break;
			case 1865670964u:
				_ = text == "108";
				break;
			case 1882448583u:
				_ = text == "109";
				break;
			case 1765152345u:
				_ = text == "114";
				break;
			case 1714819488u:
				_ = text == "117";
				break;
			case 1966630868u:
				_ = text == "126";
				break;
			case 1983408487u:
				_ = text == "127";
				break;
			case 2067443677u:
				_ = text == "132";
				break;
			case 2050666058u:
				_ = text == "133";
				break;
			case 1966777963u:
				_ = text == "134";
				break;
			case 1950000344u:
				_ = text == "135";
				break;
			case 1864979416u:
				_ = text == "140";
				break;
			case 4246856861u:
				_ = text == "190";
				break;
			case 3286718301u:
				_ = text == "200";
				break;
			case 3269940682u:
				_ = text == "201";
				break;
			case 3219607825u:
				_ = text == "204";
				break;
			case 3169274968u:
				_ = text == "207";
				break;
			case 3152497349u:
				_ = text == "208";
				break;
			case 3135719730u:
				_ = text == "209";
				break;
			case 3185905492u:
				_ = text == "210";
				break;
			case 770472809u:
				_ = text == "226";
				break;
			case 753695190u:
				_ = text == "227";
				break;
			case 1179644613u:
				_ = text == "301";
				break;
			case 2855274715u:
				_ = text == "400";
				break;
			case 883407717u:
				_ = text == "565";
				break;
			case 4291191994u:
				_ = text == "1200";
				break;
			case 13002317u:
				_ = text == "1201";
				break;
			case 4257636756u:
				_ = text == "1202";
				break;
			}
		}

		private void ParseGLine(string lineString, PrinterMachineInstruction processingMachineState)
		{
			int num = lineString.IndexOf(';');
			if (num != -1)
			{
				lineString = lineString.Substring(0, num);
			}
			string text = lineString.Split(new char[1]
			{
				' '
			})[0].Substring(1).Trim();
			double readValue2;
			double readValue3;
			double readValue4;
			double readValue5;
			double readValue6;
			switch (_003CPrivateImplementationDetails_003E.ComputeStringHash(text))
			{
			case 890022063u:
				if (!(text == "0"))
				{
					break;
				}
				goto IL_020b;
			case 822911587u:
				_ = text == "4";
				break;
			case 502098945u:
				_ = text == "04";
				break;
			case 873244444u:
				if (!(text == "1"))
				{
					break;
				}
				goto IL_020b;
			case 468396612u:
				_ = text == "10";
				break;
			case 485174231u:
				_ = text == "11";
				break;
			case 2364708844u:
				_ = text == "21";
				break;
			case 2515707415u:
				_ = text == "28";
				break;
			case 2498929796u:
				_ = text == "29";
				break;
			case 2280673654u:
				_ = text == "30";
				break;
			case 201234636u:
				if (text == "90")
				{
					processingMachineState.movementType = PrinterMachineInstruction.MovementTypes.Absolute;
				}
				break;
			case 218012255u:
				if (text == "91")
				{
					processingMachineState.movementType = PrinterMachineInstruction.MovementTypes.Relative;
				}
				break;
			case 234789874u:
				if (text == "92")
				{
					double readValue = 0.0;
					if (GCodeFile.GetFirstNumberAfter("E", lineString, ref readValue))
					{
						amountOfAccumulatedEWhileParsing = processingMachineState.EPosition - readValue;
					}
				}
				break;
			case 2033888439u:
				_ = text == "130";
				break;
			case 4230917885u:
				_ = text == "161";
				break;
			case 4180585028u:
				{
					_ = text == "162";
					break;
				}
				IL_020b:
				readValue2 = 0.0;
				if (GCodeFile.GetFirstNumberAfter("X", lineString, ref readValue2))
				{
					processingMachineState.X = readValue2;
				}
				readValue3 = 0.0;
				if (GCodeFile.GetFirstNumberAfter("Y", lineString, ref readValue3))
				{
					processingMachineState.Y = readValue3;
				}
				readValue4 = 0.0;
				if (GCodeFile.GetFirstNumberAfter("Z", lineString, ref readValue4))
				{
					processingMachineState.Z = readValue4;
				}
				readValue5 = 0.0;
				if (GCodeFile.GetFirstNumberAfter("E", lineString, ref readValue5))
				{
					if (processingMachineState.movementType == PrinterMachineInstruction.MovementTypes.Absolute)
					{
						processingMachineState.EPosition = readValue5 + amountOfAccumulatedEWhileParsing;
					}
					else
					{
						processingMachineState.EPosition += readValue5;
					}
				}
				readValue6 = 0.0;
				if (GCodeFile.GetFirstNumberAfter("F", lineString, ref readValue6))
				{
					processingMachineState.FeedRate = readValue6;
				}
				if (!gcodeHasExplicitLayerChangeInfo && indexOfChangeInZ.Count == 0)
				{
					indexOfChangeInZ.Add(GCodeCommandQueue.Count);
					movesInLayer.Add(1);
				}
				break;
			}
		}

		public override Vector2 GetWeightedCenter()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			Vector2 total = default(Vector2);
			Parallel.For<Vector2>(0, GCodeCommandQueue.Count, (Func<Vector2>)(() => default(Vector2)), (Func<int, ParallelLoopState, Vector2, Vector2>)delegate(int index, ParallelLoopState loop, Vector2 subtotal)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				PrinterMachineInstruction printerMachineInstruction = GCodeCommandQueue[index];
				subtotal += new Vector2(printerMachineInstruction.Position.x, printerMachineInstruction.Position.y);
				return subtotal;
			}, (Action<Vector2>)delegate(Vector2 x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				total += new Vector2(x.x, x.y);
			});
			return total / (double)GCodeCommandQueue.Count;
		}

		public override RectangleDouble GetBounds()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble bounds = new RectangleDouble(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);
			Parallel.For<RectangleDouble>(0, GCodeCommandQueue.Count, (Func<RectangleDouble>)(() => new RectangleDouble(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue)), (Func<int, ParallelLoopState, RectangleDouble, RectangleDouble>)delegate(int index, ParallelLoopState loop, RectangleDouble subtotal)
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				PrinterMachineInstruction printerMachineInstruction = GCodeCommandQueue[index];
				subtotal.Left = Math.Min(printerMachineInstruction.Position.x, subtotal.Left);
				subtotal.Right = Math.Max(printerMachineInstruction.Position.x, subtotal.Right);
				subtotal.Bottom = Math.Min(printerMachineInstruction.Position.y, subtotal.Bottom);
				subtotal.Top = Math.Max(printerMachineInstruction.Position.y, subtotal.Top);
				return subtotal;
			}, (Action<RectangleDouble>)delegate(RectangleDouble x)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				bounds.Left = Math.Min(x.Left, bounds.Left);
				bounds.Right = Math.Max(x.Right, bounds.Right);
				bounds.Bottom = Math.Min(x.Bottom, bounds.Bottom);
				bounds.Top = Math.Max(x.Top, bounds.Top);
			});
			return bounds;
		}

		public override bool IsExtruding(int instructionIndexToCheck)
		{
			if (instructionIndexToCheck > 1 && instructionIndexToCheck < GCodeCommandQueue.Count && GCodeCommandQueue[instructionIndexToCheck].EPosition - GCodeCommandQueue[instructionIndexToCheck - 1].EPosition > 0.0)
			{
				return true;
			}
			return false;
		}

		public override double GetFilamentUsedMm(double filamentDiameter)
		{
			if (filamentUsedMmCache == 0.0 || filamentDiameter != diameterOfFilamentUsedMmCache)
			{
				double num = 0.0;
				double num2 = 0.0;
				for (int i = 0; i < GCodeCommandQueue.Count; i++)
				{
					PrinterMachineInstruction printerMachineInstruction = GCodeCommandQueue[i];
					string line = printerMachineInstruction.Line;
					if (line.StartsWith("G0") || line.StartsWith("G1"))
					{
						double readValue = num;
						if (GCodeFile.GetFirstNumberAfter("E", line, ref readValue))
						{
							if (printerMachineInstruction.movementType == PrinterMachineInstruction.MovementTypes.Absolute)
							{
								double num3 = readValue - num;
								num2 += num3;
							}
							else
							{
								num2 += readValue;
							}
							num = readValue;
						}
					}
					else if (line.StartsWith("G92"))
					{
						double readValue2 = 0.0;
						if (GCodeFile.GetFirstNumberAfter("E", line, ref readValue2))
						{
							num = readValue2;
						}
					}
				}
				filamentUsedMmCache = num2;
				diameterOfFilamentUsedMmCache = filamentDiameter;
			}
			return filamentUsedMmCache;
		}

		public override double GetFilamentCubicMm(double filamentDiameterMm)
		{
			double filamentUsedMm = GetFilamentUsedMm(filamentDiameterMm);
			double num = filamentDiameterMm / 2.0;
			return num * num * Math.PI * filamentUsedMm;
		}

		public override double GetFilamentWeightGrams(double filamentDiameterMm, double densityGramsPerCubicCm)
		{
			double num = 1000.0;
			double num2 = densityGramsPerCubicCm / num;
			return GetFilamentCubicMm(filamentDiameterMm) * num2;
		}

		public void Save(string dest)
		{
			using StreamWriter streamWriter = new StreamWriter(dest);
			foreach (PrinterMachineInstruction item in GCodeCommandQueue)
			{
				streamWriter.WriteLine(item.Line);
			}
		}

		public override double GetFilamentDiameter()
		{
			if (filamentDiameterCache == 0.0)
			{
				for (int i = 0; i < Math.Min(20, GCodeCommandQueue.Count) && !GCodeFile.GetFirstNumberAfter("filamentDiameter =", GCodeCommandQueue[i].Line, ref filamentDiameterCache); i++)
				{
				}
				if (filamentDiameterCache == 0.0)
				{
					string stringToCheckAfter = "; filament_diameter =";
					for (int num = GCodeCommandQueue.Count - 1; num > Math.Max(0, GCodeCommandQueue.Count - 100); num--)
					{
						GCodeFile.GetFirstNumberAfter(stringToCheckAfter, GCodeCommandQueue[num].Line, ref filamentDiameterCache);
					}
				}
				if (filamentDiameterCache == 0.0)
				{
					filamentDiameterCache = 1.75;
				}
			}
			return filamentDiameterCache;
		}

		public override double GetLayerHeight()
		{
			if (layerThickness > 0.0)
			{
				return layerThickness;
			}
			if (indexOfChangeInZ.Count > 2)
			{
				return GCodeCommandQueue[IndexOfChangeInZ[2]].Z - GCodeCommandQueue[IndexOfChangeInZ[1]].Z;
			}
			return 0.5;
		}

		public override double GetFirstLayerHeight()
		{
			if (firstLayerThickness > 0.0)
			{
				return firstLayerThickness;
			}
			if (indexOfChangeInZ.Count > 1)
			{
				return GCodeCommandQueue[IndexOfChangeInZ[1]].Z - GCodeCommandQueue[IndexOfChangeInZ[0]].Z;
			}
			return 0.5;
		}

		public override int GetLayerIndex(int instructionIndex)
		{
			if (instructionIndex >= 0 && instructionIndex < LineCount)
			{
				for (int i = 0; i < NumChangesInZ; i++)
				{
					if (instructionIndex < IndexOfChangeInZ[i])
					{
						return i;
					}
				}
				return NumChangesInZ;
			}
			return -1;
		}

		public override double Ratio0to1IntoContainedLayer(int instructionIndex)
		{
			int layerIndex = GetLayerIndex(instructionIndex);
			if (layerIndex > -1)
			{
				int num = 0;
				if (layerIndex > 0)
				{
					num = IndexOfChangeInZ[layerIndex - 1];
				}
				_ = LineCount;
				if (layerIndex < NumChangesInZ - 1)
				{
					_ = IndexOfChangeInZ[layerIndex];
				}
				int num2 = 0;
				for (int i = num; i < instructionIndex; i++)
				{
					_ = Instruction(i).Line;
					if (InstructionIndexIsFeature(i))
					{
						num2++;
					}
				}
				int num3 = 1;
				if (movesInLayer[layerIndex] != 0)
				{
					num3 = movesInLayer[layerIndex];
				}
				return (double)Math.Max(0, num2) / (double)num3;
			}
			return 0.0;
		}

		private bool InstructionIndexIsFeature(int instructionIndex)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (instructionIndex > 0)
			{
				PrinterMachineInstruction printerMachineInstruction = Instruction(instructionIndex);
				PrinterMachineInstruction printerMachineInstruction2 = Instruction(instructionIndex - 1);
				if (!(printerMachineInstruction.Position == printerMachineInstruction2.Position))
				{
					return true;
				}
				if (Math.Abs(printerMachineInstruction.EPosition - printerMachineInstruction2.EPosition) > 0.0)
				{
					return true;
				}
				if (printerMachineInstruction.Line.StartsWith("G10") || printerMachineInstruction.Line.StartsWith("G11"))
				{
					return true;
				}
			}
			return false;
		}
	}
}
