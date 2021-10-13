using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class EngineMappingCura : SliceEngineMapping
	{
		public class FanTranslator : MappedSetting
		{
			public override string Value => (int.Parse(base.Value) + 1).ToString();

			public FanTranslator(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class SupportTypeMapping : MappedSetting
		{
			public override string Value
			{
				get
				{
					string value = base.Value;
					if (value == "LINES")
					{
						return "1";
					}
					return "0";
				}
			}

			public SupportTypeMapping(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class SupportMatterial : MappedSetting
		{
			public override string Value
			{
				get
				{
					if (base.Value == "0")
					{
						return "-1";
					}
					return (90.0 - ParseDoubleFromRawValue("support_material_threshold")).ToString();
				}
			}

			public SupportMatterial(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class InfillTranslator : ScaledSingleNumber
		{
			public override string Value
			{
				get
				{
					double num = ParseDouble(base.Value);
					double num2 = ParseDoubleFromRawValue("nozzle_diameter");
					double num3 = 1000.0;
					if (num > 0.01)
					{
						num3 = num2 / num;
					}
					return ((int)(num3 * 1000.0)).ToString();
				}
			}

			public InfillTranslator(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class PrintCenterX : MappedSetting
		{
			public override string Value => (ActiveSliceSettings.Instance.GetValue<Vector2>("print_center").x * 1000.0).ToString();

			public PrintCenterX(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class PrintCenterY : MappedSetting
		{
			public override string Value => (ActiveSliceSettings.Instance.GetValue<Vector2>("print_center").y * 1000.0).ToString();

			public PrintCenterY(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class MapEndGCode : InjectGCodeCommands
		{
			public override string Value
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(GCodeProcessing.ReplaceMacroValues(base.Value));
					stringBuilder.Append("\n; filament used = filament_used_replace_mm (filament_used_replace_cm3)");
					return stringBuilder.ToString();
				}
			}

			public MapEndGCode(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class SkirtLengthMapping : MappedSetting
		{
			public override string Value
			{
				get
				{
					double num = ParseDouble(base.Value);
					double num2 = ActiveSliceSettings.Instance.GetValue<double>("filament_diameter") * 6.2831854820251465 * num;
					double num3 = ActiveSliceSettings.Instance.GetValue<double>("first_layer_height") * ActiveSliceSettings.Instance.GetValue<double>("nozzle_diameter");
					return (num2 / num3 * 1000.0).ToString();
				}
			}

			public SkirtLengthMapping(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public static readonly EngineMappingCura Instance = new EngineMappingCura();

		private HashSet<string> curaSettingNames;

		private MappedSetting[] curaSettings;

		private EngineMappingCura()
			: base("Cura")
		{
			curaSettings = new MappedSetting[40]
			{
				new ScaledSingleNumber("layer_height", "layerThickness", 1000.0),
				new AsPercentOfReferenceOrDirect("first_layer_height", "initialLayerThickness", "layer_height", 1000.0),
				new ScaledSingleNumber("filament_diameter", "filamentDiameter", 1000.0),
				new ScaledSingleNumber("nozzle_diameter", "extrusionWidth", 1000.0),
				new AsCountOrDistance("perimeters", "insetCount", "nozzle_diameter"),
				new AsCountOrDistance("bottom_solid_layers", "downSkinCount", "layer_height"),
				new AsCountOrDistance("top_solid_layers", "upSkinCount", "layer_height"),
				new ScaledSingleNumber("skirt_distance", "skirtDistance", 1000.0),
				new AsCountOrDistance("skirts", "skirtLineCount", "nozzle_diameter"),
				new SkirtLengthMapping("min_skirt_length", "skirtMinLength"),
				new MappedSetting("infill_speed", "printSpeed"),
				new MappedSetting("infill_speed", "infillSpeed"),
				new MappedSetting("travel_speed", "moveSpeed"),
				new AsPercentOfReferenceOrDirect("first_layer_speed", "initialLayerSpeed", "infill_speed"),
				new MappedSetting("perimeter_speed", "insetXSpeed"),
				new AsPercentOfReferenceOrDirect("external_perimeter_speed", "inset0Speed", "perimeter_speed"),
				new ScaledSingleNumber("bottom_clip_amount", "objectSink", 1000.0),
				new MappedSetting("max_fan_speed", "fanSpeedMin"),
				new MappedSetting("min_fan_speed", "fanSpeedMax"),
				new FanTranslator("disable_fan_first_layers", "fanFullOnLayerNr"),
				new MappedSetting("cool_extruder_lift", "coolHeadLift"),
				new ScaledSingleNumber("retract_length", "retractionAmount", 1000.0),
				new MapFirstValue("retract_speed", "retractionSpeed"),
				new ScaledSingleNumber("retract_before_travel", "retractionMinimalDistance", 1000.0),
				new ScaledSingleNumber("min_extrusion_before_retract", "minimalExtrusionBeforeRetraction", 1000.0),
				new ScaledSingleNumber("retract_lift", "retractionZHop", 1000.0),
				new MappedSetting("spiral_vase", "spiralizeMode"),
				new PrintCenterX("print_center", "posx"),
				new PrintCenterY("print_center", "posy"),
				new ScaledSingleNumber("support_material_spacing", "supportLineDistance", 1000.0),
				new SupportMatterial("support_material", "supportAngle"),
				new VisibleButNotMappedToEngine("support_material_threshold"),
				new MappedSetting("support_material_create_internal_support", "supportEverywhere"),
				new ScaledSingleNumber("support_material_xy_distance", "supportXYDistance", 1000.0),
				new ScaledSingleNumber("support_material_z_distance", "supportZDistance", 1000.0),
				new SupportTypeMapping("support_type", "supportType"),
				new MappedSetting("slowdown_below_layer_time", "minimalLayerTime"),
				new InfillTranslator("fill_density", "sparseInfillLineDistance"),
				new MapStartGCode("start_gcode", "startCode", escapeNewlineCharacters: false),
				new MapEndGCode("end_gcode", "endCode")
			};
			curaSettingNames = new HashSet<string>(Enumerable.Select<MappedSetting, string>((IEnumerable<MappedSetting>)curaSettings, (Func<MappedSetting, string>)((MappedSetting m) => m.CanonicalSettingsName)));
		}

		public override bool MapContains(string canonicalSettingsName)
		{
			if (!curaSettingNames.Contains(canonicalSettingsName))
			{
				return applicationLevelSettings.Contains(canonicalSettingsName);
			}
			return true;
		}

		public static string GetCuraCommandLineSettings()
		{
			StringBuilder stringBuilder = new StringBuilder();
			MappedSetting[] array = Instance.curaSettings;
			foreach (MappedSetting mappedSetting in array)
			{
				if (!string.IsNullOrEmpty(mappedSetting.Value))
				{
					stringBuilder.AppendFormat("-s {0}=\"{1}\" ", mappedSetting.ExportedName, mappedSetting.Value);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
