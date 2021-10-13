using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PartPreviewWindow;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrintQueue
{
	public class QueueRowItem : GuiWidget
	{
		private class PartToAddToQueue
		{
			internal string FileLocation;

			internal int insertAfterIndex;

			internal string Name;

			internal PartToAddToQueue(string name, string fileLocation, int insertAfterIndex)
			{
				Name = name;
				FileLocation = fileLocation;
				this.insertAfterIndex = insertAfterIndex;
			}
		}

		public bool isActivePrint;

		public bool isHoverItem;

		public bool isSelectedItem;

		public CheckBox selectionCheckBox;

		public RGBA_Bytes WidgetBackgroundColor;

		public RGBA_Bytes WidgetTextColor;

		private static PrintItemWrapper itemToRemove;

		private SlideWidget actionButtonContainer;

		private string alsoRemoveFromSdCardMessage = "Would you also like to remove this file from the Printer's SD Card?".Localize();

		private string alsoRemoveFromSdCardTitle = "Remove From Printer's SD Card?";

		private ConditionalClickWidget conditionalClickContainer;

		private ExportPrintItemWindow exportingWindow;

		private bool exportingWindowIsOpen;

		private LinkButtonFactory linkButtonFactory = new LinkButtonFactory();

		private TextWidget partLabel;

		private TextWidget partStatus;

		private QueueDataView queueDataView;

		private GuiWidget selectionCheckBoxContainer;

		private FatFlatClickWidget viewButton;

		private TextWidget viewButtonLabel;

		private PartPreviewMainWindow viewingWindow;

		private bool viewWindowIsOpen;

		private EventHandler unregisterEvents;

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
					if (value && !queueDataView.EditMode)
					{
						actionButtonContainer.SlideIn();
					}
					else
					{
						actionButtonContainer.SlideOut();
					}
				}
			}
		}

		public PrintItemWrapper PrintItemWrapper
		{
			get;
			set;
		}

		public QueueRowItem(PrintItemWrapper printItemWrapper, QueueDataView queueDataView)
			: this()
		{
			this.queueDataView = queueDataView;
			PrintItemWrapper = printItemWrapper;
			((GuiWidget)this).set_Name("Queue Item " + printItemWrapper.Name);
			PrintItemWrapper.UseIncrementedNameDuringTypeChange = true;
			ConstructPrintQueueItem();
		}

		public override void OnMouseEnterBounds(MouseEventArgs mouseEvent)
		{
			UpdateHoverState();
			((GuiWidget)this).OnMouseEnterBounds(mouseEvent);
		}

		public override void OnMouseLeaveBounds(MouseEventArgs mouseEvent)
		{
			UpdateHoverState();
			((GuiWidget)this).OnMouseLeaveBounds(mouseEvent);
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

		public static void ShowCantFindFileMessage(PrintItemWrapper printItemWrapper)
		{
			itemToRemove = printItemWrapper;
			UiThread.RunOnIdle((Action)delegate
			{
				string text = printItemWrapper.FileLocation;
				int num = 43;
				if (text.Length > num)
				{
					string text2 = text.Substring(0, 15) + "...";
					int num2 = num - text2.Length;
					string str = text.Substring(text.Length - num2, num2);
					text = text2 + str;
				}
				string text3 = "Oops! Could not find this file".Localize();
				string text4 = "Would you like to remove it from the queue".Localize();
				string message = StringHelper.FormatWith("{0}:\n'{1}'\n\n{2}?", new object[3]
				{
					text3,
					text,
					text4
				});
				string caption = "Item not Found".Localize();
				StyledMessageBox.ShowMessageBox(onConfirmRemove, message, caption, StyledMessageBox.MessageType.YES_NO, "Remove".Localize(), "Cancel".Localize());
			});
		}

		public void ConstructPrintQueueItem()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Expected O, but got Unknown
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Expected O, but got Unknown
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Expected O, but got Unknown
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Expected O, but got Unknown
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			linkButtonFactory.fontSize = 10.0;
			linkButtonFactory.textColor = RGBA_Bytes.Black;
			WidgetTextColor = RGBA_Bytes.Black;
			WidgetBackgroundColor = RGBA_Bytes.White;
			SetDisplayAttributes();
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val).set_HAnchor((HAnchor)(((GuiWidget)val).get_HAnchor() | 5));
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)(((GuiWidget)val2).get_HAnchor() | 5));
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_VAnchor((VAnchor)12);
			selectionCheckBoxContainer = new GuiWidget();
			selectionCheckBoxContainer.set_VAnchor((VAnchor)5);
			selectionCheckBoxContainer.set_Width(40.0);
			selectionCheckBoxContainer.set_Visible(false);
			selectionCheckBoxContainer.set_Margin(new BorderDouble(6.0, 0.0, 0.0, 0.0));
			selectionCheckBox = new CheckBox("");
			((GuiWidget)selectionCheckBox).set_Name("Queue Item Checkbox");
			((GuiWidget)selectionCheckBox).set_VAnchor((VAnchor)2);
			((GuiWidget)selectionCheckBox).set_HAnchor((HAnchor)2);
			selectionCheckBoxContainer.AddChild((GuiWidget)(object)selectionCheckBox, -1);
			PartThumbnailWidget partThumbnailWidget = new PartThumbnailWidget(PrintItemWrapper, "part_icon_transparent_40x40.png", "building_thumbnail_40x40.png", PartThumbnailWidget.ImageSizes.Size50x50);
			((GuiWidget)partThumbnailWidget).set_Name("Queue Item Thumbnail");
			((GuiWidget)val3).AddChild(selectionCheckBoxContainer, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)partThumbnailWidget, -1);
			FlowLayoutWidget val4 = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)val4).set_VAnchor((VAnchor)12);
			((GuiWidget)val4).set_HAnchor((HAnchor)5);
			((GuiWidget)val4).set_Padding(new BorderDouble(8.0));
			((GuiWidget)val4).set_Margin(new BorderDouble(10.0, 0.0));
			partLabel = new TextWidget(PrintItemWrapper.GetFriendlyName(), 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			partLabel.set_TextColor(WidgetTextColor);
			((GuiWidget)partLabel).set_MinimumSize(new Vector2(1.0, 16.0));
			partStatus = new TextWidget(string.Format("{0}: {1}", "Status".Localize().ToUpper(), "Queued to Print".Localize()), 0.0, 0.0, 10.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			partStatus.set_AutoExpandBoundsToText(true);
			partStatus.set_TextColor(WidgetTextColor);
			((GuiWidget)partStatus).set_MinimumSize(new Vector2(50.0, 12.0));
			((GuiWidget)val4).AddChild((GuiWidget)(object)partLabel, -1);
			((GuiWidget)val4).AddChild((GuiWidget)(object)partStatus, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)val4, -1);
			conditionalClickContainer = new ConditionalClickWidget(() => queueDataView.EditMode);
			((GuiWidget)conditionalClickContainer).set_HAnchor((HAnchor)5);
			((GuiWidget)conditionalClickContainer).set_VAnchor((VAnchor)5);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			actionButtonContainer = getItemActionButtons();
			((GuiWidget)actionButtonContainer).set_Visible(false);
			((GuiWidget)this).AddChild((GuiWidget)(object)conditionalClickContainer, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)actionButtonContainer, -1);
			PrintItemWrapper.SlicingOutputMessage += PrintItem_SlicingOutputMessage;
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			PrintItemWrapper.SlicingOutputMessage -= PrintItem_SlicingOutputMessage;
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Expected O, but got Unknown
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			if (queueDataView.EditMode)
			{
				selectionCheckBoxContainer.set_Visible(true);
				((GuiWidget)actionButtonContainer).set_Visible(false);
			}
			else
			{
				selectionCheckBoxContainer.set_Visible(false);
			}
			((GuiWidget)this).OnDraw(graphics2D);
			RoundedRect val = new RoundedRect(((GuiWidget)this).get_LocalBounds(), 0.0);
			if (isActivePrint && !queueDataView.EditMode)
			{
				((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
				SetTextColors(RGBA_Bytes.White);
				graphics2D.Render((IVertexSource)new Stroke((IVertexSource)(object)val, 3.0), (IColorType)(object)ActiveTheme.get_Instance().get_SecondaryAccentColor());
			}
			else if (isSelectedItem)
			{
				((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
				partLabel.set_TextColor(RGBA_Bytes.White);
				partStatus.set_TextColor(RGBA_Bytes.White);
				selectionCheckBox.set_TextColor(RGBA_Bytes.White);
			}
			else if (IsHoverItem)
			{
				((GuiWidget)this).set_BackgroundColor(RGBA_Bytes.White);
				partLabel.set_TextColor(RGBA_Bytes.Black);
				selectionCheckBox.set_TextColor(RGBA_Bytes.Black);
				partStatus.set_TextColor(RGBA_Bytes.Black);
				graphics2D.Render((IVertexSource)new Stroke((IVertexSource)(object)val, 3.0), (IColorType)(object)ActiveTheme.get_Instance().get_SecondaryAccentColor());
			}
			else
			{
				((GuiWidget)this).set_BackgroundColor(new RGBA_Bytes(255, 255, 255, 255));
				SetTextColors(RGBA_Bytes.Black);
				selectionCheckBox.set_TextColor(RGBA_Bytes.Black);
				partStatus.set_TextColor(RGBA_Bytes.Black);
			}
		}

		public void OpenPartViewWindow(View3DWidget.OpenMode openMode = View3DWidget.OpenMode.Viewing)
		{
			if (!viewWindowIsOpen)
			{
				viewingWindow = new PartPreviewMainWindow(PrintItemWrapper, View3DWidget.AutoRotate.Disabled, openMode);
				((GuiWidget)viewingWindow).set_Name("Queue Item " + PrintItemWrapper.Name + " Part Preview");
				viewWindowIsOpen = true;
				((GuiWidget)viewingWindow).add_Closed((EventHandler<ClosedEventArgs>)PartPreviewWindow_Closed);
			}
			else if (viewingWindow != null)
			{
				((GuiWidget)viewingWindow).BringToFront();
			}
		}

		public void SetTextColors(RGBA_Bytes color)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (partLabel.get_TextColor() != color)
			{
				partLabel.set_TextColor(color);
				partStatus.set_TextColor(color);
			}
		}

		public void ThemeChanged(object sender, EventArgs e)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (isActivePrint)
			{
				((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
				partLabel.set_TextColor(RGBA_Bytes.White);
				partStatus.set_TextColor(RGBA_Bytes.White);
				((GuiWidget)this).Invalidate();
			}
		}

		internal void DeletePartFromQueue()
		{
			if (PrintItemWrapper.PrintItem.FileLocation == QueueData.SdCardFileName)
			{
				StyledMessageBox.ShowMessageBox(onDeleteFileConfirm, alsoRemoveFromSdCardMessage, alsoRemoveFromSdCardTitle, StyledMessageBox.MessageType.YES_NO);
			}
			int index = QueueData.Instance.GetIndex(PrintItemWrapper);
			QueueData.Instance.RemoveIndexOnIdle(index);
		}

		private static void onConfirmRemove(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				QueueData.Instance.RemoveIndexOnIdle(QueueData.Instance.GetIndex(itemToRemove));
			}
		}

		private void ExportQueueItemWindow_Closed(object sender, ClosedEventArgs e)
		{
			exportingWindowIsOpen = false;
		}

		private SlideWidget getItemActionButtons()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Expected O, but got Unknown
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Expected O, but got Unknown
			SlideWidget slideWidget = new SlideWidget();
			((GuiWidget)slideWidget).set_VAnchor((VAnchor)5);
			((GuiWidget)slideWidget).set_HAnchor((HAnchor)4);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			TextWidget val2 = new TextWidget("Remove".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val2).set_Name("Queue Item " + PrintItemWrapper.Name + " Remove");
			val2.set_TextColor(RGBA_Bytes.White);
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			((GuiWidget)val2).set_HAnchor((HAnchor)2);
			FatFlatClickWidget fatFlatClickWidget = new FatFlatClickWidget(val2);
			((GuiWidget)fatFlatClickWidget).set_VAnchor((VAnchor)5);
			((GuiWidget)fatFlatClickWidget).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			((GuiWidget)fatFlatClickWidget).set_Width(100.0);
			fatFlatClickWidget.Click += onRemovePartClick;
			viewButtonLabel = new TextWidget("View".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)viewButtonLabel).set_Name("Queue Item " + PrintItemWrapper.Name + " View");
			viewButtonLabel.set_TextColor(RGBA_Bytes.White);
			((GuiWidget)viewButtonLabel).set_VAnchor((VAnchor)2);
			((GuiWidget)viewButtonLabel).set_HAnchor((HAnchor)2);
			viewButton = new FatFlatClickWidget(viewButtonLabel);
			((GuiWidget)viewButton).set_VAnchor((VAnchor)5);
			((GuiWidget)viewButton).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			((GuiWidget)viewButton).set_Width(100.0);
			viewButton.Click += onViewPartClick;
			((GuiWidget)val).AddChild((GuiWidget)(object)viewButton, -1);
			GuiWidget val3 = new GuiWidget(2.0, 2.0, (SizeLimitsToSet)1);
			val3.set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			val3.set_VAnchor((VAnchor)5);
			((GuiWidget)val).AddChild(val3, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)fatFlatClickWidget, -1);
			((GuiWidget)slideWidget).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)slideWidget).set_Width(202.0);
			return slideWidget;
		}

		private void onDeleteFileConfirm(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				PrinterConnectionAndCommunication.Instance.DeleteFileFromSdCard(PrintItemWrapper.PrintItem.Name);
			}
		}

		private void onRemovePartClick(object sender, EventArgs e)
		{
			actionButtonContainer.SlideOut();
			UiThread.RunOnIdle((Action)DeletePartFromQueue);
		}

		private void onViewPartClick(object sender, EventArgs e)
		{
			actionButtonContainer.SlideOut();
			UiThread.RunOnIdle((Action)delegate
			{
				OpenPartViewWindow();
			});
		}

		private void OpenExportWindow()
		{
			if (!exportingWindowIsOpen)
			{
				exportingWindow = new ExportPrintItemWindow(PrintItemWrapper);
				exportingWindowIsOpen = true;
				((GuiWidget)exportingWindow).add_Closed((EventHandler<ClosedEventArgs>)ExportQueueItemWindow_Closed);
				((SystemWindow)exportingWindow).ShowAsSystemWindow();
			}
			else if (exportingWindow != null)
			{
				((GuiWidget)exportingWindow).BringToFront();
			}
		}

		private void PartPreviewWindow_Closed(object sender, ClosedEventArgs e)
		{
			viewWindowIsOpen = false;
		}

		private void PrintItem_SlicingOutputMessage(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			((GuiWidget)partStatus).set_Text(StringHelper.FormatWith("Status: {0}", new object[1]
			{
				val.get_Data()
			}));
		}

		private void SetDisplayAttributes()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			((GuiWidget)this).set_HAnchor((HAnchor)13);
			((GuiWidget)this).set_Height(50.0);
			((GuiWidget)this).set_BackgroundColor(WidgetBackgroundColor);
			((GuiWidget)this).set_Padding(new BorderDouble(0.0));
			((GuiWidget)this).set_Margin(new BorderDouble(6.0, 0.0, 6.0, 6.0));
		}
	}
}
