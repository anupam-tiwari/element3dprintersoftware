using System.Collections.Generic;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class LevelWizard13PointRadial : LevelWizardRadialBase
	{
		private static readonly int numberOfRadialSamples = 12;

		public LevelWizard13PointRadial(RuningState runningState)
			: base(runningState, 500, 370, (numberOfRadialSamples + 1) * 3, numberOfRadialSamples)
		{
		}

		public static string ApplyLeveling(string lineBeingSent, Vector3 currentDestination, PrinterMachineInstruction.MovementTypes movementMode)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			PrinterSettings instance = ActiveSliceSettings.Instance;
			if (instance != null && instance.GetValue<bool>("print_leveling_enabled") && (lineBeingSent.StartsWith("G0 ") || lineBeingSent.StartsWith("G1 ")) && lineBeingSent.Length > 2 && lineBeingSent[2] == ' ')
			{
				return LevelWizardRadialBase.GetLevelingFunctions(numberOfRadialSamples, instance.Helpers.GetPrintLevelingData(), ActiveSliceSettings.Instance.GetValue<Vector2>("print_center")).DoApplyLeveling(lineBeingSent, currentDestination, movementMode);
			}
			return lineBeingSent;
		}

		public override Vector2 GetPrintLevelPositionToSample(int index, double radius)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
			return LevelWizardRadialBase.GetLevelingFunctions(numberOfRadialSamples, printLevelingData, ActiveSliceSettings.Instance.GetValue<Vector2>("print_center")).GetPrintLevelPositionToSample(index, radius);
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
