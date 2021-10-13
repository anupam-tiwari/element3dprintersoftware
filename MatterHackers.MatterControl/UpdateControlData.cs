using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.AboutPage;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.SettingsManagement;
using MatterHackers.MatterControl.VersionManagement;

namespace MatterHackers.MatterControl
{
	public class UpdateControlData
	{
		public enum UpdateRequestType
		{
			UserRequested,
			Automatic,
			FirstTimeEver
		}

		public enum UpdateStatusStates
		{
			MayBeAvailable,
			CheckingForUpdate,
			UpdateAvailable,
			UpdateDownloading,
			ReadyToInstall,
			UpToDate,
			UnableToConnectToServer,
			UpdateRequired
		}

		private WebClient webClient;

		private int downloadPercent;

		private int downloadSize;

		private UpdateRequestType updateRequestType;

		public RootedObjectEventHandler UpdateStatusChanged = new RootedObjectEventHandler();

		private static string applicationDataPath = ApplicationDataStorage.ApplicationUserDataPath;

		private static string updateFileLocation = Path.Combine(applicationDataPath, "updates");

		private UpdateStatusStates updateStatus;

		private static bool haveShowUpdateRequired = false;

		private static UpdateControlData instance;

		private int downloadAttempts;

		private string updateFileName;

		public static EventHandler InstallUpdateFromMainActivity = null;

		public int DownloadPercent => downloadPercent;

		public UpdateStatusStates UpdateStatus => updateStatus;

		private string InstallerExtension
		{
			get
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Invalid comparison between Unknown and I4
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Invalid comparison between Unknown and I4
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Invalid comparison between Unknown and I4
				if ((int)OsInformation.get_OperatingSystem() == 2)
				{
					return "pkg";
				}
				if ((int)OsInformation.get_OperatingSystem() == 3)
				{
					return "tar.gz";
				}
				if ((int)OsInformation.get_OperatingSystem() == 5)
				{
					return "apk";
				}
				return "exe";
			}
		}

		public static UpdateControlData Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new UpdateControlData();
				}
				return instance;
			}
		}

		public bool UpdateRequired
		{
			get
			{
				if (updateStatus == UpdateStatusStates.UpdateAvailable)
				{
					return ApplicationSettings.Instance.get("UpdateRequired") == "True";
				}
				return false;
			}
			private set
			{
			}
		}

		private bool WaitingToCompleteTransaction()
		{
			UpdateStatusStates updateStatusStates = UpdateStatus;
			if (updateStatusStates == UpdateStatusStates.CheckingForUpdate || updateStatusStates == UpdateStatusStates.UpdateDownloading)
			{
				return true;
			}
			return false;
		}

		private void CheckVersionStatus()
		{
			string text = ApplicationSettings.Instance.get("CurrentBuildToken");
			string text2 = Path.Combine(updateFileLocation, $"{text}.{InstallerExtension}");
			if (VersionInfo.Instance.BuildToken == text || text == null)
			{
				SetUpdateStatus(UpdateStatusStates.MayBeAvailable);
			}
			else if (File.Exists(text2))
			{
				SetUpdateStatus(UpdateStatusStates.ReadyToInstall);
			}
			else
			{
				SetUpdateStatus(UpdateStatusStates.UpdateAvailable);
			}
		}

		private void SetUpdateStatus(UpdateStatusStates updateStatus)
		{
			if (this.updateStatus == updateStatus)
			{
				return;
			}
			this.updateStatus = updateStatus;
			OnUpdateStatusChanged(null);
			if (UpdateRequired && !haveShowUpdateRequired)
			{
				haveShowUpdateRequired = true;
				if (!UserSettings.Instance.IsTouchScreen)
				{
					UiThread.RunOnIdle((Action)CheckForUpdateWindow.Show);
				}
			}
		}

		public void CheckForUpdateUserRequested()
		{
			updateRequestType = UpdateRequestType.UserRequested;
			CheckForUpdate();
		}

		private void CheckForUpdate()
		{
			if (!WaitingToCompleteTransaction())
			{
				SetUpdateStatus(UpdateStatusStates.CheckingForUpdate);
				LatestVersionRequest latestVersionRequest = new LatestVersionRequest();
				latestVersionRequest.RequestSucceeded += onVersionRequestSucceeded;
				latestVersionRequest.RequestFailed += onVersionRequestFailed;
				latestVersionRequest.Request();
			}
		}

		private void onVersionRequestSucceeded(object sender, EventArgs e)
		{
			string updateAvailableMessage = "There is a recommended update available for MatterControl. Would you like to download it now?".Localize();
			string updateAvailableTitle = "Recommended Update Available".Localize();
			string downloadNow = "Download Now".Localize();
			string remindMeLater = "Remind Me Later".Localize();
			string text = ApplicationSettings.Instance.get("CurrentBuildToken");
			string text2 = Path.Combine(updateFileLocation, $"{text}.{InstallerExtension}");
			if (VersionInfo.Instance.BuildToken == text)
			{
				SetUpdateStatus(UpdateStatusStates.UpToDate);
				return;
			}
			if (File.Exists(text2))
			{
				SetUpdateStatus(UpdateStatusStates.ReadyToInstall);
				return;
			}
			SetUpdateStatus(UpdateStatusStates.UpdateAvailable);
			if (updateRequestType == UpdateRequestType.FirstTimeEver)
			{
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(ProcessDialogResponse, updateAvailableMessage, updateAvailableTitle, StyledMessageBox.MessageType.YES_NO, downloadNow, remindMeLater);
				});
			}
		}

		private void ProcessDialogResponse(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				InitiateUpdateDownload();
				GuiWidget obj = FindNamedWidgetRecursive((GuiWidget)(object)ApplicationController.Instance.MainView, "About Tab");
				Tab val = obj as Tab;
				if (val != null)
				{
					val.get_TabBarContaningTab().SelectTab(val);
				}
			}
		}

		private static GuiWidget FindNamedWidgetRecursive(GuiWidget root, string name)
		{
			foreach (GuiWidget item in (Collection<GuiWidget>)(object)root.get_Children())
			{
				if (item.get_Name() == name)
				{
					return item;
				}
				GuiWidget val = FindNamedWidgetRecursive(item, name);
				if (val != null)
				{
					return val;
				}
			}
			return null;
		}

		private void onVersionRequestFailed(object sender, ResponseErrorEventArgs e)
		{
			SetUpdateStatus(UpdateStatusStates.UpToDate);
		}

		public void InitiateUpdateDownload()
		{
			downloadAttempts = 0;
			DownloadUpdate();
		}

		private void DownloadUpdate()
		{
			new Thread((ThreadStart)delegate
			{
				DownloadUpdateTask();
			}).Start();
		}

		private void DownloadUpdateTask()
		{
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Expected O, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Expected O, but got Unknown
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Expected O, but got Unknown
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Expected O, but got Unknown
			if (WaitingToCompleteTransaction())
			{
				return;
			}
			string text = ApplicationSettings.Instance.get("CurrentBuildToken");
			if (text != null)
			{
				downloadAttempts++;
				SetUpdateStatus(UpdateStatusStates.UpdateDownloading);
				string text2 = string.Format("{0}/downloads/development/{1}", MatterControlApplication.MCWSBaseUri, ApplicationSettings.Instance.get("CurrentBuildToken"));
				WebRequest val = WebRequest.Create(text2);
				val.set_Method("HEAD");
				try
				{
					WebResponse response = val.GetResponse();
					downloadSize = (int)response.get_ContentLength();
				}
				catch
				{
					downloadSize = 0;
				}
				if (!Directory.Exists(updateFileLocation))
				{
					Directory.CreateDirectory(updateFileLocation);
				}
				updateFileName = Path.Combine(updateFileLocation, $"{text}.{InstallerExtension}");
				webClient = new WebClient();
				webClient.add_DownloadFileCompleted(new AsyncCompletedEventHandler(DownloadCompleted));
				webClient.add_DownloadProgressChanged(new DownloadProgressChangedEventHandler(DownloadProgressChanged));
				try
				{
					webClient.DownloadFileAsync(new Uri(text2), updateFileName);
				}
				catch
				{
				}
			}
		}

		private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			if (downloadSize > 0)
			{
				downloadPercent = (int)(e.get_BytesReceived() * 100 / downloadSize);
			}
			UiThread.RunOnIdle((Action)delegate
			{
				UpdateStatusChanged.CallEvents((object)this, (EventArgs)(object)e);
			});
		}

		private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (e.get_Error() != null)
			{
				if (File.Exists(updateFileName))
				{
					File.Delete(updateFileName);
				}
				if (downloadAttempts == 1)
				{
					DownloadUpdate();
				}
				else
				{
					UiThread.RunOnIdle((Action)delegate
					{
						SetUpdateStatus(UpdateStatusStates.UnableToConnectToServer);
					});
				}
			}
			else
			{
				UiThread.RunOnIdle((Action)delegate
				{
					SetUpdateStatus(UpdateStatusStates.ReadyToInstall);
				});
			}
			((Component)webClient).Dispose();
		}

		private UpdateControlData()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			CheckVersionStatus();
			if (ApplicationSettings.Instance.GetClientToken() != null || OemSettings.Instance.CheckForUpdatesOnFirstRun)
			{
				if (ApplicationSettings.Instance.GetClientToken() == null)
				{
					updateRequestType = UpdateRequestType.FirstTimeEver;
				}
				else
				{
					updateRequestType = UpdateRequestType.Automatic;
				}
				CheckForUpdate();
				return;
			}
			ITableQuery<ApplicationSession> tableQuery = Datastore.Instance.dbSQLite.Table<ApplicationSession>();
			ParameterExpression val = Expression.Parameter(typeof(ApplicationSession), "v");
			ApplicationSession applicationSession = tableQuery.OrderBy<DateTime>(Expression.Lambda<Func<ApplicationSession, DateTime>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1]
			{
				val
			})).Take(1).FirstOrDefault();
			if (applicationSession != null && DateTime.Compare(applicationSession.SessionStart.AddDays(7.0), DateTime.Now) < 0)
			{
				SetUpdateStatus(UpdateStatusStates.UpdateAvailable);
			}
		}

		public void OnUpdateStatusChanged(EventArgs e)
		{
			UpdateStatusChanged.CallEvents((object)this, e);
		}

		public bool InstallUpdate()
		{
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			string text = ApplicationSettings.Instance.get("CurrentBuildToken");
			string text2 = Path.Combine(updateFileLocation, StringHelper.FormatWith("{0}.{1}", new object[2]
			{
				text,
				InstallerExtension
			}));
			string text3 = ApplicationSettings.Instance.get("CurrentReleaseVersion");
			string text4 = Path.Combine(updateFileLocation, StringHelper.FormatWith("MatterControlSetup-{0}.{1}", new object[2]
			{
				text3,
				InstallerExtension
			}));
			if (File.Exists(text4))
			{
				File.Delete(text4);
			}
			try
			{
				File.Move(text2, text4);
				int num = 0;
				do
				{
					Thread.Sleep(10);
				}
				while (num++ < 100 && !File.Exists(text4));
				Process val = new Process();
				val.get_StartInfo().set_FileName(text4);
				val.Start();
				SystemWindow val2 = (SystemWindow)(object)MatterControlApplication.Instance;
				if (val2 != null)
				{
					((GuiWidget)val2).CloseOnIdle();
					return true;
				}
			}
			catch
			{
				if (File.Exists(text4))
				{
					File.Delete(text4);
				}
			}
			return false;
		}
	}
}
