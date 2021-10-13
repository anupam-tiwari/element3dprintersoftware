using MatterHackers.Agg;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class LevelingStrings
	{
		public string homingPageStepText = "Homing The Printer".Localize();

		public string waitingForTempPageStepText = "Waiting For Bed To Heat".Localize();

		public string initialPrinterSetupStepText = "Initial Printer Setup".Localize();

		public string materialStepText = "Select Material".Localize();

		public string requiredPageInstructions1 = "Congratulations on connecting to your new printer. Before starting your first print we need to run a simple calibration procedure.";

		public string requiredPageInstructions2 = "The next few screens will walk your through the print leveling wizard.";

		public string materialPageInstructions1 = "The temperature of the bed can have a significant effect on the quality of leveling.";

		public string materialPageInstructions2 = "Please select the material you will be printing, so we can adjust the temperature before calibrating.";

		public string stepTextBeg = "Step".Localize();

		public string stepTextEnd = "of".Localize();

		private string doneLine1 = "Congratulations!";

		private string doneLine1b = "Auto Print Leveling is now configured and enabled.".Localize();

		private string doneLine2 = "Remove the paper".Localize();

		private string doneLine3 = "To re-calibrate the printer, or to turn off Auto Print Leveling, the print leveling controls can be found under 'Options'->'Calibration'.";

		private string doneLine3b = "Click 'Done' to close this window.".Localize();

		private string homingLine1 = "The printer should now be 'homing'. Once it is finished homing we will heat the bed.";

		private string homingLine1b = "To complete the next few steps you will need".Localize();

		private string homingLine2 = "A standard sheet of paper".Localize();

		private string homingLine3 = "We will use this paper to measure the distance between the extruder and the bed.";

		private string homingLine3b = "Click 'Next' to continue.".Localize();

		private int stepNumber = 1;

		private string welcomeLine1 = "Welcome to the print leveling wizard. Here is a quick overview on what we are going to do.".Localize();

		private string selectMaterial = "Select the material you are printing".Localize();

		private string homeThePrinter = "Home the printer".Localize();

		private string heatTheBed = "Heat the bed".Localize();

		private string sampelAtPoints = "Sample the bed at {0} points".Localize();

		private string turnOnLeveling = "Turn auto leveling on".Localize();

		private string timeToDone = "We should be done in less than {0} minutes.".Localize();

		private string cleanExtruder = "Note: Be sure the tip of the extruder is clean and the bed is clear.".Localize();

		private string clickNext = "Click 'Next' to continue.".Localize();

		public string DoneInstructions
		{
			get
			{
				if (ActiveSliceSettings.Instance.Helpers.UseZProbe())
				{
					return StringHelper.FormatWith("{0}{1}\n\n{2}{3}", new object[4]
					{
						doneLine1,
						doneLine1b,
						doneLine3,
						doneLine3b
					});
				}
				return StringHelper.FormatWith("{0}{1}\n\n\t• {2}\n\n{3}{4}", new object[5]
				{
					doneLine1,
					doneLine1b,
					doneLine2,
					doneLine3,
					doneLine3b
				});
			}
		}

		public string waitingForTempPageInstructions => "Waiting for the bed to heat up.\nThis will improve the accuracy of print leveling.\n\nClick 'Next' when the bed reaches temp.";

		public string homingPageInstructions
		{
			get
			{
				if (ActiveSliceSettings.Instance.Helpers.UseZProbe())
				{
					return StringHelper.FormatWith("{0}\n\n{1}", new object[2]
					{
						homingLine1,
						homingLine3b
					});
				}
				return StringHelper.FormatWith("{0}\n\n{1}:\n\n\t• {2}\n\n{3}\n\n{4}", new object[5]
				{
					homingLine1,
					homingLine1b,
					homingLine2,
					homingLine3,
					homingLine3b
				});
			}
		}

		public string OverviewText
		{
			get;
		} = "Print Leveling Overview".Localize();


		public string GetStepString(int totalSteps)
		{
			return StringHelper.FormatWith("{0} {1} {2} {3}:", new object[4]
			{
				stepTextBeg,
				stepNumber++,
				stepTextEnd,
				totalSteps
			});
		}

		public string WelcomeText(int numberOfSteps, int numberOfMinutes)
		{
			if (ActiveSliceSettings.Instance.Helpers.UseZProbe())
			{
				numberOfMinutes = 2;
			}
			if (ActiveSliceSettings.Instance.GetValue<bool>("has_heated_bed"))
			{
				return StringHelper.FormatWith("{0}\n\n\t• {1}\n\t• {2}\n\t• {3}\n\t• {4}\n\t• {5}\n\n{6}\n\n{7}\n\n{8}", new object[9]
				{
					welcomeLine1,
					selectMaterial,
					homeThePrinter,
					heatTheBed,
					WelcomeLine5(numberOfSteps),
					turnOnLeveling,
					WelcomeLine7(numberOfMinutes),
					cleanExtruder,
					clickNext
				});
			}
			return StringHelper.FormatWith("{0}\n\n\t• {1}\n\t• {2}\n\t• {3}\n\n{4}\n\n{5}\n\n{6}", new object[7]
			{
				welcomeLine1,
				homeThePrinter,
				WelcomeLine5(numberOfSteps),
				turnOnLeveling,
				WelcomeLine7(numberOfMinutes),
				cleanExtruder,
				clickNext
			});
		}

		private string WelcomeLine5(int numberOfPoints)
		{
			return StringHelper.FormatWith(sampelAtPoints, new object[1]
			{
				numberOfPoints
			});
		}

		private string WelcomeLine7(int numberOfMinutes)
		{
			return StringHelper.FormatWith(timeToDone, new object[1]
			{
				numberOfMinutes
			});
		}
	}
}
