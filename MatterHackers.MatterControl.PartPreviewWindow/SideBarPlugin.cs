using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class SideBarPlugin : ISideBarToolCreator
	{
		public virtual GuiWidget CreateSideBarTool(View3DWidget widget)
		{
			return null;
		}
	}
}
