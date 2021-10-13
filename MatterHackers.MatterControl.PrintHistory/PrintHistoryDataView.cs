using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrintHistory
{
	public class PrintHistoryDataView : ScrollableWidget
	{
		public bool ShowTimestamp;

		protected FlowLayoutWidget topToBottomItemList;

		private EventHandler unregisterEvents;

		private bool settingLocalBounds;

		public int Count => ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children()).Count;

		public override RectangleDouble LocalBounds
		{
			set
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				if (settingLocalBounds)
				{
					return;
				}
				Vector2 topLeftOffset = default(Vector2);
				if (((GuiWidget)this).get_Parent() != null)
				{
					topLeftOffset = ((ScrollableWidget)this).get_TopLeftOffset();
				}
				settingLocalBounds = true;
				if (topToBottomItemList != null)
				{
					BorderDouble val;
					if (((GuiWidget)((ScrollableWidget)this).get_VerticalScrollBar()).get_Visible())
					{
						FlowLayoutWidget obj = topToBottomItemList;
						double width = ((RectangleDouble)(ref value)).get_Width();
						val = ((GuiWidget)((ScrollableWidget)this).get_ScrollArea()).get_Padding();
						double num = width - ((BorderDouble)(ref val)).get_Width();
						val = ((GuiWidget)topToBottomItemList).get_Margin();
						((GuiWidget)obj).set_Width(Math.Max(0.0, num - ((BorderDouble)(ref val)).get_Width() - ((GuiWidget)((ScrollableWidget)this).get_VerticalScrollBar()).get_Width()));
					}
					else
					{
						FlowLayoutWidget obj2 = topToBottomItemList;
						double width2 = ((RectangleDouble)(ref value)).get_Width();
						val = ((GuiWidget)((ScrollableWidget)this).get_ScrollArea()).get_Padding();
						double num2 = width2 - ((BorderDouble)(ref val)).get_Width();
						val = ((GuiWidget)topToBottomItemList).get_Margin();
						((GuiWidget)obj2).set_Width(Math.Max(0.0, num2 - ((BorderDouble)(ref val)).get_Width()));
					}
				}
				((ScrollableWidget)this).set_LocalBounds(value);
				if (((GuiWidget)this).get_Parent() != null)
				{
					((ScrollableWidget)this).set_TopLeftOffset(topLeftOffset);
				}
				settingLocalBounds = false;
			}
		}

		public event EventHandler DoneLoading;

		private void SetDisplayAttributes()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_MinimumSize(new Vector2(0.0, 200.0));
			((GuiWidget)this).AnchorAll();
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((ScrollableWidget)this).set_AutoScroll(true);
			((GuiWidget)((ScrollableWidget)this).get_ScrollArea()).set_Padding(new BorderDouble(3.0));
		}

		public void LoadHistoryItems(int NumItemsToLoad = 0)
		{
			if (NumItemsToLoad == 0 || NumItemsToLoad < PrintHistoryData.RecordLimit)
			{
				NumItemsToLoad = PrintHistoryData.RecordLimit;
			}
			RemoveListItems();
			IEnumerable<PrintTask> historyItems = PrintHistoryData.Instance.GetHistoryItems(NumItemsToLoad);
			if (historyItems != null)
			{
				foreach (PrintTask item in historyItems)
				{
					((GuiWidget)this).AddChild((GuiWidget)(object)new PrintHistoryListItem(item, ShowTimestamp), -1);
				}
			}
			OnDoneLoading(null);
		}

		private void OnDoneLoading(EventArgs e)
		{
			if (this.DoneLoading != null)
			{
				this.DoneLoading(this, e);
			}
		}

		public PrintHistoryDataView()
			: this(false)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			PrintHistoryData.Instance.HistoryCleared.RegisterEvent((EventHandler)HistoryDeleted, ref unregisterEvents);
			ShowTimestamp = UserSettings.Instance.get("PrintHistoryFilterShowTimestamp") == "true";
			SetDisplayAttributes();
			ScrollingArea scrollArea = ((ScrollableWidget)this).get_ScrollArea();
			((GuiWidget)scrollArea).set_HAnchor((HAnchor)(((GuiWidget)scrollArea).get_HAnchor() | 5));
			((ScrollableWidget)this).set_AutoScroll(true);
			topToBottomItemList = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)topToBottomItemList).set_HAnchor((HAnchor)13);
			((ScrollableWidget)this).AddChild((GuiWidget)(object)topToBottomItemList, -1);
			AddHandlers();
			LoadHistoryItems();
		}

		public void HistoryDeleted(object sender, EventArgs e)
		{
			ReloadData(this, null);
		}

		private void AddHandlers()
		{
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)ReloadData, ref unregisterEvents);
		}

		private void ReloadData(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				if (!((GuiWidget)this).get_HasBeenClosed())
				{
					LoadHistoryItems(Count);
				}
			});
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		public override void AddChild(GuiWidget child, int indexInChildrenList = -1)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Name("list item holder");
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)val).set_HAnchor((HAnchor)13);
			((GuiWidget)val).AddChild(child, -1);
			((GuiWidget)val).set_VAnchor((VAnchor)8);
			((GuiWidget)topToBottomItemList).AddChild((GuiWidget)(object)val, indexInChildrenList);
		}

		public override void RemoveChild(int index)
		{
			((GuiWidget)topToBottomItemList).RemoveChild(index);
		}

		public override void RemoveChild(GuiWidget childToRemove)
		{
			for (int num = ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children()).Count - 1; num >= 0; num--)
			{
				GuiWidget val = ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children())[num];
				if (val == childToRemove || ((Collection<GuiWidget>)(object)val.get_Children())[0] == childToRemove)
				{
					((GuiWidget)topToBottomItemList).RemoveChild(val);
				}
			}
		}

		public void RemoveListItems()
		{
			((GuiWidget)topToBottomItemList).RemoveAllChildren();
		}
	}
}
