using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrintQueue
{
	public class QueueDataView : ScrollableWidget
	{
		public delegate void SelectedValueChangedEventHandler(object sender, EventArgs e);

		public delegate void HoverValueChangedEventHandler(object sender, EventArgs e);

		private EventHandler unregisterEvents;

		private bool editMode;

		protected FlowLayoutWidget topToBottomItemList;

		private RGBA_Bytes hoverColor = new RGBA_Bytes(204, 204, 204, 255);

		private RGBA_Bytes selectedColor = new RGBA_Bytes(180, 180, 180, 255);

		private RGBA_Bytes baseColor = new RGBA_Bytes(255, 255, 255);

		private bool settingLocalBounds;

		public bool EditMode
		{
			get
			{
				return editMode;
			}
			set
			{
				if (editMode != value)
				{
					editMode = value;
					if (!editMode)
					{
						QueueData.Instance.MakeSingleSelection();
					}
					SelectedIndexChanged(null, null);
				}
			}
		}

		public int Count => ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children()).Count;

		public override RectangleDouble LocalBounds
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return ((GuiWidget)this).get_LocalBounds();
			}
			set
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				if (!settingLocalBounds && value != ((GuiWidget)this).get_LocalBounds())
				{
					Vector2 topLeftOffset = default(Vector2);
					if (((GuiWidget)this).get_Parent() != null)
					{
						topLeftOffset = ((ScrollableWidget)this).get_TopLeftOffset();
					}
					settingLocalBounds = true;
					((ScrollableWidget)this).set_LocalBounds(value);
					if (((GuiWidget)this).get_Parent() != null)
					{
						((ScrollableWidget)this).set_TopLeftOffset(topLeftOffset);
					}
					settingLocalBounds = false;
				}
			}
		}

		public event HoverValueChangedEventHandler HoverValueChanged;

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

		private void AddWatermark()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			string text = Path.Combine("OEMSettings", "watermark.png");
			if (StaticData.get_Instance().FileExists(text))
			{
				ImageBuffer val = new ImageBuffer();
				StaticData.get_Instance().LoadImage(text, val);
				GuiWidget val2 = (GuiWidget)new ImageWidget(val);
				val2.set_VAnchor((VAnchor)2);
				val2.set_HAnchor((HAnchor)2);
				((ScrollableWidget)this).AddChildToBackground(val2, 0);
			}
		}

		public QueueRowItem GetQueueRowItem(int index)
		{
			if (index >= 0 && index < ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children()).Count)
			{
				return (QueueRowItem)(object)((Collection<GuiWidget>)(object)((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children())[index].get_Children())[0];
			}
			return null;
		}

		public override void SendToChildren(object objectToRout)
		{
			((GuiWidget)this).SendToChildren(objectToRout);
		}

		public QueueDataView()
			: this(false)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			((GuiWidget)this).set_Name("PrintQueueControl");
			SetDisplayAttributes();
			AddWatermark();
			((GuiWidget)((ScrollableWidget)this).get_ScrollArea()).set_HAnchor((HAnchor)5);
			((ScrollableWidget)this).set_AutoScroll(true);
			topToBottomItemList = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)topToBottomItemList).set_Name("PrintQueueControl TopToBottom");
			((GuiWidget)topToBottomItemList).set_HAnchor((HAnchor)5);
			((ScrollableWidget)this).AddChild((GuiWidget)(object)topToBottomItemList, -1);
			for (int i = 0; i < QueueData.Instance.ItemCount; i++)
			{
				QueueRowItem queueRowItem = new QueueRowItem(QueueData.Instance.GetPrintItemWrapper(i), this);
				((GuiWidget)this).AddChild((GuiWidget)(object)queueRowItem, -1);
			}
			QueueData.Instance.SelectedIndexChanged.RegisterEvent((EventHandler)SelectedIndexChanged, ref unregisterEvents);
			QueueData.Instance.ItemAdded.RegisterEvent((EventHandler)ItemAddedToQueue, ref unregisterEvents);
			QueueData.Instance.ItemRemoved.RegisterEvent((EventHandler)ItemRemovedFromQueue, ref unregisterEvents);
			QueueData.Instance.OrderChanged.RegisterEvent((EventHandler)QueueOrderChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.ActivePrintItemChanged.RegisterEvent((EventHandler)PrintItemChange, ref unregisterEvents);
			SelectedIndexChanged(null, null);
		}

		private void PrintItemChange(object sender, EventArgs e)
		{
			QueueData.Instance.SelectedPrintItem = PrinterConnectionAndCommunication.Instance.ActivePrintItem;
		}

		private void SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!editMode)
			{
				QueueData.Instance.MakeSingleSelection();
			}
			for (int i = 0; i < ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children()).Count; i++)
			{
				QueueRowItem queueRowItem = (QueueRowItem)(object)((Collection<GuiWidget>)(object)((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children())[i].get_Children())[0];
				if (Enumerable.Contains<int>(QueueData.Instance.SelectedIndexes, i))
				{
					queueRowItem.isSelectedItem = true;
					queueRowItem.selectionCheckBox.set_Checked(true);
				}
				else
				{
					queueRowItem.isSelectedItem = false;
					queueRowItem.selectionCheckBox.set_Checked(false);
				}
			}
			if (editMode)
			{
				return;
			}
			for (int j = 0; j < ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children()).Count; j++)
			{
				QueueRowItem queueRowItem2 = (QueueRowItem)(object)((Collection<GuiWidget>)(object)((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children())[j].get_Children())[0];
				if (j == QueueData.Instance.SelectedIndex)
				{
					if (!PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && !PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
					{
						queueRowItem2.isActivePrint = true;
						PrinterConnectionAndCommunication.Instance.ActivePrintItem = queueRowItem2.PrintItemWrapper;
					}
					else if (queueRowItem2.PrintItemWrapper == PrinterConnectionAndCommunication.Instance.ActivePrintItem)
					{
						queueRowItem2.isActivePrint = true;
					}
					continue;
				}
				queueRowItem2.selectionCheckBox.set_Checked(false);
				if (queueRowItem2.isSelectedItem)
				{
					queueRowItem2.isSelectedItem = false;
				}
				if (!PrinterConnectionAndCommunication.Instance.PrinterIsPrinting && !PrinterConnectionAndCommunication.Instance.PrinterIsPaused && queueRowItem2.isActivePrint)
				{
					queueRowItem2.isActivePrint = false;
				}
			}
			if (QueueData.Instance.ItemCount == 0)
			{
				PrinterConnectionAndCommunication.Instance.ActivePrintItem = null;
			}
		}

		private void ItemAddedToQueue(object sender, EventArgs e)
		{
			IndexArgs indexArgs = e as IndexArgs;
			QueueRowItem queueRowItem = new QueueRowItem(QueueData.Instance.GetPrintItemWrapper(indexArgs.Index), this);
			((GuiWidget)this).AddChild((GuiWidget)(object)queueRowItem, indexArgs.Index);
		}

		private void ItemRemovedFromQueue(object sender, EventArgs e)
		{
			IndexArgs indexArgs = e as IndexArgs;
			((GuiWidget)topToBottomItemList).RemoveChild(indexArgs.Index);
		}

		private void QueueOrderChanged(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		public override void AddChild(GuiWidget childToAdd, int indexInChildrenList = -1)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_Name("PrintQueueControl itemHolder");
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).AddChild(childToAdd, -1);
			((GuiWidget)val).set_VAnchor((VAnchor)8);
			((GuiWidget)topToBottomItemList).AddChild((GuiWidget)(object)val, indexInChildrenList);
			AddItemHandlers((GuiWidget)(object)val);
		}

		private void AddItemHandlers(GuiWidget itemHolder)
		{
			itemHolder.add_MouseDownInBounds((EventHandler<MouseEventArgs>)itemHolder_MouseDownInBounds);
			itemHolder.add_ParentChanged((EventHandler)itemHolder_ParentChanged);
		}

		private void itemHolder_ParentChanged(object sender, EventArgs e)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			FlowLayoutWidget val = (FlowLayoutWidget)sender;
			((GuiWidget)val).remove_MouseDownInBounds((EventHandler<MouseEventArgs>)itemHolder_MouseDownInBounds);
			((GuiWidget)val).remove_ParentChanged((EventHandler)itemHolder_ParentChanged);
		}

		private void itemHolder_MouseDownInBounds(object sender, MouseEventArgs mouseEvent)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			if (PrinterConnectionAndCommunication.Instance.PrinterIsPrinting || PrinterConnectionAndCommunication.Instance.PrinterIsPaused || PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.PreparingToPrint || (!EditMode && mouseEvent.get_X() < 56.0))
			{
				return;
			}
			GuiWidget val = (GuiWidget)sender;
			for (int i = 0; i < ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children()).Count; i++)
			{
				if (((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children())[i] == val)
				{
					if (EditMode)
					{
						QueueData.Instance.ToggleSelect(i);
					}
					else
					{
						QueueData.Instance.SelectedIndex = i;
					}
				}
			}
		}

		private void itemToAdd_MouseLeaveBounds(object sender, EventArgs e)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			GuiWidget val = (GuiWidget)sender;
			if (QueueData.Instance.SelectedIndex >= 0 && val != ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children())[QueueData.Instance.SelectedIndex])
			{
				val.set_BackgroundColor(default(RGBA_Bytes));
				val.Invalidate();
				((GuiWidget)this).Invalidate();
			}
		}

		private static bool WidgetOrChildIsFirstUnderMouse(GuiWidget startWidget)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)startWidget.get_UnderMouseState() == 2)
			{
				return true;
			}
			foreach (GuiWidget item in (Collection<GuiWidget>)(object)startWidget.get_Children())
			{
				if (item != null && WidgetOrChildIsFirstUnderMouse(item))
				{
					return true;
				}
			}
			return false;
		}

		public void OnHoverIndexChanged()
		{
			((GuiWidget)this).Invalidate();
			if (this.HoverValueChanged != null)
			{
				this.HoverValueChanged(this, null);
			}
		}

		internal List<QueueRowItem> GetSelectedItems()
		{
			List<QueueRowItem> list = new List<QueueRowItem>();
			foreach (int selectedIndex in QueueData.Instance.SelectedIndexes)
			{
				QueueRowItem queueRowItem = GetQueueRowItem(selectedIndex);
				if (queueRowItem != null)
				{
					list.Add(queueRowItem);
				}
			}
			return list;
		}
	}
}
