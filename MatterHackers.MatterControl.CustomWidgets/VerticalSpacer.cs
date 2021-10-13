using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class VerticalSpacer : GuiWidget
	{
		public VerticalSpacer()
			: this()
		{
			((GuiWidget)this).set_VAnchor((VAnchor)5);
		}
	}
}
