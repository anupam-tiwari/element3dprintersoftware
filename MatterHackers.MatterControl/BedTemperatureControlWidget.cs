using System;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class BedTemperatureControlWidget : TemperatureControlBase
	{
		private EventHandler unregisterEvents;

		protected override string HelpText => "Override the current bed temperature. While printing, the bed temperature is normally determined by the 'Slice Settings'.";

		public BedTemperatureControlWidget()
			: base(0, "Bed Temperature".Localize(), "Bed Temperature Settings".Localize())
		{
			AddChildElements();
			AddHandlers();
			((GuiWidget)this).set_Name("Bed Temperature Controls Widget");
		}

		private void AddHandlers()
		{
			PrinterConnectionAndCommunication.Instance.BedTemperatureRead.RegisterEvent((EventHandler)base.onTemperatureRead, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.BedTemperatureSet.RegisterEvent((EventHandler)base.BedTemperatureSet, ref unregisterEvents);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		private void onOffButtonClicked(object sender, EventArgs e)
		{
			SetTargetTemperature(0.0);
		}

		protected override string GetTemperaturePresets()
		{
			string value = ",0,,0,,0,150";
			if (UserSettings.Instance.get("BedPresetTemps") == null)
			{
				UserSettings.Instance.set("BedPresetTemps", value);
			}
			return UserSettings.Instance.get("BedPresetTemps");
		}

		protected override void SetTemperaturePresets(object seder, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (val != null && val.get_Data() != null)
			{
				UserSettings.Instance.set("BedPresetTemps", val.get_Data());
				ApplicationController.Instance.ReloadAdvancedControlsPanel();
			}
		}

		protected override double GetPreheatTemperature()
		{
			return ActiveSliceSettings.Instance.GetValue<double>("bed_temperature");
		}

		protected override double GetActualTemperature()
		{
			return PrinterConnectionAndCommunication.Instance.ActualBedTemperature;
		}

		protected override double GetTargetTemperature()
		{
			return PrinterConnectionAndCommunication.Instance.TargetBedTemperature;
		}

		protected override void SetTargetTemperature(double targetTemp)
		{
			double num = (int)(targetTemp + 0.5);
			if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && PrinterConnectionAndCommunication.Instance.PrintingState == PrinterConnectionAndCommunication.DetailedPrintingState.HeatingBed && num != PrinterConnectionAndCommunication.Instance.TargetBedTemperature)
			{
				string arg = "Note: Slice Settings are applied before the print actually starts. Changes while printing will not effect the active print.";
				string message = $"The bed is currently heating and its target temperature cannot be changed until it reaches {PrinterConnectionAndCommunication.Instance.TargetBedTemperature}°C.\n\nYou can set the starting bed temperature in 'Slice Settings' -> 'Filament'.\n\n{arg}";
				StyledMessageBox.ShowMessageBox(null, message, "Waiting For Bed To Heat");
			}
			else
			{
				PrinterConnectionAndCommunication.Instance.TargetBedTemperature = num;
				string displayString = $"{PrinterConnectionAndCommunication.Instance.TargetBedTemperature:0.0}°C";
				targetTemperatureDisplay.SetDisplayString(displayString);
			}
		}
	}
}
