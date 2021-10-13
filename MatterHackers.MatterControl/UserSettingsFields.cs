using System.Collections.Generic;
using MatterHackers.MatterControl.SettingsManagement;

namespace MatterHackers.MatterControl
{
	public class UserSettingsFields
	{
		private List<string> acceptableTrueFalseValues = new List<string>
		{
			"true",
			"false"
		};

		private string StartCountKey = "StartCount";

		private string StartCountDurringExitKey = "StartCountDurringExit";

		private string IsSimpleModeKey = "IsSimpleMode";

		public bool IsSimpleMode
		{
			get
			{
				return GetBool(IsSimpleModeKey, OemSettings.Instance.UseSimpleModeByDefault);
			}
			set
			{
				SetBool(IsSimpleModeKey, value);
			}
		}

		public int StartCount
		{
			get
			{
				return GetInt(StartCountKey);
			}
			set
			{
				SetInt(StartCountKey, value);
			}
		}

		public int StartCountDurringExit
		{
			get
			{
				return GetInt(StartCountDurringExitKey);
			}
			set
			{
				SetInt(StartCountDurringExitKey, value);
			}
		}

		public void SetBool(string keyToSet, bool value)
		{
			if (value)
			{
				UserSettings.Instance.set(keyToSet, "true");
			}
			else
			{
				UserSettings.Instance.set(keyToSet, "false");
			}
		}

		public bool GetBool(string keyToRead, bool defaultValue)
		{
			string text = UserSettings.Instance.get(keyToRead);
			if (acceptableTrueFalseValues.IndexOf(text) == -1)
			{
				text = ((!defaultValue) ? "false" : "true");
				UserSettings.Instance.set(keyToRead, text);
			}
			return text == "true";
		}

		public void SetInt(string keyToSet, int value)
		{
			UserSettings.Instance.set(keyToSet, value.ToString());
		}

		public int GetInt(string keyToRead, int defaultValue = 0)
		{
			string text = UserSettings.Instance.get(keyToRead);
			int result = 0;
			if (!int.TryParse(text, out result))
			{
				result = defaultValue;
				UserSettings.Instance.set(keyToRead, text);
			}
			return result;
		}
	}
}
