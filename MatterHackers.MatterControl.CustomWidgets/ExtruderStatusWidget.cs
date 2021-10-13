using System;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class ExtruderStatusWidget : TemperatureStatusWidget
	{
		private int extruderIndex;

		public ExtruderStatusWidget(int extruderIndex)
			: base(string.Format("{0} {1}", "Extruder".Localize(), extruderIndex + 1))
		{
			this.extruderIndex = extruderIndex;
			PrinterConnectionAndCommunication.Instance.ExtruderTemperatureRead.RegisterEvent((EventHandler)delegate
			{
				UpdateTemperatures();
			}, ref unregisterEvents);
		}

		public override void UpdateTemperatures()
		{
			double targetExtruderTemperature = PrinterConnectionAndCommunication.Instance.GetTargetExtruderTemperature(extruderIndex);
			double num = Math.Max(0.0, PrinterConnectionAndCommunication.Instance.GetActualExtruderTemperature(extruderIndex));
			progressBar.set_RatioComplete((targetExtruderTemperature != 0.0) ? (num / targetExtruderTemperature) : 1.0);
			((GuiWidget)actualTemp).set_Text($"{num:0}".PadLeft(3, '\u2007') + "°");
			((GuiWidget)targetTemp).set_Text($"{targetExtruderTemperature:0}".PadLeft(3, '\u2007') + "°");
		}
	}
}
