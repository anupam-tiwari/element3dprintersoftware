using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintLibrary.Provider;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrintLibrary
{
	public class LibraryDataView : ScrollableWidget
	{
		public delegate void HoverValueChangedEventHandler(object sender, EventArgs e);

		internal class LastScrollPosition
		{
			internal Vector2 topLeftScrollPosition;
		}

		public SelectedListItems<LibraryRowItem> SelectedItems = new SelectedListItems<LibraryRowItem>();

		protected FlowLayoutWidget topToBottomItemList;

		private static LibraryDataView libraryDataViewInstance;

		private RGBA_Bytes baseColor = new RGBA_Bytes(255, 255, 255);

		private LibraryProvider currentLibraryProvider;

		private bool editMode;

		private RGBA_Bytes hoverColor = new RGBA_Bytes(204, 204, 204, 255);

		private RGBA_Bytes selectedColor = new RGBA_Bytes(180, 180, 180, 255);

		private bool settingLocalBounds;

		private EventHandler unregisterEvents;

		public LibraryProvider CurrentLibraryProvider
		{
			get
			{
				return currentLibraryProvider;
			}
			set
			{
				if (currentLibraryProvider != null)
				{
					currentLibraryProvider.DataReloaded -= libraryDataViewInstance.LibraryDataReloaded;
				}
				value.DataReloaded += libraryDataViewInstance.LibraryDataReloaded;
				bool flag = value.ParentLibraryProvider == currentLibraryProvider;
				while (!flag && currentLibraryProvider != value && currentLibraryProvider.ParentLibraryProvider != null)
				{
					LibraryProvider parentLibraryProvider = currentLibraryProvider.ParentLibraryProvider;
					currentLibraryProvider.Dispose();
					currentLibraryProvider = parentLibraryProvider;
				}
				LibraryProvider arg = currentLibraryProvider;
				currentLibraryProvider = value;
				if (this.ChangedCurrentLibraryProvider != null)
				{
					this.ChangedCurrentLibraryProvider(arg, value);
				}
				ClearSelectedItems();
				UiThread.RunOnIdle((Action)RebuildView);
			}
		}

		public bool EditMode
		{
			get
			{
				return editMode;
			}
			set
			{
				if (editMode == value)
				{
					return;
				}
				editMode = value;
				if (!editMode)
				{
					while (SelectedItems.Count > 1)
					{
						SelectedItems.RemoveAt(SelectedItems.Count - 1);
					}
				}
			}
		}

		public override RectangleDouble LocalBounds
		{
			set
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				if (!settingLocalBounds)
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

		private int Count => ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children()).Count;

		public event Action<LibraryProvider, LibraryProvider> ChangedCurrentLibraryProvider;

		public event HoverValueChangedEventHandler HoverValueChanged;

		public LibraryDataView()
			: this(false)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Expected O, but got Unknown
			ApplicationController.Instance.CurrentLibraryDataView = this;
			currentLibraryProvider = new LibraryProviderSelector(SetCurrentLibraryProvider, includeQueueLibraryProvider: false);
			currentLibraryProvider.DataReloaded += LibraryDataReloaded;
			if (libraryDataViewInstance != null)
			{
				throw new Exception("There should only ever be one of these, Lars.");
			}
			libraryDataViewInstance = this;
			((GuiWidget)this).AnchorAll();
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryBackgroundColor());
			((GuiWidget)((ScrollableWidget)this).get_ScrollArea()).set_Padding(new BorderDouble(3.0));
			((GuiWidget)((ScrollableWidget)this).get_ScrollArea()).set_HAnchor((HAnchor)5);
			((ScrollableWidget)this).set_AutoScroll(true);
			topToBottomItemList = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)topToBottomItemList).set_HAnchor((HAnchor)5);
			((GuiWidget)this).AddChild((GuiWidget)(object)topToBottomItemList, -1);
			AddAllItems();
		}

		public int GetItemIndex(LibraryRowItem rowItem)
		{
			for (int i = 0; i < ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children()).Count; i++)
			{
				if (((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children())[i] != null && ((Collection<GuiWidget>)(object)((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children())[i].get_Children()).Count > 0 && ((Collection<GuiWidget>)(object)((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children())[i].get_Children())[0] == rowItem)
				{
					return i;
				}
			}
			return -1;
		}

		public void AddListItemToTopToBottom(GuiWidget child, int indexInChildrenList = -1)
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

		public void ClearSelectedItems()
		{
			foreach (LibraryRowItem selectedItem in SelectedItems)
			{
				selectedItem.selectionCheckBox.set_Checked(false);
			}
			SelectedItems.Clear();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (currentLibraryProvider != null)
			{
				currentLibraryProvider.DataReloaded -= LibraryDataReloaded;
				currentLibraryProvider.Dispose();
				currentLibraryProvider = null;
			}
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
			libraryDataViewInstance = null;
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			((GuiWidget)this).OnDraw(graphics2D);
		}

		public void OnHoverIndexChanged()
		{
			((GuiWidget)this).Invalidate();
			if (this.HoverValueChanged != null)
			{
				this.HoverValueChanged(this, null);
			}
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			((ScrollableWidget)this).OnMouseDown(mouseEvent);
		}

		public override void OnMouseMove(MouseEventArgs mouseEvent)
		{
			((ScrollableWidget)this).OnMouseMove(mouseEvent);
		}

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			((ScrollableWidget)this).OnMouseUp(mouseEvent);
		}

		public void RebuildView()
		{
			AddAllItems();
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

		public void RemoveSelectedItems()
		{
			using List<LibraryRowItem>.Enumerator enumerator = SelectedItems.GetEnumerator();
			if (enumerator.MoveNext())
			{
				_ = enumerator.Current;
				throw new NotImplementedException();
			}
		}

		public void SetCurrentLibraryProvider(LibraryProvider libraryProvider)
		{
			CurrentLibraryProvider = libraryProvider;
		}

		protected GuiWidget GetThumbnailWidget(LibraryProvider parentProvider, PrintItemCollection printItemCollection, ImageBuffer imageBuffer)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Expected O, but got Unknown
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector((double)(int)(50.0 * GuiWidget.get_DeviceScale()), (double)(int)(50.0 * GuiWidget.get_DeviceScale()));
			if ((double)imageBuffer.get_Width() != val.x)
			{
				ImageBuffer val2 = new ImageBuffer((int)val.x, (int)val.y);
				val2.NewGraphics2D().Render((IImageByte)(object)imageBuffer, 0.0, 0.0, (double)val2.get_Width(), (double)val2.get_Height());
				imageBuffer = val2;
			}
			ImageWidget val3 = new ImageWidget(imageBuffer);
			((GuiWidget)val3).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			Button val4 = new Button(0.0, 0.0, (GuiWidget)(object)val3);
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate(object sender, MouseEventArgs e)
			{
				if (UserSettings.Instance.IsTouchScreen)
				{
					if (parentProvider == null)
					{
						CurrentLibraryProvider = CurrentLibraryProvider.GetProviderForCollection(printItemCollection);
					}
					else
					{
						CurrentLibraryProvider = parentProvider;
					}
				}
				else if (e != null && ((GuiWidget)this).IsDoubleClick(e))
				{
					if (parentProvider == null)
					{
						CurrentLibraryProvider = CurrentLibraryProvider.GetProviderForCollection(printItemCollection);
					}
					else
					{
						CurrentLibraryProvider = parentProvider;
					}
				}
			});
			return (GuiWidget)val4;
		}

		private void AddAllItems(object inData = null)
		{
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			SelectedItems.Clear();
			((GuiWidget)topToBottomItemList).RemoveAllChildren();
			LibraryProvider libraryProvider = CurrentLibraryProvider;
			if (libraryProvider != null)
			{
				if (libraryProvider.ProviderKey != LibraryProviderSelector.ProviderKeyName)
				{
					PrintItemCollection printItemCollection = new PrintItemCollection("..", libraryProvider.ProviderKey);
					LibraryRowItem libraryRowItem = new LibraryRowItemCollection(printItemCollection, libraryProvider, -1, this, libraryProvider.ParentLibraryProvider, GetThumbnailWidget(libraryProvider.ParentLibraryProvider, printItemCollection, LibraryProvider.UpFolderImage), "Back".Localize());
					libraryRowItem.IsViewHelperItem = true;
					AddListItemToTopToBottom((GuiWidget)(object)libraryRowItem);
				}
				for (int i = 0; i < libraryProvider.CollectionCount; i++)
				{
					PrintItemCollection collectionItem = libraryProvider.GetCollectionItem(i);
					LibraryRowItem child = new LibraryRowItemCollection(collectionItem, libraryProvider, i, this, null, GetThumbnailWidget(null, collectionItem, libraryProvider.GetCollectionFolderImage(i)), "Open".Localize());
					AddListItemToTopToBottom((GuiWidget)(object)child);
				}
				for (int j = 0; j < libraryProvider.ItemCount; j++)
				{
					GuiWidget itemThumbnail = libraryProvider.GetItemThumbnail(j);
					LibraryRowItem child2 = new LibraryRowItemPart(libraryProvider, j, this, itemThumbnail);
					AddListItemToTopToBottom((GuiWidget)(object)child2);
				}
			}
			LastScrollPosition lastScrollPosition = inData as LastScrollPosition;
			if (lastScrollPosition != null)
			{
				((ScrollableWidget)this).set_TopLeftOffset(lastScrollPosition.topLeftScrollPosition);
			}
		}

		private void LibraryDataReloaded(object sender, EventArgs e)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			UiThread.RunOnIdle((Action<object>)AddAllItems, (object)new LastScrollPosition
			{
				topLeftScrollPosition = ((ScrollableWidget)this).get_TopLeftOffset()
			}, 0.0);
		}
	}
}
