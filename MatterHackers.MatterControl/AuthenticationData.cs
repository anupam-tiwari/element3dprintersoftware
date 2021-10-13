using System;
using System.IO;
using System.Text.RegularExpressions;
using MatterHackers.Agg;
using MatterHackers.Localizations;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl
{
	public class AuthenticationData
	{
		public RootedObjectEventHandler SessionUpdateTrigger = new RootedObjectEventHandler();

		private static int failedRequestCount = int.MaxValue;

		private string activeSessionKey;

		private string activeSessionUsername;

		private string activeSessionEmail;

		private string lastSessionUsername;

		public bool IsConnected
		{
			get
			{
				if (failedRequestCount > 5)
				{
					return false;
				}
				return true;
			}
		}

		public static AuthenticationData Instance
		{
			get;
		} = new AuthenticationData();


		public bool ClientAuthenticatedSessionValid
		{
			get
			{
				if (!string.IsNullOrEmpty(ActiveSessionKey))
				{
					return UserSettings.Instance.get("CredentialsInvalid") != "true";
				}
				return false;
			}
		}

		public string ActiveSessionKey
		{
			get
			{
				return activeSessionKey;
			}
			private set
			{
				activeSessionKey = value;
				ApplicationSettings.Instance.set(string.Format("{0}ActiveSessionKey", ""), value);
			}
		}

		public string ActiveSessionUsername
		{
			get
			{
				if (!string.IsNullOrEmpty(ActiveSessionKey))
				{
					return activeSessionUsername;
				}
				return null;
			}
			private set
			{
				activeSessionUsername = value;
				ApplicationSettings.Instance.set(string.Format("{0}ActiveSessionUsername", ""), value);
			}
		}

		public string ActiveSessionEmail
		{
			get
			{
				return activeSessionEmail;
			}
			private set
			{
				activeSessionEmail = value;
				ApplicationSettings.Instance.set(string.Format("{0}ActiveSessionEmail", ""), value);
			}
		}

		public string ActiveClientToken
		{
			get
			{
				return ApplicationSettings.Instance.get(string.Format("{0}ActiveClientToken", ""));
			}
			private set
			{
				ApplicationSettings.Instance.set(string.Format("{0}ActiveClientToken", ""), value);
			}
		}

		public string LastSessionUsername
		{
			get
			{
				return lastSessionUsername;
			}
			set
			{
				lastSessionUsername = value;
				ApplicationSettings.Instance.set(string.Format("{0}LastSessionUsername", ""), value);
			}
		}

		[JsonIgnore]
		public string FileSystemSafeUserName => MakeValidFileName(ActiveSessionUsername);

		public void SetOffline()
		{
			failedRequestCount = 6;
		}

		public void WebRequestFailed()
		{
			failedRequestCount++;
		}

		public void WebRequestSucceeded()
		{
			failedRequestCount = 0;
		}

		public AuthenticationData()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			activeSessionKey = ApplicationSettings.Instance.get(string.Format("{0}ActiveSessionKey", ""));
			activeSessionUsername = ApplicationSettings.Instance.get(string.Format("{0}ActiveSessionUsername", ""));
			activeSessionEmail = ApplicationSettings.Instance.get(string.Format("{0}ActiveSessionEmail", ""));
			lastSessionUsername = ApplicationSettings.Instance.get(string.Format("{0}LastSessionUsername", ""));
		}

		public void SessionRefresh()
		{
			SessionUpdateTrigger.CallEvents((object)null, (EventArgs)null);
		}

		public void ClearActiveSession()
		{
			ActiveSessionKey = null;
			ActiveSessionUsername = null;
			ActiveSessionEmail = null;
			ActiveClientToken = null;
			ApplicationController.Instance.ChangeCloudSyncStatus(userAuthenticated: false, "Session Cleared".Localize());
			SessionUpdateTrigger.CallEvents((object)null, (EventArgs)null);
		}

		public void SetActiveSession(string userName, string userEmail, string sessionKey, string clientToken)
		{
			ActiveSessionKey = sessionKey;
			ActiveSessionUsername = userName;
			ActiveSessionEmail = userEmail;
			ActiveClientToken = clientToken;
			ApplicationController.Instance.ChangeCloudSyncStatus(userAuthenticated: true);
			SessionUpdateTrigger.CallEvents((object)null, (EventArgs)null);
		}

		private static string MakeValidFileName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return name;
			}
			string arg = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
			string text = string.Format("([{0}]*\\.+$)|([{0}]+)", arg);
			return Regex.Replace(name, text, "_");
		}
	}
}
