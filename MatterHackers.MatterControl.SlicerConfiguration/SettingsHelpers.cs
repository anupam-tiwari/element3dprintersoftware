using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.ConfigurationPage.PrintLeveling;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.VectorMath;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class SettingsHelpers
	{
		private PrinterSettings printerSettings;

		private PrintLevelingData printLevelingData;

		public SettingsHelpers(PrinterSettings printerSettings)
		{
			this.printerSettings = printerSettings;
		}

		public double ExtruderTemperature(int extruderIndex)
		{
			if (extruderIndex < ActiveSliceSettings.Instance.Tools.Count)
			{
				return ActiveSliceSettings.Instance.Tools[extruderIndex].Temperature;
			}
			return 180.0;
		}

		public int[] LayerToPauseOn()
		{
			int temp;
			return Enumerable.ToArray<int>(Enumerable.Select<string, int>(Enumerable.Where<string>((IEnumerable<string>)printerSettings.GetValue("layer_to_pause").Split(new char[1]
			{
				';'
			}), (Func<string, bool>)((string v) => int.TryParse(v, out temp))), (Func<string, int>)delegate(string v)
			{
				int num = int.Parse(v);
				return (num == 0) ? 1 : (num - 1);
			}));
		}

		internal double ParseDouble(string firstLayerValueString)
		{
			if (!double.TryParse(firstLayerValueString, out var result))
			{
				throw new Exception($"Format cannot be parsed. FirstLayerHeight '{firstLayerValueString}'");
			}
			return result;
		}

		public void SetMarkedForDelete(bool markedForDelete)
		{
			PrinterInfo activeProfile = ProfileManager.Instance.ActiveProfile;
			if (activeProfile != null)
			{
				activeProfile.MarkedForDelete = markedForDelete;
				ProfileManager.Instance.Save();
			}
			ProfileManager.Instance.LastProfileID = "";
			UiThread.RunOnIdle((Action)delegate
			{
				ActiveSliceSettings.Instance = PrinterSettings.Empty;
				ProfileManager.ProfilesListChanged.CallEvents((object)this, (EventArgs)null);
				ApplicationController.SyncPrinterProfiles("SettingsHelpers.SetMarkedForDelete()", null);
			});
		}

		public void SetBaudRate(string baudRate)
		{
			printerSettings.SetValue("baud_rate", baudRate);
		}

		public string ComPort()
		{
			return printerSettings.GetValue($"{Environment.MachineName}_com_port");
		}

		public void SetComPort(string port)
		{
			printerSettings.SetValue($"{Environment.MachineName}_com_port", port);
		}

		public void SetComPort(string port, PrinterSettingsLayer layer)
		{
			printerSettings.SetValue($"{Environment.MachineName}_com_port", port, layer);
		}

		public void SetSlicingEngine(string engine)
		{
			printerSettings.SetValue("slicing_engine", engine);
		}

		public void SetDriverType(string driver)
		{
			printerSettings.SetValue("driver_type", driver);
		}

		public void SetDeviceToken(string token)
		{
			if (printerSettings.GetValue("device_token") != token)
			{
				printerSettings.SetValue("device_token", token);
			}
		}

		public void SetName(string name)
		{
			printerSettings.SetValue("printer_name", name);
		}

		public PrintLevelingData GetPrintLevelingData()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			if (printLevelingData == null)
			{
				printLevelingData = PrintLevelingData.Create(ActiveSliceSettings.Instance, printerSettings.GetValue("print_leveling_data"));
				if (printLevelingData.SampledPositions.Count == 3)
				{
					PrintLevelingPlane.Instance.SetPrintLevelingEquation(printLevelingData.SampledPositions[0], printLevelingData.SampledPositions[1], printLevelingData.SampledPositions[2], ActiveSliceSettings.Instance.GetValue<Vector2>("print_center"));
				}
			}
			return printLevelingData;
		}

		public void SetPrintLevelingData(PrintLevelingData data, bool clearUserZOffset)
		{
			if (clearUserZOffset)
			{
				ActiveSliceSettings.Instance.SetValue("baby_step_z_offset", "0");
			}
			printLevelingData = data;
			printerSettings.SetValue("print_leveling_data", JsonConvert.SerializeObject((object)data));
		}

		public void DoPrintLeveling(bool doLeveling)
		{
			if (doLeveling != printerSettings.GetValue<bool>("print_leveling_enabled"))
			{
				printerSettings.SetValue("print_leveling_enabled", doLeveling ? "1" : "0");
				if (doLeveling)
				{
					UpdateLevelSettings();
				}
				RootedObjectEventHandler printLevelingEnabledChanged = PrinterSettings.PrintLevelingEnabledChanged;
				if (printLevelingEnabledChanged != null)
				{
					printLevelingEnabledChanged.CallEvents((object)this, (EventArgs)null);
				}
			}
		}

		public void UpdateLevelSettings()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
			if (printLevelingData.SampledPositions.Count > 2)
			{
				PrintLevelingPlane.Instance.SetPrintLevelingEquation(printLevelingData.SampledPositions[0], printLevelingData.SampledPositions[1], printLevelingData.SampledPositions[2], ActiveSliceSettings.Instance.GetValue<Vector2>("print_center"));
			}
		}

		public Vector2 ExtruderOffset(int extruderIndex)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			string[] array = printerSettings.GetValue("extruder_offset").Split(new char[1]
			{
				','
			});
			int num = 0;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (num == extruderIndex)
				{
					string[] array3 = text.Split(new char[1]
					{
						'x'
					});
					return new Vector2(double.Parse(array3[0]), double.Parse(array3[1]));
				}
				num++;
			}
			return Vector2.Zero;
		}

		public SlicingEngineTypes ActiveSliceEngineType()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			if ((int)OsInformation.get_OperatingSystem() == 5 || SlicingQueue.AvailableSliceEngines.Count == 1)
			{
				return SlicingEngineTypes.MatterSlice;
			}
			string value = printerSettings.GetValue("slicing_engine");
			if (string.IsNullOrEmpty(value))
			{
				return SlicingEngineTypes.MatterSlice;
			}
			return (SlicingEngineTypes)Enum.Parse(typeof(SlicingEngineTypes), value);
		}

		public void ActiveSliceEngineType(SlicingEngineTypes type)
		{
			printerSettings.SetValue("slicing_engine", type.ToString());
		}

		public SliceEngineMapping ActiveSliceEngine()
		{
			return ActiveSliceEngineType() switch
			{
				SlicingEngineTypes.CuraEngine => EngineMappingCura.Instance, 
				SlicingEngineTypes.MatterSlice => EngineMappingsMatterSlice.Instance, 
				SlicingEngineTypes.Slic3r => Slic3rEngineMappings.Instance, 
				_ => null, 
			};
		}

		public void ExportAsMatterControlConfig()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			SaveFileDialogParams val = new SaveFileDialogParams("MatterControl Printer Export|*.printer", "", "Export Printer Settings", "");
			((FileDialogParams)val).set_FileName(printerSettings.GetValue("printer_name"));
			Exception e = default(Exception);
			FileDialog.SaveFileDialog(val, (Action<SaveFileDialogParams>)delegate(SaveFileDialogParams saveParams)
			{
				try
				{
					if (!string.IsNullOrWhiteSpace(((FileDialogParams)saveParams).get_FileName()))
					{
						File.WriteAllText(((FileDialogParams)saveParams).get_FileName(), JsonConvert.SerializeObject((object)printerSettings, (Formatting)1));
					}
				}
				catch (Exception ex)
				{
					e = ex;
					UiThread.RunOnIdle((Action)delegate
					{
						StyledMessageBox.ShowMessageBox(null, e.Message, "Couldn't save file".Localize());
					});
				}
			});
		}

		public void ExportAsSlic3rConfig()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			SaveFileDialogParams val = new SaveFileDialogParams("Save Slice Configuration".Localize() + "|*.ini", "", "", "");
			((FileDialogParams)val).set_FileName(printerSettings.GetValue("printer_name"));
			Exception e = default(Exception);
			FileDialog.SaveFileDialog(val, (Action<SaveFileDialogParams>)delegate(SaveFileDialogParams saveParams)
			{
				try
				{
					if (!string.IsNullOrWhiteSpace(((FileDialogParams)saveParams).get_FileName()))
					{
						Slic3rEngineMappings.WriteSliceSettingsFile(((FileDialogParams)saveParams).get_FileName());
					}
				}
				catch (Exception ex)
				{
					e = ex;
					UiThread.RunOnIdle((Action)delegate
					{
						StyledMessageBox.ShowMessageBox(null, e.Message, "Couldn't save file".Localize());
					});
				}
			});
		}

		public void ExportAsCuraConfig()
		{
			throw new NotImplementedException();
		}

		public Vector3 ManualMovementSpeeds()
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = default(Vector3);
			((Vector3)(ref result))._002Ector(3000.0, 3000.0, 315.0);
			string value = ActiveSliceSettings.Instance.GetValue("manual_movement_speeds");
			if (!string.IsNullOrEmpty(value))
			{
				string[] array = value.Split(new char[1]
				{
					','
				});
				result.x = double.Parse(array[1]);
				result.y = double.Parse(array[3]);
				result.z = double.Parse(array[5]);
			}
			return result;
		}

		public Dictionary<string, double> GetMovementSpeeds()
		{
			Dictionary<string, double> dictionary = new Dictionary<string, double>();
			string[] array = GetMovementSpeedsString().Split(new char[1]
			{
				','
			});
			for (int i = 0; i < array.Length / 2; i++)
			{
				dictionary.Add(array[i * 2], double.Parse(array[i * 2 + 1]));
			}
			return dictionary;
		}

		public string GetMovementSpeedsString()
		{
			string result = "x,3000,y,3000,z,315,e0,150";
			if (PrinterConnectionAndCommunication.Instance != null)
			{
				string value = printerSettings.GetValue("manual_movement_speeds");
				if (!string.IsNullOrEmpty(value))
				{
					result = value;
				}
			}
			return result;
		}

		public int NumberOfHotEnds()
		{
			if (ActiveSliceSettings.Instance.GetValue<bool>("extruders_share_temperature"))
			{
				return 1;
			}
			return ActiveSliceSettings.Instance.GetValue<int>("extruder_count");
		}

		public bool UseZProbe()
		{
			if (ActiveSliceSettings.Instance.GetValue<bool>("has_z_probe"))
			{
				return ActiveSliceSettings.Instance.GetValue<bool>("use_z_probe");
			}
			return false;
		}
	}
}
