using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class SelectPartsOfPrinterToImport : WizardPage
	{
		private string importMessage = "Select what you would like to merge into your current profile.".Localize();

		private string settingsFilePath;

		private PrinterSettings settingsToImport;

		private int selectedMaterial = -1;

		private int selectedQuality = -1;

		private PrinterSettingsLayer destinationLayer;

		private string sectionName;

		private bool isMergeIntoUserLayer;

		private string importPrinterSuccessMessage = "Settings have been merged into your current printer.".Localize();

		public SelectPartsOfPrinterToImport(string settingsFilePath, PrinterSettingsLayer destinationLayer, string sectionName = null)
			: base("Cancel", "Import Wizard")
		{
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Expected O, but got Unknown
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Expected O, but got Unknown
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Expected O, but got Unknown
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Expected O, but got Unknown
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Expected O, but got Unknown
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Expected O, but got Unknown
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Expected O, but got Unknown
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Expected O, but got Unknown
			SelectPartsOfPrinterToImport selectPartsOfPrinterToImport = this;
			isMergeIntoUserLayer = destinationLayer == ActiveSliceSettings.Instance.UserLayer;
			this.destinationLayer = destinationLayer;
			this.sectionName = sectionName;
			settingsToImport = PrinterSettings.LoadFile(settingsFilePath);
			((GuiWidget)headerLabel).set_Text("Select What to Import".Localize());
			this.settingsFilePath = settingsFilePath;
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
			if (isMergeIntoUserLayer)
			{
				((GuiWidget)val4).AddChild((GuiWidget)new WrappedTextWidget(importMessage, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), true), -1);
			}
			TextWidget val5 = new TextWidget("Main Settings:", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val5.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val5).set_Margin(new BorderDouble(0.0, 3.0, 0.0, (double)(isMergeIntoUserLayer ? 10 : 0)));
			((GuiWidget)val4).AddChild((GuiWidget)val5, -1);
			RadioButton val6 = new RadioButton("Printer Profile", 12);
			val6.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val6).set_Margin(new BorderDouble(5.0, 0.0));
			((GuiWidget)val6).set_HAnchor((HAnchor)1);
			val6.set_Checked(true);
			RadioButton val7 = val6;
			((GuiWidget)val4).AddChild((GuiWidget)(object)val7, -1);
			if (((Collection<PrinterSettingsLayer>)(object)settingsToImport.QualityLayers).Count > 0)
			{
				TextWidget val8 = new TextWidget("Quality Presets:", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val8.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)val8).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 15.0));
				((GuiWidget)val4).AddChild((GuiWidget)val8, -1);
				int num = 0;
				foreach (PrinterSettingsLayer item2 in (Collection<PrinterSettingsLayer>)(object)settingsToImport.QualityLayers)
				{
					RadioButton val9 = new RadioButton(item2.Name, 12);
					val9.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
					((GuiWidget)val9).set_Margin(new BorderDouble(5.0, 0.0, 0.0, 0.0));
					((GuiWidget)val9).set_HAnchor((HAnchor)1);
					RadioButton qualityButton = val9;
					((GuiWidget)val4).AddChild((GuiWidget)(object)qualityButton, -1);
					int localButtonIndex2 = num;
					qualityButton.add_CheckedStateChanged((EventHandler)delegate
					{
						if (qualityButton.get_Checked())
						{
							selectPartsOfPrinterToImport.selectedQuality = localButtonIndex2;
						}
						else
						{
							selectPartsOfPrinterToImport.selectedQuality = -1;
						}
					});
					num++;
				}
			}
			if (((Collection<PrinterSettingsLayer>)(object)settingsToImport.MaterialLayers).Count > 0)
			{
				TextWidget val10 = new TextWidget("Material Presets:", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
				val10.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
				((GuiWidget)val10).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 15.0));
				((GuiWidget)val4).AddChild((GuiWidget)val10, -1);
				int num2 = 0;
				foreach (PrinterSettingsLayer item3 in (Collection<PrinterSettingsLayer>)(object)settingsToImport.MaterialLayers)
				{
					RadioButton val11 = new RadioButton(item3.Name, 12);
					val11.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
					((GuiWidget)val11).set_Margin(new BorderDouble(5.0, 0.0));
					((GuiWidget)val11).set_HAnchor((HAnchor)1);
					RadioButton materialButton = val11;
					((GuiWidget)val4).AddChild((GuiWidget)(object)materialButton, -1);
					int localButtonIndex = num2;
					materialButton.add_CheckedStateChanged((EventHandler)delegate
					{
						if (materialButton.get_Checked())
						{
							selectPartsOfPrinterToImport.selectedMaterial = localButtonIndex;
						}
						else
						{
							selectPartsOfPrinterToImport.selectedMaterial = -1;
						}
					});
					num2++;
				}
			}
			string label = (isMergeIntoUserLayer ? "Merge".Localize() : "Import".Localize());
			Button val12 = textImageButtonFactory.Generate(label);
			((GuiWidget)val12).set_Name("Merge Profile");
			((GuiWidget)val12).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					bool setLayerName = false;
					PrinterSettingsLayer item = null;
					if (selectPartsOfPrinterToImport.selectedMaterial > -1)
					{
						item = ((Collection<PrinterSettingsLayer>)(object)selectPartsOfPrinterToImport.settingsToImport.MaterialLayers)[selectPartsOfPrinterToImport.selectedMaterial];
						setLayerName = true;
					}
					else if (selectPartsOfPrinterToImport.selectedQuality > -1)
					{
						item = ((Collection<PrinterSettingsLayer>)(object)selectPartsOfPrinterToImport.settingsToImport.QualityLayers)[selectPartsOfPrinterToImport.selectedQuality];
						setLayerName = true;
					}
					List<PrinterSettingsLayer> rawSourceFilter2 = ((selectPartsOfPrinterToImport.selectedQuality != -1 || selectPartsOfPrinterToImport.selectedMaterial != -1) ? new List<PrinterSettingsLayer>
					{
						item
					} : new List<PrinterSettingsLayer>
					{
						selectPartsOfPrinterToImport.settingsToImport.OemLayer,
						selectPartsOfPrinterToImport.settingsToImport.UserLayer
					});
					ActiveSliceSettings.Instance.Merge(destinationLayer, selectPartsOfPrinterToImport.settingsToImport, rawSourceFilter2, setLayerName);
					SystemWindow obj = Enumerable.FirstOrDefault<SystemWindow>(ExtensionMethods.Parents<SystemWindow>((GuiWidget)(object)selectPartsOfPrinterToImport));
					if (obj != null)
					{
						((GuiWidget)obj).CloseOnIdle();
					}
				});
			});
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)val12, -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)footerRow).AddChild((GuiWidget)(object)cancelButton, -1);
			if (((Collection<PrinterSettingsLayer>)(object)settingsToImport.QualityLayers).Count != 0 || ((Collection<PrinterSettingsLayer>)(object)settingsToImport.MaterialLayers).Count != 0)
			{
				return;
			}
			UiThread.RunOnIdle((Action)delegate
			{
				List<PrinterSettingsLayer> rawSourceFilter = new List<PrinterSettingsLayer>
				{
					selectPartsOfPrinterToImport.settingsToImport.OemLayer ?? new PrinterSettingsLayer(),
					selectPartsOfPrinterToImport.settingsToImport.UserLayer ?? new PrinterSettingsLayer()
				};
				ActiveSliceSettings.Instance.Merge(destinationLayer, selectPartsOfPrinterToImport.settingsToImport, rawSourceFilter, setLayerName: false);
				UiThread.RunOnIdle((Action)ApplicationController.Instance.ReloadAdvancedControlsPanel);
				string successMessage = StringHelper.FormatWith(selectPartsOfPrinterToImport.importPrinterSuccessMessage, new object[1]
				{
					Path.GetFileNameWithoutExtension(settingsFilePath)
				});
				if (!selectPartsOfPrinterToImport.isMergeIntoUserLayer)
				{
					string text = (selectPartsOfPrinterToImport.isMergeIntoUserLayer ? Path.GetFileNameWithoutExtension(settingsFilePath) : destinationLayer["layer_name"]);
					successMessage = StringHelper.FormatWith("You have successfully imported a new {1} setting. You can find '{0}' in your list of {1} settings.".Localize(), new object[2]
					{
						text,
						sectionName
					});
				}
				selectPartsOfPrinterToImport.WizardWindow.ChangeToPage(new ImportSucceeded(successMessage)
				{
					WizardWindow = selectPartsOfPrinterToImport.WizardWindow
				});
			});
		}
	}
}
