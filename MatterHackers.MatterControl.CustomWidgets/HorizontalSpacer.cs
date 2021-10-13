using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class HorizontalSpacer : GuiWidget
	{
		public HorizontalSpacer()
			: this()
		{
			((GuiWidget)this).set_HAnchor((HAnchor)5);
		}
	}
}
