using System;
using System.Collections.Generic;
using System.Threading;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterSlice;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.Replay
{
	public class RewindRunner
	{
		private GCodeFile replayFile;

		private Tools printTools;

		private GCodeSelectionInfo gCodeSelectionInfo;

		private GCodeSelectionInfo gCodeSelectionInfoAtStart;

		private bool rewindDone;

		private int activeRewindToolIndex = -1;

		private EventHandler unregisterEvents;

		private List<Tuple<string, int>> rewindLines = new List<Tuple<string, int>>();

		private EventHandler lineWroteEvent;

		public RewindRunner(GCodeFile fileToReplayFrom, GCodeSelectionInfo gCodeSelectionInfo)
		{
			replayFile = fileToReplayFrom;
			printTools = Tools.Copy(fileToReplayFrom.PrintTools);
			this.gCodeSelectionInfo = gCodeSelectionInfo;
			gCodeSelectionInfoAtStart = new GCodeSelectionInfo();
			SetLineWriteCallback();
		}

		private void SetLineWriteCallback()
		{
			lineWroteEvent = (EventHandler)Delegate.Combine(lineWroteEvent, (EventHandler)delegate(object sender, EventArgs e)
			{
				lock (rewindLines)
				{
					if (rewindLines.Count > 0)
					{
						if (rewindLines[0].Item1.Trim().Length > 0)
						{
							PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(rewindLines[0].Item1);
							SetSelectStartPoint(rewindLines[0].Item2);
						}
						if (rewindLines[0].Item1.StartsWith("T"))
						{
							double readValue = 0.0;
							if (GCodeFile.GetFirstNumberAfter("T", rewindLines[0].Item1, ref readValue))
							{
								activeRewindToolIndex = (int)readValue;
							}
						}
						StringEventArgs val = e as StringEventArgs;
						if (val != null && val.get_Data().Contains("M105"))
						{
							rewindLines[0] = Tuple.Create("", rewindLines[0].Item2);
						}
						else
						{
							rewindLines.RemoveAt(0);
						}
					}
					else
					{
						rewindDone = true;
						unregisterEvents?.Invoke(this, e);
					}
				}
			});
		}

		private void AddRewindLine(string lineToAdd, int lineIndex)
		{
			rewindLines.Add(Tuple.Create(lineToAdd, lineIndex));
		}

		private void SetSelectStartPoint(int lineIndex)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			if (replayFile.Instruction(lineIndex).Line.StartsWith("G1 "))
			{
				gCodeSelectionInfo.StartLineIndex = lineIndex;
				gCodeSelectionInfo.StartBedPosition = new Vector2(replayFile.Instruction(lineIndex).Position);
			}
			else if (replayFile.Instruction(lineIndex).Line.StartsWith("G0 "))
			{
				int i;
				for (i = lineIndex + 1; !replayFile.Instruction(i).Line.StartsWith("G1 ") && i < replayFile.LineCount; i++)
				{
				}
				gCodeSelectionInfo.StartLineIndex = i;
				gCodeSelectionInfo.StartBedPosition = new Vector2(replayFile.Instruction(lineIndex).Position);
			}
		}

		public void DoRewindNow()
		{
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Expected O, but got Unknown
			CopyGCodeSelectionInfo(gCodeSelectionInfo, gCodeSelectionInfoAtStart);
			PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(StringHelper.FormatWith("G0 F{0:0.###} X{1:0.###} Y{2:0.###}", new object[3]
			{
				replayFile.Instruction(gCodeSelectionInfo.StartLineIndex).FeedRate,
				gCodeSelectionInfo.StartBedPosition.x,
				gCodeSelectionInfo.StartBedPosition.y
			}));
			rewindLines.Clear();
			int startExtruderIndex = replayFile.Instruction(gCodeSelectionInfo.StartLineIndex).ExtruderIndex;
			int endExtruderIndex = replayFile.Instruction(gCodeSelectionInfo.EndLineIndex).ExtruderIndex;
			if (PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
			{
				Array.ForEach(printTools.PreambleCode().Split(new char[1]
				{
					'\n'
				}), delegate(string line)
				{
					AddRewindLine(line, startExtruderIndex);
				});
			}
			Array.ForEach(printTools.get_Item(startExtruderIndex).Activate(startExtruderIndex).Split(new char[1]
			{
				'\n'
			}), delegate(string line)
			{
				AddRewindLine(line, startExtruderIndex);
			});
			Array.ForEach(printTools.get_Item(startExtruderIndex).Start().Split(new char[1]
			{
				'\n'
			}), delegate(string line)
			{
				AddRewindLine(line, startExtruderIndex);
			});
			for (int i = gCodeSelectionInfo.StartLineIndex; i < gCodeSelectionInfo.EndLineIndex; i++)
			{
				rewindLines.Add(Tuple.Create(replayFile.Instruction(i).Line, i));
			}
			rewindLines.Add(Tuple.Create(StringHelper.FormatWith("G1 F{0:0.###} X{1:0.###} Y{2:0.###}", new object[3]
			{
				replayFile.Instruction(gCodeSelectionInfo.StartLineIndex).FeedRate,
				gCodeSelectionInfo.EndBedPosition.x,
				gCodeSelectionInfo.EndBedPosition.y
			}), endExtruderIndex));
			printTools.get_Item(endExtruderIndex).Start();
			Array.ForEach(printTools.get_Item(endExtruderIndex).Stop().Split(new char[1]
			{
				'\n'
			}), delegate(string line)
			{
				AddRewindLine(line, endExtruderIndex);
			});
			Array.ForEach(printTools.get_Item(endExtruderIndex).Deactivate().Split(new char[1]
			{
				'\n'
			}), delegate(string line)
			{
				AddRewindLine(line, endExtruderIndex);
			});
			rewindLines.RemoveAll((Tuple<string, int> rewindItem) => string.IsNullOrWhiteSpace(rewindItem.Item1));
			activeRewindToolIndex = startExtruderIndex;
			rewindDone = false;
			lineWroteEvent(this, (EventArgs)new StringEventArgs(""));
			PrinterConnectionAndCommunication.Instance.WroteLine.RegisterEvent(lineWroteEvent, ref unregisterEvents);
			while (!rewindDone)
			{
				Thread.Sleep(100);
			}
			CopyGCodeSelectionInfo(gCodeSelectionInfoAtStart, gCodeSelectionInfo);
			activeRewindToolIndex = -1;
			unregisterEvents = null;
		}

		public void CancelRewind()
		{
			if (activeRewindToolIndex != -1 && unregisterEvents != null)
			{
				rewindDone = true;
				CopyGCodeSelectionInfo(gCodeSelectionInfoAtStart, gCodeSelectionInfo);
				unregisterEvents(this, null);
				printTools.get_Item(activeRewindToolIndex).Start();
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(printTools.get_Item(activeRewindToolIndex).Stop());
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(printTools.get_Item(activeRewindToolIndex).Deactivate());
			}
		}

		private void CopyGCodeSelectionInfo(GCodeSelectionInfo from, GCodeSelectionInfo to)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			to.StartLineIndex = from.StartLineIndex;
			to.EndLineIndex = from.EndLineIndex;
			to.StartBedPosition = from.StartBedPosition;
			to.EndBedPosition = from.EndBedPosition;
		}
	}
}
