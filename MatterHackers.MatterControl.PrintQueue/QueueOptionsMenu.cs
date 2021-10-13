using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrintQueue
{
	public class QueueOptionsMenu : GuiWidget
	{
		public DropDownMenu MenuDropList;

		private TupleList<string, Func<bool>> menuItems;

		private ExportToFolderFeedbackWindow exportingWindow;

		private string pleaseSelectPrinterMessage = "Before you can export printable files, you must select a printer.";

		private string pleaseSelectPrinterTitle = "Please select a printer";

		public QueueOptionsMenu()
			: this()
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			MenuDropList = new DropDownMenu("Queue".Localize() + "... ", (Direction)0);
			((GuiWidget)MenuDropList).set_Name("Queue... Menu");
			((GuiWidget)MenuDropList).set_VAnchor((VAnchor)5);
			MenuDropList.BorderWidth = 1;
			MenuDropList.MenuAsWideAsItems = false;
			MenuDropList.BorderColor = ActiveTheme.get_Instance().get_SecondaryTextColor();
			((GuiWidget)MenuDropList).set_Margin(new BorderDouble(4.0, 0.0, 1.0, 0.0));
			((Menu)MenuDropList).set_AlignToRightEdge(true);
			SetMenuItems();
			MenuDropList.SelectionChanged += MenuDropList_SelectionChanged;
		}

		private void MenuDropList_SelectionChanged(object sender, EventArgs e)
		{
			string selectedValue = ((DropDownMenu)sender).SelectedValue;
			foreach (Tuple<string, Func<bool>> menuItem in menuItems)
			{
				if (menuItem.Item1 == selectedValue && menuItem.Item2 != null)
				{
					menuItem.Item2();
				}
			}
		}

		private void SetMenuItems()
		{
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Invalid comparison between Unknown and I4
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			menuItems = new TupleList<string, Func<bool>>();
			menuItems.Add(new Tuple<string, Func<bool>>("Design".Localize(), null));
			menuItems.Add(new Tuple<string, Func<bool>>(" Export to Zip".Localize(), exportQueueToZipMenu_Click));
			menuItems.Add(new Tuple<string, Func<bool>>("G-Code".Localize(), null));
			menuItems.Add(new Tuple<string, Func<bool>>(" Export to Folder or SD Card".Localize(), exportGCodeToFolderButton_Click));
			if (ActiveSliceSettings.Instance.GetValue<bool>("has_sd_card_reader"))
			{
				menuItems.Add(new Tuple<string, Func<bool>>("SD Card".Localize(), null));
				menuItems.Add(new Tuple<string, Func<bool>>(" Load Files".Localize(), loadFilesFromSDButton_Click));
				menuItems.Add(new Tuple<string, Func<bool>>(" Eject SD Card".Localize(), ejectSDCardButton_Click));
			}
			if ((int)OsInformation.get_OperatingSystem() == 1)
			{
				menuItems.Add(new Tuple<string, Func<bool>>("Other".Localize(), null));
				menuItems.Add(new Tuple<string, Func<bool>>(" Create Part Sheet".Localize(), createPartsSheetsButton_Click));
				menuItems.Add(new Tuple<string, Func<bool>>(" Remove All".Localize(), removeAllFromQueueButton_Click));
			}
			else
			{
				menuItems.Add(new Tuple<string, Func<bool>>("Other".Localize(), null));
				menuItems.Add(new Tuple<string, Func<bool>>(" Remove All".Localize(), removeAllFromQueueButton_Click));
			}
			BorderDouble menuItemsPadding = MenuDropList.MenuItemsPadding;
			foreach (Tuple<string, Func<bool>> menuItem in menuItems)
			{
				if (menuItem.Item2 == null)
				{
					MenuDropList.MenuItemsPadding = new BorderDouble(5.0, 0.0, menuItemsPadding.Right, 3.0);
				}
				else
				{
					MenuDropList.MenuItemsPadding = new BorderDouble(10.0, 5.0, menuItemsPadding.Right, 5.0);
				}
				MenuItem val = MenuDropList.AddItem(menuItem.Item1);
				if (menuItem.Item2 == null)
				{
					((GuiWidget)val).set_Enabled(false);
				}
			}
			((GuiWidget)MenuDropList).set_Padding(menuItemsPadding);
		}

		private bool createPartsSheetsButton_Click()
		{
			UiThread.RunOnIdle((Action)PartSheetClickOnIdle);
			return true;
		}

		private void PartSheetClickOnIdle()
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			List<PrintItem> parts = QueueData.Instance.CreateReadOnlyPartList(includeProtectedItems: true);
			if (parts.Count <= 0)
			{
				return;
			}
			SaveFileDialogParams val = new SaveFileDialogParams("Save Parts Sheet|*.pdf", "", "", "");
			((FileDialogParams)val).set_ActionButtonLabel("Save Parts Sheet".Localize());
			((FileDialogParams)val).set_Title(string.Format("{0}: {1}", "Element".Localize(), "Save".Localize()));
			FileDialog.SaveFileDialog(val, (Action<SaveFileDialogParams>)delegate(SaveFileDialogParams saveParams)
			{
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				if (!string.IsNullOrEmpty(((FileDialogParams)saveParams).get_FileName()))
				{
					PartsSheet partsSheet = new PartsSheet(parts, ((FileDialogParams)saveParams).get_FileName());
					partsSheet.SaveSheets();
					SavePartsSheetFeedbackWindow savePartsSheetFeedbackWindow = new SavePartsSheetFeedbackWindow(parts.Count, parts[0].Name, ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
					partsSheet.UpdateRemainingItems += savePartsSheetFeedbackWindow.StartingNextPart;
					partsSheet.DoneSaving += savePartsSheetFeedbackWindow.DoneSaving;
					((SystemWindow)savePartsSheetFeedbackWindow).ShowAsSystemWindow();
				}
			});
		}

		private void MustSelectPrinterMessage()
		{
			StyledMessageBox.ShowMessageBox(null, pleaseSelectPrinterMessage, pleaseSelectPrinterTitle);
		}

		private bool exportGCodeToFolderButton_Click()
		{
			if (!ActiveSliceSettings.Instance.PrinterSelected)
			{
				UiThread.RunOnIdle((Action)MustSelectPrinterMessage);
			}
			else
			{
				if (!ActiveSliceSettings.Instance.IsValid())
				{
					return false;
				}
				UiThread.RunOnIdle((Action)SelectLocationToExportGCode);
			}
			return true;
		}

		private bool exportX3GButton_Click()
		{
			if (!ActiveSliceSettings.Instance.PrinterSelected)
			{
				UiThread.RunOnIdle((Action)MustSelectPrinterMessage);
			}
			else
			{
				UiThread.RunOnIdle((Action)SelectLocationToExportGCode);
			}
			return true;
		}

		private void ExportToFolderFeedbackWindow_Closed(object sender, ClosedEventArgs e)
		{
			exportingWindow = null;
		}

		private void SelectLocationToExportGCode()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			SelectFolderDialogParams val = new SelectFolderDialogParams("Select Location To Save Files", (RootFolderTypes)0, true, "", "");
			val.set_ActionButtonLabel("Export".Localize());
			val.set_Title("Element: Select A Folder");
			FileDialog.SelectFolderDialog(val, (Action<SelectFolderDialogParams>)onSelectFolderDialog);
		}

		private void onSelectFolderDialog(SelectFolderDialogParams openParams)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			string folderPath = openParams.get_FolderPath();
			if (folderPath == null || !(folderPath != ""))
			{
				return;
			}
			List<PrintItem> list = QueueData.Instance.CreateReadOnlyPartList(includeProtectedItems: true);
			if (list.Count > 0)
			{
				if (exportingWindow == null)
				{
					exportingWindow = new ExportToFolderFeedbackWindow(list.Count, list[0].Name, ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
					((GuiWidget)exportingWindow).add_Closed((EventHandler<ClosedEventArgs>)ExportToFolderFeedbackWindow_Closed);
					((SystemWindow)exportingWindow).ShowAsSystemWindow();
				}
				else
				{
					((GuiWidget)exportingWindow).BringToFront();
				}
				ExportToFolderProcess exportToFolderProcess = new ExportToFolderProcess(list, folderPath);
				exportToFolderProcess.StartingNextPart += exportingWindow.StartingNextPart;
				exportToFolderProcess.UpdatePartStatus += exportingWindow.UpdatePartStatus;
				exportToFolderProcess.DoneSaving += exportingWindow.DoneSaving;
				exportToFolderProcess.Start();
			}
		}

		private bool exportQueueToZipMenu_Click()
		{
			if (!ActiveSliceSettings.Instance.PrinterSelected)
			{
				UiThread.RunOnIdle((Action)MustSelectPrinterMessage);
			}
			else
			{
				if (!ActiveSliceSettings.Instance.IsValid())
				{
					return false;
				}
				UiThread.RunOnIdle((Action)ExportQueueToZipOnIdle);
			}
			return true;
		}

		private void ExportQueueToZipOnIdle()
		{
			new ProjectFileHandler(QueueData.Instance.CreateReadOnlyPartList(includeProtectedItems: false)).SaveAs();
		}

		private bool loadFilesFromSDButton_Click()
		{
			QueueData.Instance.LoadFilesFromSD();
			return true;
		}

		private bool ejectSDCardButton_Click()
		{
			QueueData.Instance.RemoveAllSdCardFiles();
			PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow("M22");
			return true;
		}

		private bool removeAllFromQueueButton_Click()
		{
			UiThread.RunOnIdle((Action)removeAllPrintsFromQueue);
			return true;
		}

		private void removeAllPrintsFromQueue()
		{
			QueueData.Instance.RemoveAll();
		}
	}
}
