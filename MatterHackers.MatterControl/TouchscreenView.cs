using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class TouchscreenView : ApplicationView
	{
		private FlowLayoutWidget TopContainer;

		private TouchscreenTabView touchscreenTabView;

		private QueueDataView queueDataView;

		private GuiWidget menuSeparator;

		private PrintProgressBar progressBar;

		private bool topIsHidden;

		public TouchscreenView()
		{
			CreateAndAddChildren();
			((GuiWidget)this).AnchorAll();
		}

		public void ToggleTopContainer()
		{
			topIsHidden = !topIsHidden;
			progressBar.WidgetIsExtended = !progressBar.WidgetIsExtended;
			menuSeparator.set_Visible(((GuiWidget)TopContainer).get_Visible());
			((GuiWidget)TopContainer).set_Visible(!((GuiWidget)TopContainer).get_Visible());
		}

		public override void CreateAndAddChildren()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			topIsHidden = false;
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).AnchorAll();
			TopContainer = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)TopContainer).set_HAnchor((HAnchor)5);
			if (!UserSettings.Instance.IsTouchScreen)
			{
				ApplicationMenuRow applicationMenuRow = new ApplicationMenuRow();
				((GuiWidget)TopContainer).AddChild((GuiWidget)(object)applicationMenuRow, -1);
			}
			menuSeparator = new GuiWidget();
			menuSeparator.set_Height(12.0);
			menuSeparator.set_HAnchor((HAnchor)5);
			menuSeparator.set_MinimumSize(new Vector2(0.0, 12.0));
			menuSeparator.set_Visible(false);
			queueDataView = new QueueDataView();
			((GuiWidget)TopContainer).AddChild((GuiWidget)(object)new ActionBarPlus(queueDataView), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)TopContainer, -1);
			progressBar = new PrintProgressBar();
			((GuiWidget)val).AddChild((GuiWidget)(object)progressBar, -1);
			((GuiWidget)val).AddChild(menuSeparator, -1);
			touchscreenTabView = new TouchscreenTabView(queueDataView);
			((GuiWidget)val).AddChild((GuiWidget)(object)touchscreenTabView, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}
	}
}
