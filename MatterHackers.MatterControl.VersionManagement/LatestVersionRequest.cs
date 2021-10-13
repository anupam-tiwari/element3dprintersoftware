using System;

namespace MatterHackers.MatterControl.VersionManagement
{
	public class LatestVersionRequest : WebRequestBase
	{
		public static class VersionKey
		{
			public const string CurrentBuildToken = "CurrentBuildToken";

			public const string CurrentBuildNumber = "CurrentBuildNumber";

			public const string CurrentBuildUrl = "CurrentBuildUrl";

			public const string CurrentReleaseVersion = "CurrentReleaseVersion";

			public const string CurrentReleaseDate = "CurrentReleaseDate";

			public const string UpdateRequired = "UpdateRequired";
		}

		public static string[] VersionKeys = new string[6]
		{
			"CurrentBuildToken",
			"CurrentBuildNumber",
			"CurrentBuildUrl",
			"CurrentReleaseVersion",
			"CurrentReleaseDate",
			"UpdateRequired"
		};

		public LatestVersionRequest()
		{
			string text = UserSettings.Instance.get("UpdateFeedType");
			if (text == null)
			{
				text = "release";
				UserSettings.Instance.set("UpdateFeedType", text);
			}
			requestValues["ProjectToken"] = VersionInfo.Instance.ProjectToken;
			requestValues["UpdateFeedType"] = text;
			uri = $"{MatterControlApplication.MCWSBaseUri}/api/1/get-current-release-version";
		}

		public override void Request()
		{
			if (ApplicationSettings.Instance.GetClientToken() == null)
			{
				ClientTokenRequest clientTokenRequest = new ClientTokenRequest();
				clientTokenRequest.RequestSucceeded += onRequestSucceeded;
				clientTokenRequest.Request();
			}
			else
			{
				onClientTokenReady();
			}
		}

		private void onRequestSucceeded(object sender, EventArgs e)
		{
			onClientTokenReady();
		}

		public void onClientTokenReady()
		{
			string clientToken = ApplicationSettings.Instance.GetClientToken();
			requestValues["ClientToken"] = clientToken;
			if (clientToken != null)
			{
				base.Request();
			}
		}

		public override void ProcessSuccessResponse(JsonResponseDictionary responseValues)
		{
			string[] versionKeys = VersionKeys;
			foreach (string key in versionKeys)
			{
				saveResponse(key, responseValues);
			}
		}

		private void saveResponse(string key, JsonResponseDictionary responseValues)
		{
			string text = responseValues.get(key);
			if (text != null)
			{
				ApplicationSettings.Instance.set(key, text);
			}
		}
	}
}
