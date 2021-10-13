using MatterHackers.Agg;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public static class GCodeProcessing
	{
		private static MappedSetting[] replaceWithSettingsStrings = new MappedSetting[22]
		{
			new AsPercentOfReferenceOrDirect("first_layer_speed", "first_layer_speed", "infill_speed", 60.0),
			new AsPercentOfReferenceOrDirect("external_perimeter_speed", "external_perimeter_speed", "perimeter_speed", 60.0),
			new AsPercentOfReferenceOrDirect("raft_print_speed", "raft_print_speed", "infill_speed", 60.0),
			new MappedSetting("bed_remove_part_temperature", "bed_remove_part_temperature"),
			new MappedSetting("bridge_fan_speed", "bridge_fan_speed"),
			new MappedSetting("bridge_speed", "bridge_speed"),
			new MappedSetting("extruder_wipe_temperature", "extruder_wipe_temperature"),
			new MappedSetting("filament_diameter", "filament_diameter"),
			new MappedSetting("first_layer_bed_temperature", "bed_temperature"),
			new MappedSetting("first_layer_temperature", "temperature"),
			new MappedSetting("max_fan_speed", "max_fan_speed"),
			new MappedSetting("min_fan_speed", "min_fan_speed"),
			new MappedSetting("retract_length", "retract_length"),
			new MappedSetting("temperature", "temperature"),
			new MappedSetting("z_offset", "z_offset"),
			new MappedSetting("bed_temperature", "bed_temperature"),
			new ScaledSingleNumber("infill_speed", "infill_speed", 60.0),
			new ScaledSingleNumber("min_print_speed", "min_print_speed", 60.0),
			new ScaledSingleNumber("perimeter_speed", "perimeter_speed", 60.0),
			new ScaledSingleNumber("retract_speed", "retract_speed", 60.0),
			new ScaledSingleNumber("support_material_speed", "support_material_speed", 60.0),
			new ScaledSingleNumber("travel_speed", "travel_speed", 60.0)
		};

		public static string ReplaceMacroValues(string gcodeWithMacros)
		{
			MappedSetting[] array = replaceWithSettingsStrings;
			foreach (MappedSetting mappedSetting in array)
			{
				string oldValue = "{" + StringHelper.FormatWith("{0}", new object[1]
				{
					mappedSetting.CanonicalSettingsName
				}) + "}";
				gcodeWithMacros = gcodeWithMacros.Replace(oldValue, mappedSetting.Value);
				string oldValue2 = "[" + StringHelper.FormatWith("{0}", new object[1]
				{
					mappedSetting.CanonicalSettingsName
				}) + "]";
				gcodeWithMacros = gcodeWithMacros.Replace(oldValue2, mappedSetting.Value);
			}
			return gcodeWithMacros;
		}
	}
}
