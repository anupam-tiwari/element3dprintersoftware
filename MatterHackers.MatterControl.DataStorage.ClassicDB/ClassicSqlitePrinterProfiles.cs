using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MatterHackers.MatterControl.ConfigurationPage.PrintLeveling;
using MatterHackers.MatterControl.SlicerConfiguration;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl.DataStorage.ClassicDB
{
	public class ClassicSqlitePrinterProfiles
	{
		public class ClassicSettingsLayer
		{
			public Dictionary<string, SliceSetting> settingsDictionary;

			public SliceSettingsCollection settingsCollectionData;

			public ClassicSettingsLayer(SliceSettingsCollection settingsCollection, Dictionary<string, SliceSetting> settingsDictionary)
			{
				settingsCollectionData = settingsCollection;
				this.settingsDictionary = settingsDictionary;
			}
		}

		public static void ImportPrinters(ProfileManager profileData, string profilePath)
		{
			foreach (Printer item in Datastore.Instance.dbSQLite.Query<Printer>("SELECT * FROM Printer;", Array.Empty<object>()))
			{
				ImportPrinter(item, profileData, profilePath);
			}
		}

		public static void ImportPrinter(Printer printer, ProfileManager profileData, string profilePath)
		{
			PrinterInfo printerInfo = new PrinterInfo
			{
				Name = printer.Name,
				ID = printer.Id.ToString(),
				Make = (printer.Make ?? ""),
				Model = (printer.Model ?? "")
			};
			((Collection<PrinterInfo>)(object)profileData.Profiles).Add(printerInfo);
			PrinterSettings printerSettings = new PrinterSettings();
			printerSettings.OemLayer = LoadOemLayer(printer);
			printerSettings.OemLayer["make"] = printerInfo.Make;
			printerSettings.OemLayer["model"] = printer.Model;
			LoadQualitySettings(printerSettings, printer);
			LoadMaterialSettings(printerSettings, printer);
			printerSettings.ID = printer.Id.ToString();
			printerSettings.UserLayer["printer_name"] = printer.Name ?? "";
			printerSettings.UserLayer["baud_rate"] = printer.BaudRate ?? "";
			printerSettings.UserLayer["auto_connect"] = (printer.AutoConnect ? "1" : "0");
			printerSettings.UserLayer["default_material_presets"] = printer.MaterialCollectionIds ?? "";
			printerSettings.UserLayer["windows_driver"] = printer.DriverType ?? "";
			printerSettings.UserLayer["device_token"] = printer.DeviceToken ?? "";
			printerSettings.UserLayer["device_type"] = printer.DeviceType ?? "";
			if (string.IsNullOrEmpty(ProfileManager.Instance.LastProfileID))
			{
				ProfileManager.Instance.LastProfileID = printer.Id.ToString();
			}
			List<CustomCommands> list = Datastore.Instance.dbSQLite.Query<CustomCommands>("SELECT * FROM CustomCommands WHERE PrinterId = " + printer.Id, Array.Empty<object>());
			printerSettings.Macros = Enumerable.ToList<GCodeMacro>(Enumerable.Select<CustomCommands, GCodeMacro>((IEnumerable<CustomCommands>)list, (Func<CustomCommands, GCodeMacro>)((CustomCommands macro) => new GCodeMacro
			{
				GCode = macro.Value.Trim(),
				Name = macro.Name,
				LastModified = macro.DateLastModified
			})));
			string query = $"SELECT * FROM PrinterSetting WHERE Name = 'PublishBedImage' and PrinterId = {printer.Id};";
			PrinterSetting printerSetting = Enumerable.FirstOrDefault<PrinterSetting>((IEnumerable<PrinterSetting>)Datastore.Instance.dbSQLite.Query<PrinterSetting>(query, Array.Empty<object>()));
			printerSettings.UserLayer["publish_bed_image"] = ((printerSetting?.Value == "true") ? "1" : "0");
			PrintLevelingData printLevelingData = PrintLevelingData.Create(printerSettings, printer.PrintLevelingJsonData, printer.PrintLevelingProbePositions);
			printerSettings.UserLayer["print_leveling_data"] = JsonConvert.SerializeObject((object)printLevelingData);
			printerSettings.UserLayer["print_leveling_enabled"] = (printer.DoPrintLeveling ? "true" : "false");
			printerSettings.UserLayer["manual_movement_speeds"] = printer.ManualMovementSpeeds;
			printerSettings.OemLayer["spiral_vase"] = "";
			printerSettings.OemLayer["bottom_clip_amount"] = "";
			printerSettings.OemLayer["layer_to_pause"] = "";
			printerSettings.ID = printer.Id.ToString();
			printerSettings.DocumentVersion = PrinterSettings.LatestVersion;
			printerSettings.Helpers.SetComPort(printer.ComPort);
			printerSettings.Save();
		}

		private static void LoadMaterialSettings(PrinterSettings layeredProfile, Printer printer)
		{
			_003C_003Ec__DisplayClass3_0 _003C_003Ec__DisplayClass3_ = new _003C_003Ec__DisplayClass3_0();
			_003C_003Ec__DisplayClass3_.printer = printer;
			if (_003C_003Ec__DisplayClass3_.printer.MaterialCollectionIds?.Split(new char[1]
			{
				','
			}) == null)
			{
				return;
			}
			ITableQuery<SliceSettingsCollection> tableQuery = Datastore.Instance.dbSQLite.Table<SliceSettingsCollection>();
			ParameterExpression val = Expression.Parameter(typeof(SliceSettingsCollection), "v");
			foreach (SliceSettingsCollection item in tableQuery.Where(Expression.Lambda<Func<SliceSettingsCollection, bool>>((Expression)(object)Expression.AndAlso((Expression)(object)Expression.Equal((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (Expression)(object)Expression.Property((Expression)(object)Expression.Field((Expression)(object)Expression.Constant((object)_003C_003Ec__DisplayClass3_, typeof(_003C_003Ec__DisplayClass3_0)), FieldInfo.GetFieldFromHandle((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/))), (Expression)(object)Expression.Equal((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (Expression)(object)Expression.Constant((object)"material", typeof(string)))), (ParameterExpression[])(object)new ParameterExpression[1]
			{
				val
			})))
			{
				((Collection<PrinterSettingsLayer>)(object)layeredProfile.MaterialLayers).Add(new PrinterSettingsLayer(LoadSettings(item))
				{
					LayerID = Guid.NewGuid().ToString(),
					Name = item.Name
				});
			}
		}

		public static void LoadQualitySettings(PrinterSettings layeredProfile, Printer printer)
		{
			_003C_003Ec__DisplayClass4_0 _003C_003Ec__DisplayClass4_ = new _003C_003Ec__DisplayClass4_0();
			_003C_003Ec__DisplayClass4_.printer = printer;
			ITableQuery<SliceSettingsCollection> tableQuery = Datastore.Instance.dbSQLite.Table<SliceSettingsCollection>();
			ParameterExpression val = Expression.Parameter(typeof(SliceSettingsCollection), "v");
			foreach (SliceSettingsCollection item in tableQuery.Where(Expression.Lambda<Func<SliceSettingsCollection, bool>>((Expression)(object)Expression.AndAlso((Expression)(object)Expression.Equal((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (Expression)(object)Expression.Property((Expression)(object)Expression.Field((Expression)(object)Expression.Constant((object)_003C_003Ec__DisplayClass4_, typeof(_003C_003Ec__DisplayClass4_0)), FieldInfo.GetFieldFromHandle((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/))), (Expression)(object)Expression.Equal((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (Expression)(object)Expression.Constant((object)"quality", typeof(string)))), (ParameterExpression[])(object)new ParameterExpression[1]
			{
				val
			})))
			{
				((Collection<PrinterSettingsLayer>)(object)layeredProfile.QualityLayers).Add(new PrinterSettingsLayer(LoadSettings(item))
				{
					LayerID = Guid.NewGuid().ToString(),
					Name = item.Name
				});
			}
		}

		public static PrinterSettingsLayer LoadOemLayer(Printer printer)
		{
			SliceSettingsCollection sliceSettingsCollection;
			if (printer.DefaultSettingsCollectionId != 0)
			{
				_003C_003Ec__DisplayClass5_0 _003C_003Ec__DisplayClass5_ = new _003C_003Ec__DisplayClass5_0();
				_003C_003Ec__DisplayClass5_.activePrinterSettingsID = printer.DefaultSettingsCollectionId;
				ITableQuery<SliceSettingsCollection> tableQuery = Datastore.Instance.dbSQLite.Table<SliceSettingsCollection>();
				ParameterExpression val = Expression.Parameter(typeof(SliceSettingsCollection), "v");
				sliceSettingsCollection = tableQuery.Where(Expression.Lambda<Func<SliceSettingsCollection, bool>>((Expression)(object)Expression.Equal((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (Expression)(object)Expression.Field((Expression)(object)Expression.Constant((object)_003C_003Ec__DisplayClass5_, typeof(_003C_003Ec__DisplayClass5_0)), FieldInfo.GetFieldFromHandle((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/))), (ParameterExpression[])(object)new ParameterExpression[1]
				{
					val
				})).Take(1).FirstOrDefault();
			}
			else
			{
				sliceSettingsCollection = new SliceSettingsCollection();
				sliceSettingsCollection.Name = printer.Name;
				sliceSettingsCollection.Commit();
				printer.DefaultSettingsCollectionId = sliceSettingsCollection.Id;
			}
			return new PrinterSettingsLayer(LoadSettings(sliceSettingsCollection));
		}

		private static Dictionary<string, string> LoadSettings(SliceSettingsCollection collection)
		{
			List<SliceSetting> list = Datastore.Instance.dbSQLite.Query<SliceSetting>(string.Format("SELECT * FROM SliceSetting WHERE SettingsCollectionID = " + collection.Id), Array.Empty<object>());
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (SliceSetting item in list)
			{
				dictionary[item.Name] = item.Value;
			}
			return dictionary;
		}
	}
}
