using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintHistory;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.ConfigurationPage
{
	public class ApplicationSettingsWidget : SettingsViewBase
	{
		private Button languageRestartButton;

		private Button configureUpdateFeedButton;

		public DropDownList releaseOptionsDropList;

		private string cannotRestartWhilePrintIsActiveMessage;

		private string cannotRestartWhileActive;

		private string rebuildThumbnailsMessage = "You are switching to a different thumbnail rendering mode. If you want, your current thumbnails can be removed and recreated in the new style. You can switch back and forth at any time. There will be some processing overhead while the new thumbnails are created.\n\nDo you want to rebuild your existing thumbnails now?".Localize();

		private string rebuildThumbnailsTitle = "Rebuild Thumbnails Now".Localize();

		public ApplicationSettingsWidget()
			: base("Application".Localize())
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			cannotRestartWhilePrintIsActiveMessage = "Oops! You cannot restart while a print is active.".Localize();
			cannotRestartWhileActive = "Unable to restart".Localize();
			if (UserSettings.Instance.IsTouchScreen)
			{
				((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
			}
			if (UserSettings.Instance.IsTouchScreen)
			{
				((GuiWidget)mainContainer).AddChild((GuiWidget)(object)GetUpdateControl(), -1);
				((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
			}
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)GetLanguageControl(), -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)GetThumbnailRenderingControl(), -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)new HorizontalLine(separatorLineColor), -1);
			((GuiWidget)mainContainer).AddChild((GuiWidget)(object)GetClearHistoryControl(), -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)mainContainer, -1);
			AddHandlers();
		}

		private FlowLayoutWidget GetClearHistoryControl()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(3.0, 4.0));
			TextWidget val2 = new TextWidget("Clear Print History".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			Button val3 = textImageButtonFactory.Generate("Remove All".Localize().ToUpper());
			((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)clearHistoryButton_Click);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			return val;
		}

		private void clearHistoryButton_Click(object sender, EventArgs e)
		{
			PrintHistoryData.Instance.ClearHistory();
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

		private FlowLayoutWidget GetThemeControl()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Expected O, but got Unknown
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 6.0));
			TextWidget val2 = new TextWidget("Theme".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_HAnchor((HAnchor)1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 4.0));
			GuiWidget val4 = new GuiWidget();
			val4.set_VAnchor((VAnchor)5);
			val4.set_Padding(new BorderDouble(5.0));
			val4.set_Width(80.0);
			val4.set_BackgroundColor(RGBA_Bytes.White);
			GuiWidget val5 = new GuiWidget();
			val5.set_HAnchor((HAnchor)5);
			val5.set_VAnchor((VAnchor)5);
			val5.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			val4.AddChild(val5, -1);
			ThemeColorSelectorWidget themeColorSelectorWidget = new ThemeColorSelectorWidget(val5);
			((GuiWidget)themeColorSelectorWidget).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
			((GuiWidget)val3).AddChild((GuiWidget)(object)themeColorSelectorWidget, -1);
			((GuiWidget)val3).AddChild(val4, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			return val;
		}

		private FlowLayoutWidget GetDisplayControl()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Expected O, but got Unknown
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Expected O, but got Unknown
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 4.0));
			TextWidget val2 = new TextWidget("Display Mode".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)4);
			Button displayControlRestartButton = textImageButtonFactory.Generate("Restart".Localize());
			((GuiWidget)displayControlRestartButton).set_VAnchor((VAnchor)2);
			((GuiWidget)displayControlRestartButton).set_Visible(false);
			((GuiWidget)displayControlRestartButton).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			((GuiWidget)displayControlRestartButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting)
				{
					StyledMessageBox.ShowMessageBox(null, cannotRestartWhilePrintIsActiveMessage, cannotRestartWhileActive);
				}
				else
				{
					RestartApplication();
				}
			});
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0, 0.0, 0.0));
			DropDownList val4 = new DropDownList("Development", (Direction)1, 200.0, false);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val3).set_Width(200.0);
			val4.AddItem("Normal".Localize(), "responsive", 12.0);
			val4.AddItem("Touchscreen".Localize(), "touchscreen", 12.0);
			List<string> obj = new List<string>
			{
				"responsive",
				"touchscreen"
			};
			string item = UserSettings.Instance.get("ApplicationDisplayMode");
			if (obj.IndexOf(item) == -1)
			{
				UserSettings.Instance.set("ApplicationDisplayMode", "responsive");
			}
			val4.set_SelectedValue(UserSettings.Instance.get("ApplicationDisplayMode"));
			val4.add_SelectionChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				string selectedValue = ((DropDownList)sender).get_SelectedValue();
				if (selectedValue != UserSettings.Instance.get("ApplicationDisplayMode"))
				{
					UserSettings.Instance.set("ApplicationDisplayMode", selectedValue);
					((GuiWidget)displayControlRestartButton).set_Visible(true);
				}
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)displayControlRestartButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			return val;
		}

		private FlowLayoutWidget GetModeControl()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Expected O, but got Unknown
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 4.0));
			TextWidget val2 = new TextWidget("Interface Mode".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)4);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0, 0.0, 0.0));
			DropDownList val4 = new DropDownList("Standard", (Direction)1, 200.0, false);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val3).set_Width(200.0);
			val4.AddItem("Standard".Localize(), "True", 12.0);
			val4.AddItem("Advanced".Localize(), "False", 12.0);
			val4.set_SelectedValue(UserSettings.Instance.Fields.IsSimpleMode.ToString());
			val4.add_SelectionChanged((EventHandler)InterfaceModeDropList_SelectionChanged);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			return val;
		}

		private FlowLayoutWidget GetUpdateControl()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Expected O, but got Unknown
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected O, but got Unknown
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Expected O, but got Unknown
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 4.0));
			configureUpdateFeedButton = textImageButtonFactory.Generate("Configure".Localize().ToUpper());
			((GuiWidget)configureUpdateFeedButton).set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			((GuiWidget)configureUpdateFeedButton).set_VAnchor((VAnchor)2);
			TextWidget val2 = new TextWidget("Update Notification Feed".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)4);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0, 0.0, 0.0));
			releaseOptionsDropList = new DropDownList("Development", (Direction)1, 200.0, false);
			((GuiWidget)releaseOptionsDropList).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).AddChild((GuiWidget)(object)releaseOptionsDropList, -1);
			((GuiWidget)val3).set_Width(200.0);
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
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			return val;
		}

		private FlowLayoutWidget GetLanguageControl()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected O, but got Unknown
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 4.0));
			TextWidget val2 = new TextWidget("Language".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)4);
			((GuiWidget)new FlowLayoutWidget((FlowDirection)0)).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0, 0.0, 0.0));
			LanguageSelector languageSelector = new LanguageSelector();
			((DropDownList)languageSelector).add_SelectionChanged((EventHandler)LanguageDropList_SelectionChanged);
			((GuiWidget)languageSelector).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).AddChild((GuiWidget)(object)languageSelector, -1);
			((GuiWidget)val3).set_Width(200.0);
			languageRestartButton = textImageButtonFactory.Generate("Restart".Localize());
			((GuiWidget)languageRestartButton).set_VAnchor((VAnchor)2);
			((GuiWidget)languageRestartButton).set_Visible(false);
			((GuiWidget)languageRestartButton).set_Margin(new BorderDouble(0.0, 0.0, 6.0, 0.0));
			((GuiWidget)languageRestartButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting)
				{
					StyledMessageBox.ShowMessageBox(null, cannotRestartWhilePrintIsActiveMessage, cannotRestartWhileActive);
				}
				else
				{
					RestartApplication();
				}
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)languageRestartButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			return val;
		}

		private FlowLayoutWidget GetSliceEngineControl()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected O, but got Unknown
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 4.0));
			TextWidget val2 = new TextWidget("Slice Engine".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)4);
			((GuiWidget)new FlowLayoutWidget((FlowDirection)0)).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0, 0.0, 0.0));
			PrinterSettings instance = ActiveSliceSettings.Instance;
			if (instance != null && instance.GetValue<int>("extruder_count") > 1 && instance.Helpers.ActiveSliceEngineType() != SlicingEngineTypes.MatterSlice)
			{
				instance.Helpers.ActiveSliceEngineType(SlicingEngineTypes.MatterSlice);
				ApplicationController.Instance.ReloadAll();
			}
			((GuiWidget)val3).AddChild((GuiWidget)(object)new SliceEngineSelector("Slice Engine".Localize()), -1);
			((GuiWidget)val3).set_Width(200.0);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			return val;
		}

		private FlowLayoutWidget GetThumbnailRenderingControl()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 4.0));
			TextWidget val2 = new TextWidget("Thumbnail Rendering".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_AutoExpandBoundsToText(true);
			val2.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).set_VAnchor((VAnchor)4);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 6.0, 0.0, 0.0));
			DropDownList val4 = new DropDownList("Development", (Direction)1, 200.0, false);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val3).set_Width(200.0);
			val4.AddItem("Flat".Localize(), "orthographic", 12.0);
			val4.AddItem("3D".Localize(), "raytraced", 12.0);
			List<string> obj = new List<string>
			{
				"orthographic",
				"raytraced"
			};
			string item = UserSettings.Instance.get("ThumbnailRenderingMode");
			if (obj.IndexOf(item) == -1)
			{
				if (!UserSettings.Instance.IsTouchScreen)
				{
					UserSettings.Instance.set("ThumbnailRenderingMode", "orthographic");
				}
				else
				{
					UserSettings.Instance.set("ThumbnailRenderingMode", "raytraced");
				}
			}
			val4.set_SelectedValue(UserSettings.Instance.get("ThumbnailRenderingMode"));
			val4.add_SelectionChanged((EventHandler)delegate(object sender, EventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				string selectedValue = ((DropDownList)sender).get_SelectedValue();
				if (selectedValue != UserSettings.Instance.get("ThumbnailRenderingMode"))
				{
					UserSettings.Instance.set("ThumbnailRenderingMode", selectedValue);
					Action<bool> removeThumbnails = delegate(bool shouldRebuildThumbnails)
					{
						if (shouldRebuildThumbnails)
						{
							string text = PartThumbnailWidget.ThumbnailPath();
							try
							{
								if (Directory.Exists(text))
								{
									Directory.Delete(text, true);
								}
							}
							catch (Exception)
							{
							}
						}
						ApplicationController.Instance.ReloadAll();
					};
					UiThread.RunOnIdle((Action)delegate
					{
						StyledMessageBox.ShowMessageBox(removeThumbnails, rebuildThumbnailsMessage, rebuildThumbnailsTitle, StyledMessageBox.MessageType.YES_NO, "Rebuild".Localize());
					});
				}
			});
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			return val;
		}

		private void AddHandlers()
		{
		}

		private void RestartApplication()
		{
			UiThread.RunOnIdle((Action)delegate
			{
				GuiWidget val = (GuiWidget)(object)this;
				while (val.get_Parent() != null)
				{
					val = val.get_Parent();
				}
				MatterControlApplication obj = ((Collection<GuiWidget>)(object)val.get_Children())[0] as MatterControlApplication;
				obj.RestartOnClose = true;
				((GuiWidget)obj).Close();
			});
		}

		private void FixTabDot(object sender, EventArgs e)
		{
			UpdateControlData.Instance.CheckForUpdateUserRequested();
		}

		private void InterfaceModeDropList_SelectionChanged(object sender, EventArgs e)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if (((DropDownList)sender).get_SelectedValue() == "True")
			{
				UserSettings.Instance.Fields.IsSimpleMode = true;
			}
			else
			{
				UserSettings.Instance.Fields.IsSimpleMode = false;
			}
			ApplicationController.Instance.ReloadAll();
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

		private void LanguageDropList_SelectionChanged(object sender, EventArgs e)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			string selectedValue = ((DropDownList)sender).get_SelectedValue();
			if (selectedValue != UserSettings.Instance.get("Language"))
			{
				UserSettings.Instance.set("Language", selectedValue);
				((GuiWidget)languageRestartButton).set_Visible(true);
				_ = selectedValue == "L10N";
			}
		}

		[Conditional("DEBUG")]
		private void GenerateLocalizationValidationFile()
		{
			char currentChar = 'A';
			string path = StaticData.get_Instance().MapPath(Path.Combine("Translations", "L10N", "Translation.txt"));
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			using StreamWriter streamWriter = new StreamWriter(path);
			string[] array = File.ReadAllLines(StaticData.get_Instance().MapPath(Path.Combine("Translations", "Master.txt")));
			foreach (string text in array)
			{
				if (text.StartsWith("Translated:"))
				{
					int num = text.IndexOf(':');
					string[] array2 = new string[2]
					{
						text.Substring(0, num),
						text.Substring(num + 1)
					};
					streamWriter.WriteLine("{0}:{1}", array2[0], new string(Enumerable.ToArray<char>(Enumerable.Select<char, char>((IEnumerable<char>)array2[1].ToCharArray(), (Func<char, char>)((char c) => (c != ' ') ? currentChar : ' ')))));
					if (currentChar++ == 'Z')
					{
						currentChar = 'A';
					}
				}
				else
				{
					streamWriter.WriteLine(text);
				}
			}
		}
	}
}
