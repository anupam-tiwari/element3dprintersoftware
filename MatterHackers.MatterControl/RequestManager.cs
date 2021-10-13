using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl
{
	public class RequestManager
	{
		private CookieContainer cookies = new CookieContainer();

		public string LastResponse
		{
			get;
			protected set;
		}

		public int Timeout
		{
			get;
			internal set;
		} = 100000;


		internal string GetCookieValue(Uri SiteUri, string name)
		{
			Cookie val = cookies.GetCookies(SiteUri).get_Item(name);
			if (val != null)
			{
				return val.get_Value();
			}
			return null;
		}

		public string GetResponseContent(HttpWebResponse response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			string text = null;
			try
			{
				using Stream stream = ((WebResponse)response).GetResponseStream();
				using StreamReader streamReader = new StreamReader(stream);
				text = streamReader.ReadToEnd();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				((WebResponse)response).Close();
			}
			LastResponse = text;
			return text;
		}

		public HttpWebResponse SendPOSTRequest(string uri, string content, string signIn, string password, bool allowAutoRedirect)
		{
			HttpWebRequest request = GenerateRequest(uri, content, "POST", null, null, allowAutoRedirect);
			return GetResponse(request);
		}

		public HttpWebResponse SendGETRequest(string uri, string signIn, string password, bool allowAutoRedirect)
		{
			HttpWebRequest request = GenerateRequest(uri, null, "GET", null, null, allowAutoRedirect);
			return GetResponse(request);
		}

		internal HttpWebRequest GenerateRequest(string uri, string content, string method, string signIn, string password, bool allowAutoRedirect)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			//IL_00af: Expected O, but got Unknown
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Invalid comparison between Unknown and I4
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			HttpWebRequest val = (HttpWebRequest)WebRequest.Create(uri);
			((WebRequest)val).set_Method(method);
			((WebRequest)val).set_Timeout(Timeout);
			val.set_CookieContainer(cookies);
			val.set_AllowAutoRedirect(false);
			if (string.IsNullOrEmpty(signIn))
			{
				((WebRequest)val).set_Credentials((ICredentials)(object)CredentialCache.get_DefaultNetworkCredentials());
			}
			else
			{
				((WebRequest)val).set_Credentials((ICredentials)new NetworkCredential(signIn, password));
			}
			if (method == "POST")
			{
				byte[] bytes = Encoding.UTF8.GetBytes(content);
				((WebRequest)val).set_ContentType("application/json");
				((WebRequest)val).set_ContentLength((long)bytes.Length);
				try
				{
					Stream requestStream = ((WebRequest)val).GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
					return val;
				}
				catch (WebException val2)
				{
					WebException val3 = val2;
					if ((int)val3.get_Status() == 14)
					{
						LastResponse = JsonConvert.SerializeObject((object)new
						{
							status = "error",
							statuscode = 408
						});
					}
					Console.WriteLine("Web exception occurred. Status code: {0}", (object)val3.get_Status());
					return val;
				}
				catch (IOException ex)
				{
					Console.WriteLine("Web exception occurred. Message: {0}", (object)ex.Message);
					return val;
				}
				catch (Exception e)
				{
					MatterControlApplication.Instance.ReportException(e);
					return val;
				}
			}
			return val;
		}

		internal HttpWebResponse GetResponse(HttpWebRequest request)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_002f: Expected O, but got Unknown
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (request == null)
			{
				return null;
			}
			HttpWebResponse val = null;
			try
			{
				val = (HttpWebResponse)((WebRequest)request).GetResponse();
				cookies.Add(val.get_Cookies());
				GetResponseContent(val);
				return val;
			}
			catch (WebException val2)
			{
				WebException val3 = val2;
				Console.WriteLine("Web exception occurred. Status code: {0}", (object)val3.get_Status());
				return val;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return val;
			}
		}
	}
}
