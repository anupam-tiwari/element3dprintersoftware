using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class LevelWizard3Point : LevelWizardBase
	{
		private LevelingStrings levelingStrings = new LevelingStrings();

		public LevelWizard3Point(RuningState runningState)
			: base(500, 370, 9)
		{
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			string arg = "Element".Localize();
			string arg2 = "Print Leveling Wizard".Localize();
			((SystemWindow)this).set_Title($"{arg} - {arg2}");
			List<ProbePosition> probePositions = new List<ProbePosition>(3)
			{
				new ProbePosition(),
				new ProbePosition(),
				new ProbePosition()
			};
			printLevelWizard = new WizardControl();
			((GuiWidget)this).AddChild((GuiWidget)(object)printLevelWizard, -1);
			if (runningState == RuningState.InitialStartupCalibration)
			{
				string instructionsText = StringHelper.FormatWith("{0}\n\n{1}", new object[2]
				{
					levelingStrings.requiredPageInstructions1,
					levelingStrings.requiredPageInstructions2
				});
				printLevelWizard.AddPage(new FirstPageInstructions(levelingStrings.initialPrinterSetupStepText, instructionsText));
			}
			printLevelWizard.AddPage(new FirstPageInstructions(levelingStrings.OverviewText, levelingStrings.WelcomeText(3, 3)));
			bool value = ActiveSliceSettings.Instance.GetValue<bool>("has_heated_bed");
			if (value)
			{
				string instructionsText2 = StringHelper.FormatWith("{0}\n\n{1}", new object[2]
				{
					levelingStrings.materialPageInstructions1,
					levelingStrings.materialPageInstructions2
				});
				printLevelWizard.AddPage(new SelectMaterialPage(levelingStrings.materialStepText, instructionsText2));
			}
			printLevelWizard.AddPage(new HomePrinterPage(levelingStrings.homingPageStepText, levelingStrings.homingPageInstructions));
			if (value)
			{
				printLevelWizard.AddPage(new WaitForTempPage(levelingStrings.waitingForTempPageStepText, levelingStrings.waitingForTempPageInstructions));
			}
			string text = "Position".Localize();
			string text2 = "Auto Calibrate".Localize();
			string text3 = "Low Precision".Localize();
			string text4 = "Medium Precision".Localize();
			string text5 = "High Precision".Localize();
			double value2 = ActiveSliceSettings.Instance.GetValue<double>("print_leveling_probe_start");
			for (int i = 0; i < 3; i++)
			{
				Vector2 printLevelPositionToSample = LevelWizardBase.GetPrintLevelPositionToSample(i);
				if (ActiveSliceSettings.Instance.Helpers.UseZProbe())
				{
					string text6 = $"{levelingStrings.stepTextBeg} {i + 1} {levelingStrings.stepTextEnd} {3}:";
					printLevelWizard.AddPage(new AutoProbeFeedback(printLevelWizard, new Vector3(printLevelPositionToSample, value2), $"{text6} {text} {i + 1} - {text2}", probePositions, i));
				}
				else
				{
					printLevelWizard.AddPage(new GetCoarseBedHeight(printLevelWizard, new Vector3(printLevelPositionToSample, value2), $"{levelingStrings.GetStepString(base.totalSteps)} {text} {i + 1} - {text3}", probePositions, i));
					printLevelWizard.AddPage(new GetFineBedHeight(printLevelWizard, $"{levelingStrings.GetStepString(base.totalSteps)} {text} {i + 1} - {text4}", probePositions, i));
					printLevelWizard.AddPage(new GetUltraFineBedHeight(printLevelWizard, $"{levelingStrings.GetStepString(base.totalSteps)} {text} {i + 1} - {text5}", probePositions, i));
				}
			}
			printLevelWizard.AddPage(new LastPagelInstructions(printLevelWizard, "Done".Localize(), levelingStrings.DoneInstructions, probePositions));
		}

		public static string ApplyLeveling(string lineBeingSent, Vector3 currentDestination, PrinterMachineInstruction.MovementTypes movementMode)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if ((ActiveSliceSettings.Instance?.GetValue<bool>("print_leveling_enabled") ?? false) && (lineBeingSent.StartsWith("G0 ") || lineBeingSent.StartsWith("G1 ")))
			{
				lineBeingSent = PrintLevelingPlane.Instance.ApplyLeveling(currentDestination, movementMode, lineBeingSent);
			}
			return lineBeingSent;
		}

		public static List<string> ProcessCommand(string lineBeingSent)
		{
			int num = lineBeingSent.IndexOf(';');
			if (num > 0)
			{
				lineBeingSent = lineBeingSent.Substring(0, num).Trim();
			}
			List<string> list = new List<string>();
			list.Add(lineBeingSent);
			if (lineBeingSent.StartsWith("G28") || lineBeingSent.StartsWith("G29"))
			{
				list.Add("M114");
			}
			return list;
		}
	}
}
