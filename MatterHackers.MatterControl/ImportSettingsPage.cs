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
	public class ImportSettingsPage : WizardPage
	{
		private RadioButton newPrinterButton;

		private RadioButton mergeButton;

		private RadioButton newQualityPresetButton;

		private RadioButton newMaterialPresetButton;

		protected string importPrinterSuccessMessage = "You have successfully imported a new printer profile. You can find '{0}' in your list of available printers.".Localize();

		protected string importSettingSuccessMessage = "You have successfully imported a new {1} setting. You can find '{0}' in your list of {1} settings.".Localize();

		public ImportSettingsPage()
			: base("Cancel", "Import Wizard")
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Expected O, but got Unknown
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val2 = val;
			((GuiWidget)contentRow).AddChild((GuiWidget)(object)val2, -1);
			TextWidget val3 = new TextWidget("Merge Into:", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val3).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 5.0));
			((GuiWidget)val2).AddChild((GuiWidget)val3, -1);
			mergeButton = new RadioButton("Current".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12);
			mergeButton.set_Checked(true);
			((GuiWidget)val2).AddChild((GuiWidget)(object)mergeButton, -1);
			TextWidget val4 = new TextWidget("Create New:", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val4).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 15.0));
			((GuiWidget)val2).AddChild((GuiWidget)val4, -1);
			newPrinterButton = new RadioButton("Printer".Localize(), ActiveTheme.get_Instance().get_PrimaryTextColor(), 12);
			((GuiWidget)val2).AddChild((GuiWidget)(object)newPrinterButton, -1);
			Button val5 = textImageButtonFactory.Generate("Choose File".Localize());
			((GuiWidget)val5).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Expected O, but got Unknown
					FileDialog.OpenFileDialog(new OpenFileDialogParams("settings files|*.ini;*.printer;*.slice", "", false, "", ""), (Action<OpenFileDialogParams>)delegate(OpenFileDialogParams dialogParams)
					{
						if (!string.IsNullOrEmpty(((FileDialogParams)dialogParams).get_FileName()))
						{
							ImportSettingsFile(((FileDialogParams)dialogParams).get_FileName());
						}
					});
				});
			});
			((GuiWidget)val5).set_Visible(true);
			((GuiWidget)cancelButton).set_Visible(true);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)val5, -1);
		}

		private GuiWidget CreateDetailInfo(string detailText)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			WrappedTextWidget val = new WrappedTextWidget(detailText, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), true);
			val.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			WrappedTextWidget val2 = val;
			GuiWidget val3 = new GuiWidget();
			val3.set_HAnchor((HAnchor)5);
			val3.set_VAnchor((VAnchor)8);
			val3.set_Margin(new BorderDouble(25.0, 15.0, 5.0, 5.0));
			val3.AddChild((GuiWidget)(object)val2, -1);
			return val3;
		}

		private void ImportSettingsFile(string settingsFilePath)
		{
			if (newPrinterButton.get_Checked())
			{
				if (ProfileManager.ImportFromExisting(settingsFilePath))
				{
					WizardWindow.ChangeToPage(new ImportSucceeded(StringHelper.FormatWith(importPrinterSuccessMessage, new object[1]
					{
						Path.GetFileNameWithoutExtension(settingsFilePath)
					}))
					{
						WizardWindow = WizardWindow
					});
				}
				else
				{
					displayFailedToImportMessage(settingsFilePath);
				}
			}
			else if (mergeButton.get_Checked())
			{
				MergeSettings(settingsFilePath);
			}
			else if (newQualityPresetButton.get_Checked())
			{
				ImportToPreset(settingsFilePath);
			}
			else if (newMaterialPresetButton.get_Checked())
			{
				ImportToPreset(settingsFilePath);
			}
		}

		private void ImportToPreset(string settingsFilePath)
		{
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(settingsFilePath) && File.Exists(settingsFilePath))
			{
				string text = (newMaterialPresetButton.get_Checked() ? "Material".Localize() : "Quality".Localize());
				switch (Path.GetExtension(settingsFilePath)!.ToLower())
				{
				case ".printer":
				{
					PrinterSettingsLayer printerSettingsLayer2 = new PrinterSettingsLayer();
					printerSettingsLayer2["layer_name"] = Path.GetFileNameWithoutExtension(settingsFilePath);
					if (newQualityPresetButton.get_Checked())
					{
						((Collection<PrinterSettingsLayer>)(object)ActiveSliceSettings.Instance.QualityLayers).Add(printerSettingsLayer2);
					}
					else
					{
						((Collection<PrinterSettingsLayer>)(object)ActiveSliceSettings.Instance.MaterialLayers).Add(printerSettingsLayer2);
					}
					WizardWindow.ChangeToPage(new SelectPartsOfPrinterToImport(settingsFilePath, printerSettingsLayer2, text));
					break;
				}
				case ".slice":
				case ".ini":
				{
					PrinterSettingsLayer printerSettingsLayer = PrinterSettingsLayer.LoadFromIni(settingsFilePath);
					bool flag = false;
					PrinterSettingsLayer printerSettingsLayer2 = new PrinterSettingsLayer();
					printerSettingsLayer2.Name = Path.GetFileNameWithoutExtension(settingsFilePath);
					List<PrinterSettingsLayer> layerCascade = new List<PrinterSettingsLayer>
					{
						ActiveSliceSettings.Instance.OemLayer,
						ActiveSliceSettings.Instance.BaseLayer
					};
					Enumerator<string> enumerator = PrinterSettings.KnownSettings.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							string current = enumerator.get_Current();
							if (ActiveSliceSettings.Instance.Contains(current))
							{
								flag = true;
								string a = ActiveSliceSettings.Instance.GetValue(current, layerCascade).Trim();
								if (printerSettingsLayer.TryGetValue(current, out var value) && a != value)
								{
									printerSettingsLayer2[current] = value;
								}
							}
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					if (flag)
					{
						if (newMaterialPresetButton.get_Checked())
						{
							((Collection<PrinterSettingsLayer>)(object)ActiveSliceSettings.Instance.MaterialLayers).Add(printerSettingsLayer2);
						}
						else
						{
							((Collection<PrinterSettingsLayer>)(object)ActiveSliceSettings.Instance.QualityLayers).Add(printerSettingsLayer2);
						}
						ActiveSliceSettings.Instance.Save();
						WizardWindow.ChangeToPage(new ImportSucceeded(StringHelper.FormatWith(importSettingSuccessMessage, new object[2]
						{
							Path.GetFileNameWithoutExtension(settingsFilePath),
							text
						}))
						{
							WizardWindow = WizardWindow
						});
					}
					else
					{
						displayFailedToImportMessage(settingsFilePath);
					}
					break;
				}
				default:
					StyledMessageBox.ShowMessageBox(null, StringHelper.FormatWith("Oops! Unable to recognize settings file '{0}'.".Localize(), new object[1]
					{
						Path.GetFileName(settingsFilePath)
					}), "Unable to Import".Localize());
					break;
				}
			}
			((GuiWidget)this).Invalidate();
		}

		private void MergeSettings(string settingsFilePath)
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(settingsFilePath) && File.Exists(settingsFilePath))
			{
				switch (Path.GetExtension(settingsFilePath)!.ToLower())
				{
				case ".printer":
					WizardWindow.ChangeToPage(new SelectPartsOfPrinterToImport(settingsFilePath, ActiveSliceSettings.Instance.UserLayer));
					break;
				case ".slice":
				case ".ini":
				{
					PrinterSettingsLayer printerSettingsLayer = PrinterSettingsLayer.LoadFromIni(settingsFilePath);
					bool flag = false;
					PrinterSettings instance = ActiveSliceSettings.Instance;
					Enumerator<string> enumerator = PrinterSettings.KnownSettings.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							string current = enumerator.get_Current();
							if (instance.Contains(current))
							{
								flag = true;
								string a = instance.GetValue(current).Trim();
								if (printerSettingsLayer.TryGetValue(current, out var value) && a != value)
								{
									instance.UserLayer[current] = value;
								}
							}
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					if (flag)
					{
						instance.Save();
						UiThread.RunOnIdle((Action)ApplicationController.Instance.ReloadAdvancedControlsPanel);
					}
					else
					{
						displayFailedToImportMessage(settingsFilePath);
					}
					((GuiWidget)WizardWindow).Close();
					break;
				}
				default:
					((GuiWidget)WizardWindow).Close();
					StyledMessageBox.ShowMessageBox(null, StringHelper.FormatWith("Oops! Unable to recognize settings file '{0}'.".Localize(), new object[1]
					{
						Path.GetFileName(settingsFilePath)
					}), "Unable to Import".Localize());
					break;
				}
			}
			((GuiWidget)this).Invalidate();
		}

		private void displayFailedToImportMessage(string settingsFilePath)
		{
			StyledMessageBox.ShowMessageBox(null, StringHelper.FormatWith("Oops! Settings file '{0}' did not contain any settings we could import.".Localize(), new object[1]
			{
				Path.GetFileName(settingsFilePath)
			}), "Unable to Import".Localize());
		}
	}
}
