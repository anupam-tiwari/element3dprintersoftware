using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class MacroControls : ControlWidgetBase
	{
		public MacroControls()
		{
			((GuiWidget)this).AddChild((GuiWidget)(object)new MacroControlsWidget(), -1);
		}
	}
}
