using System;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class Slice3rBedShape : MappedSetting
	{
		public override string Value
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0185: Unknown result type (might be due to invalid IL or missing references)
				//IL_019b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
				Vector2 value = ActiveSliceSettings.Instance.GetValue<Vector2>("print_center");
				Vector2 value2 = ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size");
				BedShape value3 = ActiveSliceSettings.Instance.GetValue<BedShape>("bed_shape");
				if (value3 != 0 && value3 == BedShape.Circular)
				{
					int num = 10;
					double num2 = 6.2831854820251465 / (double)num;
					string text = "";
					bool flag = true;
					for (int i = 0; i < num; i++)
					{
						if (!flag)
						{
							text += ",";
						}
						double num3 = Math.Cos(num2 * (double)i);
						double num4 = Math.Sin(num2 * (double)i);
						text += $"{value.x + num3 * value2.x / 2.0:0.####}x{value.y + num4 * value2.y / 2.0:0.####}";
						flag = false;
					}
					return text;
				}
				return string.Concat(string.Concat($"{value.x - value2.x / 2.0}x{value.y - value2.y / 2.0}" + $",{value.x + value2.x / 2.0}x{value.y - value2.y / 2.0}", $",{value.x + value2.x / 2.0}x{value.y + value2.y / 2.0}"), $",{value.x - value2.x / 2.0}x{value.y + value2.y / 2.0}");
			}
		}

		public Slice3rBedShape(string canonicalSettingsName)
			: base(canonicalSettingsName, canonicalSettingsName)
		{
		}
	}
}
