using System.Collections.Generic;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class LevelWizard7PointRadial : LevelWizardRadialBase
	{
		private static readonly int numberOfRadialSamples = 6;

		public LevelWizard7PointRadial(RuningState runningState)
			: base(runningState, 500, 370, 21, numberOfRadialSamples)
		{
		}

		public static string ApplyLeveling(string lineBeingSent, Vector3 currentDestination, PrinterMachineInstruction.MovementTypes movementMode)
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			if ((ActiveSliceSettings.Instance?.GetValue<bool>("print_leveling_enabled") ?? false) && (lineBeingSent.StartsWith("G0 ") || lineBeingSent.StartsWith("G1 ")) && lineBeingSent.Length > 2 && lineBeingSent[2] == ' ')
			{
				PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
				return LevelWizardRadialBase.GetLevelingFunctions(numberOfRadialSamples, printLevelingData, ActiveSliceSettings.Instance.GetValue<Vector2>("print_center")).DoApplyLeveling(lineBeingSent, currentDestination, movementMode);
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

		public override Vector2 GetPrintLevelPositionToSample(int index, double radius)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
			return LevelWizardRadialBase.GetLevelingFunctions(numberOfRadialSamples, printLevelingData, ActiveSliceSettings.Instance.GetValue<Vector2>("print_center")).GetPrintLevelPositionToSample(index, radius);
		}
	}
}
