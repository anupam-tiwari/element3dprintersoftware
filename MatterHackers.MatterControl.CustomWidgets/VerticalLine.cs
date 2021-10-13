using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class VerticalLine : GuiWidget
	{
		public VerticalLine()
			: this(1.0, 1.0, (SizeLimitsToSet)1)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryTextColor());
			((GuiWidget)this).set_VAnchor((VAnchor)5);
		}
	}
}
