using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class CopyGuestProfilesToUser : WizardPage
	{
		private string importMessage = "It's time to copy your existing printer settings to your MatterHackers account. Once copied, these printers will be available whenever you sign in to MatterControl. Printers that are not copied will only be available when not signed in.".Localize();

		private List<CheckBox> checkBoxes = new List<CheckBox>();

		public CopyGuestProfilesToUser()
			: base("Close", "Copy Printers to Account")
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Expected O, but got Unknown
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Expected O, but got Unknown
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Expected O, but got Unknown
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Expected O, but got Unknown
			ScrollableWidget val = new ScrollableWidget(false);
			val.set_AutoScroll(true);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			ScrollableWidget val2 = val;
			((GuiWidget)val2.get_ScrollArea()).set_HAnchor((HAnchor)5);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)val2, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val4 = val3;
			((GuiWidget)val2).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)val4).AddChild((GuiWidget)new WrappedTextWidget(importMessage, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), true), -1);
			Dictionary<CheckBox, PrinterInfo> byCheckbox = new Dictionary<CheckBox, PrinterInfo>();
			ProfileManager guest = ProfileManager.Load("guest");
			ProfileManager profileManager = guest;
			if (profileManager != null && ((Collection<PrinterInfo>)(object)profileManager.Profiles).Count > 0)
			{
				TextWidget val5 = new TextWidget("Printers to Copy:".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val5.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)val5).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 15.0));
				((GuiWidget)val4).AddChild((GuiWidget)val5, -1);
				foreach (PrinterInfo item in (Collection<PrinterInfo>)(object)guest.Profiles)
				{
					CheckBox val6 = new CheckBox(item.Name);
					val6.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
					((GuiWidget)val6).set_Margin(new BorderDouble(5.0, 0.0, 0.0, 0.0));
					((GuiWidget)val6).set_HAnchor((HAnchor)1);
					val6.set_Checked(true);
					CheckBox val7 = val6;
					checkBoxes.Add(val7);
					((GuiWidget)val4).AddChild((GuiWidget)(object)val7, -1);
					byCheckbox[val7] = item;
				}
			}
			Button val8 = textImageButtonFactory.Generate("Copy".Localize());
			((GuiWidget)val8).set_Name("CopyProfilesButton");
			((GuiWidget)val8).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				foreach (CheckBox checkBox in checkBoxes)
				{
					if (checkBox.get_Checked())
					{
						PrinterInfo printerInfo = byCheckbox[checkBox];
						string text = guest.ProfilePath(printerInfo);
						if (File.Exists(text))
						{
							File.Copy(text, printerInfo.ProfilePath);
							((Collection<PrinterInfo>)(object)ProfileManager.Instance.Profiles).Add(printerInfo);
						}
					}
				}
				guest.Save();
				UiThread.RunOnIdle((Action)delegate
				{
					((GuiWidget)WizardWindow).Close();
					ProfileManager.Instance.PrintersImported = true;
					ProfileManager.Instance.Save();
				});
			});
			CheckBox rememberChoice = new CheckBox("Don't remind me again".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12.0);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)rememberChoice, -1);
			((GuiWidget)val8).set_Visible(true);
			((GuiWidget)cancelButton).set_Visible(true);
			((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					((GuiWidget)WizardWindow).Close();
					if (rememberChoice.get_Checked())
					{
						ProfileManager.Instance.PrintersImported = true;
						ProfileManager.Instance.Save();
					}
				});
			});
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)val8, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			((GuiWidget)footerRow).set_Visible(true);
		}
	}
}
