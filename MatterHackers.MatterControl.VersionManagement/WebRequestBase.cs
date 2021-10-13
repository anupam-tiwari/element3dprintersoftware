using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using MatterHackers.Localizations;
using Newtonsoft.Json;

namespace MatterHackers.MatterControl.VersionManagement
{
	public class WebRequestBase<ResponseType> where ResponseType : class
	{
		protected Dictionary<string, string> requestValues;

		protected string uri;

		public int Timeout
		{
			get;
			set;
		} = 100000;


		public event EventHandler RequestComplete;

		public event EventHandler<ResponseErrorEventArgs> RequestFailed;

		public event EventHandler<ResponseSuccessEventArgs<ResponseType>> RequestSucceeded;

		public WebRequestBase()
		{
			requestValues = new Dictionary<string, string>();
		}

		public static void Request(string requestUrl, string[] requestStringPairs)
		{
			WebRequestBase<ResponseType> webRequestBase = new WebRequestBase<ResponseType>();
			webRequestBase.SetRquestValues(requestUrl, requestStringPairs);
			webRequestBase.Request();
		}

		public void SetRquestValues(string requestUrl, string[] requestStringPairs)
		{
			uri = requestUrl;
			for (int i = 0; i < requestStringPairs.Length; i += 2)
			{
				requestValues[requestStringPairs[i]] = requestStringPairs[i + 1];
			}
		}

		public virtual void ProcessErrorResponse(JsonResponseDictionary responseValues)
		{
			string text = responseValues.get("ErrorMessage");
			if (text != null)
			{
				Console.WriteLine($"Request Failed: {text}");
			}
			else
			{
				Console.WriteLine($"Request Failed: Unknown Reason");
			}
		}

		public async void Request()
		{
			await Task.Run(new Action(SendRequest));
		}

		protected void OnRequestComplete()
		{
			if (this.RequestComplete != null)
			{
				this.RequestComplete(this, null);
			}
		}

		protected void OnRequestFailed(JsonResponseDictionary responseValues)
		{
			if (this.RequestFailed != null)
			{
				this.RequestFailed(this, new ResponseErrorEventArgs
				{
					ResponseValues = responseValues
				});
			}
			ApplicationController.WebRequestFailed?.Invoke();
		}

		protected void OnRequestSuceeded(ResponseType responseItem)
		{
			this.RequestSucceeded?.Invoke(this, new ResponseSuccessEventArgs<ResponseType>
			{
				ResponseItem = responseItem
			});
			ApplicationController.WebRequestSucceeded?.Invoke();
		}

		protected void SendRequest()
		{
			RequestManager requestManager = new RequestManager();
			requestManager.Timeout = Timeout;
			string text = JsonConvert.SerializeObject((object)requestValues);
			Trace.Write(string.Format("ServiceRequest: {0}\r\n  {1}\r\n", uri, string.Join("\r\n\t", text.Split(new char[1]
			{
				','
			}))));
			requestManager.SendPOSTRequest(uri, text, "", "", allowAutoRedirect: false);
			ResponseType val = null;
			JsonResponseDictionary responseValues = null;
			if (requestManager.LastResponse != null)
			{
				try
				{
					val = JsonConvert.DeserializeObject<ResponseType>(requestManager.LastResponse);
				}
				catch
				{
					responseValues = JsonConvert.DeserializeObject<JsonResponseDictionary>(requestManager.LastResponse);
				}
			}
			if (val != null)
			{
				OnRequestSuceeded(val);
			}
			else
			{
				OnRequestFailed(responseValues);
			}
			OnRequestComplete();
		}
	}
	public class WebRequestBase
	{
		protected Dictionary<string, string> requestValues;

		protected string uri;

		public int Timeout
		{
			get;
			set;
		} = 100000;


		public event EventHandler RequestComplete;

		public event EventHandler<ResponseErrorEventArgs> RequestFailed;

		public event EventHandler RequestSucceeded;

		public WebRequestBase()
		{
			requestValues = new Dictionary<string, string>();
		}

		public static void Request(string requestUrl, string[] requestStringPairs)
		{
			WebRequestBase webRequestBase = new WebRequestBase();
			webRequestBase.uri = requestUrl;
			for (int i = 0; i < requestStringPairs.Length; i += 2)
			{
				webRequestBase.requestValues[requestStringPairs[i]] = requestStringPairs[i + 1];
			}
			webRequestBase.Request();
		}

		public virtual void ProcessErrorResponse(JsonResponseDictionary responseValues)
		{
			string text = responseValues.get("ErrorMessage");
			if (text != null)
			{
				Console.WriteLine($"Request Failed: {text}");
			}
			else
			{
				Console.WriteLine($"Request Failed: Unknown Reason");
			}
		}

		public virtual void ProcessSuccessResponse(JsonResponseDictionary responseValues)
		{
		}

		public virtual void Request()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			BackgroundWorker val = new BackgroundWorker();
			val.add_DoWork(new DoWorkEventHandler(SendRequest));
			val.add_RunWorkerCompleted(new RunWorkerCompletedEventHandler(ProcessResponse));
			val.RunWorkerAsync();
		}

		protected virtual string getJsonToSend()
		{
			return SerializeObject(requestValues);
		}

		protected void OnRequestComplete()
		{
			if (this.RequestComplete != null)
			{
				this.RequestComplete(this, null);
			}
		}

		protected void OnRequestFailed(JsonResponseDictionary responseValues)
		{
			if (this.RequestFailed != null)
			{
				this.RequestFailed(this, new ResponseErrorEventArgs
				{
					ResponseValues = responseValues
				});
			}
		}

		protected void OnRequestSuceeded()
		{
			if (this.RequestSucceeded != null)
			{
				this.RequestSucceeded(this, null);
			}
		}

		protected virtual void ProcessResponse(object sender, RunWorkerCompletedEventArgs e)
		{
			JsonResponseDictionary jsonResponseDictionary = e.get_Result() as JsonResponseDictionary;
			if (jsonResponseDictionary != null)
			{
				string text = jsonResponseDictionary.get("Status");
				if (jsonResponseDictionary != null && text != null && text == "success")
				{
					ProcessSuccessResponse(jsonResponseDictionary);
					OnRequestSuceeded();
				}
				else
				{
					ProcessErrorResponse(jsonResponseDictionary);
					OnRequestFailed(jsonResponseDictionary);
				}
				OnRequestComplete();
			}
		}

		protected virtual void SendRequest(object sender, DoWorkEventArgs e)
		{
			RequestManager requestManager = new RequestManager
			{
				Timeout = Timeout
			};
			string jsonToSend = getJsonToSend();
			Trace.Write(string.Format("ServiceRequest: {0}\r\n  {1}\r\n", uri, string.Join("\r\n\t", jsonToSend.Split(new char[1]
			{
				','
			}))));
			requestManager.SendPOSTRequest(uri, jsonToSend, "", "", allowAutoRedirect: false);
			JsonResponseDictionary jsonResponseDictionary;
			if (requestManager.LastResponse == null)
			{
				jsonResponseDictionary = new JsonResponseDictionary();
				jsonResponseDictionary["Status"] = "error";
				jsonResponseDictionary["ErrorMessage"] = "Unable to connect to server";
				jsonResponseDictionary["ErrorCode"] = "00";
				ApplicationController.WebRequestFailed?.Invoke();
			}
			else
			{
				try
				{
					jsonResponseDictionary = JsonConvert.DeserializeObject<JsonResponseDictionary>(requestManager.LastResponse);
					if (jsonResponseDictionary.TryGetValue("ErrorMessage", out var value) && value.IndexOf("expired session", StringComparison.OrdinalIgnoreCase) != -1)
					{
						ApplicationController.Instance.ChangeCloudSyncStatus(userAuthenticated: false, "Session Expired".Localize());
					}
					ApplicationController.WebRequestSucceeded?.Invoke();
				}
				catch
				{
					jsonResponseDictionary = new JsonResponseDictionary();
					jsonResponseDictionary["Status"] = "error";
					jsonResponseDictionary["ErrorMessage"] = "Unexpected response";
					jsonResponseDictionary["ErrorCode"] = "01";
				}
			}
			e.set_Result((object)jsonResponseDictionary);
		}

		protected string SerializeObject(object requestObject)
		{
			return JsonConvert.SerializeObject(requestObject);
		}
	}
}
