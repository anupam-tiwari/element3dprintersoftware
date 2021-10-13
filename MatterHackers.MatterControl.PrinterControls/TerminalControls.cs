using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class TerminalControls : ControlWidgetBase
	{
		public TerminalControls()
		{
			((GuiWidget)this).AddChild((GuiWidget)(object)new TerminalWidget(showInWindow: false), -1);
		}
	}
}
