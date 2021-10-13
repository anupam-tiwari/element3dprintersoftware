using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg.PlatformAbstract;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class SliceSettingsOrganizer
	{
		private static Dictionary<string, string> defaultSettings;

		private static SliceSettingsOrganizer instance;

		public Dictionary<string, OrganizerUserLevel> UserLevels
		{
			get;
			set;
		} = new Dictionary<string, OrganizerUserLevel>();


		public static SliceSettingsOrganizer Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SliceSettingsOrganizer();
				}
				return instance;
			}
		}

		private SliceSettingsOrganizer()
		{
			LoadAndParseSettingsFiles();
		}

		public bool Contains(string userLevel, string slicerConfigName)
		{
			foreach (OrganizerCategory categories in UserLevels[userLevel].CategoriesList)
			{
				foreach (OrganizerGroup groups in categories.GroupsList)
				{
					foreach (OrganizerSubGroup subGroups in groups.SubGroupsList)
					{
						foreach (SliceSettingData settingData in subGroups.SettingDataList)
						{
							if (settingData.SlicerConfigName == slicerConfigName)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public SliceSettingData GetSettingsData(string slicerConfigName)
		{
			foreach (SliceSettingData settingsDatum in ActiveSliceSettings.SettingsData)
			{
				if (settingsDatum.SlicerConfigName == slicerConfigName)
				{
					return settingsDatum;
				}
			}
			return null;
		}

		private void LoadAndParseSettingsFiles()
		{
			OrganizerUserLevel organizerUserLevel = null;
			OrganizerCategory organizerCategory = null;
			OrganizerGroup organizerGroup = null;
			OrganizerSubGroup organizerSubGroup = null;
			string[] array = StaticData.get_Instance().ReadAllLines(Path.Combine("SliceSettings", "Layouts.txt"));
			foreach (string text in array)
			{
				if (text.Length <= 0)
				{
					continue;
				}
				string text2 = text.Replace('"', ' ').Trim();
				switch (CountLeadingSpaces(text))
				{
				case 0:
					organizerUserLevel = new OrganizerUserLevel(text2);
					UserLevels.Add(text2, organizerUserLevel);
					break;
				case 2:
					organizerCategory = new OrganizerCategory(text2);
					organizerUserLevel.CategoriesList.Add(organizerCategory);
					break;
				case 4:
					organizerGroup = new OrganizerGroup(text2);
					organizerCategory.GroupsList.Add(organizerGroup);
					break;
				case 6:
					organizerSubGroup = new OrganizerSubGroup(text2);
					organizerGroup.SubGroupsList.Add(organizerSubGroup);
					break;
				case 8:
				{
					SliceSettingData settingsData = GetSettingsData(text2);
					if (settingsData != null)
					{
						organizerSubGroup.SettingDataList.Add(settingsData);
					}
					break;
				}
				default:
					throw new Exception("Bad file, too many spaces (must be 0, 2, 4 or 6).");
				}
			}
		}

		private static int CountLeadingSpaces(string line)
		{
			int i;
			for (i = 0; line[i] == ' ' && i < line.Length; i++)
			{
			}
			return i;
		}

		public PrinterSettingsLayer GetDefaultSettings()
		{
			if (defaultSettings == null)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (SliceSettingData settingsDatum in ActiveSliceSettings.SettingsData)
				{
					dictionary[settingsDatum.SlicerConfigName] = settingsDatum.DefaultValue;
				}
				defaultSettings = dictionary;
			}
			return new PrinterSettingsLayer(defaultSettings);
		}
	}
}
