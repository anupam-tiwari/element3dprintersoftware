using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrintLibrary
{
	public abstract class LibraryRowItem : GuiWidget
	{
		public static readonly string LoadingPlaceholderToken = "!Placeholder_ItemToken!";

		public static readonly string LoadFailedPlaceholderToken = "!Placeholder_LoadFailedToken!";

		public static readonly string SearchResultsNotAvailableToken = "!Placeholder_SearchResultsNotAvailable!";

		public CheckBox selectionCheckBox;

		public RGBA_Bytes WidgetBackgroundColor;

		public RGBA_Bytes WidgetTextColor;

		protected LibraryDataView libraryDataView;

		protected TextWidget partLabel;

		protected SlideWidget rightButtonOverlay;

		protected GuiWidget selectionCheckBoxContainer;

		private bool isHoverItem;

		private LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		private GuiWidget thumbnailWidget;

		protected GuiWidget middleColumn;

		private EventHandler unregisterEvents;

		public bool IsSelectedItem => libraryDataView.SelectedItems.Contains(this);

		public bool IsViewHelperItem
		{
			get;
			set;
		}

		public bool EnableSlideInActions
		{
			get;
			set;
		}

		public string ItemName
		{
			get;
			protected set;
		}

		public bool IsHoverItem
		{
			get
			{
				return isHoverItem;
			}
			set
			{
				if (isHoverItem != value && EnableSlideInActions)
				{
					isHoverItem = value;
					if (value && !libraryDataView.EditMode)
					{
						rightButtonOverlay.SlideIn();
					}
					else
					{
						rightButtonOverlay.SlideOut();
					}
				}
			}
		}

		public abstract bool Protected
		{
			get;
		}

		public LibraryRowItem(LibraryDataView libraryDataView, GuiWidget thumbnailWidget)
			: this()
		{
			this.thumbnailWidget = thumbnailWidget;
			this.libraryDataView = libraryDataView;
			IsViewHelperItem = false;
			EnableSlideInActions = true;
			((GuiWidget)this).add_MouseEnterBounds((EventHandler)delegate
			{
				UpdateHoverState();
			});
			((GuiWidget)this).add_MouseLeaveBounds((EventHandler)delegate
			{
				UpdateHoverState();
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

		private void selectionCheckBox_Click(object sender, EventArgs e)
		{
			if (selectionCheckBox.get_Checked())
			{
				if (!libraryDataView.SelectedItems.Contains(this))
				{
					libraryDataView.SelectedItems.Add(this);
				}
			}
			else if (libraryDataView.SelectedItems.Contains(this))
			{
				libraryDataView.SelectedItems.Remove(this);
			}
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Expected O, but got Unknown
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			if (libraryDataView.EditMode && !IsViewHelperItem)
			{
				selectionCheckBox.set_Checked(IsSelectedItem);
				selectionCheckBoxContainer.set_Visible(true);
				((GuiWidget)rightButtonOverlay).set_Visible(false);
			}
			else
			{
				selectionCheckBoxContainer.set_Visible(false);
			}
			((GuiWidget)this).OnDraw(graphics2D);
			if (IsSelectedItem && !IsViewHelperItem)
			{
				((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
				partLabel.set_TextColor(RGBA_Bytes.White);
				selectionCheckBox.set_TextColor(RGBA_Bytes.White);
			}
			else if (IsHoverItem)
			{
				RoundedRect val = new RoundedRect(((GuiWidget)this).get_LocalBounds(), 0.0);
				((GuiWidget)this).set_BackgroundColor(RGBA_Bytes.White);
				partLabel.set_TextColor(RGBA_Bytes.Black);
				selectionCheckBox.set_TextColor(RGBA_Bytes.Black);
				graphics2D.Render((IVertexSource)new Stroke((IVertexSource)(object)val, 3.0), (IColorType)(object)ActiveTheme.get_Instance().get_SecondaryAccentColor());
			}
			else
			{
				((GuiWidget)this).set_BackgroundColor(new RGBA_Bytes(255, 255, 255, 255));
				partLabel.set_TextColor(RGBA_Bytes.Black);
				selectionCheckBox.set_TextColor(RGBA_Bytes.Black);
			}
		}

		protected void CreateGuiElements()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Expected O, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Expected O, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Expected O, but got Unknown
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Expected O, but got Unknown
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Expected O, but got Unknown
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Expected O, but got Unknown
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Cursor((Cursors)3);
			linkButtonFactory.fontSize = 10.0;
			linkButtonFactory.textColor = RGBA_Bytes.White;
			WidgetTextColor = RGBA_Bytes.Black;
			WidgetBackgroundColor = RGBA_Bytes.White;
			SetDisplayAttributes();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			partLabel = new TextWidget(ItemName.Replace('_', ' '), 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			GuiWidget val2 = new GuiWidget();
			val2.set_HAnchor((HAnchor)5);
			val2.set_VAnchor((VAnchor)5);
			val2.set_Name("Row Item " + ((GuiWidget)partLabel).get_Text());
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)5);
			selectionCheckBoxContainer = new GuiWidget();
			selectionCheckBoxContainer.set_VAnchor((VAnchor)5);
			selectionCheckBoxContainer.set_Width(40.0);
			selectionCheckBoxContainer.set_Visible(false);
			selectionCheckBoxContainer.set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			selectionCheckBox = new CheckBox("");
			((GuiWidget)selectionCheckBox).add_Click((EventHandler<MouseEventArgs>)selectionCheckBox_Click);
			((GuiWidget)selectionCheckBox).set_Name("Row Item Select Checkbox");
			((GuiWidget)selectionCheckBox).set_VAnchor((VAnchor)2);
			((GuiWidget)selectionCheckBox).set_HAnchor((HAnchor)2);
			selectionCheckBoxContainer.AddChild((GuiWidget)(object)selectionCheckBox, -1);
			middleColumn = new GuiWidget(0.0, 0.0, (SizeLimitsToSet)1);
			middleColumn.set_HAnchor((HAnchor)5);
			middleColumn.set_VAnchor((VAnchor)5);
			middleColumn.set_Margin(new BorderDouble(10.0, 3.0));
			partLabel.set_TextColor(WidgetTextColor);
			((GuiWidget)partLabel).set_MinimumSize(new Vector2(1.0, 18.0));
			((GuiWidget)partLabel).set_VAnchor((VAnchor)2);
			middleColumn.AddChild((GuiWidget)(object)partLabel, -1);
			bool mouseDownOnMiddle = false;
			middleColumn.add_MouseDown((EventHandler<MouseEventArgs>)delegate
			{
				if (!IsViewHelperItem)
				{
					mouseDownOnMiddle = true;
				}
			});
			middleColumn.add_MouseUp((EventHandler<MouseEventArgs>)delegate(object sender, MouseEventArgs e)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				bool num = mouseDownOnMiddle;
				RectangleDouble localBounds = middleColumn.get_LocalBounds();
				if (num & ((RectangleDouble)(ref localBounds)).Contains(e.get_Position()))
				{
					if (libraryDataView.EditMode)
					{
						if (IsSelectedItem)
						{
							libraryDataView.SelectedItems.Remove(this);
						}
						else
						{
							libraryDataView.SelectedItems.Add(this);
						}
						((GuiWidget)this).Invalidate();
					}
					else if (!IsSelectedItem)
					{
						libraryDataView.ClearSelectedItems();
						libraryDataView.SelectedItems.Add(this);
						((GuiWidget)this).Invalidate();
					}
				}
				mouseDownOnMiddle = false;
			});
			((GuiWidget)val3).AddChild(selectionCheckBoxContainer, -1);
			((GuiWidget)val3).AddChild(thumbnailWidget, -1);
			((GuiWidget)val3).AddChild(middleColumn, -1);
			val2.AddChild((GuiWidget)(object)val3, -1);
			rightButtonOverlay = GetItemActionButtons();
			((GuiWidget)rightButtonOverlay).set_Visible(false);
			((GuiWidget)val).AddChild(val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)rightButtonOverlay, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			AddHandlers();
		}

		public abstract void AddToQueue();

		public abstract void Edit();

		public abstract void Export();

		public abstract void RemoveFromCollection();

		protected abstract SlideWidget GetItemActionButtons();

		protected abstract void RemoveThisFromPrintLibrary();

		public override void OnMouseMove(MouseEventArgs mouseEvent)
		{
			UpdateHoverState();
			((GuiWidget)this).OnMouseMove(mouseEvent);
		}

		private void UpdateHoverState()
		{
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected I4, but got Unknown
				UnderMouseState underMouseState = ((GuiWidget)this).get_UnderMouseState();
				switch ((int)underMouseState)
				{
				case 0:
					IsHoverItem = false;
					break;
				case 2:
					IsHoverItem = true;
					break;
				case 1:
					if (((GuiWidget)this).ContainsFirstUnderMouseRecursive())
					{
						IsHoverItem = true;
					}
					else
					{
						IsHoverItem = false;
					}
					break;
				}
			});
		}

		private void AddHandlers()
		{
			((GuiWidget)this).add_GestureFling((EventHandler<FlingEventArgs>)delegate(object sender, FlingEventArgs eventArgs)
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Invalid comparison between Unknown and I4
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Invalid comparison between Unknown and I4
				if (!libraryDataView.EditMode)
				{
					if ((int)eventArgs.get_Direction() == 2)
					{
						rightButtonOverlay.SlideIn();
					}
					else if ((int)eventArgs.get_Direction() == 3)
					{
						rightButtonOverlay.SlideOut();
					}
				}
				((GuiWidget)this).Invalidate();
			});
		}

		private void onAddLinkClick(object sender, EventArgs e)
		{
		}

		private void onConfirmRemove(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				((GuiWidget)libraryDataView).RemoveChild((GuiWidget)(object)this);
			}
		}

		private void onThemeChanged(object sender, EventArgs e)
		{
			((GuiWidget)this).Invalidate();
		}

		private void SetDisplayAttributes()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_HAnchor((HAnchor)5);
			((GuiWidget)this).set_Height(50.0 * GuiWidget.get_DeviceScale());
			((GuiWidget)this).set_Padding(new BorderDouble(0.0));
			((GuiWidget)this).set_Margin(new BorderDouble(6.0, 0.0, 6.0, 6.0));
		}
	}
}
