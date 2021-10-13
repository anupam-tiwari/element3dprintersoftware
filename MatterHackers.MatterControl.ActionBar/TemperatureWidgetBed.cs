using System;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.ActionBar
{
	internal class TemperatureWidgetBed : TemperatureWidgetBase
	{
		private EventHandler unregisterEvents;

		private string sliceSettingsNote = "Note: Slice Settings are applied before the print actually starts. Changes while printing will not effect the active print.".Localize();

		private string waitingForBedToHeatMessage = "The bed is currently heating and its target temperature cannot be changed until it reaches {0}°C.\n\nYou can set the starting bed temperature in SETTINGS -> Filament -> Temperatures.\n\n{1}".Localize();

		private string waitingForBedToHeatTitle = "Waiting For Bed To Heat".Localize();

		public TemperatureWidgetBed()
			: base("150.3°")
		{
			((GuiWidget)temperatureTypeName).set_Text("Print Bed");
			DisplayCurrentTemperature();
			((GuiWidget)this).set_ToolTipText("Current bed temperature".Localize());
			((GuiWidget)preheatButton).set_ToolTipText("Preheat the Bed".Localize());
			PrinterConnectionAndCommunication.Instance.BedTemperatureRead.RegisterEvent((EventHandler)delegate
			{
				DisplayCurrentTemperature();
			}, ref unregisterEvents);
		}

		private void DisplayCurrentTemperature()
		{
			string arg = "";
			if (PrinterConnectionAndCommunication.Instance.TargetBedTemperature > 0.0)
			{
				if ((int)(PrinterConnectionAndCommunication.Instance.TargetBedTemperature + 0.5) < (int)(PrinterConnectionAndCommunication.Instance.ActualBedTemperature + 0.5))
				{
					arg = "↓";
				}
				else if ((int)(PrinterConnectionAndCommunication.Instance.TargetBedTemperature + 0.5) > (int)(PrinterConnectionAndCommunication.Instance.ActualBedTemperature + 0.5))
				{
					arg = "↑";
				}
			}
			base.IndicatorValue = $" {PrinterConnectionAndCommunication.Instance.ActualBedTemperature:0.#}°{arg}";
		}

		protected override void SetTargetTemperature()
		{
			double value = ActiveSliceSettings.Instance.GetValue<double>("bed_temperature");
			if (value != 0.0)
			{
				double num = (int)(value + 0.5);
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && PrinterConnectionAndCommunication.Instance.PrintingState == PrinterConnectionAndCommunication.DetailedPrintingState.HeatingBed && num != PrinterConnectionAndCommunication.Instance.TargetBedTemperature)
				{
					string message = string.Format(waitingForBedToHeatMessage, PrinterConnectionAndCommunication.Instance.TargetBedTemperature, sliceSettingsNote);
					StyledMessageBox.ShowMessageBox(null, message, waitingForBedToHeatTitle);
				}
				else
				{
					PrinterConnectionAndCommunication.Instance.TargetBedTemperature = (int)(value + 0.5);
				}
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
