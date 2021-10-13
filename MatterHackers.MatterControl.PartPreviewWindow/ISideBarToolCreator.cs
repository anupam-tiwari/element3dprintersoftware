using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public interface ISideBarToolCreator
	{
		GuiWidget CreateSideBarTool(View3DWidget widget);
	}
}
