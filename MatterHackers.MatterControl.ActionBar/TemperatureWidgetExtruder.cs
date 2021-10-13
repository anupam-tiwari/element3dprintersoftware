using System;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.ActionBar
{
	internal class TemperatureWidgetExtruder : TemperatureWidgetBase
	{
		private const int extruderIndex = 0;

		private EventHandler unregisterEvents;

		private string sliceSettingsNote = "Note: Slice Settings are applied before the print actually starts. Changes while printing will not effect the active print.".Localize();

		private string waitingForExtruderToHeatMessage = "The extruder is currently heating and its target temperature cannot be changed until it reaches {0}°C.\n\nYou can set the starting extruder temperature in 'Slice Settings' -> 'Filament'.\n\n{1}".Localize();

		public TemperatureWidgetExtruder()
			: base("150.3°")
		{
			((GuiWidget)temperatureTypeName).set_Text("Extruder");
			DisplayCurrentTemperature();
			((GuiWidget)this).set_ToolTipText("Current extruder temperature".Localize());
			((GuiWidget)preheatButton).set_ToolTipText("Preheat the Extruder".Localize());
			PrinterConnectionAndCommunication.Instance.ExtruderTemperatureRead.RegisterEvent((EventHandler)delegate
			{
				DisplayCurrentTemperature();
			}, ref unregisterEvents);
		}

		private void DisplayCurrentTemperature()
		{
			string arg = "";
			if (PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(0) > 0.0)
			{
				if ((int)(PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(0) + 0.5) < (int)(PrinterConnectionAndCommunication.Instance.GetActualExtruderTemperature(0) + 0.5))
				{
					arg = "↓";
				}
				else if ((int)(PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(0) + 0.5) > (int)(PrinterConnectionAndCommunication.Instance.GetActualExtruderTemperature(0) + 0.5))
				{
					arg = "↑";
				}
			}
			base.IndicatorValue = $" {PrinterConnectionAndCommunication.Instance.GetActualExtruderTemperature(0):0.#}°{arg}";
		}

		protected override void SetTargetTemperature()
		{
			if (double.TryParse(ActiveSliceSettings.Instance.GetValue("temperature"), out var result))
			{
				double num = (int)(result + 0.5);
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && PrinterConnectionAndCommunication.Instance.PrintingState == PrinterConnectionAndCommunication.DetailedPrintingState.HeatingExtruder && num != PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(0))
				{
					string message = string.Format(waitingForExtruderToHeatMessage, PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(0), sliceSettingsNote);
					StyledMessageBox.ShowMessageBox(null, message, "Waiting For Extruder To Heat".Localize());
				}
				else
				{
					PrinterConnectionAndCommunication.Instance.SetTargetExtruderTemperature(0, (int)(result + 0.5));
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
