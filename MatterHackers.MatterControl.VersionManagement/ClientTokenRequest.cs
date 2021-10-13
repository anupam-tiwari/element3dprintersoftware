namespace MatterHackers.MatterControl.VersionManagement
{
	public class ClientTokenRequest : WebRequestBase
	{
		public ClientTokenRequest()
		{
			requestValues["RequestToken"] = "ekshdsd5d5ssss5kels";
			requestValues["ProjectToken"] = VersionInfo.Instance.ProjectToken;
			uri = $"{MatterControlApplication.MCWSBaseUri}/api/1/get-client-consumer-token";
		}

		public override void ProcessSuccessResponse(JsonResponseDictionary responseValues)
		{
			string text = responseValues.get("ClientToken");
			if (text != null)
			{
				ApplicationSettings.Instance.SetClientToken(text);
			}
		}
	}
}
