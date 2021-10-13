using System;
using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.DataStorage;

namespace MatterHackers.MatterControl
{
	public class UserSettings
	{
		private static UserSettings globalInstance = null;

		private static readonly object syncRoot = new object();

		private Dictionary<string, UserSetting> settingsDictionary;

		public static UserSettings Instance
		{
			get
			{
				if (globalInstance == null)
				{
					lock (syncRoot)
					{
						if (globalInstance == null)
						{
							globalInstance = new UserSettings();
							ToolTipManager.set_AllowToolTips(!Instance.IsTouchScreen);
						}
					}
				}
				return globalInstance;
			}
		}

		public string Language
		{
			get;
			private set;
		}

		public UserSettingsFields Fields
		{
			get;
			private set;
		} = new UserSettingsFields();


		public bool IsTouchScreen => get("ApplicationDisplayMode") == "touchscreen";

		private UserSettings()
		{
			settingsDictionary = new Dictionary<string, UserSetting>();
			foreach (UserSetting item in Datastore.Instance.dbSQLite.Query<UserSetting>("SELECT * FROM UserSetting;", Array.Empty<object>()))
			{
				settingsDictionary[item.Name] = item;
			}
			if (string.IsNullOrEmpty(get("Language")))
			{
				set("Language", "en");
			}
			Language = get("Language");
		}

		public string get(string key)
		{
			if (settingsDictionary.TryGetValue(key, out var value))
			{
				return value.Value;
			}
			return null;
		}

		public void set(string key, string value)
		{
			if (!settingsDictionary.TryGetValue(key, out var value2))
			{
				value2 = new UserSetting
				{
					Name = key
				};
				settingsDictionary[key] = value2;
			}
			if (key == "Language")
			{
				Language = value;
			}
			value2.Value = value;
			value2.Commit();
		}
	}
}
