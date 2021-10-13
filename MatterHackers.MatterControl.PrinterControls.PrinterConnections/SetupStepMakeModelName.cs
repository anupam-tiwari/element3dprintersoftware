using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintLibrary.Provider;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SettingsManagement;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class SetupStepMakeModelName : ConnectionWizardPage
	{
		private FlowLayoutWidget printerModelContainer;

		private FlowLayoutWidget printerMakeContainer;

		private MHTextEditWidget printerNameInput;

		private TextWidget printerNameError;

		private Button nextButton;

		private bool usingDefaultName;

		private static BorderDouble elementMargin = new BorderDouble(0.0, 0.0, 0.0, 3.0);

		private BoundDropList printerManufacturerSelector;

		private BoundDropList printerModelSelector;

		private string activeMake;

		private string activeModel;

		private string activeName;

		public SetupStepMakeModelName()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			BoundDropList boundDropList = new BoundDropList(string.Format("- {0} -", "Select Make".Localize()), 200);
			((GuiWidget)boundDropList).set_HAnchor((HAnchor)5);
			((GuiWidget)boundDropList).set_Margin(elementMargin);
			((GuiWidget)boundDropList).set_Name("Select Make");
			boundDropList.ListSource = OemSettings.Instance.AllOems;
			((GuiWidget)boundDropList).set_TabStop(true);
			printerManufacturerSelector = boundDropList;
			((DropDownList)printerManufacturerSelector).add_SelectionChanged((EventHandler)ManufacturerDropList_SelectionChanged);
			printerMakeContainer = CreateSelectionContainer("Make".Localize() + ":", "Select the printer manufacturer".Localize(), (DropDownList)(object)printerManufacturerSelector);
			BoundDropList boundDropList2 = new BoundDropList(string.Format("- {0} -", "Select Model".Localize()), 200);
			((GuiWidget)boundDropList2).set_Name("Select Model");
			((GuiWidget)boundDropList2).set_HAnchor((HAnchor)5);
			((GuiWidget)boundDropList2).set_Margin(elementMargin);
			((GuiWidget)boundDropList2).set_TabStop(true);
			printerModelSelector = boundDropList2;
			((DropDownList)printerModelSelector).add_SelectionChanged((EventHandler)ModelDropList_SelectionChanged);
			printerModelContainer = CreateSelectionContainer("Model".Localize() + ":", "Select the printer model".Localize(), (DropDownList)(object)printerModelSelector);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)printerMakeContainer, -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)printerModelContainer, -1);
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)createPrinterNameContainer(), -1);
			nextButton = textImageButtonFactory.Generate("Save & Continue".Localize());
			((GuiWidget)nextButton).set_Name("Save & Continue Button");
			((GuiWidget)nextButton).add_Click((EventHandler<MouseEventArgs>)async delegate
			{
				if (ValidateControls())
				{
					if (!(await ProfileManager.CreateProfileAsync(activeMake, activeModel, activeName)))
					{
						((GuiWidget)printerNameError).set_Text("Error creating profile".Localize());
						((GuiWidget)printerNameError).set_Visible(true);
					}
					else
					{
						LoadCalibrationPrints();
						if ((int)OsInformation.get_OperatingSystem() == 1)
						{
							UiThread.RunOnIdle((Action)WizardWindow.ChangeToPage<SetupStepInstallDriver>);
						}
						else
						{
							UiThread.RunOnIdle((Action)WizardWindow.ChangeToPage<SetupStepComPortOne>);
						}
					}
				}
			});
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)nextButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			usingDefaultName = true;
			if (((Collection<MenuItem>)(object)((Menu)printerManufacturerSelector).MenuItems).Count == 1)
			{
				((DropDownList)printerManufacturerSelector).set_SelectedIndex(0);
			}
			SetElementVisibility();
		}

		private void SetElementVisibility()
		{
			((GuiWidget)nextButton).set_Visible(activeModel != null && activeMake != null);
		}

		private FlowLayoutWidget createPrinterNameContainer()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Expected O, but got Unknown
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Expected O, but got Unknown
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Expected O, but got Unknown
			TextWidget val = new TextWidget("Name".Localize() + ":", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 4.0, 0.0, 1.0));
			TextWidget val2 = val;
			MHTextEditWidget mHTextEditWidget = new MHTextEditWidget();
			((GuiWidget)mHTextEditWidget).set_HAnchor((HAnchor)5);
			printerNameInput = mHTextEditWidget;
			((GuiWidget)printerNameInput).add_KeyPressed((EventHandler<KeyPressEventArgs>)delegate
			{
				usingDefaultName = false;
			});
			TextWidget val3 = new TextWidget("", 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 3.0));
			printerNameError = val3;
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val4).set_Margin(new BorderDouble(0.0, 5.0));
			((GuiWidget)val4).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)printerNameInput, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)printerNameError, -1);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			return val4;
		}

		private FlowLayoutWidget CreateSelectionContainer(string labelText, string validationMessage, DropDownList selector)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Expected O, but got Unknown
			TextWidget val = new TextWidget(labelText, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Margin(elementMargin);
			TextWidget val2 = val;
			TextWidget val3 = new TextWidget(validationMessage, 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Margin(elementMargin);
			TextWidget validationTextWidget = val3;
			selector.add_SelectionChanged((EventHandler)delegate
			{
				((GuiWidget)validationTextWidget).set_Visible(selector.get_SelectedLabel().StartsWith("-"));
			});
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val4).set_Margin(new BorderDouble(0.0, 5.0));
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)selector, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)validationTextWidget, -1);
			return val4;
		}

		private void ManufacturerDropList_SelectionChanged(object sender, EventArgs e)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			activeMake = ((DropDownList)sender).get_SelectedValue();
			activeModel = null;
			if (!OemSettings.Instance.OemProfiles.TryGetValue(activeMake, out var value))
			{
				value = new Dictionary<string, PublicDevice>();
			}
			printerModelSelector.ListSource = Enumerable.ToList<KeyValuePair<string, string>>(Enumerable.Select<KeyValuePair<string, PublicDevice>, KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, PublicDevice>>)Enumerable.OrderBy<KeyValuePair<string, PublicDevice>, string>((IEnumerable<KeyValuePair<string, PublicDevice>>)value, (Func<KeyValuePair<string, PublicDevice>, string>)((KeyValuePair<string, PublicDevice> p) => p.Key)), (Func<KeyValuePair<string, PublicDevice>, KeyValuePair<string, string>>)((KeyValuePair<string, PublicDevice> p) => new KeyValuePair<string, string>(p.Key, p.Value.ProfileToken))));
			if (((Collection<MenuItem>)(object)((Menu)printerModelSelector).MenuItems).Count == 1)
			{
				((DropDownList)printerModelSelector).set_SelectedIndex(0);
			}
			((GuiWidget)contentRow).Invalidate();
			SetElementVisibility();
		}

		private void ModelDropList_SelectionChanged(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Expected O, but got Unknown
				DropDownList val = (DropDownList)sender;
				activeModel = val.get_SelectedLabel();
				SetElementVisibility();
				if (usingDefaultName)
				{
					string selectedLabel = ((DropDownList)printerManufacturerSelector).get_SelectedLabel();
					IEnumerable<string> enumerable = Enumerable.Select<PrinterInfo, string>(ProfileManager.Instance.ActiveProfiles, (Func<PrinterInfo, string>)((PrinterInfo p) => p.Name));
					((GuiWidget)printerNameInput).set_Text(agg_basics.GetNonCollidingName(enumerable, $"{selectedLabel} {activeModel}"));
				}
			});
		}

		public void LoadCalibrationPrints()
		{
			string value = ActiveSliceSettings.Instance.GetValue("calibration_files");
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			string[] array = value.Split(new char[1]
			{
				';'
			});
			if (array.Length < 1)
			{
				return;
			}
			LibraryProviderSQLite libraryProviderSQLite = new LibraryProviderSQLite(null, null, null, "Local Library");
			libraryProviderSQLite.EnsureSamplePartsExist(array);
			string[] itemNames = QueueData.Instance.GetItemNames();
			foreach (string item in Enumerable.Select<string, string>((IEnumerable<string>)array, (Func<string, string>)((string f) => Path.GetFileNameWithoutExtension(f))))
			{
				if (!Enumerable.Contains<string>((IEnumerable<string>)itemNames, item))
				{
					PrintItem printItem = Enumerable.FirstOrDefault<PrintItem>(libraryProviderSQLite.GetLibraryItems(item));
					if (printItem != null)
					{
						QueueData.Instance.AddItem(new PrintItemWrapper(printItem));
					}
				}
			}
			libraryProviderSQLite.Dispose();
		}

		private bool ValidateControls()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(((GuiWidget)printerNameInput).get_Text()))
			{
				activeName = ((GuiWidget)printerNameInput).get_Text();
				if (activeMake == null || activeModel == null)
				{
					return false;
				}
				return true;
			}
			printerNameError.set_TextColor(RGBA_Bytes.Red);
			((GuiWidget)printerNameError).set_Text("Printer name cannot be blank".Localize());
			((GuiWidget)printerNameError).set_Visible(true);
			return false;
		}
	}
}
