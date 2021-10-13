using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterSlice;

namespace MatterHackers.MatterControl.Replay
{
	public class ReplayWidget : GuiWidget
	{
		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private GCodeFile replayFile;

		private List<int> replayInstructionIndices;

		private Tools printTools;

		private bool stopReplay;

		public ReplayWidget(GCodeFile fileToReplayFrom, List<int> currentlySelectedInstructionIndices)
			: this()
		{
			replayFile = fileToReplayFrom;
			replayInstructionIndices = currentlySelectedInstructionIndices;
			replayInstructionIndices.Sort();
			printTools = Tools.Copy(fileToReplayFrom.PrintTools);
			CreateAndAddChildren();
		}

		private void CreateAndAddChildren()
		{
			Button val = textImageButtonFactory.Generate("Do Replay".Localize());
			((GuiWidget)val).AnchorCenter();
			((GuiWidget)val).add_Click((EventHandler<MouseEventArgs>)DoReplayButton_Click);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private async void DoReplayButton_Click(object sender, EventArgs e)
		{
			GuiWidget cover = new GuiWidget();
			cover.AnchorAll();
			cover.set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 200));
			((GuiWidget)this).AddChild(cover, -1);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorCenter();
			((GuiWidget)val).set_HAnchor((HAnchor)(((GuiWidget)val).get_HAnchor() | 8));
			((GuiWidget)val).set_VAnchor((VAnchor)(((GuiWidget)val).get_VAnchor() | 8));
			cover.AddChild((GuiWidget)(object)val, -1);
			TextWidget val2 = new TextWidget("The replay is running...".Localize(), 0.0, 0.0, 20.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_HAnchor((HAnchor)2);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			Button val3 = textImageButtonFactory.Generate("Cancel".Localize());
			((GuiWidget)val3).set_HAnchor((HAnchor)2);
			((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				stopReplay = true;
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			(sender as GuiWidget).set_Enabled(false);
			await Task.Run(delegate
			{
				DoReplay();
			});
			(sender as GuiWidget).set_Enabled(true);
			((GuiWidget)this).RemoveChild(cover);
		}

		private void DoReplay()
		{
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Expected O, but got Unknown
			List<string> replayLines = new List<string>();
			int num = -1;
			foreach (int replayInstructionIndex in replayInstructionIndices)
			{
				PrinterMachineInstruction printerMachineInstruction = replayFile.Instruction((replayInstructionIndex > 0) ? (replayInstructionIndex - 1) : 0);
				PrinterMachineInstruction printerMachineInstruction2 = replayFile.Instruction(replayInstructionIndex);
				string item = StringHelper.FormatWith("G0 F{0:0.000} X{1:0.000} Y{2:0.000} Z{3:0.000}", new object[4]
				{
					printerMachineInstruction.FeedRate,
					printerMachineInstruction.X,
					printerMachineInstruction.Y,
					printerMachineInstruction.Z
				});
				string item2 = StringHelper.FormatWith("G1 F{0:0.000} X{1:0.000} Y{2:0.000} Z{3:0.000}", new object[4]
				{
					printerMachineInstruction2.FeedRate,
					printerMachineInstruction2.X,
					printerMachineInstruction2.Y,
					printerMachineInstruction2.Z
				});
				if (num != printerMachineInstruction2.ExtruderIndex)
				{
					if (num != -1)
					{
						Array.ForEach(printTools.get_ActiveTool().Deactivate().Split(new char[1]
						{
							'\n'
						}), new Action<string>(replayLines.Add));
					}
					Array.ForEach(printTools.ActivateTool(printerMachineInstruction2.ExtruderIndex).Split(new char[1]
					{
						'\n'
					}), new Action<string>(replayLines.Add));
					num = printerMachineInstruction2.ExtruderIndex;
				}
				replayLines.Add(item);
				Array.ForEach(printTools.get_ActiveTool().Start().Split(new char[1]
				{
					'\n'
				}), new Action<string>(replayLines.Add));
				replayLines.Add(item2);
				Array.ForEach(printTools.get_ActiveTool().Stop().Split(new char[1]
				{
					'\n'
				}), new Action<string>(replayLines.Add));
				if (stopReplay)
				{
					break;
				}
			}
			if (num != -1)
			{
				Array.ForEach(printTools.get_ActiveTool().Deactivate().Split(new char[1]
				{
					'\n'
				}), new Action<string>(replayLines.Add));
			}
			bool replayDone = false;
			EventHandler unregisterEvents = null;
			EventHandler eventHandler = delegate(object sender, EventArgs e)
			{
				lock (replayLines)
				{
					if (replayLines.Count > 0)
					{
						if (replayLines[0].Trim().Length > 0)
						{
							PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(replayLines[0]);
						}
						StringEventArgs val = e as StringEventArgs;
						if (val != null && val.get_Data().Contains("M105"))
						{
							replayLines[0] = "";
						}
						else
						{
							replayLines.RemoveAt(0);
						}
					}
					else
					{
						replayDone = true;
						unregisterEvents?.Invoke(this, e);
					}
				}
			};
			eventHandler(this, (EventArgs)new StringEventArgs(""));
			PrinterConnectionAndCommunication.Instance.WroteLine.RegisterEvent(eventHandler, ref unregisterEvents);
			while (!replayDone && !stopReplay)
			{
				Thread.Sleep(100);
			}
			if (stopReplay)
			{
				unregisterEvents?.Invoke(this, null);
			}
			stopReplay = false;
		}
	}
}
