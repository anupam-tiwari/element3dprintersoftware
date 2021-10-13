using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintLibrary.Provider;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.CustomWidgets.LibrarySelector
{
	public class LibrarySelectorRowItem : GuiWidget
	{
		private LibraryProvider parentProvider;

		private PrintItemCollection printItemCollection;

		public RGBA_Bytes WidgetBackgroundColor;

		public RGBA_Bytes WidgetTextColor;

		protected TextWidget partLabel;

		protected SlideWidget rightButtonOverlay;

		private bool isHoverItem;

		private LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		private GuiWidget thumbnailWidget;

		private EventHandler unregisterEvents;

		public int CollectionIndex
		{
			get;
			private set;
		}

		public LibrarySelectorWidget libraryDataView
		{
			get;
			private set;
		}

		public PrintItemCollection PrintItemCollection => printItemCollection;

		public bool Protected => false;

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
				if (isHoverItem != value)
				{
					isHoverItem = value;
					if (value)
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

		public LibrarySelectorRowItem(PrintItemCollection collection, int collectionIndex, LibrarySelectorWidget libraryDataView, LibraryProvider parentProvider, GuiWidget thumbnailWidget, string openButtonText)
			: this()
		{
			this.thumbnailWidget = thumbnailWidget;
			this.libraryDataView = libraryDataView;
			CollectionIndex = collectionIndex;
			this.parentProvider = parentProvider;
			printItemCollection = collection;
			ItemName = printItemCollection.Name;
			((GuiWidget)this).set_Name(ItemName + " Row Item Collection");
			CreateGuiElements(openButtonText);
			((GuiWidget)this).add_MouseEnterBounds((EventHandler)delegate
			{
				UpdateHoverState();
			});
			((GuiWidget)this).add_MouseLeaveBounds((EventHandler)delegate
			{
				UpdateHoverState();
			});
		}

		private void ProcessDialogResponse(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				libraryDataView.CurrentLibraryProvider.RemoveCollection(CollectionIndex);
			}
		}

		private void ChangeCollection()
		{
			if (parentProvider == null)
			{
				libraryDataView.CurrentLibraryProvider = libraryDataView.CurrentLibraryProvider.GetProviderForCollection(printItemCollection);
			}
			else
			{
				libraryDataView.CurrentLibraryProvider = parentProvider;
			}
			UiThread.RunOnIdle((Action)libraryDataView.RebuildView);
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			if (((GuiWidget)this).IsDoubleClick(mouseEvent))
			{
				UiThread.RunOnIdle((Action)ChangeCollection);
			}
			((GuiWidget)this).OnMouseDown(mouseEvent);
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

		protected SlideWidget GetItemActionButtons(string openButtonText)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected O, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			SlideWidget slideWidget = new SlideWidget();
			((GuiWidget)slideWidget).set_VAnchor((VAnchor)5);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			TextWidget val2 = new TextWidget(openButtonText, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(RGBA_Bytes.White);
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			((GuiWidget)val2).set_HAnchor((HAnchor)2);
			FatFlatClickWidget fatFlatClickWidget = new FatFlatClickWidget(val2);
			((GuiWidget)fatFlatClickWidget).set_Cursor((Cursors)3);
			((GuiWidget)fatFlatClickWidget).set_Name("Open Collection");
			((GuiWidget)fatFlatClickWidget).set_VAnchor((VAnchor)5);
			((GuiWidget)fatFlatClickWidget).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			((GuiWidget)fatFlatClickWidget).set_Width(100.0);
			fatFlatClickWidget.Click += delegate
			{
				ChangeCollection();
			};
			((GuiWidget)val).AddChild((GuiWidget)(object)fatFlatClickWidget, -1);
			((GuiWidget)slideWidget).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)slideWidget).set_Width(100.0);
			return slideWidget;
		}

		private void onThemeChanged(object sender, EventArgs e)
		{
			((GuiWidget)this).Invalidate();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).OnDraw(graphics2D);
			if (IsHoverItem)
			{
				RoundedRect val = new RoundedRect(((GuiWidget)this).get_LocalBounds(), 0.0);
				((GuiWidget)this).set_BackgroundColor(RGBA_Bytes.White);
				partLabel.set_TextColor(RGBA_Bytes.Black);
				graphics2D.Render((IVertexSource)new Stroke((IVertexSource)(object)val, 3.0), (IColorType)(object)ActiveTheme.get_Instance().get_SecondaryAccentColor());
			}
			else
			{
				((GuiWidget)this).set_BackgroundColor(new RGBA_Bytes(255, 255, 255, 255));
				partLabel.set_TextColor(RGBA_Bytes.Black);
			}
		}

		protected void CreateGuiElements(string openButtonText)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected O, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Expected O, but got Unknown
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Expected O, but got Unknown
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Cursor((Cursors)3);
			linkButtonFactory.fontSize = 10.0;
			linkButtonFactory.textColor = RGBA_Bytes.White;
			WidgetTextColor = RGBA_Bytes.Black;
			WidgetBackgroundColor = RGBA_Bytes.White;
			SetDisplayAttributes();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			GuiWidget val2 = new GuiWidget();
			val2.set_HAnchor((HAnchor)5);
			val2.set_VAnchor((VAnchor)5);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_VAnchor((VAnchor)5);
			GuiWidget val4 = new GuiWidget(0.0, 0.0, (SizeLimitsToSet)1);
			val4.set_HAnchor((HAnchor)5);
			val4.set_VAnchor((VAnchor)5);
			val4.set_Margin(new BorderDouble(10.0, 6.0));
			partLabel = new TextWidget(ItemName.Replace('_', ' '), 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)partLabel).set_Name("Row Item " + ((GuiWidget)partLabel).get_Text());
			partLabel.set_TextColor(WidgetTextColor);
			((GuiWidget)partLabel).set_MinimumSize(new Vector2(1.0, 18.0));
			((GuiWidget)partLabel).set_VAnchor((VAnchor)2);
			val4.AddChild((GuiWidget)(object)partLabel, -1);
			((GuiWidget)val3).AddChild(thumbnailWidget, -1);
			((GuiWidget)val3).AddChild(val4, -1);
			val2.AddChild((GuiWidget)(object)val3, -1);
			rightButtonOverlay = GetItemActionButtons(openButtonText);
			((GuiWidget)rightButtonOverlay).set_Visible(false);
			((GuiWidget)val).AddChild(val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)rightButtonOverlay, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			AddHandlers();
		}

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
	}
}
