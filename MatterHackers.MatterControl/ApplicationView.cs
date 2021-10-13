using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public abstract class ApplicationView : GuiWidget
	{
		public abstract void CreateAndAddChildren();

		protected ApplicationView()
			: this()
		{
		}
	}
}
