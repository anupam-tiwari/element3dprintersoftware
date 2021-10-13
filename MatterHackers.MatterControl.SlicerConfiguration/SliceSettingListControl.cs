using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	internal class SliceSettingListControl : ScrollableWidget
	{
		private FlowLayoutWidget topToBottomItemList;

		public SliceSettingListControl()
			: this(false)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).AnchorAll();
			((ScrollableWidget)this).set_AutoScroll(true);
			ScrollingArea scrollArea = ((ScrollableWidget)this).get_ScrollArea();
			((GuiWidget)scrollArea).set_HAnchor((HAnchor)(((GuiWidget)scrollArea).get_HAnchor() | 5));
			topToBottomItemList = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)topToBottomItemList).set_HAnchor((HAnchor)13);
			((GuiWidget)topToBottomItemList).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 3.0));
			((ScrollableWidget)this).AddChild((GuiWidget)(object)topToBottomItemList, -1);
		}

		public override void AddChild(GuiWidget child, int indexInChildrenList = -1)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)val).set_HAnchor((HAnchor)13);
			((GuiWidget)val).AddChild(child, -1);
			((GuiWidget)val).set_VAnchor((VAnchor)8);
			((GuiWidget)topToBottomItemList).AddChild((GuiWidget)(object)val, indexInChildrenList);
		}
	}
}
