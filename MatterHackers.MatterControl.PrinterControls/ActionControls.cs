using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterControls
{
	public class ActionControls : ControlWidgetBase
	{
		public ActionControls()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (!Enumerable.Any<GCodeMacro>(ActiveSliceSettings.Instance.ActionMacros()))
			{
				((GuiWidget)this).set_Margin(default(BorderDouble));
			}
			else
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)new ActionControlsWidget(), -1);
			}
		}
	}
}
