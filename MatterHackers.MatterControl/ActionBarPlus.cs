using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.ActionBar;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl
{
	public class ActionBarPlus : FlowLayoutWidget
	{
		public ActionBarPlus(QueueDataView queueDataView)
			: this((FlowDirection)3)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			if (UserSettings.Instance.IsTouchScreen)
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)new TouchScreenPrintStatusRow(queueDataView), -1);
			}
			else
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)new PrinterConnectAndSelectControl(), -1);
				((GuiWidget)this).AddChild((GuiWidget)(object)new PrintStatusRow(queueDataView), -1);
			}
			((GuiWidget)this).set_Padding(new BorderDouble(0.0, 6.0, 0.0, 0.0));
		}
	}
}
