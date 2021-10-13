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

namespace MatterHackers.MatterControl.CustomWidgets.LibrarySelector
{
	public class LibrarySelectorWidget : ScrollableWidget
	{
		public delegate void HoverValueChangedEventHandler(object sender, EventArgs e);

		public SelectedListItems<LibrarySelectorRowItem> SelectedItems = new SelectedListItems<LibrarySelectorRowItem>();

		protected FlowLayoutWidget topToBottomItemList;

		private LibraryProvider currentLibraryProvider;

		private RGBA_Bytes baseColor = new RGBA_Bytes(255, 255, 255);

		private bool editMode;

		private RGBA_Bytes hoverColor = new RGBA_Bytes(204, 204, 204, 255);

		private RGBA_Bytes selectedColor = new RGBA_Bytes(180, 180, 180, 255);

		private int selectedIndex = -1;

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
				if (currentLibraryProvider != value)
				{
					currentLibraryProvider.DataReloaded -= LibraryDataReloaded;
					value.DataReloaded += LibraryDataReloaded;
					bool flag = value.ParentLibraryProvider == currentLibraryProvider;
					while (!flag && currentLibraryProvider != value && currentLibraryProvider.ParentLibraryProvider != null)
					{
						LibraryProvider parentLibraryProvider = currentLibraryProvider.ParentLibraryProvider;
						currentLibraryProvider.Dispose();
						currentLibraryProvider = parentLibraryProvider;
					}
					currentLibraryProvider = value;
					if (this.ChangedCurrentLibraryProvider != null)
					{
						this.ChangedCurrentLibraryProvider(null, value);
					}
				}
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

		private int Count => ((Collection<GuiWidget>)(object)((GuiWidget)topToBottomItemList).get_Children()).Count;

		public event Action<LibraryProvider, LibraryProvider> ChangedCurrentLibraryProvider;

		public event HoverValueChangedEventHandler HoverValueChanged;

		public event Action<object, EventArgs> SelectedIndexChanged;

		public void SetCurrentLibraryProvider(LibraryProvider libraryProvider)
		{
			CurrentLibraryProvider = libraryProvider;
			UiThread.RunOnIdle((Action)RebuildView);
		}

		public LibrarySelectorWidget(bool includeQueueLibraryProvider)
			: this(false)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Expected O, but got Unknown
			currentLibraryProvider = new LibraryProviderSelector(SetCurrentLibraryProvider, includeQueueLibraryProvider);
			currentLibraryProvider.DataReloaded += LibraryDataReloaded;
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

		public void ClearSelected()
		{
			if (selectedIndex != -1)
			{
				selectedIndex = -1;
				OnSelectedIndexChanged();
			}
		}

		public void ClearSelectedItems()
		{
			SelectedItems.Clear();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			currentLibraryProvider.DataReloaded -= LibraryDataReloaded;
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			while (currentLibraryProvider != null && currentLibraryProvider.ParentLibraryProvider != null)
			{
				LibraryProvider parentLibraryProvider = currentLibraryProvider.ParentLibraryProvider;
				currentLibraryProvider.Dispose();
				currentLibraryProvider = parentLibraryProvider;
			}
			((GuiWidget)this).OnClosed(e);
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

		public void OnSelectedIndexChanged()
		{
			((GuiWidget)this).Invalidate();
			if (this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged(this, null);
			}
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
			using List<LibrarySelectorRowItem>.Enumerator enumerator = SelectedItems.GetEnumerator();
			if (enumerator.MoveNext())
			{
				_ = enumerator.Current;
				throw new NotImplementedException();
			}
		}

		protected GuiWidget GetThumbnailWidget(LibraryProvider parentProvider, PrintItemCollection printItemCollection, ImageBuffer imageBuffer)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
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
			PrintItemCollection localPrintItemCollection = printItemCollection;
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)delegate(object sender, MouseEventArgs e)
			{
				if (UserSettings.Instance.IsTouchScreen)
				{
					if (parentProvider == null)
					{
						CurrentLibraryProvider = CurrentLibraryProvider.GetProviderForCollection(localPrintItemCollection);
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
						CurrentLibraryProvider = CurrentLibraryProvider.GetProviderForCollection(localPrintItemCollection);
					}
					else
					{
						CurrentLibraryProvider = parentProvider;
					}
				}
				UiThread.RunOnIdle((Action)RebuildView);
			});
			return (GuiWidget)val4;
		}

		private void AddAllItems()
		{
			((GuiWidget)topToBottomItemList).RemoveAllChildren();
			LibraryProvider libraryProvider = CurrentLibraryProvider;
			if (libraryProvider == null)
			{
				return;
			}
			if (libraryProvider.ProviderKey != LibraryProviderSelector.ProviderKeyName)
			{
				PrintItemCollection printItemCollection = new PrintItemCollection("..", libraryProvider.ProviderKey);
				LibrarySelectorRowItem child = new LibrarySelectorRowItem(printItemCollection, -1, this, libraryProvider.ParentLibraryProvider, GetThumbnailWidget(libraryProvider.ParentLibraryProvider, printItemCollection, LibraryProvider.UpFolderImage), "Back".Localize());
				AddListItemToTopToBottom((GuiWidget)(object)child);
			}
			for (int i = 0; i < libraryProvider.CollectionCount; i++)
			{
				PrintItemCollection collectionItem = libraryProvider.GetCollectionItem(i);
				if (collectionItem.Key != "LibraryProviderPurchasedKey" && collectionItem.Key != "LibraryProviderSharedKey")
				{
					LibrarySelectorRowItem child2 = new LibrarySelectorRowItem(collectionItem, i, this, null, GetThumbnailWidget(null, collectionItem, libraryProvider.GetCollectionFolderImage(i)), "Open".Localize());
					AddListItemToTopToBottom((GuiWidget)(object)child2);
				}
			}
		}

		private void LibraryDataReloaded(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)AddAllItems);
		}
	}
}
