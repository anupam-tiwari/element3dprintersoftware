using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.Replay
{
	public class ModalTermWithTab : SystemWindow
	{
		private static readonly int initialWidth = 600;

		private static readonly int initialHeight = 400;

		public ModalTermWithTab(GuiWidget tabWidget, string tabName)
			: this((double)initialWidth, (double)initialHeight)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((SystemWindow)this).set_IsModal(true);
			((GuiWidget)this).set_MinimumSize(new Vector2((double)initialWidth, (double)initialHeight));
			CreateAndAddChildren(tabWidget, tabName);
		}

		public ModalTermWithTab(GuiWidget tabWidget, string tabName, Vector2 initialSize)
			: this(initialSize.x, initialSize.y)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((SystemWindow)this).set_IsModal(true);
			((GuiWidget)this).set_MinimumSize(new Vector2(initialSize.x, initialSize.y));
			CreateAndAddChildren(tabWidget, tabName);
		}

		private void CreateAndAddChildren(GuiWidget tabWidget, string tabName)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Expected O, but got Unknown
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Expected O, but got Unknown
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Expected O, but got Unknown
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Expected O, but got Unknown
			int num = 12;
			TabControl val = new TabControl((Orientation)0);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)val.get_TabBar()).set_Padding(new BorderDouble(0.0, 6.0, 0.0, 6.0));
			((GuiWidget)val.get_TabBar()).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			tabWidget.AnchorAll();
			Tab val2 = (Tab)new SimpleTextTabWidget(new TabPage(tabWidget, tabName), "Widget Tab", (double)num, ActiveTheme.get_Instance().get_TabLabelSelected(), default(RGBA_Bytes), ActiveTheme.get_Instance().get_TabLabelUnselected(), default(RGBA_Bytes));
			val.AddTab(val2);
			TerminalWidget terminalWidget = new TerminalWidget(showInWindow: false);
			((GuiWidget)terminalWidget).AnchorAll();
			Tab val3 = (Tab)new SimpleTextTabWidget(new TabPage((GuiWidget)(object)terminalWidget, "Terminal".Localize()), "Term Tab", (double)num, ActiveTheme.get_Instance().get_TabLabelSelected(), default(RGBA_Bytes), ActiveTheme.get_Instance().get_TabLabelUnselected(), default(RGBA_Bytes));
			val.AddTab(val3);
		}
	}
}
