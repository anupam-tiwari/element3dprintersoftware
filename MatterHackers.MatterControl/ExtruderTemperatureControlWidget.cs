using System;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class ExtruderTemperatureControlWidget : TemperatureControlBase
	{
		private EventHandler unregisterEvents;

		protected override string HelpText => "Override the current extruder temperature. While printing, the extruder temperature is normally determined by the 'Slice Settings'.";

		public ExtruderTemperatureControlWidget(int extruderIndex0Based)
			: base(extruderIndex0Based, "Extruder Temperature".Localize(), "Extruder Temperature Settings".Localize())
		{
			AddChildElements();
			AddHandlers();
		}

		public ExtruderTemperatureControlWidget(int extruderIndex0Based, int extruderNumber)
			: base(extruderIndex0Based, string.Format("{0} {1}", "Extruder Temperature".Localize(), extruderNumber + 1), "Extruder Temperature Settings".Localize())
		{
			AddChildElements();
			AddHandlers();
		}

		private void AddHandlers()
		{
			PrinterConnectionAndCommunication.Instance.ExtruderTemperatureRead.RegisterEvent((EventHandler)base.onTemperatureRead, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.ExtruderTemperatureSet.RegisterEvent((EventHandler)base.ExtruderTemperatureSet, ref unregisterEvents);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		protected override string GetTemperaturePresets()
		{
			string value = ",0,,0,,0,250";
			string key = $"Extruder{extruderIndex0Based + 1}PresetTemps";
			if (UserSettings.Instance.get(key) == null)
			{
				UserSettings.Instance.set(key, value);
			}
			return UserSettings.Instance.get(key);
		}

		protected override void SetTemperaturePresets(object seder, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (val != null && val.get_Data() != null)
			{
				UserSettings.Instance.set($"Extruder{extruderIndex0Based + 1}PresetTemps", val.get_Data());
				ApplicationController.Instance.ReloadAdvancedControlsPanel();
			}
		}

		protected override double GetPreheatTemperature()
		{
			return ActiveSliceSettings.Instance.Helpers.ExtruderTemperature(extruderIndex0Based);
		}

		protected override double GetTargetTemperature()
		{
			return PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(extruderIndex0Based);
		}

		protected override double GetActualTemperature()
		{
			return PrinterConnectionAndCommunication.Instance.GetActualExtruderTemperature(extruderIndex0Based);
		}

		protected override void SetTargetTemperature(double targetTemp)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				double num = (int)(targetTemp + 0.5);
				if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && PrinterConnectionAndCommunication.Instance.PrintingState == PrinterConnectionAndCommunication.DetailedPrintingState.HeatingExtruder && num != PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(extruderIndex0Based))
				{
					string arg = "Note: Slice Settings are applied before the print actually starts. Changes while printing will not effect the active print.";
					string message = $"The extruder is currently heating and its target temperature cannot be changed until it reaches {PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(extruderIndex0Based)}°C.\n\nYou can set the starting extruder temperature in 'Slice Settings' -> 'Filament'.\n\n{arg}";
					StyledMessageBox.ShowMessageBox(null, message, "Waiting For Extruder To Heat");
				}
				else
				{
					PrinterConnectionAndCommunication.Instance.SetTargetExtruderTemperature(extruderIndex0Based, (int)(targetTemp + 0.5));
					string displayString = $"{PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(extruderIndex0Based):0.0}°C";
					targetTemperatureDisplay.SetDisplayString(displayString);
				}
			});
		}
	}
}
