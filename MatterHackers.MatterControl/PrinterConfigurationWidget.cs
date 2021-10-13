using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.ConfigurationPage;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class PrinterConfigurationWidget : GuiWidget
	{
		private readonly int TallButtonHeight = 25;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		private EventHandler unregisterEvents;

		public PrinterConfigurationWidget()
			: this()
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			SetDisplayAttributes();
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_VAnchor((VAnchor)8);
			((GuiWidget)val).set_Padding(new BorderDouble(0.0, 0.0, 0.0, 10.0));
			((GuiWidget)val).AddChild((GuiWidget)(object)new HardwareSettingsWidget(), -1);
			ApplicationSettingsWidget applicationSettingsWidget = new ApplicationSettingsWidget();
			((GuiWidget)val).AddChild((GuiWidget)(object)applicationSettingsWidget, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			AddHandlers();
		}

		private void AddThemeControls(FlowLayoutWidget controlsTopToBottomLayout)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Expected O, but got Unknown
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			DisableableWidget disableableWidget = new DisableableWidget();
			AltGroupBox altGroupBox = new AltGroupBox("Theme Settings".Localize());
			altGroupBox.TextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			altGroupBox.BorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)altGroupBox).set_HAnchor((HAnchor)5);
			((GuiWidget)altGroupBox).set_VAnchor((VAnchor)8);
			((GuiWidget)altGroupBox).set_Height(78.0);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			GuiWidget val2 = new GuiWidget();
			val2.set_HAnchor((HAnchor)5);
			val2.set_VAnchor((VAnchor)5);
			val2.set_Margin(new BorderDouble(0.0, 2.0, 0.0, 2.0));
			val2.set_Padding(new BorderDouble(4.0));
			val2.set_BackgroundColor(RGBA_Bytes.White);
			GuiWidget val3 = new GuiWidget();
			val3.set_HAnchor((HAnchor)5);
			val3.set_VAnchor((VAnchor)5);
			val3.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			ThemeColorSelectorWidget themeColorSelectorWidget = new ThemeColorSelectorWidget(val3);
			((GuiWidget)themeColorSelectorWidget).set_Margin(new BorderDouble(0.0, 0.0, 5.0, 0.0));
			((GuiWidget)altGroupBox).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)themeColorSelectorWidget, -1);
			((GuiWidget)val).AddChild(val2, -1);
			val2.AddChild(val3, -1);
			((GuiWidget)disableableWidget).AddChild((GuiWidget)(object)altGroupBox, -1);
			((GuiWidget)controlsTopToBottomLayout).AddChild((GuiWidget)(object)disableableWidget, -1);
		}

		private void RestartApplication()
		{
			UiThread.RunOnIdle((Action)delegate
			{
				GuiWidget val = (GuiWidget)(object)this;
				while (!(val is MatterControlApplication))
				{
					val = val.get_Parent();
				}
				MatterControlApplication obj = val as MatterControlApplication;
				obj.RestartOnClose = true;
				((GuiWidget)obj).Close();
			});
		}

		private void LanguageDropList_SelectionChanged(object sender, EventArgs e)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			string selectedLabel = ((DropDownList)sender).get_SelectedLabel();
			if (selectedLabel != UserSettings.Instance.get("Language"))
			{
				UserSettings.Instance.set("Language", selectedLabel);
			}
		}

		public void AddReleaseOptions(FlowLayoutWidget controlsTopToBottom)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			AltGroupBox altGroupBox = new AltGroupBox("Update Feed".Localize());
			((GuiWidget)altGroupBox).set_Margin(new BorderDouble(0.0));
			altGroupBox.TextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			altGroupBox.BorderColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)altGroupBox).set_HAnchor((HAnchor)5);
			((GuiWidget)altGroupBox).set_VAnchor((VAnchor)4);
			((GuiWidget)altGroupBox).set_Height(68.0);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)(((GuiWidget)val).get_HAnchor() | 2));
			DropDownList val2 = new DropDownList("Development", (Direction)1, 0.0, false);
			((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 3.0));
			val2.AddItem("Release", "release", 12.0).add_Selected((EventHandler)FixTabDot);
			val2.AddItem("Pre-Release", "pre-release", 12.0).add_Selected((EventHandler)FixTabDot);
			val2.AddItem("Development", "development", 12.0).add_Selected((EventHandler)FixTabDot);
			RectangleDouble localBounds = ((GuiWidget)val2).get_LocalBounds();
			double width = ((RectangleDouble)(ref localBounds)).get_Width();
			localBounds = ((GuiWidget)val2).get_LocalBounds();
			((GuiWidget)val2).set_MinimumSize(new Vector2(width, ((RectangleDouble)(ref localBounds)).get_Height()));
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
			val2.set_SelectedValue(UserSettings.Instance.get("UpdateFeedType"));
			val2.add_SelectionChanged((EventHandler)ReleaseOptionsDropList_SelectionChanged);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)altGroupBox).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)controlsTopToBottom).AddChild((GuiWidget)(object)altGroupBox, -1);
		}

		private void FixTabDot(object sender, EventArgs e)
		{
			UpdateControlData.Instance.CheckForUpdateUserRequested();
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

		private static GuiWidget CreateSeparatorLine()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			GuiWidget val = new GuiWidget(10.0, 1.0, (SizeLimitsToSet)1);
			val.set_Margin(new BorderDouble(0.0, 5.0));
			val.set_HAnchor((HAnchor)5);
			val.set_BackgroundColor(RGBA_Bytes.White);
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

		private void SetDisplayAttributes()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
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

		private void AddHandlers()
		{
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.EnableChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
		}

		private void onPrinterStatusChanged(object sender, EventArgs e)
		{
			((GuiWidget)this).Invalidate();
		}
	}
}
