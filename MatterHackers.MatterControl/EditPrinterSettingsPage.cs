using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class EditPrinterSettingsPage : WizardPage
	{
		private EventHandler unregisterEvents;

		public EditPrinterSettingsPage()
			: base("Done")
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)headerLabel).set_Text("Current Settings".Localize());
			textImageButtonFactory.borderWidth = 1.0;
			textImageButtonFactory.normalBorderColor = RGBA_Bytes.White;
			int tabIndex = 0;
			AddNameSetting("printer_name", contentRow, ref tabIndex);
			AddNameSetting("auto_connect", contentRow, ref tabIndex);
			if (ActiveSliceSettings.Instance.GetValue<bool>("enable_network_printing"))
			{
				AddNameSetting("ip_address", contentRow, ref tabIndex);
				AddNameSetting("ip_port", contentRow, ref tabIndex);
			}
			else
			{
				AddNameSetting("baud_rate", contentRow, ref tabIndex);
				AddNameSetting("com_port", contentRow, ref tabIndex);
			}
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)new VerticalSpacer(), -1);
			((GuiWidget)contentRow).AddChild(SliceSettingsWidget.CreatePrinterExtraControls(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			((GuiWidget)cancelButton).set_Text("Back".Localize());
			ActiveSliceSettings.ActivePrinterChanged.RegisterEvent((EventHandler)delegate
			{
				if (!((GuiWidget)WizardWindow).get_HasBeenClosed())
				{
					UiThread.RunOnIdle((Action)((GuiWidget)WizardWindow).Close);
				}
			}, ref unregisterEvents);
		}

		private void AddNameSetting(string sliceSettingsKey, FlowLayoutWidget contentRow, ref int tabIndex)
		{
			GuiWidget val = SliceSettingsWidget.CreateSettingControl(sliceSettingsKey, ref tabIndex);
			if (val != null)
			{
				((GuiWidget)contentRow).AddChild(val, -1);
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
