using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class ViewControlsBase : FlowLayoutWidget
	{
		protected int buttonHeight;

		public ViewControlsBase()
			: this((FlowDirection)0)
		{
			if (UserSettings.Instance.IsTouchScreen)
			{
				buttonHeight = 40;
			}
			else
			{
				buttonHeight = 20;
			}
		}
	}
}
