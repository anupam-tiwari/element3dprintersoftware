using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.ConfigurationPage.PrintLeveling;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.ActionBar
{
	internal class PrintActionRow : FlowLayoutWidget
	{
		private List<Button> activePrintButtons = new List<Button>();

		private Button addButton;

		private List<Button> allPrintButtons = new List<Button>();

		private Button cancelButton;

		private Button cancelConnectButton;

		private Button touchScreenConnectButton;

		private Button addPrinterButton;

		private Button selectPrinterButton;

		private Button resetConnectionButton;

		private Button dequeueCurrentPartButton;

		private Button doneWithCurrentPartButton;

		private Button pauseButton;

		private QueueDataView queueDataView;

		private Button removeButton;

		private Button reprintButton;

		private Button resumeButton;

		private Button skipButton;

		private Button startButton;

		private Button finishSetupButton;

		private TextImageButtonFactory textImageButtonFactory;

		private EventHandler unregisterEvents;

		public PrintActionRow(QueueDataView queueDataView)
			: this((FlowDirection)0)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			textImageButtonFactory = new TextImageButtonFactory
			{
				normalTextColor = RGBA_Bytes.White,
				disabledTextColor = RGBA_Bytes.LightGray,
				hoverTextColor = RGBA_Bytes.White,
				pressedTextColor = RGBA_Bytes.White,
				AllowThemeToAdjustImage = false,
				borderWidth = 1.0,
				FixedHeight = 52.0 * GuiWidget.get_DeviceScale(),
				fontSize = 14.0,
				normalBorderColor = new RGBA_Bytes(255, 255, 255, 100),
				hoverBorderColor = new RGBA_Bytes(255, 255, 255, 100)
			};
			this.queueDataView = queueDataView;
			AddChildElements();
			PrinterConnectionAndCommunication.Instance.ActivePrintItemChanged.RegisterEvent((EventHandler)onStateChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)onStateChanged, ref unregisterEvents);
			ProfileManager.ProfilesListChanged.RegisterEvent((EventHandler)onStateChanged, ref unregisterEvents);
		}

		protected void AddChildElements()
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			addButton = textImageButtonFactory.GenerateTooltipButton("Add".Localize(), ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("icon_circle_plus.png", 32, 32)));
			((GuiWidget)addButton).set_ToolTipText("Add a file to be printed".Localize());
			((GuiWidget)addButton).set_Margin(new BorderDouble(6.0, 6.0, 6.0, 3.0));
			((GuiWidget)addButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)AddButtonOnIdle);
			});
			startButton = textImageButtonFactory.GenerateTooltipButton("Print".Localize(), ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("icon_play_32x32.png", 32, 32)));
			((GuiWidget)startButton).set_Name("Start Print Button");
			((GuiWidget)startButton).set_ToolTipText("Begin printing the selected item.".Localize());
			((GuiWidget)startButton).set_Margin(new BorderDouble(6.0, 6.0, 6.0, 3.0));
			((GuiWidget)startButton).add_Click((EventHandler<MouseEventArgs>)onStartButton_Click);
			finishSetupButton = textImageButtonFactory.GenerateTooltipButton("Finish Setup...".Localize());
			((GuiWidget)finishSetupButton).set_Name("Finish Setup Button");
			((GuiWidget)finishSetupButton).set_ToolTipText("Run setup configuration for printer.".Localize());
			((GuiWidget)finishSetupButton).set_Margin(new BorderDouble(6.0, 6.0, 6.0, 3.0));
			((GuiWidget)finishSetupButton).add_Click((EventHandler<MouseEventArgs>)onStartButton_Click);
			touchScreenConnectButton = textImageButtonFactory.GenerateTooltipButton("Connect".Localize(), ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("connect.png", 32, 32)));
			((GuiWidget)touchScreenConnectButton).set_ToolTipText("Connect to the printer".Localize());
			((GuiWidget)touchScreenConnectButton).set_Margin(new BorderDouble(6.0, 6.0, 6.0, 3.0));
			((GuiWidget)touchScreenConnectButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (ActiveSliceSettings.Instance.PrinterSelected)
				{
					PrinterConnectionAndCommunication.Instance.HaltConnectionThread();
					PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter(showHelpIfNoPort: true);
				}
			});
			addPrinterButton = textImageButtonFactory.GenerateTooltipButton("Add Printer".Localize());
			((GuiWidget)addPrinterButton).set_ToolTipText("Select and add a new printer.".Localize());
			((GuiWidget)addPrinterButton).set_Margin(new BorderDouble(6.0, 6.0, 6.0, 3.0));
			((GuiWidget)addPrinterButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					WizardWindow.ShowPrinterSetup(userRequestedNewPrinter: true);
				});
			});
			selectPrinterButton = textImageButtonFactory.GenerateTooltipButton("Select Printer".Localize());
			((GuiWidget)selectPrinterButton).set_ToolTipText("Select an existing printer.".Localize());
			((GuiWidget)selectPrinterButton).set_Margin(new BorderDouble(6.0, 6.0, 6.0, 3.0));
			((GuiWidget)selectPrinterButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				WizardWindow.Show<SetupOptionsPage>("/SetupOptions", "Setup Wizard");
			});
			resetConnectionButton = textImageButtonFactory.GenerateTooltipButton("Reset".Localize(), ExtensionMethods.InvertLightness(StaticData.get_Instance().LoadIcon("e_stop4.png", 32, 32)));
			((GuiWidget)resetConnectionButton).set_ToolTipText("Reboots the firmware on the controller".Localize());
			((GuiWidget)resetConnectionButton).set_Margin(new BorderDouble(6.0, 6.0, 6.0, 3.0));
			((GuiWidget)resetConnectionButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)PrinterConnectionAndCommunication.Instance.RebootBoard);
			});
			skipButton = makeButton("Skip".Localize(), "Skip the current item and move to the next in queue".Localize());
			((GuiWidget)skipButton).add_Click((EventHandler<MouseEventArgs>)onSkipButton_Click);
			removeButton = makeButton("Remove".Localize(), "Remove current item from queue".Localize());
			((GuiWidget)removeButton).add_Click((EventHandler<MouseEventArgs>)onRemoveButton_Click);
			pauseButton = makeButton("Pause".Localize(), "Pause the current print".Localize());
			((GuiWidget)pauseButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)PrinterConnectionAndCommunication.Instance.RequestPause);
				((GuiWidget)pauseButton).set_Enabled(false);
			});
			((GuiWidget)this).AddChild((GuiWidget)(object)pauseButton, -1);
			allPrintButtons.Add(pauseButton);
			cancelConnectButton = makeButton("Cancel Connect".Localize(), "Stop trying to connect to the printer.".Localize());
			((GuiWidget)cancelConnectButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					ApplicationController.Instance.ConditionalCancelPrint();
					UiThread.RunOnIdle((Action)SetButtonStates);
				});
			});
			cancelButton = makeButton("Cancel".Localize(), "Stop the current print".Localize());
			((GuiWidget)cancelButton).set_Name("Cancel Print Button");
			((GuiWidget)cancelButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					ApplicationController.Instance.ConditionalCancelPrint();
					SetButtonStates();
				});
			});
			resumeButton = makeButton("Resume".Localize(), "Resume the current print".Localize());
			((GuiWidget)resumeButton).set_Name("Resume Button");
			((GuiWidget)resumeButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
				{
					PrinterConnectionAndCommunication.Instance.Resume();
				}
				((GuiWidget)pauseButton).set_Enabled(true);
			});
			((GuiWidget)this).AddChild((GuiWidget)(object)resumeButton, -1);
			allPrintButtons.Add(resumeButton);
			reprintButton = makeButton("Print Again".Localize(), "Print current item again".Localize());
			((GuiWidget)reprintButton).set_Name("Print Again Button");
			((GuiWidget)reprintButton).add_Click((EventHandler<MouseEventArgs>)onReprintButton_Click);
			doneWithCurrentPartButton = makeButton("Done".Localize(), "Finished printing for now".Localize());
			((GuiWidget)doneWithCurrentPartButton).set_Name("Done Button");
			((GuiWidget)doneWithCurrentPartButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				PrinterConnectionAndCommunication.Instance.ResetToReadyState();
			});
			dequeueCurrentPartButton = makeButton("Dequeue".Localize(), "Remove part and move to next print in queue".Localize());
			((GuiWidget)dequeueCurrentPartButton).set_Name("Dequeue Button");
			((GuiWidget)dequeueCurrentPartButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					PrinterConnectionAndCommunication.Instance.ResetToReadyState();
					QueueData.Instance.RemoveAt(QueueData.Instance.SelectedIndex);
				});
			});
			((GuiWidget)this).set_Margin(new BorderDouble(0.0, 0.0, 10.0, 0.0));
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).AddChild((GuiWidget)(object)touchScreenConnectButton, -1);
			allPrintButtons.Add(touchScreenConnectButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)addPrinterButton, -1);
			allPrintButtons.Add(addPrinterButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)selectPrinterButton, -1);
			allPrintButtons.Add(selectPrinterButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)addButton, -1);
			allPrintButtons.Add(addButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)startButton, -1);
			allPrintButtons.Add(startButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)finishSetupButton, -1);
			allPrintButtons.Add(finishSetupButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)doneWithCurrentPartButton, -1);
			allPrintButtons.Add(doneWithCurrentPartButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)dequeueCurrentPartButton, -1);
			allPrintButtons.Add(dequeueCurrentPartButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)skipButton, -1);
			allPrintButtons.Add(skipButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)cancelButton, -1);
			allPrintButtons.Add(cancelButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)cancelConnectButton, -1);
			allPrintButtons.Add(cancelConnectButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)reprintButton, -1);
			allPrintButtons.Add(reprintButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)removeButton, -1);
			allPrintButtons.Add(removeButton);
			((GuiWidget)this).AddChild((GuiWidget)(object)resetConnectionButton, -1);
			allPrintButtons.Add(resetConnectionButton);
			SetButtonStates();
			PrinterSettings.PrintLevelingEnabledChanged.RegisterEvent((EventHandler)delegate
			{
				SetButtonStates();
			}, ref unregisterEvents);
		}

		protected void DisableActiveButtons()
		{
			foreach (Button activePrintButton in activePrintButtons)
			{
				((GuiWidget)activePrintButton).set_Enabled(false);
			}
		}

		protected void EnableActiveButtons()
		{
			foreach (Button activePrintButton in activePrintButtons)
			{
				((GuiWidget)activePrintButton).set_Enabled(true);
			}
		}

		protected Button makeButton(string buttonText, string buttonToolTip = "")
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			Button obj = textImageButtonFactory.GenerateTooltipButton(buttonText);
			((GuiWidget)obj).set_ToolTipText(buttonToolTip);
			((GuiWidget)obj).set_Margin(new BorderDouble(0.0, 6.0, 6.0, 3.0));
			return obj;
		}

		protected void SetButtonStates()
		{
			activePrintButtons.Clear();
			if (!PrinterConnectionAndCommunication.Instance.PrinterIsConnected && PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect)
			{
				if (!Enumerable.Any<PrinterInfo>(ProfileManager.Instance.ActiveProfiles))
				{
					activePrintButtons.Add(addPrinterButton);
				}
				else if (UserSettings.Instance.IsTouchScreen)
				{
					if (ActiveSliceSettings.Instance.PrinterSelected)
					{
						activePrintButtons.Add(touchScreenConnectButton);
					}
					else
					{
						activePrintButtons.Add(selectPrinterButton);
					}
				}
				ShowActiveButtons();
				EnableActiveButtons();
			}
			else if (PrinterConnectionAndCommunication.Instance.ActivePrintItem == null)
			{
				activePrintButtons.Add(addButton);
				ShowActiveButtons();
				EnableActiveButtons();
			}
			else
			{
				switch (PrinterConnectionAndCommunication.Instance.CommunicationState)
				{
				case PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect:
					activePrintButtons.Add(cancelConnectButton);
					EnableActiveButtons();
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.Connected:
				{
					PrintLevelingData printLevelingData = ActiveSliceSettings.Instance.Helpers.GetPrintLevelingData();
					if (printLevelingData != null && ActiveSliceSettings.Instance.GetValue<bool>("print_leveling_required_to_print") && !printLevelingData.HasBeenRunAndEnabled())
					{
						activePrintButtons.Add(finishSetupButton);
					}
					else
					{
						activePrintButtons.Add(startButton);
						if (QueueData.Instance.ItemCount > 1)
						{
							activePrintButtons.Add(skipButton);
						}
						activePrintButtons.Add(removeButton);
					}
					EnableActiveButtons();
					break;
				}
				case PrinterConnectionAndCommunication.CommunicationStates.PreparingToPrint:
					activePrintButtons.Add(cancelButton);
					EnableActiveButtons();
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.Printing:
				case PrinterConnectionAndCommunication.CommunicationStates.PrintingFromSd:
					if (!PrinterConnectionAndCommunication.Instance.PrintWasCanceled)
					{
						activePrintButtons.Add(pauseButton);
						activePrintButtons.Add(cancelButton);
					}
					else if (UserSettings.Instance.IsTouchScreen)
					{
						activePrintButtons.Add(resetConnectionButton);
					}
					EnableActiveButtons();
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.Paused:
					activePrintButtons.Add(resumeButton);
					activePrintButtons.Add(cancelButton);
					EnableActiveButtons();
					break;
				case PrinterConnectionAndCommunication.CommunicationStates.FinishedPrint:
					activePrintButtons.Add(reprintButton);
					activePrintButtons.Add(doneWithCurrentPartButton);
					activePrintButtons.Add(dequeueCurrentPartButton);
					EnableActiveButtons();
					break;
				default:
					DisableActiveButtons();
					break;
				}
			}
			if (PrinterConnectionAndCommunication.Instance.PrinterIsConnected && ActiveSliceSettings.Instance.GetValue<bool>("show_reset_connection") && UserSettings.Instance.IsTouchScreen)
			{
				activePrintButtons.Add(resetConnectionButton);
				ShowActiveButtons();
				EnableActiveButtons();
			}
			ShowActiveButtons();
		}

		protected void ShowActiveButtons()
		{
			foreach (Button allPrintButton in allPrintButtons)
			{
				if (activePrintButtons.IndexOf(allPrintButton) >= 0)
				{
					((GuiWidget)allPrintButton).set_Visible(true);
				}
				else
				{
					((GuiWidget)allPrintButton).set_Visible(false);
				}
			}
		}

		private void AddButtonOnIdle()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			FileDialog.OpenFileDialog(new OpenFileDialogParams(ApplicationSettings.OpenPrintableFileParams, "", true, "", ""), (Action<OpenFileDialogParams>)delegate(OpenFileDialogParams openParams)
			{
				if (((FileDialogParams)openParams).get_FileNames() != null)
				{
					string[] fileNames = ((FileDialogParams)openParams).get_FileNames();
					foreach (string path in fileNames)
					{
						QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(Path.GetFileNameWithoutExtension(path), Path.GetFullPath(path))));
					}
				}
			});
		}

		private void RunTroubleShooting()
		{
			WizardWindow.Show<SetupWizardTroubleshooting>("TroubleShooting", "Trouble Shooting");
		}

		private void onRemoveButton_Click(object sender, EventArgs mouseEvent)
		{
			QueueData.Instance.RemoveAt(QueueData.Instance.SelectedIndex);
		}

		private void onReprintButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				PrinterConnectionAndCommunication.Instance.PrintActivePartIfPossible();
			});
		}

		private void onSkipButton_Click(object sender, EventArgs mouseEvent)
		{
			if (QueueData.Instance.ItemCount > 1)
			{
				QueueData.Instance.MoveToNext();
			}
		}

		private void onStartButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				PrinterConnectionAndCommunication.Instance.PrintActivePartIfPossible();
			});
		}

		private void onStateChanged(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)SetButtonStates);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
