using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrinterControls;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class ManualPrinterControlsDesktop : ScrollableWidget
	{
		private DisableableWidget fanControlsContainer;

		private DisableableWidget macroControlsContainer;

		private DisableableWidget actionControlsContainer;

		private MovementControls movementControlsContainer;

		private TemperatureControls temperatureControlsContainer;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private DisableableWidget tuningAdjustmentControlsContainer;

		private EventHandler unregisterEvents;

		public ManualPrinterControlsDesktop()
			: this(false)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected O, but got Unknown
			ScrollingArea scrollArea = ((ScrollableWidget)this).get_ScrollArea();
			((GuiWidget)scrollArea).set_HAnchor((HAnchor)(((GuiWidget)scrollArea).get_HAnchor() | 5));
			((GuiWidget)this).AnchorAll();
			((ScrollableWidget)this).set_AutoScroll(true);
			((GuiWidget)this).set_HAnchor((HAnchor)13);
			((GuiWidget)this).set_VAnchor((VAnchor)5);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)13);
			((GuiWidget)val).set_VAnchor((VAnchor)8);
			((GuiWidget)val).set_Name("ManualPrinterControls.ControlsContainer");
			((GuiWidget)val).set_Margin(new BorderDouble(0.0));
			AddActionControls(val);
			AddTemperatureControls(val);
			AddMovementControls(val);
			AddMacroControls(val);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			AddAtxPowerControls(val2);
			AddAdjustmentControls(val);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			AddHandlers();
			SetVisibleControls();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private void AddAdjustmentControls(FlowLayoutWidget controlsTopToBottomLayout)
		{
			tuningAdjustmentControlsContainer = new AdjustmentControls();
			((GuiWidget)controlsTopToBottomLayout).AddChild((GuiWidget)(object)tuningAdjustmentControlsContainer, -1);
			UiThread.RunOnIdle((Action)delegate
			{
				((GuiWidget)tuningAdjustmentControlsContainer).set_Width(((GuiWidget)tuningAdjustmentControlsContainer).get_Width() + 1.0);
			});
		}

		private void AddFanControls(FlowLayoutWidget controlsTopToBottomLayout)
		{
			fanControlsContainer = new FanControls();
			if (ActiveSliceSettings.Instance.GetValue<bool>("has_fan"))
			{
				((GuiWidget)controlsTopToBottomLayout).AddChild((GuiWidget)(object)fanControlsContainer, -1);
			}
		}

		private void AddAtxPowerControls(FlowLayoutWidget controlsTopToBottomLayout)
		{
			((GuiWidget)controlsTopToBottomLayout).AddChild((GuiWidget)(object)new PowerControls(), -1);
		}

		private void AddHandlers()
		{
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.EnableChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref unregisterEvents);
		}

		private void AddActionControls(FlowLayoutWidget controlsTopToBottomLayout)
		{
			actionControlsContainer = new ActionControls();
			((GuiWidget)controlsTopToBottomLayout).AddChild((GuiWidget)(object)actionControlsContainer, -1);
		}

		private void AddMacroControls(FlowLayoutWidget controlsTopToBottomLayout)
		{
			macroControlsContainer = new MacroControls();
			((GuiWidget)controlsTopToBottomLayout).AddChild((GuiWidget)(object)macroControlsContainer, -1);
		}

		private void AddMovementControls(FlowLayoutWidget controlsTopToBottomLayout)
		{
			movementControlsContainer = new MovementControls();
			((GuiWidget)controlsTopToBottomLayout).AddChild((GuiWidget)(object)movementControlsContainer, -1);
		}

		private void AddTemperatureControls(FlowLayoutWidget controlsTopToBottomLayout)
		{
			temperatureControlsContainer = new TemperatureControls();
			((GuiWidget)controlsTopToBottomLayout).AddChild((GuiWidget)(object)temperatureControlsContainer, -1);
		}

		private void invalidateWidget()
		{
			((GuiWidget)this).Invalidate();
		}

		private void onPrinterStatusChanged(object sender, EventArgs e)
		{
			SetVisibleControls();
			UiThread.RunOnIdle((Action)invalidateWidget);
		}

		private void SetVisibleControls()
		{
			if (!ActiveSliceSettings.Instance.PrinterSelected)
			{
				foreach (DisableableWidget extruderWidgetContainer in temperatureControlsContainer.ExtruderWidgetContainers)
				{
					extruderWidgetContainer.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				}
				temperatureControlsContainer?.BedTemperatureControlWidget.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				movementControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				fanControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				macroControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				actionControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				return;
			}
			switch (PrinterConnectionAndCommunication.Instance.CommunicationState)
			{
			case PrinterConnectionAndCommunication.CommunicationStates.Disconnected:
			case PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect:
			case PrinterConnectionAndCommunication.CommunicationStates.FailedToConnect:
			case PrinterConnectionAndCommunication.CommunicationStates.Disconnecting:
			case PrinterConnectionAndCommunication.CommunicationStates.ConnectionLost:
				foreach (DisableableWidget extruderWidgetContainer2 in temperatureControlsContainer.ExtruderWidgetContainers)
				{
					extruderWidgetContainer2.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
				}
				temperatureControlsContainer?.BedTemperatureControlWidget.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
				movementControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
				fanControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				macroControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
				actionControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				foreach (DisableableWidget disableableWidget in movementControlsContainer.DisableableWidgets)
				{
					disableableWidget?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				}
				movementControlsContainer?.jogControls.SetEnabledLevels(enableBabysteppingMode: false, enableEControls: false);
				break;
			case PrinterConnectionAndCommunication.CommunicationStates.Connected:
			case PrinterConnectionAndCommunication.CommunicationStates.FinishedPrint:
				foreach (DisableableWidget extruderWidgetContainer3 in temperatureControlsContainer.ExtruderWidgetContainers)
				{
					extruderWidgetContainer3.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				}
				temperatureControlsContainer?.BedTemperatureControlWidget.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				movementControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				fanControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				macroControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				actionControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				foreach (DisableableWidget disableableWidget2 in movementControlsContainer.DisableableWidgets)
				{
					disableableWidget2?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				}
				movementControlsContainer?.jogControls.SetEnabledLevels(enableBabysteppingMode: false, enableEControls: true);
				break;
			case PrinterConnectionAndCommunication.CommunicationStates.PrintingFromSd:
				foreach (DisableableWidget extruderWidgetContainer4 in temperatureControlsContainer.ExtruderWidgetContainers)
				{
					extruderWidgetContainer4?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				}
				temperatureControlsContainer?.BedTemperatureControlWidget.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				movementControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
				fanControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				macroControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
				actionControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				break;
			case PrinterConnectionAndCommunication.CommunicationStates.PreparingToPrint:
			case PrinterConnectionAndCommunication.CommunicationStates.Printing:
			{
				PrinterConnectionAndCommunication.DetailedPrintingState printingState = PrinterConnectionAndCommunication.Instance.PrintingState;
				if ((uint)printingState <= 3u)
				{
					foreach (DisableableWidget extruderWidgetContainer5 in temperatureControlsContainer.ExtruderWidgetContainers)
					{
						extruderWidgetContainer5?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					}
					temperatureControlsContainer?.BedTemperatureControlWidget.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					fanControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					macroControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
					actionControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
					tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					foreach (DisableableWidget disableableWidget3 in movementControlsContainer.DisableableWidgets)
					{
						disableableWidget3?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
					}
					movementControlsContainer?.jogControls.SetEnabledLevels(enableBabysteppingMode: true, enableEControls: false);
					break;
				}
				throw new NotImplementedException();
			}
			case PrinterConnectionAndCommunication.CommunicationStates.Paused:
				foreach (DisableableWidget extruderWidgetContainer6 in temperatureControlsContainer.ExtruderWidgetContainers)
				{
					extruderWidgetContainer6?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				}
				temperatureControlsContainer?.BedTemperatureControlWidget.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				movementControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				fanControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				macroControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				actionControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				foreach (DisableableWidget disableableWidget4 in movementControlsContainer.DisableableWidgets)
				{
					disableableWidget4?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				}
				movementControlsContainer?.jogControls.SetEnabledLevels(enableBabysteppingMode: false, enableEControls: true);
				break;
			default:
				throw new NotImplementedException();
			}
		}
	}
}
