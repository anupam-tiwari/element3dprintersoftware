using System;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrinterControls;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class ManualPrinterControlsTouchScreen : TabControl
	{
		private TemperatureControls temperatureControlsContainer;

		private MovementControls movementControlsContainer;

		private DisableableWidget fanControlsContainer;

		private DisableableWidget tuningAdjustmentControlsContainer;

		private DisableableWidget terminalControlsContainer;

		private DisableableWidget macroControlsContainer;

		private DisableableWidget actionControlsContainer;

		private int TabTextSize;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private event EventHandler unregisterEvents;

		public ManualPrinterControlsTouchScreen()
			: this((Orientation)1)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Expected O, but got Unknown
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Expected O, but got Unknown
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Expected O, but got Unknown
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Expected O, but got Unknown
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Expected O, but got Unknown
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Expected O, but got Unknown
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Expected O, but got Unknown
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Expected O, but got Unknown
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Expected O, but got Unknown
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Expected O, but got Unknown
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Expected O, but got Unknown
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Expected O, but got Unknown
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Expected O, but got Unknown
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Expected O, but got Unknown
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Expected O, but got Unknown
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Expected O, but got Unknown
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Expected O, but got Unknown
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Expected O, but got Unknown
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Expected O, but got Unknown
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Expected O, but got Unknown
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Expected O, but got Unknown
			RGBA_Bytes tabLabelUnselected = ActiveTheme.get_Instance().get_TabLabelUnselected();
			((GuiWidget)((TabControl)this).get_TabBar()).set_BackgroundColor(ActiveTheme.get_Instance().get_TransparentLightOverlay());
			((TabControl)this).get_TabBar().set_BorderColor(new RGBA_Bytes(0, 0, 0, 0));
			((GuiWidget)((TabControl)this).get_TabBar()).set_Margin(new BorderDouble(0.0));
			((GuiWidget)((TabControl)this).get_TabBar()).set_Padding(new BorderDouble(4.0, 4.0));
			((GuiWidget)this).AnchorAll();
			((GuiWidget)this).set_VAnchor((VAnchor)(((GuiWidget)this).get_VAnchor() | 8));
			((GuiWidget)this).set_Margin(new BorderDouble(0.0));
			TabTextSize = 13;
			GuiWidget val = new GuiWidget();
			val.set_Padding(new BorderDouble(6.0));
			val.AnchorAll();
			actionControlsContainer = new ActionControls();
			((GuiWidget)actionControlsContainer).set_VAnchor((VAnchor)4);
			if (Enumerable.Any<GCodeMacro>(ActiveSliceSettings.Instance.ActionMacros()))
			{
				val.AddChild((GuiWidget)(object)actionControlsContainer, -1);
			}
			if (Enumerable.Any<GCodeMacro>(ActiveSliceSettings.Instance.ActionMacros()))
			{
				TabPage val2 = new TabPage(val, "Actions".Localize().ToUpper());
				((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(val2, "Actions Tab", (double)TabTextSize, ActiveTheme.get_Instance().get_SecondaryAccentColor(), default(RGBA_Bytes), tabLabelUnselected, default(RGBA_Bytes)));
			}
			GuiWidget val3 = new GuiWidget();
			val3.set_Padding(new BorderDouble(6.0));
			val3.AnchorAll();
			temperatureControlsContainer = new TemperatureControls();
			TemperatureControls temperatureControls = temperatureControlsContainer;
			((GuiWidget)temperatureControls).set_VAnchor((VAnchor)(((GuiWidget)temperatureControls).get_VAnchor() | 4));
			val3.AddChild((GuiWidget)(object)temperatureControlsContainer, -1);
			TabPage val4 = new TabPage(val3, "Temperature".Localize().ToUpper());
			((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(val4, "Temperature Tab", (double)TabTextSize, ActiveTheme.get_Instance().get_SecondaryAccentColor(), default(RGBA_Bytes), tabLabelUnselected, default(RGBA_Bytes)));
			GuiWidget val5 = new GuiWidget();
			val5.set_Padding(new BorderDouble(6.0));
			val5.AnchorAll();
			movementControlsContainer = new MovementControls();
			((GuiWidget)movementControlsContainer).set_VAnchor((VAnchor)4);
			val5.AddChild((GuiWidget)(object)movementControlsContainer, -1);
			TabPage val6 = new TabPage(val5, "Movement".Localize().ToUpper());
			((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(val6, "Movement Tab", (double)TabTextSize, ActiveTheme.get_Instance().get_SecondaryAccentColor(), default(RGBA_Bytes), tabLabelUnselected, default(RGBA_Bytes)));
			GuiWidget val7 = new GuiWidget();
			val7.set_Padding(new BorderDouble(6.0));
			val7.AnchorAll();
			macroControlsContainer = new MacroControls();
			DisableableWidget disableableWidget = macroControlsContainer;
			((GuiWidget)disableableWidget).set_VAnchor((VAnchor)(((GuiWidget)disableableWidget).get_VAnchor() | 4));
			val7.AddChild((GuiWidget)(object)macroControlsContainer, -1);
			TabPage val8 = new TabPage(val7, "Macros".Localize().ToUpper());
			((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(val8, "Macros Tab", (double)TabTextSize, ActiveTheme.get_Instance().get_SecondaryAccentColor(), default(RGBA_Bytes), tabLabelUnselected, default(RGBA_Bytes)));
			if (ActiveSliceSettings.Instance.GetValue<bool>("has_fan"))
			{
				GuiWidget val9 = new GuiWidget();
				val9.set_Padding(new BorderDouble(6.0));
				val9.AnchorAll();
				fanControlsContainer = new FanControls();
				((GuiWidget)fanControlsContainer).set_VAnchor((VAnchor)4);
				val9.AddChild((GuiWidget)(object)fanControlsContainer, -1);
				TabPage val10 = new TabPage(val9, "Fan Controls".Localize().ToUpper());
				((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(val10, "Fan Controls Tab", (double)TabTextSize, ActiveTheme.get_Instance().get_SecondaryAccentColor(), default(RGBA_Bytes), tabLabelUnselected, default(RGBA_Bytes)));
			}
			GuiWidget val11 = new GuiWidget();
			val11.set_Padding(new BorderDouble(6.0));
			val11.AnchorAll();
			tuningAdjustmentControlsContainer = new AdjustmentControls();
			((GuiWidget)tuningAdjustmentControlsContainer).set_VAnchor((VAnchor)4);
			val11.AddChild((GuiWidget)(object)tuningAdjustmentControlsContainer, -1);
			TabPage val12 = new TabPage(val11, "Tuning Adjust".Localize().ToUpper());
			((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(val12, "Tuning Tab", (double)TabTextSize, ActiveTheme.get_Instance().get_SecondaryAccentColor(), default(RGBA_Bytes), tabLabelUnselected, default(RGBA_Bytes)));
			GuiWidget val13 = new GuiWidget();
			val13.set_Padding(new BorderDouble(6.0));
			val13.AnchorAll();
			terminalControlsContainer = new TerminalControls();
			DisableableWidget disableableWidget2 = terminalControlsContainer;
			((GuiWidget)disableableWidget2).set_VAnchor((VAnchor)(((GuiWidget)disableableWidget2).get_VAnchor() | 5));
			val13.AddChild((GuiWidget)(object)terminalControlsContainer, -1);
			TabPage val14 = new TabPage(val13, "Terminal".Localize().ToUpper());
			((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(val14, "Terminal Tab", (double)TabTextSize, ActiveTheme.get_Instance().get_SecondaryAccentColor(), default(RGBA_Bytes), tabLabelUnselected, default(RGBA_Bytes)));
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref this.unregisterEvents);
			PrinterConnectionAndCommunication.Instance.EnableChanged.RegisterEvent((EventHandler)onPrinterStatusChanged, ref this.unregisterEvents);
			SetVisibleControls();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			this.unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private void SetVisibleControls()
		{
			if (ActiveSliceSettings.Instance == null)
			{
				foreach (DisableableWidget extruderWidgetContainer in temperatureControlsContainer.ExtruderWidgetContainers)
				{
					extruderWidgetContainer.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				}
				temperatureControlsContainer?.BedTemperatureControlWidget.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				movementControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				fanControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				macroControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				actionControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
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
					extruderWidgetContainer2?.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
				}
				temperatureControlsContainer?.BedTemperatureControlWidget.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
				movementControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
				fanControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				macroControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
				actionControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				foreach (DisableableWidget disableableWidget in movementControlsContainer.DisableableWidgets)
				{
					disableableWidget.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				}
				movementControlsContainer.jogControls.SetEnabledLevels(enableBabysteppingMode: false, enableEControls: false);
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
				tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
				foreach (DisableableWidget disableableWidget2 in movementControlsContainer.DisableableWidgets)
				{
					disableableWidget2.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
				}
				movementControlsContainer.jogControls.SetEnabledLevels(enableBabysteppingMode: false, enableEControls: true);
				break;
			case PrinterConnectionAndCommunication.CommunicationStates.PrintingFromSd:
				foreach (DisableableWidget extruderWidgetContainer4 in temperatureControlsContainer.ExtruderWidgetContainers)
				{
					extruderWidgetContainer4.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
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
						extruderWidgetContainer5.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					}
					temperatureControlsContainer?.BedTemperatureControlWidget.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					fanControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
					macroControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.ConfigOnly);
					actionControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Disabled);
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
				tuningAdjustmentControlsContainer?.SetEnableLevel(DisableableWidget.EnableLevel.Enabled);
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

		private void onPrinterStatusChanged(object sender, EventArgs e)
		{
			SetVisibleControls();
			UiThread.RunOnIdle((Action)base.Invalidate);
		}
	}
}
