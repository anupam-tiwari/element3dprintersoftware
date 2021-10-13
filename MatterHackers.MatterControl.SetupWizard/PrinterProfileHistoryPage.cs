using System;
using System.Collections.Generic;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.SetupWizard
{
	internal class PrinterProfileHistoryPage : WizardPage
	{
		private List<RadioButton> radioButtonList = new List<RadioButton>();

		private Dictionary<string, string> printerProfileData = new Dictionary<string, string>();

		private List<string> orderedProfiles = new List<string>();

		private ScrollableWidget scrollWindow;

		public PrinterProfileHistoryPage()
			: base("Cancel", "Restore Settings")
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			ScrollableWidget val = new ScrollableWidget(false);
			val.set_AutoScroll(true);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			scrollWindow = val;
			((GuiWidget)scrollWindow.get_ScrollArea()).set_HAnchor((HAnchor)5);
			contentRow.set_FlowDirection((FlowDirection)3);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)scrollWindow, -1);
			Button val2 = textImageButtonFactory.Generate("Restore".Localize());
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)async delegate
			{
				int num = radioButtonList.IndexOf(Enumerable.FirstOrDefault<RadioButton>(Enumerable.Where<RadioButton>((IEnumerable<RadioButton>)radioButtonList, (Func<RadioButton, bool>)((RadioButton r) => r.get_Checked()))));
				if (num != -1)
				{
					string arg = printerProfileData[orderedProfiles[num]];
					PrinterInfo activeProfile = ProfileManager.Instance.ActiveProfile;
					PrinterSettings printerSettings = await ApplicationController.GetPrinterProfileAsync(activeProfile, arg);
					if (printerSettings != null)
					{
						printerSettings.Save();
						ActiveSliceSettings.RefreshActiveInstance(printerSettings);
					}
					UiThread.RunOnIdle((Action)((GuiWidget)WizardWindow).Close);
				}
			});
			LoadHistoryItems();
		}

		private async void LoadHistoryItems()
		{
			TextWidget loadingText = new TextWidget("Retrieving History from Web...", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			loadingText.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)scrollWindow).AddChild((GuiWidget)(object)loadingText, -1);
			Dictionary<string, string> dictionary = (printerProfileData = await (ApplicationController.GetProfileHistory?.Invoke(ProfileManager.Instance.ActiveProfile.DeviceToken)));
			if (printerProfileData != null)
			{
				((GuiWidget)loadingText).set_Visible(false);
				List<DateTime> list = new List<DateTime>();
				foreach (KeyValuePair<string, string> item in (IEnumerable<KeyValuePair<string, string>>)Enumerable.OrderByDescending<KeyValuePair<string, string>, string>((IEnumerable<KeyValuePair<string, string>>)dictionary, (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> d) => d.Key)))
				{
					list.Add(Convert.ToDateTime(item.Key).ToLocalTime());
				}
				Dictionary<TimeBlock, Dictionary<int, string>> dictionary2 = RelativeTime.GroupTimes(DateTime.Now, list);
				FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
				((GuiWidget)scrollWindow).AddChild((GuiWidget)(object)val, -1);
				foreach (KeyValuePair<TimeBlock, Dictionary<int, string>> item2 in dictionary2)
				{
					TextWidget val2 = new TextWidget(RelativeTime.get_BlockDescriptions()[item2.Key], 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
					((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 5.0));
					((GuiWidget)val).AddChild((GuiWidget)val2, -1);
					foreach (KeyValuePair<int, string> item3 in item2.Value)
					{
						RadioButton val3 = new RadioButton(item3.Value, ActiveTheme.get_Instance().get_PrimaryTextColor(), 12);
						((GuiWidget)val3).set_Margin(new BorderDouble(5.0, 0.0));
						RadioButton val4 = val3;
						val4.set_Checked(false);
						radioButtonList.Add(val4);
						((GuiWidget)val).AddChild((GuiWidget)(object)val4, -1);
					}
				}
				foreach (KeyValuePair<string, string> item4 in dictionary)
				{
					orderedProfiles.Add(item4.Key.ToString());
				}
			}
			else
			{
				((GuiWidget)loadingText).set_Text("Failed To Download History!");
				loadingText.set_TextColor(RGBA_Bytes.Red);
			}
		}
	}
}
