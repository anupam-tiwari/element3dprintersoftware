using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.ConfigurationPage
{
	public class CloudSettingsWidget : SettingsViewBase
	{
		private DisableableWidget notificationSettingsContainer;

		private DisableableWidget cloudSyncContainer;

		private Button configureNotificationSettingsButton;

		public static Action enableCloudMonitorFunction;

		private TextWidget notificationSettingsLabel;

		public static Action openUserDashBoardFunction;

		private EventHandler unregisterEvents;

		public static Action openPrintNotificationFunction;

		public CloudSettingsWidget()
			: base("Cloud".Localize())
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
			notificationSettingsContainer = new DisableableWidget();
			((GuiWidget)notificationSettingsContainer).AddChild((GuiWidget)(object)GetNotificationControls(), -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)notificationSettingsContainer, -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
			cloudSyncContainer = new DisableableWidget();
			((GuiWidget)cloudSyncContainer).AddChild((GuiWidget)(object)GetCloudSyncDashboardControls(), -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)cloudSyncContainer, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)mainContainer, -1);
			AddHandlers();
		}

		private void SetDisplayAttributes()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Margin(new BorderDouble(2.0, 4.0, 2.0, 0.0));
			textImageButtonFactory.normalFillColor = RGBA_Bytes.White;
			textImageButtonFactory.disabledFillColor = RGBA_Bytes.White;
			textImageButtonFactory.FixedHeight = TallButtonHeight;
			textImageButtonFactory.fontSize = 11.0;
			textImageButtonFactory.disabledTextColor = RGBA_Bytes.DarkGray;
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.normalTextColor = RGBA_Bytes.Black;
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			linkButtonFactory.fontSize = 11.0;
		}

		private void enableCloudMonitor_Click(object sender, EventArgs mouseEvent)
		{
			if (enableCloudMonitorFunction != null)
			{
				UiThread.RunOnIdle(enableCloudMonitorFunction);
			}
		}

		private FlowLayoutWidget GetCloudSyncDashboardControls()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Expected O, but got Unknown
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Expected O, but got Unknown
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Expected O, but got Unknown
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)(((GuiWidget)val).get_HAnchor() | 5));
			((GuiWidget)val).set_VAnchor((VAnchor)(((GuiWidget)val).get_VAnchor() | 2));
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			ImageBuffer val2 = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("cloud-24x24.png"));
			val2.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
			GuiWidget.get_DeviceScale();
			if (!ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val2);
			}
			ImageWidget val3 = new ImageWidget(val2);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0, 6.0, 0.0));
			((GuiWidget)val3).set_VAnchor((VAnchor)2);
			TextWidget val4 = new TextWidget("Cloud Sync".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_AutoExpandBoundsToText(true);
			val4.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			linkButtonFactory.fontSize = 10.0;
			Button val5 = linkButtonFactory.Generate("Go to Dashboard".Localize().ToUpper());
			((GuiWidget)val5).set_ToolTipText("Open cloud sync dashboard in web browser".Localize());
			((GuiWidget)val5).add_Click((EventHandler<MouseEventArgs>)cloudSyncGoButton_Click);
			((GuiWidget)val5).set_VAnchor((VAnchor)2);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
			HorizontalSpacer horizontalSpacer = new HorizontalSpacer();
			((GuiWidget)horizontalSpacer).set_VAnchor((VAnchor)2);
			((GuiWidget)val).AddChild((GuiWidget)(object)horizontalSpacer, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val5, -1);
			return val;
		}

		private FlowLayoutWidget GetNotificationControls()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Expected O, but got Unknown
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Expected O, but got Unknown
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Expected O, but got Unknown
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Expected O, but got Unknown
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)(((GuiWidget)val).get_HAnchor() | 5));
			((GuiWidget)val).set_VAnchor((VAnchor)(((GuiWidget)val).get_VAnchor() | 2));
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			textImageButtonFactory.FixedHeight = TallButtonHeight;
			ImageBuffer val2 = ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("notify-24x24.png"));
			val2.SetRecieveBlender((IRecieveBlenderByte)new BlenderPreMultBGRA());
			GuiWidget.get_DeviceScale();
			if (!ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				ExtensionMethods.InvertLightness(val2);
			}
			ImageWidget val3 = new ImageWidget(val2);
			((GuiWidget)val3).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0, 6.0, 0.0));
			configureNotificationSettingsButton = textImageButtonFactory.Generate("Configure".Localize().ToUpper());
			((GuiWidget)configureNotificationSettingsButton).set_Name("Configure Notification Settings Button");
			((GuiWidget)configureNotificationSettingsButton).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			((GuiWidget)configureNotificationSettingsButton).set_VAnchor((VAnchor)2);
			((GuiWidget)configureNotificationSettingsButton).add_Click((EventHandler<MouseEventArgs>)configureNotificationSettingsButton_Click);
			notificationSettingsLabel = new TextWidget("Notifications".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			notificationSettingsLabel.set_AutoExpandBoundsToText(true);
			notificationSettingsLabel.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)notificationSettingsLabel).set_VAnchor((VAnchor)2);
			GuiWidget val4 = (GuiWidget)new FlowLayoutWidget((FlowDirection)0);
			val4.set_VAnchor((VAnchor)2);
			val4.set_Margin(new BorderDouble(16.0, 0.0, 0.0, 0.0));
			CheckBox enablePrintNotificationsSwitch = ImageButtonFactory.CreateToggleSwitch(UserSettings.Instance.get("PrintNotificationsEnabled") == "true");
			((GuiWidget)enablePrintNotificationsSwitch).set_VAnchor((VAnchor)2);
			enablePrintNotificationsSwitch.add_CheckedStateChanged((EventHandler)delegate
			{
				UserSettings.Instance.set("PrintNotificationsEnabled", enablePrintNotificationsSwitch.get_Checked() ? "true" : "false");
			});
			val4.AddChild((GuiWidget)(object)enablePrintNotificationsSwitch, -1);
			val4.SetBoundsToEncloseChildren();
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)notificationSettingsLabel, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)configureNotificationSettingsButton, -1);
			((GuiWidget)val).AddChild(val4, -1);
			return val;
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		private void cloudSyncGoButton_Click(object sender, EventArgs mouseEvent)
		{
			if (openUserDashBoardFunction != null)
			{
				UiThread.RunOnIdle(openUserDashBoardFunction);
			}
		}

		private void AddHandlers()
		{
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.EnableChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
		}

		private void onPrinterStatusChanged(object sender, EventArgs e)
		{
			((GuiWidget)this).Invalidate();
		}

		private void configureNotificationSettingsButton_Click(object sender, EventArgs mouseEvent)
		{
			if (openPrintNotificationFunction != null)
			{
				UiThread.RunOnIdle(openPrintNotificationFunction);
			}
		}
	}
}
