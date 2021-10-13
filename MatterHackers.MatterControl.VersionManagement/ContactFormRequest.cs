using System;

namespace MatterHackers.MatterControl.VersionManagement
{
	internal class ContactFormRequest : WebRequestBase
	{
		public ContactFormRequest(string question, string details, string email, string firstName, string lastName)
		{
			requestValues["FirstName"] = firstName;
			requestValues["LastName"] = lastName;
			requestValues["Email"] = email;
			requestValues["FeedbackType"] = "Question";
			requestValues["Comment"] = $"{question}\n{details}";
			uri = $"{MatterControlApplication.MCWSBaseUri}/api/1/submit-feedback";
		}

		public override void ProcessSuccessResponse(JsonResponseDictionary responseValues)
		{
		}

		public override void Request()
		{
			if (ApplicationSettings.Instance.GetClientToken() == null)
			{
				ClientTokenRequest clientTokenRequest = new ClientTokenRequest();
				clientTokenRequest.RequestSucceeded += onClientTokenRequestSucceeded;
				clientTokenRequest.Request();
			}
			else
			{
				onClientTokenReady();
			}
		}

		private void onClientTokenRequestSucceeded(object sender, EventArgs e)
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
	}
}
