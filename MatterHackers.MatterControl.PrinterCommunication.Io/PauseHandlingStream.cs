using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.MatterSlice;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class PauseHandlingStream : GCodeStreamProxy
	{
		public enum PauseReason
		{
			UserRequested,
			PauseLayerReached,
			GCodeRequest,
			FilamentRunout
		}

		protected PrinterMove lastDestination;

		protected PrinterMove lastPrintMove;

		private List<string> commandQueue = new List<string>();

		private object locker = new object();

		private PrinterMove moveLocationAtEndOfPauseCode;

		private Stopwatch timeSinceLastEndstopRead = new Stopwatch();

		private bool readOutOfFilament;

		private Tools tools;

		private int activeToolIndex;

		private bool inCriticalSection;

		private EventHandler unregisterEvents;

		private string pauseCaption = "Printer Paused".Localize();

		private string layerPauseMessage = "Your 3D print has been auto-paused.\nPause layer{0} reached.".Localize();

		private string filamentPauseMessage = "Out of filament detected\nYour 3D print has been paused.".Localize();

		public PrinterMove LastDestination => lastDestination;

		public PauseHandlingStream(GCodeStream internalStream, Tools aetherTools = null)
			: base(internalStream)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			tools = aetherTools;
			if (tools == null)
			{
				aetherTools = new Tools(new ConfigSettings());
			}
			activeToolIndex = -1;
			inCriticalSection = false;
			PrinterConnectionAndCommunication.Instance.ReadLine.RegisterEvent((EventHandler)delegate(object s, EventArgs e)
			{
				StringEventArgs val = e as StringEventArgs;
				if (val != null && val.get_Data().Contains("ros_") && val.get_Data().Contains("TRIGGERED"))
				{
					readOutOfFilament = true;
				}
			}, ref unregisterEvents);
		}

		public override void Dispose()
		{
			unregisterEvents?.Invoke(this, null);
			base.Dispose();
		}

		public void Add(string line)
		{
			lock (locker)
			{
				commandQueue.Add(line);
			}
		}

		public void DoPause(PauseReason pauseReason, string layerNumber = "")
		{
			PrinterConnectionAndCommunication instance = PrinterConnectionAndCommunication.Instance;
			switch (pauseReason)
			{
			case PauseReason.PauseLayerReached:
			case PauseReason.GCodeRequest:
				instance.PauseOnLayer.CallEvents((object)instance, (EventArgs)new PrintItemWrapperEventArgs(instance.ActivePrintItem));
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(ResumePrint, StringHelper.FormatWith(layerPauseMessage, new object[1]
					{
						layerNumber
					}), pauseCaption, StyledMessageBox.MessageType.YES_NO, "Ok".Localize(), "Resume".Localize());
				});
				break;
			case PauseReason.FilamentRunout:
				instance.FilamentRunout.CallEvents((object)instance, (EventArgs)new PrintItemWrapperEventArgs(instance.ActivePrintItem));
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(ResumePrint, filamentPauseMessage, pauseCaption, StyledMessageBox.MessageType.YES_NO, "Ok".Localize(), "Resume".Localize());
				});
				break;
			}
			string value = ActiveSliceSettings.Instance.GetValue("pause_gcode");
			InjectPauseGCode(value);
			if (activeToolIndex != -1 && tools.get_Count() > activeToolIndex)
			{
				if (inCriticalSection)
				{
					tools.get_ActiveTool().Start();
					InjectPauseGCode(tools.get_ActiveTool().Stop());
				}
				InjectPauseGCode(tools.get_ActiveTool().Deactivate());
			}
			InjectPauseGCode("M114");
			InjectPauseGCode("MH_PAUSE");
		}

		private void ResumePrint(bool clickedOk)
		{
			if (!clickedOk && PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
			{
				PrinterConnectionAndCommunication.Instance.Resume();
			}
		}

		public override string ReadLine()
		{
			string text = null;
			lock (locker)
			{
				if (commandQueue.Count > 0)
				{
					text = commandQueue[0];
					commandQueue.RemoveAt(0);
				}
			}
			if (text == null)
			{
				if (!PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
				{
					text = base.ReadLine();
					if (text == null)
					{
						return text;
					}
					if (ActiveSliceSettings.Instance.GetValue<bool>("filament_runout_sensor") && (!timeSinceLastEndstopRead.get_IsRunning() || timeSinceLastEndstopRead.get_ElapsedMilliseconds() > 5000))
					{
						PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow("M119");
						timeSinceLastEndstopRead.Restart();
					}
				}
				else
				{
					text = "";
				}
			}
			if (GCodeFile.IsLayerChange(text))
			{
				string text2 = text.Split(new char[1]
				{
					':'
				})[1];
				if (PauseOnLayer(text2))
				{
					DoPause(PauseReason.PauseLayerReached, $" {text2}");
				}
			}
			else if (text.StartsWith("M226") || text.StartsWith("@pause"))
			{
				DoPause(PauseReason.GCodeRequest);
				text = "";
			}
			else if (text == "MH_PAUSE")
			{
				moveLocationAtEndOfPauseCode = lastPrintMove;
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting)
				{
					PrinterConnectionAndCommunication.Instance.CommunicationState = PrinterConnectionAndCommunication.CommunicationStates.Paused;
				}
				text = "";
			}
			else if (readOutOfFilament)
			{
				readOutOfFilament = false;
				DoPause(PauseReason.FilamentRunout);
				text = "";
			}
			else if (text.StartsWith("T"))
			{
				int readValue = -1;
				GCodeFile.GetFirstNumberAfter("T", text, ref readValue);
				if (readValue != -1 && readValue < tools.get_Count())
				{
					activeToolIndex = readValue;
					tools.ActivateTool(activeToolIndex);
				}
			}
			if (text != null && GCodeStream.LineIsMovement(text))
			{
				if (PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.Paused)
				{
					if (text.StartsWith("G0"))
					{
						inCriticalSection = false;
					}
					else if (text.StartsWith("G1"))
					{
						inCriticalSection = true;
					}
				}
				lastPrintMove = (lastDestination = GCodeStream.GetPosition(text, lastDestination));
			}
			return text;
		}

		public void Resume()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = moveLocationAtEndOfPauseCode.position;
			InjectPauseGCode(StringHelper.FormatWith("G92 E{0:0.###}", new object[1]
			{
				moveLocationAtEndOfPauseCode.extrusion
			}));
			Vector3 val = position + new Vector3(0.01, 0.01, 0.01);
			Vector3 val2 = ActiveSliceSettings.Instance.Helpers.ManualMovementSpeeds();
			InjectPauseGCode(StringHelper.FormatWith("G1 X{0:0.###} Y{1:0.###} Z{2:0.###} F{3}", new object[4]
			{
				val.x,
				val.y,
				val.z,
				val2.x + 1.0
			}));
			InjectPauseGCode(StringHelper.FormatWith("G1 X{0:0.###} Y{1:0.###} Z{2:0.###} F{3}", new object[4]
			{
				position.x,
				position.y,
				position.z,
				val2
			}));
			string value = ActiveSliceSettings.Instance.GetValue("resume_gcode");
			InjectPauseGCode(value);
			InjectPauseGCode("M114");
			if (activeToolIndex != -1 && tools.get_Count() > activeToolIndex)
			{
				InjectPauseGCode(tools.get_ActiveTool().Activate(activeToolIndex));
				if (inCriticalSection)
				{
					tools.get_ActiveTool().Stop();
					InjectPauseGCode(tools.get_ActiveTool().Start());
				}
			}
		}

		public override void SetPrinterPosition(PrinterMove position)
		{
			lastDestination = position;
			internalStream.SetPrinterPosition(lastDestination);
		}

		private void InjectPauseGCode(string codeToInject)
		{
			codeToInject = GCodeProcessing.ReplaceMacroValues(codeToInject);
			codeToInject = codeToInject.Replace("\\n", "\n");
			string[] array = codeToInject.Split(new char[1]
			{
				'\n'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Split(new char[1]
				{
					';'
				})[0].Trim().ToUpper();
				if (text != "")
				{
					Add(text);
				}
			}
		}

		private bool PauseOnLayer(string layer)
		{
			if (int.TryParse(layer, out var result) && Enumerable.Contains<int>((IEnumerable<int>)ActiveSliceSettings.Instance.Helpers.LayerToPauseOn(), result))
			{
				return true;
			}
			return false;
		}
	}
}
