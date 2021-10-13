using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MatterHackers.Agg;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class EngineMappingsMatterSlice : SliceEngineMapping
	{
		public class ExtruderOffsets : MappedSetting
		{
			public override string Value
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder("[");
					string[] array = base.Value.Split(new char[1]
					{
						','
					});
					bool flag = true;
					int i = 0;
					string[] array2 = array;
					foreach (string obj in array2)
					{
						if (!flag)
						{
							stringBuilder.Append(",");
						}
						string[] array3 = obj.Split(new char[1]
						{
							'x'
						});
						stringBuilder.Append(StringHelper.FormatWith("[{0},{1}]", new object[2]
						{
							double.Parse(array3[0]),
							double.Parse(array3[1])
						}));
						flag = false;
						i++;
					}
					for (; i < 16; i++)
					{
						stringBuilder.Append(",[0,0]");
					}
					stringBuilder.Append("]");
					return stringBuilder.ToString();
				}
			}

			public ExtruderOffsets(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class FanTranslator : MappedSetting
		{
			public override string Value => (int.Parse(base.Value) + 1).ToString();

			public FanTranslator(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class GCodeForSlicer : InjectGCodeCommands
		{
			public override string Value => GCodeProcessing.ReplaceMacroValues(base.Value.Replace("\n", "\\n"));

			public GCodeForSlicer(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class InfillTranslator : MappedSetting
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

		public class MapPositionToPlaceObjectCenter : MappedSetting
		{
			public override string Value
			{
				get
				{
					//IL_000a: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					Vector2 value = ActiveSliceSettings.Instance.GetValue<Vector2>("print_center");
					return StringHelper.FormatWith("[{0},{1}]", new object[2]
					{
						value.x,
						value.y
					});
				}
			}

			public MapPositionToPlaceObjectCenter(string canonicalSettingsName, string exportedName)
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
					return (num2 / num3).ToString();
				}
			}

			public SkirtLengthMapping(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class SupportExtrusionWidth : MappedSetting
		{
			public override string Value
			{
				get
				{
					double value = ActiveSliceSettings.Instance.GetValue<double>("nozzle_diameter");
					string value2 = base.Value;
					if (value2 == "0")
					{
						return "100";
					}
					if (value2.Contains("%"))
					{
						return value2.Replace("%", "");
					}
					if (!double.TryParse(value2, out var result))
					{
						result = value;
					}
					return (result / value * 100.0).ToString();
				}
			}

			public SupportExtrusionWidth(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public class ValuePlusConstant : MappedSetting
		{
			private double constant;

			public override string Value => (ParseDouble(base.Value) + constant).ToString();

			public ValuePlusConstant(string canonicalSettingsName, string exportedName, double constant)
				: base(canonicalSettingsName, exportedName)
			{
				this.constant = constant;
			}
		}

		public class MappedTools : MappedSetting
		{
			public override string Value
			{
				get
				{
					bool flag = true;
					StringBuilder stringBuilder = new StringBuilder();
					foreach (ToolSettings tool in ActiveSliceSettings.Instance.Tools)
					{
						if (tool.toolType == TOOL_TYPE.NONE)
						{
							continue;
						}
						if (!flag)
						{
							stringBuilder.Append("^");
						}
						flag = false;
						stringBuilder.Append(tool.toolType.ToString().Substring(0, 2));
						stringBuilder.Append(":");
						bool flag2 = true;
						PropertyInfo[] properties = tool.GetType().GetProperties();
						foreach (PropertyInfo propertyInfo in properties)
						{
							bool flag3 = false;
							object value = propertyInfo.GetValue(tool);
							switch (propertyInfo.PropertyType.Name)
							{
							case "Int32":
							case "Double":
								if (Convert.ToDouble(value) != 0.0)
								{
									flag3 = true;
								}
								break;
							case "String":
								if (!string.IsNullOrEmpty(Convert.ToString(value)))
								{
									flag3 = true;
								}
								break;
							}
							if (flag3)
							{
								if (!flag2)
								{
									stringBuilder.Append(",");
								}
								flag2 = false;
								stringBuilder.Append(propertyInfo.Name.ToLower());
								stringBuilder.Append("=");
								stringBuilder.Append(value);
							}
						}
					}
					return stringBuilder.ToString();
				}
			}

			public MappedTools(string canonicalSettingsName, string exportedName)
				: base(canonicalSettingsName, exportedName)
			{
			}
		}

		public static readonly EngineMappingsMatterSlice Instance = new EngineMappingsMatterSlice();

		private HashSet<string> matterSliceSettingNames;

		private MappedSetting[] mappedSettings;

		private EngineMappingsMatterSlice()
			: base("MatterSlice")
		{
			mappedSettings = new MappedSetting[86]
			{
				new AsCountOrDistance("bottom_solid_layers", "numberOfBottomLayers", "layer_height"),
				new AsCountOrDistance("perimeters", "numberOfPerimeters", "nozzle_diameter"),
				new AsCountOrDistance("raft_extra_distance_around_part", "raftExtraDistanceAroundPart", "nozzle_diameter"),
				new AsCountOrDistance("skirts", "numberOfSkirtLoops", "nozzle_diameter"),
				new AsCountOrDistance("support_material_interface_layers", "supportInterfaceLayers", "layer_height"),
				new AsCountOrDistance("top_solid_layers", "numberOfTopLayers", "layer_height"),
				new AsCountOrDistance("brims", "numberOfBrimLoops", "nozzle_diameter"),
				new AsPercentOfReferenceOrDirect("external_perimeter_extrusion_width", "outsidePerimeterExtrusionWidth", "nozzle_diameter"),
				new AsPercentOfReferenceOrDirect("external_perimeter_speed", "outsidePerimeterSpeed", "perimeter_speed"),
				new AsPercentOfReferenceOrDirect("first_layer_speed", "firstLayerSpeed", "infill_speed"),
				new AsPercentOfReferenceOrDirect("raft_print_speed", "raftPrintSpeed", "infill_speed"),
				new AsPercentOfReferenceOrDirect("top_solid_infill_speed", "topInfillSpeed", "infill_speed"),
				new AsPercentOfReferenceOrDirect("first_layer_extrusion_width", "firstLayerExtrusionWidth", "nozzle_diameter"),
				new AsPercentOfReferenceOrDirect("first_layer_height", "firstLayerThickness", "layer_height"),
				new GCodeForSlicer("before_toolchange_gcode", "beforeToolchangeCode"),
				new GCodeForSlicer("end_gcode", "endCode"),
				new GCodeForSlicer("toolchange_gcode", "toolChangeCode"),
				new MapFirstValue("retract_before_travel", "minimumTravelToCauseRetraction"),
				new MapFirstValue("retract_length", "retractionOnTravel"),
				new MapFirstValue("retract_lift", "retractionZHop"),
				new MapFirstValue("retract_restart_extra", "unretractExtraExtrusion"),
				new MapFirstValue("retract_restart_extra_time_to_apply", "retractRestartExtraTimeToApply"),
				new MapFirstValue("retract_speed", "retractionSpeed"),
				new MappedSetting("bridge_fan_speed", "bridgeFanSpeedPercent"),
				new MappedSetting("bridge_speed", "bridgeSpeed"),
				new MappedSetting("disable_fan_first_layers", "firstLayerToAllowFan"),
				new MappedSetting("extrusion_multiplier", "extrusionMultiplier"),
				new MappedSetting("fill_angle", "infillStartingAngle"),
				new MappedSetting("infill_overlap_perimeter", "infillExtendIntoPerimeter"),
				new MappedSetting("infill_speed", "infillSpeed"),
				new MappedSetting("infill_type", "infillType"),
				new GCodeForSlicer("layer_gcode", "layerChangeCode"),
				new MappedSetting("max_fan_speed", "fanSpeedMaxPercent"),
				new MappedSetting("min_extrusion_before_retract", "minimumExtrusionBeforeRetraction"),
				new MappedSetting("min_fan_speed", "fanSpeedMinPercent"),
				new MappedSetting("min_print_speed", "minimumPrintingSpeed"),
				new MappedSetting("perimeter_speed", "insidePerimetersSpeed"),
				new MappedSetting("raft_air_gap", "raftAirGap"),
				new MappedSetting("raft_fan_speed_percent", "raftFanSpeedPercent"),
				new MappedSetting("retract_length_tool_change", "retractionOnExtruderSwitch"),
				new MappedSetting("retract_restart_extra_toolchange", "unretractExtraOnExtruderSwitch"),
				new MappedSetting("skirt_distance", "skirtDistanceFromObject"),
				new MappedSetting("slowdown_below_layer_time", "minimumLayerTimeSeconds"),
				new MappedSetting("support_air_gap", "supportAirGap"),
				new MappedSetting("support_material_infill_angle", "supportInfillStartingAngle"),
				new MappedSetting("support_material_percent", "supportPercent"),
				new MappedSetting("support_material_spacing", "supportLineSpacing"),
				new MappedSetting("support_material_speed", "supportMaterialSpeed"),
				new MappedSetting("support_material_xy_distance", "supportXYDistanceFromObject"),
				new MappedSetting("support_type", "supportType"),
				new MappedSetting("travel_speed", "travelSpeed"),
				new MappedSetting("wipe_shield_distance", "wipeShieldDistanceFromObject"),
				new MappedSetting("wipe_tower_size", "wipeTowerSize"),
				new MappedSetting("z_offset", "zOffset"),
				new MappedSetting("bottom_clip_amount", "bottomClipAmount"),
				new MappedSetting("filament_diameter", "filamentDiameter"),
				new MappedSetting("layer_height", "layerThickness"),
				new MappedToBoolString("avoid_crossing_perimeters", "avoidCrossingPerimeters"),
				new MappedToBoolString("create_raft", "enableRaft"),
				new MappedToBoolString("external_perimeters_first", "outsidePerimetersFirst"),
				new MappedToBoolString("output_only_first_layer", "outputOnlyFirstLayer"),
				new MappedToBoolString("retract_when_changing_islands", "retractWhenChangingIslands"),
				new MappedToBoolString("support_material", "generateSupport"),
				new MappedToBoolString("support_material_create_internal_support", "generateInternalSupport"),
				new MappedToBoolString("support_material_create_perimeter", "generateSupportPerimeter"),
				new MappedToBoolString("wipe", "wipeAfterRetraction"),
				new MappedToBoolString("center_part_on_bed", "centerObjectInXy"),
				new MappedToBoolString("expand_thin_walls", "expandThinWalls"),
				new MappedToBoolString("merge_overlapping_lines", "MergeOverlappingLines"),
				new MappedToBoolString("fill_thin_gaps", "fillThinGaps"),
				new MappedToBoolString("spiral_vase", "continuousSpiralOuterPerimeter"),
				new MapPositionToPlaceObjectCenter("print_center", "positionToPlaceObjectCenter"),
				new MapStartGCode("start_gcode", "startCode", escapeNewlineCharacters: true),
				new ScaledSingleNumber("fill_density", "infillPercent", 100.0),
				new ScaledSingleNumber("perimeter_start_end_overlap", "perimeterStartEndOverlapRatio", 0.01),
				new SkirtLengthMapping("min_skirt_length", "skirtMinLength"),
				new SupportExtrusionWidth("support_material_extrusion_width", "supportExtrusionPercent"),
				new ValuePlusConstant("raft_extruder", "raftExtruder", -1.0),
				new ValuePlusConstant("support_material_extruder", "supportExtruder", -1.0),
				new ValuePlusConstant("support_material_interface_extruder", "supportInterfaceExtruder", -1.0),
				new VisibleButNotMappedToEngine("extruder_count"),
				new VisibleButNotMappedToEngine("g0"),
				new VisibleButNotMappedToEngine("solid_shell"),
				new MappedTools("extruder_tools", "NewTools"),
				new MappedToBoolString("tool_specific_infill", "ToolSpecificInfill"),
				new VisibleButNotMappedToEngine("export_one_at_a_time")
			};
			matterSliceSettingNames = new HashSet<string>(Enumerable.Select<MappedSetting, string>((IEnumerable<MappedSetting>)mappedSettings, (Func<MappedSetting, string>)((MappedSetting m) => m.CanonicalSettingsName)));
		}

		public static void WriteSliceSettingsFile(string outputFilename)
		{
			using StreamWriter streamWriter = new StreamWriter(outputFilename);
			MappedSetting[] array = Instance.mappedSettings;
			foreach (MappedSetting mappedSetting in array)
			{
				if (mappedSetting.Value != null)
				{
					streamWriter.WriteLine(StringHelper.FormatWith("{0} = {1}", new object[2]
					{
						mappedSetting.ExportedName,
						mappedSetting.Value
					}));
				}
			}
		}

		public override bool MapContains(string canonicalSettingsName)
		{
			if (canonicalSettingsName == "extruder_wipe_temperature" || canonicalSettingsName == "temperature")
			{
				return false;
			}
			if (!matterSliceSettingNames.Contains(canonicalSettingsName))
			{
				return applicationLevelSettings.Contains(canonicalSettingsName);
			}
			return true;
		}
	}
}
