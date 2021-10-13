using System.Collections.Generic;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public abstract class SliceEngineMapping
	{
		protected HashSet<string> applicationLevelSettings;

		public string Name
		{
			get;
		}

		public SliceEngineMapping(string engineName)
		{
			HashSet<string> obj = new HashSet<string>();
			obj.Add("bed_shape");
			obj.Add("bed_size");
			obj.Add("bed_temperature");
			obj.Add("build_height");
			obj.Add("cancel_gcode");
			obj.Add("connect_gcode");
			obj.Add("write_regex");
			obj.Add("read_regex");
			obj.Add("has_fan");
			obj.Add("has_hardware_leveling");
			obj.Add("has_heated_bed");
			obj.Add("has_power_control");
			obj.Add("has_sd_card_reader");
			obj.Add("printer_name");
			obj.Add("auto_connect");
			obj.Add("baud_rate");
			obj.Add("com_port");
			obj.Add("filament_cost");
			obj.Add("filament_density");
			obj.Add("filament_runout_sensor");
			obj.Add("leveling_manual_positions");
			obj.Add("z_probe_z_offset");
			obj.Add("use_z_probe");
			obj.Add("z_probe_samples");
			obj.Add("has_z_probe");
			obj.Add("has_z_servo");
			obj.Add("z_probe_xy_offset");
			obj.Add("z_servo_depolyed_angle");
			obj.Add("z_servo_retracted_angle");
			obj.Add("pause_gcode");
			obj.Add("print_leveling_probe_start");
			obj.Add("print_leveling_required_to_print");
			obj.Add("print_leveling_solution");
			obj.Add("recover_first_layer_speed");
			obj.Add("recover_is_enabled");
			obj.Add("recover_position_before_z_home");
			obj.Add("resume_gcode");
			obj.Add("temperature");
			obj.Add("z_homes_to_max");
			obj.Add("bed_remove_part_temperature");
			obj.Add("extruder_wipe_temperature");
			obj.Add("heat_extruder_before_homing");
			obj.Add("include_firmware_updater");
			obj.Add("layer_to_pause");
			obj.Add("validate_layer_height");
			obj.Add("show_reset_connection");
			obj.Add("make");
			obj.Add("model");
			obj.Add("enable_network_printing");
			obj.Add("enable_sailfish_communication");
			obj.Add("ip_address");
			obj.Add("ip_port");
			applicationLevelSettings = obj;
			base._002Ector();
			Name = engineName;
		}

		public abstract bool MapContains(string canonicalSettingsName);
	}
}
