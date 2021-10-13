using System;
using System.IO;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrintHistory
{
	public static class PrintRecovery
	{
		private static PrintTask lastPrintTask;

		public static void CheckIfNeedToRecoverPrint(object sender, EventArgs e)
		{
			string yesOk = "Recover Print".Localize();
			string noCancel = "Cancel".Localize();
			string str = "WARNING: In order to perform print recovery, your printer must move down to reach its home position.\nIf your print is too large, part of your printer may collide with it when moving down.\nMake sure it is safe to perform this operation before proceeding.".Localize();
			string text = "It appears your last print failed to complete.\n\nWould your like to attempt to recover from the last know position?".Localize();
			string caption = "Recover Last Print".Localize();
			foreach (PrintTask historyItem in PrintHistoryData.Instance.GetHistoryItems(1))
			{
				if (!historyItem.PrintComplete && !string.IsNullOrEmpty(historyItem.PrintingGCodeFileName) && File.Exists(historyItem.PrintingGCodeFileName) && historyItem.PercentDone > 0.0 && ActiveSliceSettings.Instance.GetValue<bool>("recover_is_enabled") && !ActiveSliceSettings.Instance.GetValue<bool>("has_hardware_leveling"))
				{
					lastPrintTask = historyItem;
					if (ActiveSliceSettings.Instance.GetValue<bool>("z_homes_to_max"))
					{
						StyledMessageBox.ShowMessageBox(RecoverPrintProcessDialogResponse, text, caption, StyledMessageBox.MessageType.YES_NO, yesOk, noCancel);
					}
					else
					{
						StyledMessageBox.ShowMessageBox(RecoverPrintProcessDialogResponse, text + "\n\n" + str, caption, StyledMessageBox.MessageType.YES_NO, yesOk, noCancel);
					}
				}
			}
		}

		private static void RecoverPrintProcessDialogResponse(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				UiThread.RunOnIdle((Action)delegate
				{
					if (PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.Connected)
					{
						PrinterConnectionAndCommunication.Instance.CommunicationState = PrinterConnectionAndCommunication.CommunicationStates.PreparingToPrint;
						PrinterConnectionAndCommunication.Instance.StartPrint(lastPrintTask.PrintingGCodeFileName, lastPrintTask);
					}
				});
			}
			else
			{
				lastPrintTask.PrintingGCodeFileName = null;
				lastPrintTask.Commit();
			}
		}
	}
}
