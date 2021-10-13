using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class MenuOptionFile : MenuBase
	{
		public static MenuOptionFile CurrentMenuOptionFile;

		public MenuOptionFile()
			: base("File".Localize())
		{
			((GuiWidget)this).set_Name("File Menu");
			CurrentMenuOptionFile = this;
		}

		protected override IEnumerable<MenuItemAction> GetMenuActions()
		{
			return new List<MenuItemAction>
			{
				new MenuItemAction("Add Printer".Localize(), AddPrinter_Click),
				new MenuItemAction("Import Printer".Localize(), ImportPrinter),
				new MenuItemAction("Add File To Queue".Localize(), importFile_Click),
				new MenuItemAction("Redeem Design Code".Localize(), delegate
				{
					ApplicationController.Instance.RedeemDesignCode?.Invoke();
				}),
				new MenuItemAction("Enter Share Code".Localize(), delegate
				{
					ApplicationController.Instance.EnterShareCode?.Invoke();
				}),
				new MenuItemAction("------------------------", null),
				new MenuItemAction("Exit".Localize(), delegate
				{
					MatterControlApplication matterControlApplication = Enumerable.FirstOrDefault<MatterControlApplication>(ExtensionMethods.Parents<MatterControlApplication>((GuiWidget)(object)this));
					matterControlApplication.RestartOnClose = false;
					((GuiWidget)matterControlApplication).Close();
				})
			};
		}

		private void ImportPrinter()
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
		}

		private void ImportSettingsFile(string settingsFilePath)
		{
			if (!ProfileManager.ImportFromExisting(settingsFilePath))
			{
				StyledMessageBox.ShowMessageBox(null, StringHelper.FormatWith("Oops! Settings file '{0}' did not contain any settings we could import.".Localize(), new object[1]
				{
					Path.GetFileName(settingsFilePath)
				}), "Unable to Import".Localize());
			}
		}

		private void AddPrinter_Click()
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
				WizardWindow.ShowPrinterSetup(userRequestedNewPrinter: true);
			}
		}

		private void importFile_Click()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			OpenFileDialogParams val = new OpenFileDialogParams(ApplicationSettings.OpenPrintableFileParams, "", false, "", "");
			val.set_MultiSelect(true);
			((FileDialogParams)val).set_ActionButtonLabel("Add to Queue");
			((FileDialogParams)val).set_Title("Element: Select A File");
			FileDialog.OpenFileDialog(val, (Action<OpenFileDialogParams>)delegate(OpenFileDialogParams openParams)
			{
				if (((FileDialogParams)openParams).get_FileNames() != null)
				{
					string[] fileNames = ((FileDialogParams)openParams).get_FileNames();
					foreach (string text in fileNames)
					{
						if (Path.GetExtension(text)!.ToUpper() == ".ZIP")
						{
							List<PrintItem> list = new ProjectFileHandler(null).ImportFromProjectArchive(text);
							if (list != null)
							{
								foreach (PrintItem item in list)
								{
									QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(item.Name, item.FileLocation)));
								}
							}
						}
						else
						{
							QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(Path.GetFileNameWithoutExtension(text), Path.GetFullPath(text))));
						}
					}
				}
			});
		}
	}
}
