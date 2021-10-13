using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class PrinterSelector : DropDownList
	{
		private EventHandler unregisterEvents;

		private int lastSelectedIndex = -1;

		public event EventHandler AddPrinter;

		public PrinterSelector()
			: this("Printers".Localize() + "... ", (Direction)1, 0.0, false)
		{
			Rebuild();
			((GuiWidget)this).set_Name("Printers... Menu");
			((DropDownList)this).add_SelectionChanged((EventHandler)delegate
			{
				string printerID = ((DropDownList)this).get_SelectedValue();
				if (!(printerID == "new") && !string.IsNullOrEmpty(printerID) && !(printerID == ActiveSliceSettings.Instance.ID))
				{
					if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting || PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
					{
						if (((DropDownList)this).get_SelectedIndex() != lastSelectedIndex)
						{
							UiThread.RunOnIdle((Action)delegate
							{
								StyledMessageBox.ShowMessageBox(null, "Please wait until the print has finished and try again.".Localize(), "Can't switch printers while printing".Localize());
							});
							((DropDownList)this).set_SelectedIndex(lastSelectedIndex);
						}
					}
					else
					{
						lastSelectedIndex = ((DropDownList)this).get_SelectedIndex();
						UiThread.RunOnIdle((Action)delegate
						{
							ActiveSliceSettings.SwitchToProfile(printerID);
						});
					}
				}
			});
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)SettingChanged, ref unregisterEvents);
			ProfileManager.ProfilesListChanged.RegisterEvent((EventHandler)delegate
			{
				Rebuild();
			}, ref unregisterEvents);
		}

		public void Rebuild()
		{
			((Collection<MenuItem>)(object)base.MenuItems).Clear();
			foreach (PrinterInfo item in (IEnumerable<PrinterInfo>)Enumerable.OrderBy<PrinterInfo, string>(ProfileManager.Instance.ActiveProfiles, (Func<PrinterInfo, string>)((PrinterInfo p) => p.Name)))
			{
				((DropDownList)this).AddItem(item.Name, item.ID.ToString(), 12.0);
			}
			if (ActiveSliceSettings.Instance.PrinterSelected)
			{
				((DropDownList)this).set_SelectedValue(ActiveSliceSettings.Instance.ID);
				lastSelectedIndex = ((DropDownList)this).get_SelectedIndex();
				((GuiWidget)base.mainControlText).set_Text(ActiveSliceSettings.Instance.GetValue("printer_name"));
			}
			MenuItem obj = ((DropDownList)this).AddItem(StaticData.get_Instance().LoadIcon("icon_plus.png", 32, 32), "Add New Printer".Localize() + "...", "new", 12.0);
			obj.set_CanHeldSelection(false);
			((GuiWidget)obj).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (this.AddPrinter != null)
				{
					if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting || PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
					{
						UiThread.RunOnIdle((Action)delegate
						{
							StyledMessageBox.ShowMessageBox(null, "Please wait until the print has finished and try again.".Localize(), "Can't add printers while printing".Localize());
						});
					}
					else
					{
						UiThread.RunOnIdle((Action)delegate
						{
							this.AddPrinter(this, null);
						});
					}
				}
			});
		}

		private void SettingChanged(object sender, EventArgs e)
		{
			object obj = (object)(e as StringEventArgs);
			string text = ((obj != null) ? ((StringEventArgs)obj).get_Data() : null);
			if (text != null && text == "printer_name" && ProfileManager.Instance.ActiveProfile != null)
			{
				ProfileManager.Instance.ActiveProfile.Name = ActiveSliceSettings.Instance.GetValue("printer_name");
				Rebuild();
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
