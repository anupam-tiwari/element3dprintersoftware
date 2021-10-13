using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public abstract class LevelWizardMeshBase : LevelWizardBase
	{
		private static MeshLevlingFunctions currentLevelingFunctions;

		private LevelingStrings levelingStrings = new LevelingStrings();

		public LevelWizardMeshBase(RuningState runningState, int width, int height, int totalSteps, int gridWidth, int gridHeight)
			: base(width, height, totalSteps)
		{
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			string arg = "MatterControl";
			string arg2 = "Print Leveling Wizard".Localize();
			((SystemWindow)this).set_Title($"{arg} - {arg2}");
			int num = gridWidth * gridHeight;
			List<ProbePosition> list = new List<ProbePosition>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(new ProbePosition());
			}
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
			printLevelWizard.AddPage(new FirstPageInstructions(levelingStrings.OverviewText, levelingStrings.WelcomeText(num, 5)));
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
			_ = Math.Min(ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size").x, ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size").y) / 2.0;
			double value2 = ActiveSliceSettings.Instance.GetValue<double>("print_leveling_probe_start");
			for (int j = 0; j < num; j++)
			{
				Vector2 printLevelPositionToSample = GetPrintLevelPositionToSample(j);
				if (ActiveSliceSettings.Instance.Helpers.UseZProbe())
				{
					string text6 = $"{levelingStrings.stepTextBeg} {j + 1} {levelingStrings.stepTextEnd} {num}:";
					printLevelWizard.AddPage(new AutoProbeFeedback(printLevelWizard, new Vector3(printLevelPositionToSample, value2), $"{text6} {text} {j + 1} - {text2}", list, j));
				}
				else
				{
					printLevelWizard.AddPage(new GetCoarseBedHeight(printLevelWizard, new Vector3(printLevelPositionToSample, value2), $"{levelingStrings.GetStepString(totalSteps)} {text} {j + 1} - {text3}", list, j));
					printLevelWizard.AddPage(new GetFineBedHeight(printLevelWizard, $"{levelingStrings.GetStepString(totalSteps)} {text} {j + 1} - {text4}", list, j));
					printLevelWizard.AddPage(new GetUltraFineBedHeight(printLevelWizard, $"{levelingStrings.GetStepString(totalSteps)} {text} {j + 1} - {text5}", list, j));
				}
			}
			printLevelWizard.AddPage(new LastPagelInstructions(printLevelWizard, "Done".Localize(), levelingStrings.DoneInstructions, list));
		}

		public static MeshLevlingFunctions GetLevelingFunctions(int gridWidth, int gridHeight, PrintLevelingData levelingData)
		{
			if (currentLevelingFunctions == null || !levelingData.SamplesAreSame(currentLevelingFunctions.SampledPositions))
			{
				if (currentLevelingFunctions != null)
				{
					currentLevelingFunctions.Dispose();
				}
				currentLevelingFunctions = new MeshLevlingFunctions(gridWidth, gridHeight, levelingData);
			}
			return currentLevelingFunctions;
		}

		public new abstract Vector2 GetPrintLevelPositionToSample(int index);
	}
}
