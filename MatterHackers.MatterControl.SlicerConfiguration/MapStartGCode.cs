using System;
using System.Collections.Generic;
using System.Text;
using MatterHackers.Agg;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class MapStartGCode : InjectGCodeCommands
	{
		private bool escapeNewlineCharacters;

		public override string Value
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item in PreStartGCode(SlicingQueue.extrudersUsed))
				{
					stringBuilder.Append(item + "\n");
				}
				stringBuilder.Append(GCodeProcessing.ReplaceMacroValues(base.Value));
				foreach (string item2 in PostStartGCode(SlicingQueue.extrudersUsed))
				{
					stringBuilder.Append("\n");
					stringBuilder.Append(item2);
				}
				if (escapeNewlineCharacters)
				{
					return stringBuilder.ToString().Replace("\n", "\\n");
				}
				return stringBuilder.ToString();
			}
		}

		public MapStartGCode(string canonicalSettingsName, string exportedName, bool escapeNewlineCharacters)
			: base(canonicalSettingsName, exportedName)
		{
			this.escapeNewlineCharacters = escapeNewlineCharacters;
		}

		public List<string> PreStartGCode(List<bool> extrudersUsed)
		{
			string[] linesToCheckIfAlreadyPresent = ActiveSliceSettings.Instance.GetValue("start_gcode").Split(new string[1]
			{
				"\\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			List<string> list = new List<string>();
			list.Add("; automatic settings before start_gcode");
			AddDefaultIfNotPresent(list, "G21", linesToCheckIfAlreadyPresent, "set units to millimeters");
			AddDefaultIfNotPresent(list, "M107", linesToCheckIfAlreadyPresent, "fan off");
			double value = ActiveSliceSettings.Instance.GetValue<double>("bed_temperature");
			if (value > 0.0)
			{
				string commandToAdd = $"M190 S{value}";
				AddDefaultIfNotPresent(list, commandToAdd, linesToCheckIfAlreadyPresent, "wait for bed temperature to be reached");
			}
			int num = ActiveSliceSettings.Instance.Helpers.NumberOfHotEnds();
			for (int i = 0; i < num; i++)
			{
				if (extrudersUsed.Count > i && extrudersUsed[i])
				{
					ToolSettings toolSettings = ActiveSliceSettings.Instance.Tools[i];
					if (toolSettings.temperature != 0.0 && toolSettings.toolType == TOOL_TYPE.FFF)
					{
						string commandToAdd2 = StringHelper.FormatWith("M104 T{0} S{1}", new object[2]
						{
							toolSettings.position - 1,
							toolSettings.temperature
						});
						AddDefaultIfNotPresent(list, commandToAdd2, linesToCheckIfAlreadyPresent, $"start heating extruder {i + 1}");
					}
				}
			}
			if (ActiveSliceSettings.Instance.GetValue("heat_extruder_before_homing") == "1")
			{
				for (int j = 0; j < num; j++)
				{
					if (extrudersUsed.Count > j && extrudersUsed[j])
					{
						ToolSettings toolSettings2 = ActiveSliceSettings.Instance.Tools[j];
						if (toolSettings2.temperature != 0.0 && toolSettings2.toolType == TOOL_TYPE.FFF)
						{
							string commandToAdd3 = StringHelper.FormatWith("M109 T{0} S{1}", new object[2]
							{
								toolSettings2.position - 1,
								toolSettings2.temperature
							});
							AddDefaultIfNotPresent(list, commandToAdd3, linesToCheckIfAlreadyPresent, $"wait for extruder {j + 1}");
						}
					}
				}
			}
			list.Add("; settings from start_gcode");
			return list;
		}

		private void SwitchToFirstActiveExtruder(List<bool> extrudersUsed, string[] preStartGCodeLines, List<string> preStartGCode)
		{
			for (int i = 0; i < extrudersUsed.Count; i++)
			{
				if (extrudersUsed[i])
				{
					AddDefaultIfNotPresent(preStartGCode, StringHelper.FormatWith("T{0}", new object[1]
					{
						i
					}), preStartGCodeLines, StringHelper.FormatWith("set the active extruder to {0}", new object[1]
					{
						i
					}));
					break;
				}
			}
		}

		public List<string> PostStartGCode(List<bool> extrudersUsed)
		{
			string[] linesToCheckIfAlreadyPresent = ActiveSliceSettings.Instance.GetValue("start_gcode").Split(new string[1]
			{
				"\\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			List<string> list = new List<string>();
			list.Add("; automatic settings after start_gcode");
			int value = ActiveSliceSettings.Instance.GetValue<int>("extruder_count");
			if (ActiveSliceSettings.Instance.GetValue("heat_extruder_before_homing") != "1" && extrudersUsed[0])
			{
				ToolSettings toolSettings = ActiveSliceSettings.Instance.Tools[0];
				if (toolSettings.temperature != 0.0 && toolSettings.toolType == TOOL_TYPE.FFF)
				{
					string commandToAdd = $"M109 T{toolSettings.position - 1} S{toolSettings.temperature}";
					AddDefaultIfNotPresent(list, commandToAdd, linesToCheckIfAlreadyPresent, $"wait for extruder {1} to reach temperature");
				}
			}
			if (extrudersUsed.Count > 1)
			{
				for (int i = 1; i < value; i++)
				{
					if (i < extrudersUsed.Count && extrudersUsed[i])
					{
						ToolSettings toolSettings2 = ActiveSliceSettings.Instance.Tools[i];
						if (toolSettings2.temperature != 0.0 && toolSettings2.toolType == TOOL_TYPE.FFF)
						{
							list.Add($"M104 T{toolSettings2.position - 1} S{toolSettings2.temperature} ; Start heating extruder{i + 1}");
						}
					}
				}
				for (int j = 1; j < value; j++)
				{
					if (j < extrudersUsed.Count && extrudersUsed[j])
					{
						ToolSettings toolSettings3 = ActiveSliceSettings.Instance.Tools[j];
						if (toolSettings3.temperature != 0.0 && toolSettings3.toolType == TOOL_TYPE.FFF)
						{
							list.Add($"M109 T{toolSettings3.position - 1} S{toolSettings3.temperature} ; Finish heating extruder{j + 1}");
						}
					}
				}
			}
			AddDefaultIfNotPresent(list, "G90", linesToCheckIfAlreadyPresent, "use absolute coordinates");
			list.Add(string.Format("{0} ; {1}", "G92 E0", "reset the expected extruder position"));
			AddDefaultIfNotPresent(list, "M82", linesToCheckIfAlreadyPresent, "use absolute distance for extrusion");
			return list;
		}
	}
}
