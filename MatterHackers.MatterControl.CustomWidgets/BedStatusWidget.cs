using System;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class BedStatusWidget : TemperatureStatusWidget
	{
		public BedStatusWidget(bool smallScreen)
			: base(smallScreen ? "Bed".Localize() : "Bed Temperature".Localize())
		{
			PrinterConnectionAndCommunication.Instance.BedTemperatureRead.RegisterEvent((EventHandler)delegate
			{
				UpdateTemperatures();
			}, ref unregisterEvents);
		}

		public override void UpdateTemperatures()
		{
			double targetBedTemperature = PrinterConnectionAndCommunication.Instance.TargetBedTemperature;
			double num = Math.Max(0.0, PrinterConnectionAndCommunication.Instance.ActualBedTemperature);
			progressBar.set_RatioComplete((targetBedTemperature != 0.0) ? (num / targetBedTemperature) : 1.0);
			((GuiWidget)actualTemp).set_Text($"{num:0}".PadLeft(3, '\u2007') + "°");
			((GuiWidget)targetTemp).set_Text($"{targetBedTemperature:0}".PadLeft(3, '\u2007') + "°");
		}
	}
}
