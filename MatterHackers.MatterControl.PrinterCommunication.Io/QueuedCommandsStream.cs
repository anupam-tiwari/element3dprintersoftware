using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterControls;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class QueuedCommandsStream : GCodeStreamProxy
	{
		public const string MacroPrefix = "; host.";

		private List<string> commandQueue = new List<string>();

		private List<string> commandsToRepeat = new List<string>();

		private object locker = new object();

		private double maxTimeToWaitForOk;

		private int repeatCommandIndex;

		private bool runningMacro;

		private double startingBedTemp;

		private List<double> startingExtruderTemps = new List<double>();

		private Stopwatch timeHaveBeenWaiting = new Stopwatch();

		private bool waitingForUserInput;

		public QueuedCommandsStream(GCodeStream internalStream)
			: base(internalStream)
		{
		}//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown


		public void Add(string line, bool forceTopOfQueue = false)
		{
			lock (locker)
			{
				if (forceTopOfQueue)
				{
					commandQueue.Insert(0, line);
				}
				else
				{
					commandQueue.Add(line);
				}
			}
		}

		public void Cancel()
		{
			Reset();
		}

		public void Continue()
		{
			waitingForUserInput = false;
			timeHaveBeenWaiting.Reset();
			maxTimeToWaitForOk = 0.0;
			commandsToRepeat.Clear();
		}

		public ImageBuffer LoadImageAsset(string uri)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			string text = Path.Combine("Images", "Macros", uri);
			bool flag = false;
			if (uri.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
			{
				try
				{
					flag = StaticData.get_Instance().FileExists(text);
				}
				catch
				{
					flag = false;
				}
			}
			if (flag)
			{
				return StaticData.get_Instance().LoadImage(text);
			}
			ImageBuffer val = new ImageBuffer(320, 10);
			ApplicationController.Instance.DownloadToImageAsync(val, uri, scaleToImageX: true);
			return val;
		}

		public override string ReadLine()
		{
			string text = null;
			if (waitingForUserInput)
			{
				text = "";
				Thread.Sleep(100);
				if (timeHaveBeenWaiting.get_IsRunning() && timeHaveBeenWaiting.get_Elapsed().TotalSeconds > maxTimeToWaitForOk)
				{
					if (commandsToRepeat.Count > 0)
					{
						Reset();
					}
					else
					{
						Continue();
					}
				}
				if (maxTimeToWaitForOk > 0.0 && timeHaveBeenWaiting.get_Elapsed().TotalSeconds < maxTimeToWaitForOk && commandsToRepeat.Count > 0)
				{
					text = commandsToRepeat[repeatCommandIndex % commandsToRepeat.Count];
					repeatCommandIndex++;
				}
			}
			else
			{
				lock (locker)
				{
					if (commandQueue.Count > 0)
					{
						text = commandQueue[0];
						text = GCodeProcessing.ReplaceMacroValues(text);
						commandQueue.RemoveAt(0);
					}
				}
				if (text != null)
				{
					if (text.StartsWith("; host.") && text.TrimEnd(Array.Empty<char>()).EndsWith(")"))
					{
						if (!runningMacro)
						{
							runningMacro = true;
							int value = ActiveSliceSettings.Instance.GetValue<int>("extruder_count");
							for (int i = 0; i < value; i++)
							{
								startingExtruderTemps.Add(PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(i));
							}
							if (ActiveSliceSettings.Instance.GetValue<bool>("has_heated_bed"))
							{
								startingBedTemp = PrinterConnectionAndCommunication.Instance.TargetBedTemperature;
							}
						}
						int num = text.IndexOf('(', "; host.".Length);
						string text2 = "";
						if (num > 0)
						{
							text2 = text.Substring("; host.".Length, num - "; host.".Length);
						}
						RunningMacroPage.MacroCommandData macroData = new RunningMacroPage.MacroCommandData();
						string value2 = "";
						if (TryGetAfterString(text, "title", out value2))
						{
							macroData.title = value2;
						}
						if (TryGetAfterString(text, "expire", out value2))
						{
							double.TryParse(value2, out macroData.expireTime);
							maxTimeToWaitForOk = macroData.expireTime;
						}
						if (TryGetAfterString(text, "count_down", out value2))
						{
							double.TryParse(value2, out macroData.countDown);
						}
						if (TryGetAfterString(text, "image", out value2))
						{
							macroData.image = LoadImageAsset(value2);
						}
						if (TryGetAfterString(text, "wait_ok", out value2))
						{
							macroData.waitOk = value2 == "true";
						}
						if (TryGetAfterString(text, "repeat_gcode", out value2))
						{
							string[] array = value2.Split(new char[1]
							{
								'|'
							});
							foreach (string item in array)
							{
								commandsToRepeat.Add(item);
							}
						}
						switch (text2)
						{
						case "choose_material":
							waitingForUserInput = true;
							macroData.showMaterialSelector = true;
							macroData.waitOk = true;
							UiThread.RunOnIdle((Action)delegate
							{
								RunningMacroPage.Show(macroData);
							});
							break;
						case "close":
							runningMacro = false;
							UiThread.RunOnIdle((Action)delegate
							{
								WizardWindow.Close("Macro");
							});
							break;
						case "ding":
							MatterControlApplication.Instance.PlaySound("timer-done.wav");
							break;
						case "show_message":
							waitingForUserInput = macroData.waitOk | (macroData.expireTime > 0.0);
							UiThread.RunOnIdle((Action)delegate
							{
								RunningMacroPage.Show(macroData);
							});
							break;
						}
					}
				}
				else
				{
					text = base.ReadLine();
				}
			}
			return text;
		}

		public void Reset()
		{
			lock (locker)
			{
				commandQueue.Clear();
			}
			if (runningMacro)
			{
				runningMacro = false;
				for (int i = 0; i < startingExtruderTemps.Count; i++)
				{
					PrinterConnectionAndCommunication.Instance.SetTargetExtruderTemperature(i, startingExtruderTemps[i]);
				}
				if (ActiveSliceSettings.Instance.GetValue<bool>("has_heated_bed"))
				{
					PrinterConnectionAndCommunication.Instance.TargetBedTemperature = startingBedTemp;
				}
			}
			waitingForUserInput = false;
			timeHaveBeenWaiting.Reset();
			maxTimeToWaitForOk = 0.0;
			UiThread.RunOnIdle((Action)delegate
			{
				WizardWindow.Close("Macro");
			});
		}

		private bool TryGetAfterString(string macroLine, string variableName, out string value)
		{
			Match val = Regex.Match(macroLine, variableName + ":\"([^\"]+)");
			value = (((Group)val).get_Success() ? ((Capture)val.get_Groups().get_Item(1)).get_Value() : null);
			return ((Group)val).get_Success();
		}
	}
}
