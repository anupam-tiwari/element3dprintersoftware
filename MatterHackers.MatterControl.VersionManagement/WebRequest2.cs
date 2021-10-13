using Newtonsoft.Json;

namespace MatterHackers.MatterControl.VersionManagement
{
	public class WebRequest2<RequestType> : WebRequestBase where RequestType : class
	{
		private RequestType localRequestValues;

		public void Request(string requestUrl, RequestType requestValues)
		{
			uri = requestUrl;
			localRequestValues = requestValues;
			Request();
		}

		protected override string getJsonToSend()
		{
			return JsonConvert.SerializeObject((object)localRequestValues);
		}
	}
}
