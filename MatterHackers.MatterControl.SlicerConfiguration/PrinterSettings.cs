using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.PartPreviewWindow;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SettingsManagement;
using MatterHackers.VectorMath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class PrinterSettings
	{
		public static RootedObjectEventHandler PrintLevelingEnabledChanged;

		private static object writeLock;

		private EventHandler unregisterEvents;

		private static bool warningWindowOpen;

		private static Dictionary<string, Type> expectedMappingTypes;

		[JsonIgnore]
		private static HashSet<string> knownSettings;

		public static int LatestVersion
		{
			get;
		}

		public static PrinterSettings Empty
		{
			get;
		}

		public int DocumentVersion
		{
			get;
			set;
		} = LatestVersion;


		public string ID
		{
			get;
			set;
		}

		public static Func<bool> ShouldShowAuthPanel
		{
			get;
			set;
		}

		[JsonIgnore]
		internal PrinterSettingsLayer QualityLayer
		{
			get;
			private set;
		}

		[JsonIgnore]
		internal PrinterSettingsLayer MaterialLayer
		{
			get;
			private set;
		}

		public PrinterSettingsLayer StagedUserSettings
		{
			get;
			set;
		} = new PrinterSettingsLayer();


		public List<ToolSettings> Tools
		{
			get;
			set;
		} = new List<ToolSettings>();


		public List<GCodeMacro> Macros
		{
			get;
			set;
		} = new List<GCodeMacro>();


		public PrinterSettingsLayer OemLayer
		{
			get;
			set;
		}

		public string ActiveQualityKey
		{
			get
			{
				return GetValue("active_quality_key");
			}
			internal set
			{
				SetValue("active_quality_key", value);
				QualityLayer = GetQualityLayer(value);
				Save();
			}
		}

		public List<string> MaterialSettingsKeys
		{
			get;
			set;
		} = new List<string>();


		private string DocumentPath => ProfileManager.Instance.ProfilePath(ID);

		[JsonIgnore]
		public bool AutoSave
		{
			get;
			set;
		} = true;


		public PrinterSettingsLayer UserLayer
		{
			get;
		} = new PrinterSettingsLayer();


		public ObservableCollection<PrinterSettingsLayer> MaterialLayers
		{
			get;
		} = new ObservableCollection<PrinterSettingsLayer>();


		public ObservableCollection<PrinterSettingsLayer> QualityLayers
		{
			get;
		} = new ObservableCollection<PrinterSettingsLayer>();


		[JsonIgnore]
		public PrinterSettingsLayer BaseLayer
		{
			get;
			set;
		} = SliceSettingsOrganizer.Instance.GetDefaultSettings();


		internal IEnumerable<PrinterSettingsLayer> defaultLayerCascade
		{
			get
			{
				if (UserLayer != null)
				{
					yield return UserLayer;
				}
				if (MaterialLayer != null)
				{
					yield return MaterialLayer;
				}
				if (QualityLayer != null)
				{
					yield return QualityLayer;
				}
				if (OemLayer != null)
				{
					yield return OemLayer;
				}
				yield return BaseLayer;
			}
		}

		[JsonIgnore]
		public SettingsHelpers Helpers
		{
			get;
			set;
		}

		[JsonIgnore]
		public bool PrinterSelected
		{
			get
			{
				PrinterSettingsLayer oemLayer = OemLayer;
				if (oemLayer == null)
				{
					return false;
				}
				return oemLayer.Keys.Count > 0;
			}
		}

		public static HashSet<string> KnownSettings
		{
			get
			{
				if (knownSettings == null)
				{
					knownSettings = LoadSettingsNamesFromPropertiesJson();
				}
				return knownSettings;
			}
		}

		static PrinterSettings()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			LatestVersion = 201606271;
			PrintLevelingEnabledChanged = new RootedObjectEventHandler();
			writeLock = new object();
			warningWindowOpen = false;
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			dictionary["extruders_share_temperature"] = typeof(int);
			dictionary["extruders_share_temperature"] = typeof(bool);
			dictionary["has_heated_bed"] = typeof(bool);
			dictionary["nozzle_diameter"] = typeof(double);
			dictionary["bed_temperature"] = typeof(double);
			expectedMappingTypes = dictionary;
			Empty = new PrinterSettings
			{
				ID = "EmptyProfile"
			};
			Empty.UserLayer["printer_name"] = "Printers...".Localize();
		}

		public PrinterSettings()
		{
			Helpers = new SettingsHelpers(this);
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (((StringEventArgs)e).get_Data() == "extruder_count")
				{
					string value = ActiveSliceSettings.Instance.GetValue("extruder_count");
					if (int.Parse(value) < ActiveSliceSettings.Instance.Tools.Count)
					{
						ActiveSliceSettings.Instance.Tools = ActiveSliceSettings.Instance.Tools.GetRange(0, int.Parse(value));
						ActiveSliceSettings.Instance.Save();
					}
				}
			}, ref unregisterEvents);
		}

		~PrinterSettings()
		{
			unregisterEvents?.Invoke(this, null);
		}

		public IEnumerable<GCodeMacro> UserMacros()
		{
			return Enumerable.Where<GCodeMacro>((IEnumerable<GCodeMacro>)Macros, (Func<GCodeMacro, bool>)((GCodeMacro m) => !m.ActionGroup));
		}

		public IEnumerable<GCodeMacro> ActionMacros()
		{
			return Enumerable.Where<GCodeMacro>((IEnumerable<GCodeMacro>)Macros, (Func<GCodeMacro, bool>)((GCodeMacro m) => m.ActionGroup));
		}

		public void RestoreConflictingUserOverrides(PrinterSettingsLayer settingsLayer)
		{
			if (settingsLayer == null)
			{
				return;
			}
			foreach (string key in settingsLayer.Keys)
			{
				RestoreUserOverride(settingsLayer, key);
			}
		}

		private void RestoreUserOverride(PrinterSettingsLayer settingsLayer, string settingsKey)
		{
			if (StagedUserSettings.TryGetValue(settingsKey, out var value))
			{
				StagedUserSettings.Remove(settingsKey);
				UserLayer[settingsKey] = value;
			}
		}

		public void DeactivateConflictingUserOverrides(PrinterSettingsLayer settingsLayer)
		{
			if (settingsLayer == null)
			{
				return;
			}
			foreach (string key in settingsLayer.Keys)
			{
				StashUserOverride(settingsLayer, key);
			}
		}

		public bool ParseShowString(string unsplitSettings, List<PrinterSettingsLayer> layerCascade)
		{
			if (!string.IsNullOrEmpty(unsplitSettings))
			{
				string[] array = unsplitSettings.Split(new char[1]
				{
					'&'
				});
				foreach (string obj in array)
				{
					bool flag = false;
					string[] array2 = obj.Split(new char[1]
					{
						'|'
					});
					foreach (string obj2 in array2)
					{
						string b = "1";
						string text = obj2;
						bool flag2 = text.StartsWith("!");
						if (flag2)
						{
							text = text.Substring(1);
						}
						string text2 = "";
						if (text.Contains("="))
						{
							string[] array3 = text.Split(new char[1]
							{
								'='
							});
							text2 = GetValue(array3[0], layerCascade);
							b = array3[1];
						}
						else
						{
							text2 = GetValue(text, layerCascade);
						}
						if ((!flag2 && text2 == b) || (flag2 && text2 != b))
						{
							flag = true;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void StashUserOverride(PrinterSettingsLayer settingsLayer, string settingsKey)
		{
			if (UserLayer.TryGetValue(settingsKey, out var value))
			{
				UserLayer.Remove(settingsKey);
				StagedUserSettings.Add(settingsKey, value);
			}
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			QualityLayer = GetQualityLayer(ActiveQualityKey);
			string materialPresetKey = GetMaterialPresetKey(0);
			if (!string.IsNullOrEmpty(materialPresetKey))
			{
				MaterialLayer = GetMaterialLayer(materialPresetKey);
			}
		}

		public void Merge(PrinterSettingsLayer destinationLayer, PrinterSettings settingsToImport, List<PrinterSettingsLayer> rawSourceFilter, bool setLayerName)
		{
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			HashSet<string> obj = new HashSet<string>();
			obj.Add("layer_id");
			HashSet<string> val = obj;
			if (!setLayerName)
			{
				val.Add("layer_name");
			}
			IEnumerable<PrinterSettingsLayer> layerCascade = Enumerable.Where<PrinterSettingsLayer>((IEnumerable<PrinterSettingsLayer>)new List<PrinterSettingsLayer>
			{
				OemLayer,
				BaseLayer,
				destinationLayer
			}, (Func<PrinterSettingsLayer, bool>)((PrinterSettingsLayer layer) => layer != null));
			IEnumerable<PrinterSettingsLayer> layerCascade2 = Enumerable.Where<PrinterSettingsLayer>((IEnumerable<PrinterSettingsLayer>)rawSourceFilter, (Func<PrinterSettingsLayer, bool>)((PrinterSettingsLayer layer) => layer != null));
			Enumerator<string> enumerator = KnownSettings.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					if (settingsToImport.Contains(current))
					{
						string a = GetValue(current, layerCascade).Trim();
						string value = settingsToImport.GetValue(current, layerCascade2).Trim();
						if (!string.IsNullOrEmpty(value) && a != current && !val.Contains(current))
						{
							destinationLayer[current] = value;
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			if (setLayerName)
			{
				destinationLayer["layer_name"] = settingsToImport.GetValue("layer_name", layerCascade2);
			}
			Save();
			ApplicationController.Instance.ReloadAdvancedControlsPanel();
		}

		internal PrinterSettingsLayer GetMaterialLayer(string layerID)
		{
			if (string.IsNullOrEmpty(layerID))
			{
				return null;
			}
			return Enumerable.FirstOrDefault<PrinterSettingsLayer>(Enumerable.Where<PrinterSettingsLayer>((IEnumerable<PrinterSettingsLayer>)MaterialLayers, (Func<PrinterSettingsLayer, bool>)((PrinterSettingsLayer layer) => layer.LayerID == layerID)));
		}

		private PrinterSettingsLayer GetQualityLayer(string layerID)
		{
			return Enumerable.FirstOrDefault<PrinterSettingsLayer>(Enumerable.Where<PrinterSettingsLayer>((IEnumerable<PrinterSettingsLayer>)QualityLayers, (Func<PrinterSettingsLayer, bool>)((PrinterSettingsLayer layer) => layer.LayerID == layerID)));
		}

		public string GetMaterialPresetKey(int extruderIndex)
		{
			if (extruderIndex >= MaterialSettingsKeys.Count)
			{
				return null;
			}
			return MaterialSettingsKeys[extruderIndex];
		}

		public void SetMaterialPreset(int extruderIndex, string materialKey)
		{
			if (extruderIndex >= 16)
			{
				throw new ArgumentOutOfRangeException("Requested extruder index is outside of bounds: " + extruderIndex);
			}
			if (MaterialSettingsKeys.Count <= extruderIndex)
			{
				string[] array = new string[extruderIndex + 1];
				MaterialSettingsKeys.CopyTo(array);
				MaterialSettingsKeys = new List<string>(array);
			}
			MaterialSettingsKeys[extruderIndex] = materialKey;
			if (extruderIndex == 0)
			{
				MaterialLayer = GetMaterialLayer(materialKey);
				ApplicationController.Instance.ReloadAdvancedControlsPanel();
			}
			Save();
		}

		private string GenerateSha1()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			using FileStream fileStream = new FileStream(DocumentPath, FileMode.Open);
			BufferedStream val = new BufferedStream((Stream)fileStream, 1200000);
			try
			{
				return GenerateSha1((Stream)(object)val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		private string GenerateSha1(Stream stream)
		{
			SHA1 val = SHA1.Create();
			try
			{
				return BitConverter.ToString(((HashAlgorithm)val).ComputeHash(stream)).Replace("-", string.Empty);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		public void Save()
		{
			if (PrinterSelected && AutoSave)
			{
				Save(DocumentPath);
			}
		}

		public void Save(string filePath)
		{
			lock (writeLock)
			{
				string text = ToJson((Formatting)1);
				PrinterInfo printerInfo = ProfileManager.Instance[ID];
				if (printerInfo != null)
				{
					printerInfo.ContentSHA1 = ComputeSha1(text);
					ProfileManager.Instance.Save();
				}
				File.WriteAllText(filePath, text);
			}
			if (ActiveSliceSettings.Instance?.ID == ID)
			{
				ActiveSliceSettings.ActiveProfileModified.CallEvents((object)null, (EventArgs)null);
			}
		}

		public string ComputeSha1()
		{
			return ComputeSha1(ToJson((Formatting)1));
		}

		private string ComputeSha1(string json)
		{
			using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
			return GenerateSha1(stream);
		}

		public static PrinterSettings LoadFile(string printerProfilePath, bool performMigrations = false)
		{
			if (performMigrations)
			{
				JObject val = null;
				try
				{
					val = JObject.Parse(File.ReadAllText(printerProfilePath));
				}
				catch
				{
					return null;
				}
				int? obj2;
				if (val == null)
				{
					obj2 = null;
				}
				else
				{
					JToken value = val.GetValue("DocumentVersion");
					obj2 = ((value != null) ? new int?(Extensions.Value<int>((IEnumerable<JToken>)value)) : null);
				}
				int num = obj2 ?? LatestVersion;
				if (num < LatestVersion)
				{
					printerProfilePath = ProfileMigrations.MigrateDocument(printerProfilePath, num);
				}
			}
			try
			{
				return JsonConvert.DeserializeObject<PrinterSettings>(File.ReadAllText(printerProfilePath));
			}
			catch
			{
				return null;
			}
		}

		public static async Task<PrinterSettings> RecoverProfile(PrinterInfo printerInfo)
		{
			Func<bool> shouldShowAuthPanel = ShouldShowAuthPanel;
			if (shouldShowAuthPanel != null && !shouldShowAuthPanel() && printerInfo != null)
			{
				PrinterSettings printerSettings = await GetFirstValidHistoryItem(printerInfo);
				if (printerSettings == null)
				{
					printerSettings = RestoreFromOemProfile(printerInfo);
				}
				if (printerSettings == null)
				{
					printerSettings = Empty;
					printerSettings.ID = printerInfo.ID;
					printerSettings.UserLayer["device_token"] = printerInfo.DeviceToken;
					printerSettings.Helpers.SetComPort(printerInfo.ComPort);
					printerSettings.SetValue("printer_name", printerInfo.Name);
					printerSettings.OemLayer = new PrinterSettingsLayer();
					printerSettings.OemLayer.Add("empty", "setting");
					printerSettings.Save();
				}
				if (printerSettings != null)
				{
					printerSettings.Save();
					ActiveSliceSettings.RefreshActiveInstance(printerSettings);
					WarnAboutRevert(printerInfo);
				}
				return printerSettings;
			}
			return null;
		}

		public static void WarnAboutRevert(PrinterInfo profile)
		{
			if (warningWindowOpen)
			{
				return;
			}
			warningWindowOpen = true;
			UiThread.RunOnIdle((Action)delegate
			{
				StyledMessageBox.ShowMessageBox(delegate
				{
					warningWindowOpen = false;
				}, string.Format("The profile you are attempting to load has been corrupted. We loaded your last usable {0} {1} profile from your recent profile history instead.".Localize(), profile.Make, profile.Model), "Recovered printer profile".Localize());
			});
		}

		public static PrinterSettings RestoreFromOemProfile(PrinterInfo profile)
		{
			PrinterSettings printerSettings = null;
			try
			{
				PublicDevice publicDevice = OemSettings.Instance.OemProfiles[profile.Make][profile.Model];
				printerSettings = JsonConvert.DeserializeObject<PrinterSettings>(File.ReadAllText(ApplicationController.CacheablePath(Path.Combine("public-profiles", profile.Make), publicDevice.CacheKey)));
				printerSettings.ID = profile.ID;
				printerSettings.SetValue("printer_name", profile.Name);
				printerSettings.DocumentVersion = LatestVersion;
				printerSettings.Helpers.SetComPort(profile.ComPort);
				printerSettings.Save();
				return printerSettings;
			}
			catch
			{
				return printerSettings;
			}
		}

		private static async Task<PrinterSettings> GetFirstValidHistoryItem(PrinterInfo printerInfo)
		{
			Dictionary<string, string> dictionary = await (ApplicationController.GetProfileHistory?.Invoke(printerInfo.DeviceToken));
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, string> item in Enumerable.Take<KeyValuePair<string, string>>(Enumerable.Skip<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>)Enumerable.OrderByDescending<KeyValuePair<string, string>, string>((IEnumerable<KeyValuePair<string, string>>)dictionary, (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kvp) => kvp.Key)), 1), 5))
				{
					try
					{
						PrinterSettings printerSettings = await ApplicationController.GetPrinterProfileAsync(printerInfo, item.Value);
						if (printerSettings != null)
						{
							return printerSettings;
						}
					}
					catch
					{
					}
				}
			}
			return null;
		}

		public string GetValue(string sliceSetting, IEnumerable<PrinterSettingsLayer> layerCascade = null)
		{
			if (layerCascade == null)
			{
				layerCascade = defaultLayerCascade;
			}
			foreach (PrinterSettingsLayer item in layerCascade)
			{
				if (item.TryGetValue(sliceSetting, out var value))
				{
					return value;
				}
			}
			return "";
		}

		public Tuple<string, string> GetValueAndLayerName(string sliceSetting, IEnumerable<PrinterSettingsLayer> layerCascade = null)
		{
			if (layerCascade == null)
			{
				layerCascade = defaultLayerCascade;
			}
			foreach (PrinterSettingsLayer item2 in layerCascade)
			{
				if (item2.TryGetValue(sliceSetting, out var value))
				{
					string item = "User";
					if (item2 == BaseLayer)
					{
						item = "Base";
					}
					else if (item2 == OemLayer)
					{
						item = "Oem";
					}
					else if (item2 == MaterialLayer)
					{
						item = "Material";
					}
					else if (item2 == QualityLayer)
					{
						item = "Quality";
					}
					return new Tuple<string, string>(value, item);
				}
			}
			return new Tuple<string, string>("", "");
		}

		public bool Contains(string sliceSetting, IEnumerable<PrinterSettingsLayer> layerCascade = null)
		{
			if (layerCascade == null)
			{
				layerCascade = defaultLayerCascade;
			}
			foreach (PrinterSettingsLayer item in layerCascade)
			{
				if (item.ContainsKey(sliceSetting))
				{
					return true;
				}
			}
			return false;
		}

		internal void RunInTransaction(Action<PrinterSettings> action)
		{
			action(this);
		}

		public void ClearUserOverrides()
		{
			string[] array = Enumerable.ToArray<string>((IEnumerable<string>)UserLayer.Keys);
			HashSet<string> keysToRetain = new HashSet<string>(Enumerable.Except<string>((IEnumerable<string>)array, (IEnumerable<string>)KnownSettings));
			keysToRetain.Remove("print_leveling_data");
			keysToRetain.Remove("print_leveling_enabled");
			foreach (SliceSettingData item in Enumerable.Where<SliceSettingData>((IEnumerable<SliceSettingData>)ActiveSliceSettings.SettingsData, (Func<SliceSettingData, bool>)((SliceSettingData settingsItem) => !settingsItem.ShowAsOverride)))
			{
				string slicerConfigName = item.SlicerConfigName;
				if (!(slicerConfigName == "baud_rate") && !(slicerConfigName == "auto_connect"))
				{
					keysToRetain.Add(item.SlicerConfigName);
				}
			}
			foreach (string item2 in Enumerable.ToList<string>(Enumerable.Select<KeyValuePair<string, string>, string>(Enumerable.Where<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>)UserLayer, (Func<KeyValuePair<string, string>, bool>)((KeyValuePair<string, string> keyValue) => !keysToRetain.Contains(keyValue.Key))), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> keyValue) => keyValue.Key))))
			{
				UserLayer.Remove(item2);
			}
			Tools = new List<ToolSettings>();
		}

		private void ValidateType<T>(string settingsKey)
		{
			if (expectedMappingTypes.ContainsKey(settingsKey) && expectedMappingTypes[settingsKey] != typeof(T))
			{
				throw new Exception("You must request the correct type of this settingsKey.");
			}
			if (settingsKey.Contains("%") && typeof(T) != typeof(double))
			{
				throw new Exception("To get processing of a % you must request the type as double.");
			}
		}

		public T GetValue<T>(string settingsKey) where T : IConvertible
		{
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			if (typeof(T) == typeof(bool))
			{
				return (T)(object)(GetValue(settingsKey) == "1");
			}
			if (typeof(T) == typeof(int))
			{
				int.TryParse(GetValue(settingsKey), out var result);
				return (T)(object)result;
			}
			if (typeof(T) == typeof(Vector2))
			{
				string[] array = GetValue(settingsKey).Split(new char[1]
				{
					','
				});
				if (array.Length != 2)
				{
					throw new Exception($"Not parsing {settingsKey} as a Vector2");
				}
				Vector2 val = default(Vector2);
				val.x = Helpers.ParseDouble(array[0]);
				val.y = Helpers.ParseDouble(array[1]);
				return (T)(object)val;
			}
			if (typeof(T) == typeof(double))
			{
				string value = GetValue(settingsKey);
				if (value.Contains("%"))
				{
					string firstLayerValueString = value.Replace("%", "");
					double num = Helpers.ParseDouble(firstLayerValueString) / 100.0;
					switch (settingsKey)
					{
					case "first_layer_height":
						return (T)(object)(GetValue<double>("layer_height") * num);
					case "first_layer_extrusion_width":
					case "external_perimeter_extrusion_width":
						return (T)(object)(GetValue<double>("nozzle_diameter") * num);
					default:
						return (T)(object)num;
					}
				}
				switch (settingsKey)
				{
				case "first_layer_extrusion_width":
				case "external_perimeter_extrusion_width":
				{
					double.TryParse(GetValue(settingsKey), out var result2);
					return (T)(object)((result2 == 0.0) ? GetValue<double>("nozzle_diameter") : result2);
				}
				case "bed_temperature":
					if (!GetValue<bool>("has_heated_bed"))
					{
						return (T)Convert.ChangeType(0, typeof(double));
					}
					break;
				}
				double.TryParse(GetValue(settingsKey), out var result3);
				return (T)(object)result3;
			}
			if (typeof(T) == typeof(BedShape))
			{
				string value2 = GetValue(settingsKey);
				if (!(value2 == "rectangular"))
				{
					if (value2 == "circular")
					{
						return (T)(object)BedShape.Circular;
					}
					return (T)(object)BedShape.Rectangular;
				}
				return (T)(object)BedShape.Rectangular;
			}
			return default(T);
		}

		public bool SettingExistsInLayer(string sliceSetting, NamedSettingsLayers layer)
		{
			return layer switch
			{
				NamedSettingsLayers.Quality => QualityLayer?.ContainsKey(sliceSetting) ?? false, 
				NamedSettingsLayers.Material => MaterialLayer?.ContainsKey(sliceSetting) ?? false, 
				NamedSettingsLayers.User => UserLayer?.ContainsKey(sliceSetting) ?? false, 
				_ => false, 
			};
		}

		public long GetLongHashCode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in BaseLayer)
			{
				if (SliceSettingsOrganizer.Instance.GetSettingsData(item.Key).RebuildGCodeOnChange)
				{
					string value = GetValue(item.Key);
					stringBuilder.Append(item.Key);
					stringBuilder.Append(value);
				}
			}
			foreach (ToolSettings tool in Tools)
			{
				stringBuilder.Append(tool.GetHashString());
			}
			stringBuilder.ToString();
			return agg_basics.ComputeHash(stringBuilder.ToString());
		}

		public bool IsValid()
		{
			try
			{
				foreach (SystemWindow allOpenSystemWindow in SystemWindow.get_AllOpenSystemWindows())
				{
					View3DWidget view3D = Enumerable.FirstOrDefault<View3DWidget>(ExtensionMethods.ChildrenRecursive<View3DWidget>((GuiWidget)(object)allOpenSystemWindow));
					if (view3D == null || !view3D.ShouldBeSaved || !(PrinterConnectionAndCommunication.Instance.ActivePrintItem.FileLocation == view3D.ActivePrintItem.FileLocation))
					{
						continue;
					}
					bool shouldFail = true;
					string arg = "Unsaved Changes".Localize();
					string arg2 = StringHelper.FormatWith("'{0}' is being edited. You can save and continue, or cancel".Localize(), new object[1]
					{
						view3D.ActivePrintItem.Name
					});
					StyledMessageBox.ShowMessageBox(delegate(bool shouldSave)
					{
						if (shouldSave)
						{
							view3D.SaveYourselfNow(delegate
							{
								shouldFail = false;
							});
						}
					}, $"{arg}\n\n{arg2}", "Save Error".Localize(), StyledMessageBox.MessageType.YES_NO, "Save and Continue".Localize(), "Cancel".Localize());
					if (shouldFail)
					{
						return false;
					}
				}
				if (GetValue<bool>("recover_is_enabled"))
				{
					string arg3 = "Location: 'Settings & Controls' -> 'Settings' -> 'Printer' -> 'Print Recovery' -> 'Enable Recovery'".Localize();
					string[] array = GetValue("start_gcode").Replace("\\n", "\n").Split(new char[1]
					{
						'\n'
					});
					foreach (string text in array)
					{
						if (text.StartsWith("G29"))
						{
							string arg4 = "Start G-Code cannot contain G29 if Print Recovery is enabled.".Localize();
							string arg5 = "Your Start G-Code should not contain a G29 if you are planning on using Print Recovery. Change your start G-Code or turn off Print Recovery".Localize();
							StyledMessageBox.ShowMessageBox(null, $"{arg4}\n\n{arg5}\n\n{arg3}", "Slice Error".Localize());
							return false;
						}
						if (text.StartsWith("G30"))
						{
							string arg6 = "Start G-Code cannot contain G30 if Print Leveling is enabled.".Localize();
							string arg7 = "Your Start G-Code should not contain a G30 if you are planning on using Print Recovery. Change your start G-Code or turn off Print Recovery".Localize();
							StyledMessageBox.ShowMessageBox(null, $"{arg6}\n\n{arg7}\n\n{arg3}", "Slice Error".Localize());
							return false;
						}
					}
				}
				if (GetValue<bool>("print_leveling_enabled"))
				{
					string arg8 = "Location: 'Settings & Controls' -> 'Settings' -> 'Printer' -> 'Custom G-Code' -> 'Start G-Code'".Localize();
					string[] array = GetValue("start_gcode").Replace("\\n", "\n").Split(new char[1]
					{
						'\n'
					});
					foreach (string text2 in array)
					{
						if (text2.StartsWith("G29"))
						{
							string arg9 = "Start G-Code cannot contain G29 if Print Leveling is enabled.".Localize();
							string arg10 = "Your Start G-Code should not contain a G29 if you are planning on using print leveling. Change your start G-Code or turn off print leveling".Localize();
							StyledMessageBox.ShowMessageBox(null, $"{arg9}\n\n{arg10}\n\n{arg8}", "Slice Error".Localize());
							return false;
						}
						if (text2.StartsWith("G30"))
						{
							string arg11 = "Start G-Code cannot contain G30 if Print Leveling is enabled.".Localize();
							string arg12 = "Your Start G-Code should not contain a G30 if you are planning on using print leveling. Change your start G-Code or turn off print leveling".Localize();
							StyledMessageBox.ShowMessageBox(null, $"{arg11}\n\n{arg12}\n\n{arg8}", "Slice Error".Localize());
							return false;
						}
					}
				}
				if (Math.Abs(GetValue<double>("baby_step_z_offset")) > 2.0)
				{
					string arg13 = "Location: 'Controls' -> 'Movement' -> 'Z Offset'".Localize();
					string arg14 = "Z Offset is too large.".Localize();
					string arg15 = "The Z Offset for your printer, sometimes called Babby Stepping, is greater than 2mm and invalid. Clear the value and re-level the bed.".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg14}\n\n{arg15}\n\n{arg13}", "Calibration Error".Localize());
					return false;
				}
				if (GetValue<double>("first_layer_extrusion_width") > GetValue<double>("nozzle_diameter") * 4.0)
				{
					string arg16 = "'First Layer Extrusion Width' must be less than or equal to the 'Nozzle Diameter' * 4.".Localize();
					string arg17 = string.Format("First Layer Extrusion Width = {0}\nNozzle Diameter = {1}".Localize(), GetValue("first_layer_extrusion_width"), GetValue<double>("nozzle_diameter"));
					string arg18 = "Location: 'Settings & Controls' -> 'Settings' -> 'Filament' -> 'Extrusion' -> 'First Layer'".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg16}\n\n{arg17}\n\n{arg18}", "Slice Error".Localize());
					return false;
				}
				if (GetValue<double>("first_layer_extrusion_width") <= 0.0)
				{
					string arg19 = "'First Layer Extrusion Width' must be greater than 0.".Localize();
					string arg20 = string.Format("First Layer Extrusion Width = {0}".Localize(), GetValue("first_layer_extrusion_width"));
					string arg21 = "Location: 'Settings & Controls' -> 'Settings' -> 'Filament' -> 'Extrusion' -> 'First Layer'".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg19}\n\n{arg20}\n\n{arg21}", "Slice Error".Localize());
					return false;
				}
				if (GetValue<double>("external_perimeter_extrusion_width") > GetValue<double>("nozzle_diameter") * 4.0)
				{
					string arg22 = "'External Perimeter Extrusion Width' must be less than or equal to the 'Nozzle Diameter' * 4.".Localize();
					string arg23 = string.Format("External Perimeter Extrusion Width = {0}\nNozzle Diameter = {1}".Localize(), GetValue("external_perimeter_extrusion_width"), GetValue<double>("nozzle_diameter"));
					string arg24 = "Location: 'Settings & Controls' -> 'Settings' -> 'Filament' -> 'Extrusion' -> 'External Perimeter'".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg22}\n\n{arg23}\n\n{arg24}", "Slice Error".Localize());
					return false;
				}
				if (GetValue<double>("external_perimeter_extrusion_width") <= 0.0)
				{
					string arg25 = "'External Perimeter Extrusion Width' must be greater than 0.".Localize();
					string arg26 = string.Format("External Perimeter Extrusion Width = {0}".Localize(), GetValue("external_perimeter_extrusion_width"));
					string arg27 = "Location: 'Settings & Controls' -> 'Settings' -> 'Filament' -> 'Extrusion' -> 'External Perimeter'".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg25}\n\n{arg26}\n\n{arg27}", "Slice Error".Localize());
					return false;
				}
				if (GetValue<double>("min_fan_speed") > 100.0)
				{
					string arg28 = "The Minimum Fan Speed can only go as high as 100%.".Localize();
					string arg29 = string.Format("It is currently set to {0}.".Localize(), GetValue<double>("min_fan_speed"));
					string arg30 = "Location: 'Settings & Controls' -> 'Settings' -> 'Filament' -> 'Cooling'".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg28}\n\n{arg29}\n\n{arg30}", "Slice Error".Localize());
					return false;
				}
				if (GetValue<double>("max_fan_speed") > 100.0)
				{
					string arg31 = "The Maximum Fan Speed can only go as high as 100%.".Localize();
					string arg32 = string.Format("It is currently set to {0}.".Localize(), GetValue<double>("max_fan_speed"));
					string arg33 = "Location: 'Settings & Controls' -> 'Settings' -> 'Filament' -> 'Cooling'".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg31}\n\n{arg32}\n\n{arg33}", "Slice Error".Localize());
					return false;
				}
				if (GetValue<int>("extruder_count") < 1)
				{
					string arg34 = "The Extruder Count must be at least 1.".Localize();
					string arg35 = string.Format("It is currently set to {0}.".Localize(), GetValue<int>("extruder_count"));
					string arg36 = "Location: 'Settings & Controls' -> 'Settings' -> 'General' -> 'Tools'".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg34}\n\n{arg35}\n\n{arg36}", "Slice Error".Localize());
					return false;
				}
				if (GetValue<double>("fill_density") < 0.0 || GetValue<double>("fill_density") > 1.0)
				{
					string arg37 = "The Fill Density must be between 0 and 1.".Localize();
					string arg38 = string.Format("It is currently set to {0}.".Localize(), GetValue<double>("fill_density"));
					string arg39 = "Location: 'Settings & Controls' -> 'Settings' -> 'General' -> 'Infill'".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg37}\n\n{arg38}\n\n{arg39}", "Slice Error".Localize());
					return false;
				}
				int num = 0;
				bool flag = true;
				int num2 = 1;
				foreach (ToolSettings tool in Tools)
				{
					if (tool.toolType != 0)
					{
						num++;
					}
					if (GetValue<bool>("validate_layer_height") && tool.toolType != TOOL_TYPE.MICRO_VALVE && tool.toolType != TOOL_TYPE.LASER && tool.toolType != TOOL_TYPE.LED && tool.toolType != TOOL_TYPE.IO)
					{
						if (GetValue<double>("layer_height") > tool.width)
						{
							string arg40 = "'Layer Height' must be less than or equal to all the tool width's.".Localize();
							string arg41 = string.Format("Layer Height = {0}\nTool {1} width = {2}".Localize(), GetValue<double>("layer_height"), num2, tool.width);
							string arg42 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Width'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg40}\n\n{arg41}\n\n{arg42}", "Tool Error".Localize());
							flag = false;
						}
						else if (GetValue<double>("first_layer_height") > tool.width)
						{
							string arg43 = "'First Layer Height' must be less than or equal to all the tool width's.".Localize();
							string arg44 = string.Format("First Layer Height = {0}\nTool {1} width = {2}".Localize(), GetValue<double>("layer_height"), num2, tool.width);
							string arg45 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Width'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg43}\n\n{arg44}\n\n{arg45}", "Tool Error".Localize());
							flag = false;
						}
					}
					if (tool.position == 0 && tool.toolType != 0)
					{
						string arg46 = "A tool has an invalid position.".Localize();
						string arg47 = string.Format("Tool {0} has an invalid position.".Localize(), num2);
						string arg48 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Position'".Localize(), num2);
						StyledMessageBox.ShowMessageBox(null, $"{arg46}\n\n{arg47}\n\n{arg48}", "Tool Error".Localize());
						flag = false;
					}
					if (GetValue<bool>("tool_specific_infill") && (tool.infillPercent < 0.0 || tool.infillPercent > 100.0))
					{
						string arg49 = "A tool has an invalid infill amount.".Localize();
						string arg50 = string.Format("Tool {0} has an invalid infill amount.".Localize(), num2);
						string arg51 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Infill Amount'".Localize(), num2);
						StyledMessageBox.ShowMessageBox(null, $"{arg49}\n\n{arg50}\n\n{arg51}", "Tool Error".Localize());
						flag = false;
					}
					switch (tool.toolType)
					{
					case TOOL_TYPE.SYRINGE:
						if ((tool.psi < 2.0 || tool.psi > 100.0) && tool.psi != 0.0)
						{
							string arg67 = "A tool has an invalid PSI.".Localize();
							string arg68 = string.Format("Tool {0} has an invalid PSI.".Localize(), num2);
							string arg69 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'PSI'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg67}\n\n{arg68}\n\n{arg69}", "Tool Error".Localize());
							flag = false;
						}
						if (tool.width <= 0.0)
						{
							string arg70 = "A tool has an invalid width.".Localize();
							string arg71 = string.Format("Tool {0} may not have width 0.".Localize(), num2);
							string arg72 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Width'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg70}\n\n{arg71}\n\n{arg72}", "Tool Error".Localize());
							flag = false;
						}
						break;
					case TOOL_TYPE.TSYRINGE:
						if (tool.temperaturePosition == 0)
						{
							string arg58 = "A tool has an invalid temperature port.".Localize();
							string arg59 = string.Format("Tool {0} has an invalid temperature port.".Localize(), num2);
							string arg60 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Temperature Port'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg58}\n\n{arg59}\n\n{arg60}", "Tool Error".Localize());
							flag = false;
						}
						if (tool.psi < 2.0 || tool.psi > 100.0)
						{
							string arg61 = "A tool has an invalid PSI.".Localize();
							string arg62 = string.Format("Tool {0} has an invalid PSI.".Localize(), num2);
							string arg63 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'PSI'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg61}\n\n{arg62}\n\n{arg63}", "Tool Error".Localize());
							flag = false;
						}
						if (tool.width <= 0.0)
						{
							string arg64 = "A tool has an invalid width.".Localize();
							string arg65 = string.Format("Tool {0} may not have width 0.".Localize(), num2);
							string arg66 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Width'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg64}\n\n{arg65}\n\n{arg66}", "Tool Error".Localize());
							flag = false;
						}
						break;
					case TOOL_TYPE.MICRO_VALVE:
						if (tool.psi < 2.0 || tool.psi > 40.0)
						{
							string arg73 = "A tool has an invalid PSI.".Localize();
							string arg74 = string.Format("Tool {0} has an invalid PSI.".Localize(), num2);
							string arg75 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'PSI'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg73}\n\n{arg74}\n\n{arg75}", "Tool Error".Localize());
							flag = false;
						}
						if (tool.width <= 0.0)
						{
							string arg76 = "A tool has an invalid width.".Localize();
							string arg77 = string.Format("Tool {0} may not have width 0.".Localize(), num2);
							string arg78 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Width'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg76}\n\n{arg77}\n\n{arg78}", "Tool Error".Localize());
							flag = false;
						}
						if (tool.period < 0.1)
						{
							string arg79 = "A tool has an invalid period.".Localize();
							string arg80 = string.Format("Tool {0} has an invalid period.".Localize(), num2);
							string arg81 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Period'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg79}\n\n{arg80}\n\n{arg81}", "Tool Error".Localize());
							flag = false;
						}
						else if (tool.duty <= 0.0 || tool.duty > 100.0)
						{
							string arg82 = "A tool has an invalid duty cycle.".Localize();
							string arg83 = string.Format("Tool {0} has an invalid duty cycle.".Localize(), num2);
							string arg84 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Duty Cycle'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg82}\n\n{arg83}\n\n{arg84}", "Tool Error".Localize());
							flag = false;
						}
						break;
					case TOOL_TYPE.LASER:
					case TOOL_TYPE.LED:
					case TOOL_TYPE.IO:
						if (tool.intensity < 0.0 || tool.intensity > 100.0)
						{
							string arg55 = "A tool has an invalid intensity.".Localize();
							string arg56 = string.Format("Tool {0} has an invalid intensity.".Localize(), num2);
							string arg57 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}' -> 'Intensity'".Localize(), num2);
							StyledMessageBox.ShowMessageBox(null, $"{arg55}\n\n{arg56}\n\n{arg57}", "Tool Error".Localize());
							flag = false;
						}
						break;
					default:
					{
						string arg52 = "A tool is not configured.".Localize();
						string arg53 = string.Format("Tool {0} is not configured.".Localize(), num2);
						string arg54 = string.Format("Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool {0}'".Localize(), num2);
						StyledMessageBox.ShowMessageBox(null, $"{arg52}\n\n{arg53}\n\n{arg54}", "Tool Warning".Localize());
						break;
					}
					case TOOL_TYPE.FFF:
						break;
					}
					num2++;
				}
				if (num == 0)
				{
					string arg85 = "You must configure at least one tool.".Localize();
					string arg86 = "There are currently none configured.".Localize();
					string arg87 = "Location: 'Settings & Controls' -> 'Printer' -> 'Tools' -> 'Tool X'".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg85}\n\n{arg86}\n\n{arg87}", "Tool Error".Localize());
					return false;
				}
				if (!flag)
				{
					return false;
				}
				if (GetValue<double>("fill_density") == 1.0 && GetValue("infill_type") != "LINES")
				{
					string arg88 = "Solid Infill works best when set to LINES.".Localize();
					string arg89 = string.Format("It is currently set to {0}.".Localize(), GetValue("infill_type"));
					string arg90 = "Location: 'Settings & Controls' -> 'Settings' -> 'General' -> 'Infill Type'".Localize();
					StyledMessageBox.ShowMessageBox(null, $"{arg88}\n\n{arg89}\n\n{arg90}", "Slice Error".Localize());
					return true;
				}
				string speedLocation = "Location: 'Settings & Controls' -> 'Settings' -> 'General' -> 'Speed'".Localize();
				if (!ValidateGoodSpeedSettingGreaterThan0("bridge_speed", speedLocation))
				{
					return false;
				}
				if (!ValidateGoodSpeedSettingGreaterThan0("external_perimeter_speed", speedLocation))
				{
					return false;
				}
				if (!ValidateGoodSpeedSettingGreaterThan0("first_layer_speed", speedLocation))
				{
					return false;
				}
				if (!ValidateGoodSpeedSettingGreaterThan0("gap_fill_speed", speedLocation))
				{
					return false;
				}
				if (!ValidateGoodSpeedSettingGreaterThan0("infill_speed", speedLocation))
				{
					return false;
				}
				if (!ValidateGoodSpeedSettingGreaterThan0("perimeter_speed", speedLocation))
				{
					return false;
				}
				if (!ValidateGoodSpeedSettingGreaterThan0("small_perimeter_speed", speedLocation))
				{
					return false;
				}
				if (!ValidateGoodSpeedSettingGreaterThan0("solid_infill_speed", speedLocation))
				{
					return false;
				}
				if (!ValidateGoodSpeedSettingGreaterThan0("support_material_speed", speedLocation))
				{
					return false;
				}
				if (!ValidateGoodSpeedSettingGreaterThan0("top_solid_infill_speed", speedLocation))
				{
					return false;
				}
				if (!ValidateGoodSpeedSettingGreaterThan0("travel_speed", speedLocation))
				{
					return false;
				}
				string speedLocation2 = "Location: 'Settings & Controls' -> 'Settings' -> 'Filament' -> 'Filament' -> 'Retraction'".Localize();
				if (!ValidateGoodSpeedSettingGreaterThan0("retract_speed", speedLocation2))
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				ex.StackTrace!.Replace("\r", "");
				return false;
			}
			return true;
		}

		private bool ValidateGoodSpeedSettingGreaterThan0(string speedSetting, string speedLocation)
		{
			string value = GetValue(speedSetting);
			string text = value;
			if (text.EndsWith("%"))
			{
				text = text.Substring(0, text.Length - 1);
			}
			bool flag = true;
			if (!double.TryParse(text, out var result))
			{
				flag = false;
			}
			if (!flag || (ActiveSliceSettings.Instance.Helpers.ActiveSliceEngine().MapContains(speedSetting) && result <= 0.0))
			{
				SliceSettingData settingsData = SliceSettingsOrganizer.Instance.GetSettingsData(speedSetting);
				if (settingsData != null)
				{
					string text2 = string.Format("The '{0}' must be greater than 0.".Localize(), settingsData.PresentationName);
					string text3 = string.Format("It is currently set to {0}.".Localize(), value);
					StyledMessageBox.ShowMessageBox(null, $"{text2}\n\n{text3}\n\n{speedLocation} -> '{settingsData.PresentationName}'", "Slice Error".Localize());
				}
				return false;
			}
			return true;
		}

		private static HashSet<string> LoadSettingsNamesFromPropertiesJson()
		{
			return new HashSet<string>(Enumerable.Select<JToken, string>((IEnumerable<JToken>)JArray.Parse(StaticData.get_Instance().ReadAllText(Path.Combine("SliceSettings", "Properties.json"))), (Func<JToken, string>)((JToken s) => Extensions.Value<string>((IEnumerable<JToken>)s.get_Item((object)"SlicerConfigName")))));
		}

		public void SetValue(string settingsKey, string settingsValue, PrinterSettingsLayer layer = null)
		{
			if (layer != null && layer != UserLayer)
			{
				StashUserOverride(layer, settingsKey);
			}
			else if (StagedUserSettings.ContainsKey(settingsKey))
			{
				StagedUserSettings.Remove(settingsKey);
			}
			PrinterSettingsLayer printerSettingsLayer = layer ?? UserLayer;
			if (!printerSettingsLayer.TryGetValue(settingsKey, out var value) || !(value == settingsValue))
			{
				printerSettingsLayer[settingsKey] = settingsValue;
				Save();
				ActiveSliceSettings.OnSettingChanged(settingsKey);
			}
		}

		public string ToJson(Formatting formatting = 1)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return JsonConvert.SerializeObject((object)this, formatting);
		}

		internal void ClearValue(string settingsKey, PrinterSettingsLayer layer = null)
		{
			PrinterSettingsLayer printerSettingsLayer = layer ?? UserLayer;
			if (printerSettingsLayer.ContainsKey(settingsKey))
			{
				printerSettingsLayer.Remove(settingsKey);
				if (layer != null && layer != UserLayer)
				{
					RestoreUserOverride(layer, settingsKey);
				}
				Save();
				ActiveSliceSettings.OnSettingChanged(settingsKey);
			}
		}
	}
}
