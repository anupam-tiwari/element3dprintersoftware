using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class PrinterConfigurationScrollWidget : ScrollableWidget
	{
		public PrinterConfigurationScrollWidget()
			: this(true)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			ScrollingArea scrollArea = ((ScrollableWidget)this).get_ScrollArea();
			((GuiWidget)scrollArea).set_HAnchor((HAnchor)(((GuiWidget)scrollArea).get_HAnchor() | 5));
			((GuiWidget)this).AnchorAll();
			((GuiWidget)this).AddChild((GuiWidget)(object)new PrinterConfigurationWidget(), -1);
		}
	}
}
