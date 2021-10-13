using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.SetupWizard;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class SliceSettingsDetailControl : FlowLayoutWidget
	{
		private const string SliceSettingsLevelEntry = "SliceSettingsLevel";

		private const string SliceSettingsShowHelpEntry = "SliceSettingsShowHelp";

		private DropDownList settingsDetailSelector;

		private CheckBox showHelpBox;

		private TupleList<string, Func<bool>> slicerOptionsMenuItems;

		private string resetToDefaultsMessage = "Resetting to default values will remove your current overrides and restore your original printer settings.\nAre you sure you want to continue?".Localize();

		private string resetToDefaultsWindowTitle = "Revert Settings".Localize();

		private bool primarySettingsView;

		public string SelectedValue => settingsDetailSelector.get_SelectedValue();

		public bool ShowingHelp
		{
			get
			{
				if (!primarySettingsView)
				{
					return false;
				}
				return showHelpBox.get_Checked();
			}
		}

		public event EventHandler ShowHelpChanged;

		public SliceSettingsDetailControl(List<PrinterSettingsLayer> layerCascade)
			: this((FlowDirection)0)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			primarySettingsView = layerCascade == null;
			settingsDetailSelector = new DropDownList("Basic", (Direction)1, 200.0, false);
			((GuiWidget)settingsDetailSelector).set_Name("User Level Dropdown");
			settingsDetailSelector.AddItem("Basic".Localize(), "Simple", 12.0);
			settingsDetailSelector.AddItem("Standard".Localize(), "Intermediate", 12.0);
			settingsDetailSelector.AddItem("Advanced".Localize(), "Advanced", 12.0);
			if (primarySettingsView)
			{
				if (UserSettings.Instance.get("SliceSettingsLevel") != null && SliceSettingsOrganizer.Instance.UserLevels.ContainsKey(UserSettings.Instance.get("SliceSettingsLevel")))
				{
					settingsDetailSelector.set_SelectedValue(UserSettings.Instance.get("SliceSettingsLevel"));
				}
			}
			else
			{
				settingsDetailSelector.set_SelectedValue("Advanced");
			}
			settingsDetailSelector.add_SelectionChanged((EventHandler)delegate
			{
				RebuildSlicerSettings(null, null);
			});
			((GuiWidget)settingsDetailSelector).set_VAnchor((VAnchor)2);
			((GuiWidget)settingsDetailSelector).set_Margin(new BorderDouble(5.0, 3.0));
			settingsDetailSelector.set_BorderColor(new RGBA_Bytes(ActiveTheme.get_Instance().get_SecondaryTextColor(), 100));
			if (primarySettingsView)
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)settingsDetailSelector, -1);
				((GuiWidget)this).AddChild((GuiWidget)(object)GetSliceOptionsMenuDropList(), -1);
			}
			((GuiWidget)this).set_VAnchor((VAnchor)2);
		}

		private DropDownMenu GetSliceOptionsMenuDropList()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Expected O, but got Unknown
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Expected O, but got Unknown
			DropDownMenu obj = new DropDownMenu("Options".Localize() + "... ", (Direction)1)
			{
				HoverColor = new RGBA_Bytes(0, 0, 0, 50),
				NormalColor = new RGBA_Bytes(0, 0, 0, 0),
				BorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_SecondaryTextColor(), 100)
			};
			((GuiWidget)obj).set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 0));
			obj.BorderWidth = 1;
			obj.MenuAsWideAsItems = false;
			((Menu)obj).set_AlignToRightEdge(true);
			DropDownMenu dropDownMenu = obj;
			((GuiWidget)dropDownMenu).set_Name("Slice Settings Options Menu");
			((GuiWidget)dropDownMenu).set_VAnchor((VAnchor)(((GuiWidget)dropDownMenu).get_VAnchor() | 2));
			showHelpBox = new CheckBox("Show Help".Localize());
			if (primarySettingsView)
			{
				showHelpBox.set_Checked(UserSettings.Instance.get("SliceSettingsShowHelp") == "true");
			}
			showHelpBox.add_CheckedStateChanged((EventHandler)delegate
			{
				if (primarySettingsView)
				{
					UserSettings.Instance.set("SliceSettingsShowHelp", showHelpBox.get_Checked().ToString().ToLower());
				}
				this.ShowHelpChanged?.Invoke(this, null);
			});
			MenuItem val = new MenuItem((GuiWidget)(object)showHelpBox, "Show Help Checkbox");
			((GuiWidget)val).set_Padding(dropDownMenu.MenuItemsPadding);
			MenuItem item = val;
			((Collection<MenuItem>)(object)((Menu)dropDownMenu).MenuItems).Add(item);
			((Menu)dropDownMenu).AddHorizontalLine();
			dropDownMenu.AddItem("Import".Localize()).add_Selected((EventHandler)delegate
			{
				ImportSettingsMenu_Click();
			});
			dropDownMenu.AddItem("Export".Localize()).add_Selected((EventHandler)delegate
			{
				ActiveSliceSettings.Instance.Helpers.ExportAsMatterControlConfig();
			});
			dropDownMenu.AddItem("Duplicate".Localize()).add_Selected((EventHandler)delegate
			{
				ProfileManager.DuplicateFromExisting();
			});
			MenuItem obj2 = dropDownMenu.AddItem("Restore Settings".Localize());
			obj2.add_Selected((EventHandler)delegate
			{
				WizardWindow.Show<PrinterProfileHistoryPage>("PrinterProfileHistory", "Restore Settings");
			});
			((GuiWidget)obj2).set_Enabled(!string.IsNullOrEmpty(AuthenticationData.Instance.ActiveSessionUsername));
			dropDownMenu.AddItem("Reset to Defaults".Localize()).add_Selected((EventHandler)delegate
			{
				UiThread.RunOnIdle((Action)ResetToDefaults);
			});
			return dropDownMenu;
		}

		private bool ImportSettingsMenu_Click()
		{
			UiThread.RunOnIdle((Action)delegate
			{
				WizardWindow.Show<ImportSettingsPage>("ImportSettingsPage", "Import Settings Page");
			});
			return true;
		}

		private void RebuildSlicerSettings(object sender, EventArgs e)
		{
			UserSettings.Instance.set("SliceSettingsLevel", settingsDetailSelector.get_SelectedValue());
			ApplicationController.Instance.ReloadAdvancedControlsPanel();
		}

		private void ResetToDefaults()
		{
			StyledMessageBox.ShowMessageBox(delegate(bool revertSettings)
			{
				if (revertSettings)
				{
					bool flag = true;
					if (ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_required_to_print") && ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_enabled"))
					{
						flag = false;
					}
					ActiveSliceSettings.Instance.ClearUserOverrides();
					ActiveSliceSettings.Instance.Save();
					if (flag)
					{
						ApplicationController.Instance.ReloadAdvancedControlsPanel();
					}
					else
					{
						ApplicationController.Instance.ReloadAll();
					}
				}
			}, resetToDefaultsMessage, resetToDefaultsWindowTitle, StyledMessageBox.MessageType.YES_NO);
		}
	}
}
