using System;
using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class LevelWizardBase : SystemWindow
	{
		public enum RuningState
		{
			InitialStartupCalibration,
			UserRequestedCalibration
		}

		private LevelingStrings levelingStrings = new LevelingStrings();

		protected WizardControl printLevelWizard;

		private static SystemWindow printLevelWizardWindow;

		protected int totalSteps
		{
			get;
			private set;
		}

		public LevelWizardBase(int width, int height, int totalSteps)
			: this((double)width, (double)height)
		{
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			this.totalSteps = totalSteps;
		}

		public static List<Vector2> GetManualPositions(string settingsValue, int requiredCount)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(settingsValue))
			{
				string[] array = settingsValue.Split(new char[1]
				{
					':'
				});
				if (array.Length == requiredCount)
				{
					List<Vector2> list = new List<Vector2>();
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string[] array3 = array2[i].Split(new char[1]
						{
							','
						});
						if (array3.Length != 2)
						{
							return null;
						}
						Vector2 item = default(Vector2);
						if (!double.TryParse(array3[0], out item.x))
						{
							return null;
						}
						if (!double.TryParse(array3[1], out item.y))
						{
							return null;
						}
						list.Add(item);
					}
					if (list.Count == requiredCount)
					{
						return list;
					}
				}
			}
			return null;
		}

		public static Vector2 GetPrintLevelPositionToSample(int index)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			List<Vector2> manualPositions = GetManualPositions(ActiveSliceSettings.Instance.GetValue("leveling_manual_positions"), 3);
			if (manualPositions != null)
			{
				return manualPositions[index];
			}
			Vector2 value = ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size");
			Vector2 value2 = ActiveSliceSettings.Instance.GetValue<Vector2>("print_center");
			BedShape value3 = ActiveSliceSettings.Instance.GetValue<BedShape>("bed_shape");
			if (value3 != 0 && value3 == BedShape.Circular)
			{
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector(value2.x, value2.y + value.y / 2.0 * 0.5);
				return (Vector2)(index switch
				{
					0 => val, 
					1 => Vector2.Rotate(val, 2.094395160675049), 
					2 => Vector2.Rotate(val, 4.188790321350098), 
					_ => throw new IndexOutOfRangeException(), 
				});
			}
			return (Vector2)(index switch
			{
				0 => new Vector2(value2.x, value2.y + value.y / 2.0 * 0.8), 
				1 => new Vector2(value2.x - value.x / 2.0 * 0.8, value2.y - value.y / 2.0 * 0.8), 
				2 => new Vector2(value2.x + value.x / 2.0 * 0.8, value2.y - value.y / 2.0 * 0.8), 
				_ => throw new IndexOutOfRangeException(), 
			});
		}

		public static void ShowPrintLevelWizard()
		{
			RuningState runningState = RuningState.UserRequestedCalibration;
			if (ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_required_to_print"))
			{
				runningState = RuningState.InitialStartupCalibration;
			}
			ShowPrintLevelWizard(runningState);
		}

		public static void ShowPrintLevelWizard(RuningState runningState)
		{
			if (printLevelWizardWindow == null)
			{
				printLevelWizardWindow = (SystemWindow)(object)CreateAndShowWizard(runningState);
				((GuiWidget)printLevelWizardWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					printLevelWizardWindow = null;
					if (ActiveSliceSettings.Instance.GetValue<bool>("has_z_probe") && ActiveSliceSettings.Instance.GetValue<bool>("use_z_probe") && ActiveSliceSettings.Instance.GetValue<bool>("has_z_servo"))
					{
						double value = ActiveSliceSettings.Instance.GetValue<double>("z_servo_retracted_angle");
						PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow($"M280 P0 S{value}");
					}
				});
			}
			else
			{
				((GuiWidget)printLevelWizardWindow).BringToFront();
			}
		}

		private static LevelWizardBase CreateAndShowWizard(RuningState runningState)
		{
			ActiveSliceSettings.Instance.Helpers.DoPrintLeveling(doLeveling: false);
			PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
			printLevelingData.SampledPositions.Clear();
			ActiveSliceSettings.Instance.SetValue("baby_step_z_offset", "0");
			ApplicationController.Instance.ReloadAdvancedControlsPanel();
			LevelWizardBase levelWizardBase = printLevelingData.CurrentPrinterLevelingSystem switch
			{
				PrintLevelingData.LevelingSystem.Probe3Points => new LevelWizard3Point(runningState), 
				PrintLevelingData.LevelingSystem.Probe7PointRadial => new LevelWizard7PointRadial(runningState), 
				PrintLevelingData.LevelingSystem.Probe13PointRadial => new LevelWizard13PointRadial(runningState), 
				PrintLevelingData.LevelingSystem.Probe3x3Mesh => new LevelWizard3x3Mesh(runningState), 
				_ => throw new NotImplementedException(), 
			};
			((SystemWindow)levelWizardBase).ShowAsSystemWindow();
			return levelWizardBase;
		}
	}
}
