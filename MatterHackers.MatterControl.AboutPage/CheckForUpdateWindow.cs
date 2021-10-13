using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;

namespace MatterHackers.MatterControl.AboutPage
{
	public class CheckForUpdateWindow : SystemWindow
	{
		private static CheckForUpdateWindow checkUpdate;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		private DropDownList releaseOptionsDropList;

		private TextWidget stableInfoLabel;

		private TextWidget alphaInfoLabel;

		private TextWidget betaInfoLabel;

		private TextWidget updateChannelLabel;

		private TextWidget currentBuildInfo;

		public CheckForUpdateWindow()
			: this(540.0, 350.0)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Expected O, but got Unknown
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Expected O, but got Unknown
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Expected O, but got Unknown
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Expected O, but got Unknown
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Expected O, but got Unknown
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Expected O, but got Unknown
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Expected O, but got Unknown
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Expected O, but got Unknown
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Expected O, but got Unknown
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Expected O, but got Unknown
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_068c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0696: Expected O, but got Unknown
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0730: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_073c: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Expected O, but got Unknown
			//IL_0754: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b8: Expected O, but got Unknown
			//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0845: Unknown result type (might be due to invalid IL or missing references)
			//IL_093d: Unknown result type (might be due to invalid IL or missing references)
			linkButtonFactory.fontSize = 10.0;
			linkButtonFactory.textColor = ActiveTheme.get_Instance().get_SecondaryAccentColor();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)this).set_Padding(new BorderDouble(5.0, 0.0, 5.0, 0.0));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)8);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_VAnchor((VAnchor)8);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 5.0, 0.0, 0.0));
			((GuiWidget)val3).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			TextWidget val4 = new TextWidget("Check for Update".Localize(), 0.0, 0.0, 20.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			if (UpdateControlData.Instance.UpdateRequired)
			{
				val4 = new TextWidget("Update Required".Localize(), 0.0, 0.0, 20.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			}
			val4.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val4).set_Margin(new BorderDouble(2.0, 10.0, 10.0, 5.0));
			UpdateControlView updateControlView = new UpdateControlView();
			TextWidget val5 = new TextWidget("Update Channel".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val5.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val5).set_VAnchor((VAnchor)2);
			((GuiWidget)val5).set_Margin(new BorderDouble(5.0, 0.0, 0.0, 0.0));
			releaseOptionsDropList = new DropDownList("Development", (Direction)1, 200.0, false);
			((GuiWidget)releaseOptionsDropList).set_HAnchor((HAnchor)5);
			releaseOptionsDropList.AddItem("Stable".Localize(), "release", 12.0).add_Selected((EventHandler)FixTabDot);
			releaseOptionsDropList.AddItem("Beta".Localize(), "pre-release", 12.0).add_Selected((EventHandler)FixTabDot);
			releaseOptionsDropList.AddItem("Alpha".Localize(), "development", 12.0).add_Selected((EventHandler)FixTabDot);
			List<string> obj = new List<string>
			{
				"release",
				"pre-release",
				"development"
			};
			string item = UserSettings.Instance.get("UpdateFeedType");
			if (obj.IndexOf(item) == -1)
			{
				UserSettings.Instance.set("UpdateFeedType", "release");
			}
			releaseOptionsDropList.set_SelectedValue(UserSettings.Instance.get("UpdateFeedType"));
			releaseOptionsDropList.add_SelectionChanged((EventHandler)ReleaseOptionsDropList_SelectionChanged);
			string buildVersion = VersionInfo.Instance.BuildVersion;
			string englishString = $"Current Build : {buildVersion}";
			currentBuildInfo = new TextWidget(englishString.Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)currentBuildInfo).set_HAnchor((HAnchor)5);
			((GuiWidget)currentBuildInfo).set_Margin(new BorderDouble(5.0, 15.0, 0.0, 20.0));
			currentBuildInfo.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			FlowLayoutWidget additionalInfoContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)additionalInfoContainer).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)additionalInfoContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)additionalInfoContainer).set_Padding(new BorderDouble(6.0, 0.0, 0.0, 6.0));
			string text = "Changing your update channel will change the version of MatterControl  \nthat you receive when updating:";
			updateChannelLabel = new TextWidget(text, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			updateChannelLabel.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)updateChannelLabel).set_HAnchor((HAnchor)5);
			((GuiWidget)updateChannelLabel).set_Margin(new BorderDouble(0.0, 20.0, 0.0, 0.0));
			((GuiWidget)additionalInfoContainer).AddChild((GuiWidget)(object)updateChannelLabel, -1);
			string text2 = "Stable: The current release version of MatterControl (recommended).".Localize();
			stableInfoLabel = new TextWidget(text2, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			stableInfoLabel.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)stableInfoLabel).set_HAnchor((HAnchor)5);
			((GuiWidget)stableInfoLabel).set_Margin(new BorderDouble(0.0, 10.0, 0.0, 0.0));
			((GuiWidget)additionalInfoContainer).AddChild((GuiWidget)(object)stableInfoLabel, -1);
			string text3 = "Beta: The release candidate version of MatterControl.".Localize();
			betaInfoLabel = new TextWidget(text3, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			betaInfoLabel.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)betaInfoLabel).set_HAnchor((HAnchor)5);
			((GuiWidget)betaInfoLabel).set_Margin(new BorderDouble(0.0, 10.0, 0.0, 0.0));
			((GuiWidget)additionalInfoContainer).AddChild((GuiWidget)(object)betaInfoLabel, -1);
			string text4 = "Alpha: The in development version of MatterControl.".Localize();
			alphaInfoLabel = new TextWidget(text4, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			alphaInfoLabel.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)alphaInfoLabel).set_HAnchor((HAnchor)5);
			((GuiWidget)alphaInfoLabel).set_Margin(new BorderDouble(0.0, 10.0, 0.0, 0.0));
			((GuiWidget)additionalInfoContainer).AddChild((GuiWidget)(object)alphaInfoLabel, -1);
			FlowLayoutWidget val6 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val6).set_HAnchor((HAnchor)5);
			((GuiWidget)val6).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			Button val7 = textImageButtonFactory.Generate("Close".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val7).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				((GuiWidget)this).CloseOnIdle();
			});
			Button val8 = linkButtonFactory.Generate("What's this?".Localize());
			((GuiWidget)val8).set_VAnchor((VAnchor)2);
			((GuiWidget)val8).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					if (!((GuiWidget)additionalInfoContainer).get_Visible())
					{
						((GuiWidget)additionalInfoContainer).set_Visible(true);
					}
					else
					{
						((GuiWidget)additionalInfoContainer).set_Visible(false);
					}
				});
			});
			((GuiWidget)val2).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)updateControlView, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)currentBuildInfo, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val5, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val8, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)releaseOptionsDropList, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)additionalInfoContainer, -1);
			((GuiWidget)val6).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val6).AddChild((GuiWidget)(object)val7, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new VerticalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val6, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)additionalInfoContainer).set_Visible(false);
			if (UpdateControlData.Instance.UpdateRequired)
			{
				((SystemWindow)this).set_Title("Update Required".Localize());
			}
			else
			{
				((SystemWindow)this).set_Title("Check for Update".Localize());
			}
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((SystemWindow)this).ShowAsSystemWindow();
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
		}

		public static void Show()
		{
			if (checkUpdate == null)
			{
				checkUpdate = new CheckForUpdateWindow();
				((GuiWidget)checkUpdate).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					checkUpdate = null;
				});
			}
			else
			{
				((GuiWidget)checkUpdate).BringToFront();
			}
		}

		private void ReleaseOptionsDropList_SelectionChanged(object sender, EventArgs e)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			string selectedValue = ((DropDownList)sender).get_SelectedValue();
			if (selectedValue != UserSettings.Instance.get("UpdateFeedType"))
			{
				UserSettings.Instance.set("UpdateFeedType", selectedValue);
			}
		}

		private void FixTabDot(object sender, EventArgs e)
		{
			UpdateControlData.Instance.CheckForUpdateUserRequested();
		}
	}
}
