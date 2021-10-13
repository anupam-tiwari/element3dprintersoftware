using System;
using System.Collections.Generic;
using System.Linq;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.SettingsManagement;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl
{
	public class ApplicationSettings
	{
		private static ApplicationSettings globalInstance;

		public Dictionary<string, SystemSetting> settingsDictionary;

		private string oemName;

		private string runningTokensKeyName = string.Format("{0}ClientToken_RunningTokens", "");

		private string claimedClientToken;

		public static string LibraryFilterFileExtensions => ".stl,.amf,.gcode";

		public static string OpenPrintableFileParams => "STL, AMF, ZIP, GCODE|*.stl;*.amf;*.zip;*.gcode";

		public static string OpenDesignFileParams => "STL, AMF, ZIP, GCODE|*.stl;*.amf;*.zip;*.gcode";

		public static ApplicationSettings Instance
		{
			get
			{
				if (globalInstance == null)
				{
					globalInstance = new ApplicationSettings();
					globalInstance.LoadData();
				}
				return globalInstance;
			}
		}

		public string GetOEMName()
		{
			if (oemName == null)
			{
				string[] array = OemSettings.Instance.PrinterWhiteList.ToArray();
				if (array.Length == 0 || array.Length > 1)
				{
					oemName = "Aether";
				}
				else
				{
					oemName = array[0];
				}
			}
			return oemName;
		}

		public string GetClientToken()
		{
			if (!string.IsNullOrEmpty(claimedClientToken))
			{
				return claimedClientToken;
			}
			List<string> allocatedClientTokens = GetAllocatedClientTokens();
			HashSet<string> runningClientTokens = GetRunningClientTokens();
			if (ApplicationController.ApplicationInstanceCount == 1 && !string.IsNullOrEmpty(AuthenticationData.Instance.ActiveClientToken))
			{
				claimedClientToken = AuthenticationData.Instance.ActiveClientToken;
			}
			else
			{
				IEnumerable<string> enumerable = Enumerable.Except<string>((IEnumerable<string>)allocatedClientTokens, (IEnumerable<string>)runningClientTokens);
				claimedClientToken = Enumerable.FirstOrDefault<string>(enumerable);
			}
			if (!string.IsNullOrEmpty(claimedClientToken))
			{
				runningClientTokens.Add(claimedClientToken);
			}
			SetRunningClientTokens(runningClientTokens);
			return claimedClientToken;
		}

		public void ReleaseClientToken()
		{
			HashSet<string> runningClientTokens = GetRunningClientTokens();
			runningClientTokens.Remove(claimedClientToken);
			SetRunningClientTokens(runningClientTokens);
		}

		private List<string> GetAllocatedClientTokens()
		{
			List<string> list = new List<string>();
			int num = 0;
			string text2;
			do
			{
				string text = string.Format("{0}ClientToken", "");
				if (num > 0)
				{
					text = text + "_" + num;
				}
				text2 = get(text);
				if (!string.IsNullOrEmpty(text2))
				{
					list.Add(text2);
				}
				num++;
			}
			while (!string.IsNullOrEmpty(text2));
			return list;
		}

		private HashSet<string> GetRunningClientTokens()
		{
			HashSet<string> result = new HashSet<string>();
			if (ApplicationController.ApplicationInstanceCount > 1)
			{
				try
				{
					string text = get(runningTokensKeyName);
					if (string.IsNullOrEmpty(text))
					{
						return result;
					}
					result = JsonConvert.DeserializeObject<HashSet<string>>(text);
					return result;
				}
				catch
				{
					return result;
				}
			}
			return result;
		}

		private void SetRunningClientTokens(HashSet<string> runningClientTokens)
		{
			set(runningTokensKeyName, JsonConvert.SerializeObject((object)runningClientTokens));
		}

		public void SetClientToken(string clientToken)
		{
			AuthenticationData.Instance.ClearActiveSession();
			int num = 0;
			bool flag = false;
			do
			{
				string text = string.Format("{0}ClientToken", "");
				if (num > 0)
				{
					text = text + "_" + num;
				}
				flag = string.IsNullOrEmpty(get(text));
				if (flag)
				{
					set(text, clientToken);
				}
				num++;
			}
			while (!flag);
		}

		public string get(string key)
		{
			if (settingsDictionary == null)
			{
				globalInstance.LoadData();
			}
			if (settingsDictionary.ContainsKey(key))
			{
				return settingsDictionary[key].Value;
			}
			return null;
		}

		public void set(string key, string value)
		{
			SystemSetting systemSetting;
			if (settingsDictionary.ContainsKey(key))
			{
				systemSetting = settingsDictionary[key];
			}
			else
			{
				systemSetting = new SystemSetting();
				systemSetting.Name = key;
				settingsDictionary[key] = systemSetting;
			}
			systemSetting.Value = value;
			systemSetting.Commit();
		}

		private void LoadData()
		{
			settingsDictionary = new Dictionary<string, SystemSetting>();
			foreach (SystemSetting applicationSetting in GetApplicationSettings())
			{
				settingsDictionary[applicationSetting.Name] = applicationSetting;
			}
		}

		private IEnumerable<SystemSetting> GetApplicationSettings()
		{
			return Datastore.Instance.dbSQLite.Query<SystemSetting>("SELECT * FROM SystemSetting;", Array.Empty<object>());
		}
	}
}
