using System;
using System.Collections.Generic;
using MatterHackers.MatterControl.GCodeVisualizer;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class LevelWizard3x3Mesh : LevelWizardMeshBase
	{
		public LevelWizard3x3Mesh(RuningState runningState)
			: base(runningState, 500, 370, 21, 3, 3)
		{
		}

		public static string ApplyLeveling(string lineBeingSent, Vector3 currentDestination, PrinterMachineInstruction.MovementTypes movementMode)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if ((ActiveSliceSettings.Instance?.GetValue<bool>("print_leveling_enabled") ?? false) && (lineBeingSent.StartsWith("G0 ") || lineBeingSent.StartsWith("G1 ")) && lineBeingSent.Length > 2 && lineBeingSent[2] == ' ')
			{
				PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
				return LevelWizardMeshBase.GetLevelingFunctions(3, 3, printLevelingData).DoApplyLeveling(lineBeingSent, currentDestination, movementMode);
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

		public override Vector2 GetPrintLevelPositionToSample(int index)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			List<Vector2> manualPositions = LevelWizardBase.GetManualPositions(ActiveSliceSettings.Instance.GetValue("leveling_manual_positions"), 9);
			if (manualPositions != null)
			{
				return manualPositions[index];
			}
			Vector2 val = ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size");
			Vector2 value = ActiveSliceSettings.Instance.GetValue<Vector2>("print_center");
			if (ActiveSliceSettings.Instance.GetValue<BedShape>("bed_shape") == BedShape.Circular)
			{
				val *= 1.0 / Math.Sqrt(2.0);
			}
			int num = index % 3;
			int num2 = index / 3;
			Vector2 result = default(Vector2);
			switch (num)
			{
			case 0:
				result.x = value.x - val.x / 2.0 * 0.8;
				break;
			case 1:
				result.x = value.x;
				break;
			case 2:
				result.x = value.x + val.x / 2.0 * 0.8;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
			switch (num2)
			{
			case 0:
				result.y = value.y - val.y / 2.0 * 0.8;
				break;
			case 1:
				result.y = value.y;
				break;
			case 2:
				result.y = value.y + val.y / 2.0 * 0.8;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
			return result;
		}
	}
}
