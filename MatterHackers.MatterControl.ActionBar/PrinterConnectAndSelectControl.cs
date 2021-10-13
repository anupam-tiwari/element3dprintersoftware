using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.ActionBar
{
	public class PrinterConnectAndSelectControl : FlowLayoutWidget
	{
		private TextImageButtonFactory actionBarButtonFactory = new TextImageButtonFactory();

		private Button connectPrinterButton;

		private Button editPrinterButton;

		private string disconnectAndCancelTitle = "Disconnect and stop the current print?".Localize();

		private string disconnectAndCancelMessage = "WARNING: Disconnecting will stop the current print.\n\nAre you sure you want to disconnect?".Localize();

		private Button disconnectPrinterButton;

		private PrinterSelector printerSelector;

		private GuiWidget printerSelectorAndEditOverlay;

		private EventHandler unregisterEvents;

		private static EventHandler staticUnregisterEvents;

		public PrinterConnectAndSelectControl()
			: this((FlowDirection)0)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			actionBarButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			actionBarButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			actionBarButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			actionBarButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_TabLabelUnselected();
			actionBarButtonFactory.disabledFillColor = ActiveTheme.get_Instance().get_PrimaryBackgroundColor();
			actionBarButtonFactory.disabledBorderColor = ActiveTheme.get_Instance().get_SecondaryBackgroundColor();
			actionBarButtonFactory.hoverFillColor = ActiveTheme.get_Instance().get_PrimaryBackgroundColor();
			actionBarButtonFactory.invertImageLocation = true;
			actionBarButtonFactory.borderWidth = 0.0;
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			AddChildElements();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		protected void AddChildElements()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Expected O, but got Unknown
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Expected O, but got Unknown
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Expected O, but got Unknown
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			actionBarButtonFactory.invertImageLocation = false;
			actionBarButtonFactory.borderWidth = 1.0;
			if (ActiveTheme.get_Instance().get_IsDarkTheme())
			{
				actionBarButtonFactory.normalBorderColor = new RGBA_Bytes(77, 77, 77);
			}
			else
			{
				actionBarButtonFactory.normalBorderColor = new RGBA_Bytes(190, 190, 190);
			}
			actionBarButtonFactory.hoverBorderColor = new RGBA_Bytes(128, 128, 128);
			ImageBuffer normalImage = StaticData.get_Instance().LoadIcon("connect.png", 32, 32);
			connectPrinterButton = actionBarButtonFactory.Generate("Connect".Localize().ToUpper(), normalImage);
			((GuiWidget)connectPrinterButton).set_Name("Connect to printer button");
			((GuiWidget)connectPrinterButton).set_ToolTipText("Connect to the currently selected printer".Localize());
			((GuiWidget)connectPrinterButton).set_Margin(new BorderDouble(6.0, 0.0, 3.0, 3.0));
			((GuiWidget)connectPrinterButton).set_VAnchor((VAnchor)4);
			((GuiWidget)connectPrinterButton).set_Cursor((Cursors)3);
			((GuiWidget)connectPrinterButton).add_Click((EventHandler<MouseEventArgs>)delegate(object s, MouseEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (((GuiWidget)(Button)s).get_Enabled() && ActiveSliceSettings.Instance.PrinterSelected)
				{
					UserRequestedConnectToActivePrinter();
				}
			});
			disconnectPrinterButton = actionBarButtonFactory.Generate("Disconnect".Localize().ToUpper(), StaticData.get_Instance().LoadIcon("connect.png", 32, 32));
			((GuiWidget)disconnectPrinterButton).set_Name("Disconnect from printer button");
			((GuiWidget)disconnectPrinterButton).set_ToolTipText("Disconnect from current printer".Localize());
			((GuiWidget)disconnectPrinterButton).set_Margin(new BorderDouble(6.0, 0.0, 3.0, 3.0));
			((GuiWidget)disconnectPrinterButton).set_VAnchor((VAnchor)4);
			((GuiWidget)disconnectPrinterButton).set_Cursor((Cursors)3);
			((GuiWidget)disconnectPrinterButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)OnIdleDisconnect);
			});
			actionBarButtonFactory.invertImageLocation = true;
			((GuiWidget)this).AddChild((GuiWidget)(object)connectPrinterButton, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)disconnectPrinterButton, -1);
			GuiWidget val = new GuiWidget();
			val.set_HAnchor((HAnchor)5);
			val.set_VAnchor((VAnchor)8);
			GuiWidget val2 = val;
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val4 = val3;
			PrinterSelector obj = new PrinterSelector();
			((GuiWidget)obj).set_HAnchor((HAnchor)5);
			((GuiWidget)obj).set_Cursor((Cursors)3);
			((GuiWidget)obj).set_Margin(new BorderDouble(0.0, 6.0, 0.0, 3.0));
			printerSelector = obj;
			printerSelector.AddPrinter += delegate
			{
				WizardWindow.ShowPrinterSetup(userRequestedNewPrinter: true);
			};
			((GuiWidget)printerSelector).set_MinimumSize(new Vector2(0.0, ((GuiWidget)connectPrinterButton).get_MinimumSize().y));
			((GuiWidget)val4).AddChild((GuiWidget)(object)printerSelector, -1);
			editPrinterButton = TextImageButtonFactory.GetThemedEditButton();
			((GuiWidget)editPrinterButton).set_Name("Edit Printer Button");
			((GuiWidget)editPrinterButton).set_VAnchor((VAnchor)2);
			((GuiWidget)editPrinterButton).add_Click((EventHandler<MouseEventArgs>)UiNavigation.OpenEditPrinterWizard_Click);
			((GuiWidget)val4).AddChild((GuiWidget)(object)editPrinterButton, -1);
			val2.AddChild((GuiWidget)(object)val4, -1);
			GuiWidget val5 = new GuiWidget();
			val5.set_HAnchor((HAnchor)5);
			val5.set_VAnchor((VAnchor)5);
			val5.set_Selectable(false);
			printerSelectorAndEditOverlay = val5;
			val2.AddChild(printerSelectorAndEditOverlay, -1);
			((GuiWidget)this).AddChild(val2, -1);
			string label = "Reset\nConnection".Localize().ToUpper();
			Button resetConnectionButton = actionBarButtonFactory.Generate(label, "e_stop4.png");
			((GuiWidget)resetConnectionButton).set_Margin(new BorderDouble(6.0, 0.0, 3.0, 3.0));
			((GuiWidget)this).AddChild((GuiWidget)(object)resetConnectionButton, -1);
			((GuiWidget)resetConnectionButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)PrinterConnectionAndCommunication.Instance.RebootBoard);
			});
			((GuiWidget)resetConnectionButton).set_Visible(ActiveSliceSettings.Instance.GetValue<bool>("show_reset_connection"));
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)delegate(object sender, EventArgs e)
			{
				StringEventArgs val6 = e as StringEventArgs;
				if (val6 != null && val6.get_Data() == "show_reset_connection")
				{
					((GuiWidget)resetConnectionButton).set_Visible(ActiveSliceSettings.Instance.GetValue<bool>("show_reset_connection"));
				}
			}, ref unregisterEvents);
			SetConnectionButtonVisibleState();
			PrinterConnectionAndCommunication.Instance.EnableChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
		}

		public static void UserRequestedConnectToActivePrinter()
		{
			if (staticUnregisterEvents != null)
			{
				staticUnregisterEvents(null, null);
				staticUnregisterEvents = null;
			}
			PrinterConnectionAndCommunication.Instance.HaltConnectionThread();
			PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter(showHelpIfNoPort: true);
		}

		private void onConfirmStopPrint(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				PrinterConnectionAndCommunication.Instance.Stop(markPrintCanceled: false);
				PrinterConnectionAndCommunication.Instance.Disable();
				((GuiWidget)printerSelector).Invalidate();
			}
		}

		private void OnIdleDisconnect()
		{
			if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting)
			{
				StyledMessageBox.ShowMessageBox(onConfirmStopPrint, disconnectAndCancelMessage, disconnectAndCancelTitle, StyledMessageBox.MessageType.YES_NO, "Disconnect".Localize(), "Stay Connected".Localize());
				return;
			}
			PrinterConnectionAndCommunication.Instance.Disable();
			((GuiWidget)printerSelector).Invalidate();
		}

		private void onPrinterStatusChanged(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)SetConnectionButtonVisibleState);
		}

		private void SetConnectionButtonVisibleState()
		{
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			if (PrinterConnectionAndCommunication.Instance.PrinterIsConnected)
			{
				((GuiWidget)disconnectPrinterButton).set_Visible(true);
				((GuiWidget)connectPrinterButton).set_Visible(false);
			}
			else
			{
				((GuiWidget)disconnectPrinterButton).set_Visible(false);
				((GuiWidget)connectPrinterButton).set_Visible(true);
			}
			PrinterConnectionAndCommunication.CommunicationStates communicationState = PrinterConnectionAndCommunication.Instance.CommunicationState;
			((GuiWidget)connectPrinterButton).set_Enabled(communicationState != PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect && ActiveSliceSettings.Instance.PrinterSelected);
			bool flag = PrinterConnectionAndCommunication.Instance.PrinterIsPrinting || PrinterConnectionAndCommunication.Instance.PrinterIsPaused;
			((GuiWidget)editPrinterButton).set_Enabled(ActiveSliceSettings.Instance.PrinterSelected && !flag);
			((GuiWidget)printerSelector).set_Enabled(!flag);
			if (flag)
			{
				printerSelectorAndEditOverlay.set_BackgroundColor(new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryBackgroundColor(), 150));
			}
			else
			{
				printerSelectorAndEditOverlay.set_BackgroundColor(new RGBA_Bytes(0, 0, 0, 0));
			}
			((GuiWidget)disconnectPrinterButton).set_Enabled(communicationState != PrinterConnectionAndCommunication.CommunicationStates.Disconnecting);
		}
	}
}
