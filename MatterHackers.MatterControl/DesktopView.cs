using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class DesktopView : ApplicationView
	{
		private WidescreenPanel widescreenPanel;

		public DesktopView()
		{
			CreateAndAddChildren();
			((GuiWidget)this).AnchorAll();
		}

		public override void CreateAndAddChildren()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Expected O, but got Unknown
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			if (!UserSettings.Instance.IsTouchScreen)
			{
				ApplicationMenuRow applicationMenuRow = new ApplicationMenuRow();
				((GuiWidget)val).AddChild((GuiWidget)(object)applicationMenuRow, -1);
			}
			GuiWidget val2 = new GuiWidget();
			val2.set_BackgroundColor(new RGBA_Bytes(200, 200, 200));
			val2.set_Height(2.0);
			val2.set_HAnchor((HAnchor)5);
			val2.set_Margin(new BorderDouble(3.0, 6.0, 3.0, 3.0));
			GuiWidget val3 = val2;
			((GuiWidget)val).AddChild(val3, -1);
			widescreenPanel = new WidescreenPanel();
			((GuiWidget)val).AddChild((GuiWidget)(object)widescreenPanel, -1);
			PerformanceTimer val4 = new PerformanceTimer("ReloadAll", "AddChild");
			try
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
		}
	}
}
