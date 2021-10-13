using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MatterHackers.Agg;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class Slic3rEngineMappings : SliceEngineMapping
	{
		public static readonly Slic3rEngineMappings Instance = new Slic3rEngineMappings();

		private List<MappedSetting> mappedSettings = new List<MappedSetting>();

		private HashSet<string> slic3rSliceSettingNames;

		private Slic3rEngineMappings()
			: base("Slic3r")
		{
			foreach (string item in Enumerable.Where<string>((IEnumerable<string>)PrinterSettings.KnownSettings, (Func<string, bool>)((string k) => !k.StartsWith("MatterControl."))))
			{
				mappedSettings.Add(new MappedSetting(item, item));
			}
			string[] array = new string[36]
			{
				"cool_extruder_lift",
				"support_material_create_internal_support",
				"support_material_create_perimeter",
				"min_extrusion_before_retract",
				"support_material_xy_distance",
				"support_material_z_distance",
				"print_center",
				"expand_thin_walls",
				"merge_overlapping_lines",
				"fill_thin_gaps",
				"infill_overlap_perimeter",
				"support_type",
				"infill_type",
				"create_raft",
				"z_gap",
				"bottom_clip_amount",
				"gcode_output_type",
				"raft_extra_distance_around_part",
				"output_only_first_layer",
				"raft_air_gap",
				"support_air_gap",
				"repair_outlines_extensive_stitching",
				"repair_outlines_keep_open",
				"complete_objects",
				"output_filename_format",
				"support_material_percent",
				"post_process",
				"extruder_clearance_height",
				"extruder_clearance_radius",
				"wipe_shield_distance",
				"heat_extruder_before_homing",
				"extruders_share_temperature",
				"solid_shell",
				"retractWhenChangingIslands",
				"perimeter_start_end_overlap",
				"bed_shape"
			};
			foreach (string b in array)
			{
				for (int num = mappedSettings.Count - 1; num >= 0; num--)
				{
					if (mappedSettings[num].CanonicalSettingsName == b)
					{
						mappedSettings.RemoveAt(num);
					}
				}
			}
			mappedSettings.Add(new Slice3rBedShape("bed_shape"));
			slic3rSliceSettingNames = new HashSet<string>(Enumerable.Select<MappedSetting, string>((IEnumerable<MappedSetting>)mappedSettings, (Func<MappedSetting, string>)((MappedSetting m) => m.CanonicalSettingsName)));
		}

		public static void WriteSliceSettingsFile(string outputFilename)
		{
			using StreamWriter streamWriter = new StreamWriter(outputFilename);
			foreach (MappedSetting mappedSetting in Instance.mappedSettings)
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
			if (!slic3rSliceSettingNames.Contains(canonicalSettingsName))
			{
				return applicationLevelSettings.Contains(canonicalSettingsName);
			}
			return true;
		}
	}
}
