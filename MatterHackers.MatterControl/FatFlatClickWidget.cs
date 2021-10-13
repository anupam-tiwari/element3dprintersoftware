using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class FatFlatClickWidget : ClickWidget
	{
		private GuiWidget overlay;

		public FatFlatClickWidget(TextWidget textWidget)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			overlay = new GuiWidget();
			overlay.AnchorAll();
			overlay.set_BackgroundColor(ActiveTheme.get_Instance().get_TransparentDarkOverlay());
			overlay.set_Visible(false);
			GetChildClicks = true;
			((GuiWidget)this).AddChild((GuiWidget)(object)textWidget, -1);
			((GuiWidget)this).AddChild(overlay, -1);
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			if (((GuiWidget)this).PositionWithinLocalBounds(mouseEvent.get_X(), mouseEvent.get_Y()))
			{
				overlay.set_Visible(true);
			}
			((GuiWidget)this).OnMouseDown(mouseEvent);
		}

		public override void OnMouseLeave(MouseEventArgs mouseEvent)
		{
			((GuiWidget)this).OnMouseLeave(mouseEvent);
		}

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			base.OnMouseUp(mouseEvent);
			overlay.set_Visible(false);
		}
	}
}
