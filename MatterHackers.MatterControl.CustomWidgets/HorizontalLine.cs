using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.CustomWidgets
{
	public class HorizontalLine : GuiWidget
	{
		public HorizontalLine()
			: this(1.0, 1.0, (SizeLimitsToSet)1)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryTextColor());
			((GuiWidget)this).set_HAnchor((HAnchor)5);
		}

		public HorizontalLine(RGBA_Bytes backgroundColor)
			: this(1.0, 1.0, (SizeLimitsToSet)1)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_BackgroundColor(backgroundColor);
			((GuiWidget)this).set_HAnchor((HAnchor)5);
		}
	}
}
