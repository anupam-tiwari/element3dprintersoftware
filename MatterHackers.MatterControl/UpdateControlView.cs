using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class UpdateControlView : FlowLayoutWidget
	{
		private Button downloadUpdateLink;

		private Button checkUpdateLink;

		private Button installUpdateLink;

		private TextWidget updateStatusText;

		private EventHandler unregisterEvents;

		private RGBA_Bytes offWhite = new RGBA_Bytes(245, 245, 245);

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private string recommendedUpdateAvailable = "There is a recommended update available.".Localize();

		private string requiredUpdateAvailable = "There is a required update available.".Localize();

		public UpdateControlView()
			: this((FlowDirection)0)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Expected O, but got Unknown
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			textImageButtonFactory.normalFillColor = RGBA_Bytes.Gray;
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_TransparentDarkOverlay());
			((GuiWidget)this).set_Padding(new BorderDouble(6.0, 5.0));
			updateStatusText = new TextWidget(string.Format(""), 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			updateStatusText.set_AutoExpandBoundsToText(true);
			((GuiWidget)updateStatusText).set_VAnchor((VAnchor)2);
			checkUpdateLink = textImageButtonFactory.Generate("Check for Update".Localize());
			((GuiWidget)checkUpdateLink).set_VAnchor((VAnchor)2);
			((GuiWidget)checkUpdateLink).add_Click((EventHandler<MouseEventArgs>)CheckForUpdate);
			((GuiWidget)checkUpdateLink).set_Visible(false);
			downloadUpdateLink = textImageButtonFactory.Generate("Download Update".Localize());
			((GuiWidget)downloadUpdateLink).set_VAnchor((VAnchor)2);
			((GuiWidget)downloadUpdateLink).add_Click((EventHandler<MouseEventArgs>)DownloadUpdate);
			((GuiWidget)downloadUpdateLink).set_Visible(false);
			installUpdateLink = textImageButtonFactory.Generate("Install Update".Localize());
			((GuiWidget)installUpdateLink).set_VAnchor((VAnchor)2);
			((GuiWidget)installUpdateLink).add_Click((EventHandler<MouseEventArgs>)InstallUpdate);
			((GuiWidget)installUpdateLink).set_Visible(false);
			((GuiWidget)this).AddChild((GuiWidget)(object)updateStatusText, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)checkUpdateLink, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)downloadUpdateLink, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)installUpdateLink, -1);
			UpdateControlData.Instance.UpdateStatusChanged.RegisterEvent((EventHandler)UpdateStatusChanged, ref unregisterEvents);
			((GuiWidget)this).set_MinimumSize(new Vector2(0.0, 50.0));
			UpdateStatusChanged(null, null);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		public void CheckForUpdate(object sender, EventArgs e)
		{
			UpdateControlData.Instance.CheckForUpdateUserRequested();
		}

		public void InstallUpdate(object sender, EventArgs e)
		{
			try
			{
				if (!UpdateControlData.Instance.InstallUpdate())
				{
					((GuiWidget)installUpdateLink).set_Visible(false);
					((GuiWidget)updateStatusText).set_Text(string.Format("Oops! Unable to install update.".Localize()));
				}
			}
			catch
			{
				((GuiWidget)installUpdateLink).set_Visible(false);
				((GuiWidget)updateStatusText).set_Text(string.Format("Oops! Unable to install update.".Localize()));
			}
		}

		public void DownloadUpdate(object sender, EventArgs e)
		{
			((GuiWidget)downloadUpdateLink).set_Visible(false);
			((GuiWidget)updateStatusText).set_Text(string.Format("Retrieving download info...".Localize()));
			UpdateControlData.Instance.InitiateUpdateDownload();
		}

		private void UpdateStatusChanged(object sender, EventArgs e)
		{
			switch (UpdateControlData.Instance.UpdateStatus)
			{
			case UpdateControlData.UpdateStatusStates.MayBeAvailable:
				((GuiWidget)updateStatusText).set_Text(string.Format("New updates may be available.".Localize()));
				((GuiWidget)checkUpdateLink).set_Visible(true);
				break;
			case UpdateControlData.UpdateStatusStates.CheckingForUpdate:
				((GuiWidget)updateStatusText).set_Text("Checking for updates...".Localize());
				((GuiWidget)checkUpdateLink).set_Visible(false);
				break;
			case UpdateControlData.UpdateStatusStates.UnableToConnectToServer:
				((GuiWidget)updateStatusText).set_Text("Oops! Unable to connect to server.".Localize());
				((GuiWidget)downloadUpdateLink).set_Visible(false);
				((GuiWidget)installUpdateLink).set_Visible(false);
				((GuiWidget)checkUpdateLink).set_Visible(true);
				break;
			case UpdateControlData.UpdateStatusStates.UpdateAvailable:
				if (UpdateControlData.Instance.UpdateRequired)
				{
					((GuiWidget)updateStatusText).set_Text(requiredUpdateAvailable);
				}
				else
				{
					((GuiWidget)updateStatusText).set_Text(recommendedUpdateAvailable);
				}
				((GuiWidget)downloadUpdateLink).set_Visible(true);
				((GuiWidget)installUpdateLink).set_Visible(false);
				((GuiWidget)checkUpdateLink).set_Visible(false);
				break;
			case UpdateControlData.UpdateStatusStates.UpdateDownloading:
			{
				string text = "Downloading updates...".Localize();
				text = StringHelper.FormatWith("{0} {1}%", new object[2]
				{
					text,
					UpdateControlData.Instance.DownloadPercent
				});
				((GuiWidget)updateStatusText).set_Text(text);
				break;
			}
			case UpdateControlData.UpdateStatusStates.ReadyToInstall:
				((GuiWidget)updateStatusText).set_Text(string.Format("New updates are ready to install.".Localize()));
				((GuiWidget)downloadUpdateLink).set_Visible(false);
				((GuiWidget)installUpdateLink).set_Visible(true);
				((GuiWidget)checkUpdateLink).set_Visible(false);
				break;
			case UpdateControlData.UpdateStatusStates.UpToDate:
				((GuiWidget)updateStatusText).set_Text(string.Format("Your application is up-to-date.".Localize()));
				((GuiWidget)downloadUpdateLink).set_Visible(false);
				((GuiWidget)installUpdateLink).set_Visible(false);
				((GuiWidget)checkUpdateLink).set_Visible(true);
				break;
			default:
				throw new NotImplementedException();
			}
		}
	}
}
