using System;
using MatterHackers.Agg;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterSlice;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.Replay
{
	public class Rewinder
	{
		private GCodeFile replayFile;

		private Tools printTools;

		private GCodeSelectionInfo gCodeSelectionInfo;

		public bool StopRewind;

		public double StepAmount = 1.0;

		private RewindRunner rewindRunner;

		private int activeToolIndex = -1;

		private bool startIsCurrent = true;

		private Vector2 point
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (startIsCurrent)
				{
					return gCodeSelectionInfo.StartBedPosition;
				}
				return gCodeSelectionInfo.EndBedPosition;
			}
			set
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				if (startIsCurrent)
				{
					gCodeSelectionInfo.StartBedPosition = value;
				}
				else
				{
					gCodeSelectionInfo.EndBedPosition = value;
				}
			}
		}

		private int lineIndex
		{
			get
			{
				if (startIsCurrent)
				{
					return gCodeSelectionInfo.StartLineIndex;
				}
				return gCodeSelectionInfo.EndLineIndex;
			}
			set
			{
				if (startIsCurrent)
				{
					gCodeSelectionInfo.StartLineIndex = value;
				}
				else
				{
					gCodeSelectionInfo.EndLineIndex = value;
				}
			}
		}

		private Vector2 otherPoint
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (!startIsCurrent)
				{
					return gCodeSelectionInfo.StartBedPosition;
				}
				return gCodeSelectionInfo.EndBedPosition;
			}
		}

		private int otherLineIndex
		{
			get
			{
				if (!startIsCurrent)
				{
					return gCodeSelectionInfo.StartLineIndex;
				}
				return gCodeSelectionInfo.EndLineIndex;
			}
		}

		public Rewinder(GCodeFile fileToReplayFrom, GCodeSelectionInfo gCodeSelectionInfo)
		{
			replayFile = fileToReplayFrom;
			printTools = Tools.Copy(fileToReplayFrom.PrintTools);
			this.gCodeSelectionInfo = gCodeSelectionInfo;
			rewindRunner = new RewindRunner(fileToReplayFrom, gCodeSelectionInfo);
		}

		public void SetStartAsCurrent()
		{
			startIsCurrent = true;
		}

		public void SetEndAsCurrent()
		{
			startIsCurrent = false;
		}

		public void MoveToCurrent()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			MoveToPoint(point);
		}

		public void JogCurrent(bool forward = true)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector((lineIndex > 1) ? replayFile.Instruction(lineIndex - 1).Position : Vector3.Zero);
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector(replayFile.Instruction(lineIndex).Position);
			Vector2 val3 = val2 - val;
			Vector2 val4 = (forward ? (val3 / ((Vector2)(ref val3)).get_Length()) : (val3 / (0.0 - ((Vector2)(ref val3)).get_Length())));
			val4 *= StepAmount;
			Vector2 val5 = point + val4;
			RectangleDouble val6 = default(RectangleDouble);
			((RectangleDouble)(ref val6))._002Ector(Math.Min(val.x, val2.x), Math.Min(val.y, val2.y), Math.Max(val.x, val2.x), Math.Max(val.y, val2.y));
			if (lineIndex != otherLineIndex)
			{
				if (((RectangleDouble)(ref val6)).Contains(val5))
				{
					point = val5;
				}
				else
				{
					lineIndex = ClosestPrintLine(forward);
					point = new Vector2(forward ? replayFile.Instruction(lineIndex - 1).Position : replayFile.Instruction(lineIndex).Position);
				}
			}
			else
			{
				Vector2 val7 = (startIsCurrent ? val : val2);
				RectangleDouble val8 = default(RectangleDouble);
				((RectangleDouble)(ref val8))._002Ector(Math.Min(val7.x, otherPoint.x), Math.Min(val7.y, otherPoint.y), Math.Max(val7.x, otherPoint.x), Math.Max(val7.y, otherPoint.y));
				if (((RectangleDouble)(ref val8)).Contains(val5))
				{
					point = val5;
				}
				else if (!((RectangleDouble)(ref val6)).Contains(val5) && startIsCurrent != forward)
				{
					lineIndex = ClosestPrintLine(forward);
					point = new Vector2(forward ? replayFile.Instruction(lineIndex - 1).Position : replayFile.Instruction(lineIndex).Position);
				}
			}
			MoveToCurrent();
		}

		public void SkipCurrentLine(bool forward = true)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(forward ? replayFile.Instruction(lineIndex).Position : replayFile.Instruction(lineIndex - 1).Position);
			Vector2 val2 = point - val;
			if (((Vector2)(ref val2)).get_Length() < 0.25)
			{
				int num = ClosestPrintLine(forward);
				if (num != otherLineIndex)
				{
					lineIndex = num;
					point = new Vector2(forward ? replayFile.Instruction(lineIndex).Position : replayFile.Instruction(lineIndex - 1).Position);
				}
			}
			else if (lineIndex != otherLineIndex || startIsCurrent != forward)
			{
				point = val;
			}
			MoveToCurrent();
		}

		public void DoRewindNow()
		{
			rewindRunner.DoRewindNow();
		}

		public void CancelRewind()
		{
			rewindRunner.CancelRewind();
		}

		private void MoveToPoint(Vector2 pointToMoveTo)
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			if (!PrinterConnectionAndCommunication.Instance.HasHomedSinceConnect)
			{
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow("G28");
			}
			if (activeToolIndex != replayFile.Instruction(lineIndex).ExtruderIndex)
			{
				activeToolIndex = replayFile.Instruction(lineIndex).ExtruderIndex;
				PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(printTools.get_Item(activeToolIndex).Activate(activeToolIndex));
			}
			PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(StringHelper.FormatWith("G0 F{0:0.###} X{1:0.###} Y{2:0.###}", new object[3]
			{
				replayFile.Instruction(lineIndex).FeedRate,
				pointToMoveTo.x,
				pointToMoveTo.y
			}));
		}

		private int ClosestPrintLine(bool forward)
		{
			if (lineIndex == otherLineIndex && startIsCurrent == forward)
			{
				return lineIndex;
			}
			int num = lineIndex;
			int num2 = (forward ? 1 : (-1));
			do
			{
				num += num2;
				if (num < 0 || num >= replayFile.LineCount)
				{
					num = lineIndex;
					break;
				}
			}
			while (!replayFile.Instruction(num).Line.StartsWith("G1 "));
			if (replayFile.GetLayerIndex(lineIndex) != replayFile.GetLayerIndex(num))
			{
				num = lineIndex;
			}
			return num;
		}
	}
}
