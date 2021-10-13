using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PartPreviewWindow;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintLibrary.Provider;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.PrintLibrary
{
	public class LibraryRowItemPart : LibraryRowItem
	{
		public bool isActivePrint;

		private LibraryProvider libraryProvider;

		private double thumbnailWidth;

		private ExportPrintItemWindow exportingWindow;

		private PartPreviewMainWindow viewingWindow;

		private ProgressControl processingProgressControl;

		public int ItemIndex
		{
			get;
			private set;
		}

		public override bool Protected => libraryProvider.IsItemProtected(ItemIndex);

		public LibraryRowItemPart(LibraryProvider libraryProvider, int itemIndex, LibraryDataView libraryDataView, GuiWidget thumbnailWidget)
			: base(libraryDataView, thumbnailWidget)
		{
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Expected O, but got Unknown
			thumbnailWidth = thumbnailWidget.get_Width();
			if (thumbnailWidget != null)
			{
				thumbnailWidget.add_Click((EventHandler<MouseEventArgs>)onViewPartClick);
			}
			base.ItemName = libraryProvider.GetPrintItemName(itemIndex);
			if (base.ItemName == LibraryRowItem.LoadingPlaceholderToken)
			{
				base.ItemName = "Retrieving Contents...".Localize();
				base.IsViewHelperItem = true;
				base.EnableSlideInActions = false;
			}
			else if (base.ItemName == LibraryRowItem.LoadFailedPlaceholderToken)
			{
				base.ItemName = "Error Loading Contents".Localize();
				base.IsViewHelperItem = true;
				base.EnableSlideInActions = false;
			}
			else if (base.ItemName == LibraryRowItem.SearchResultsNotAvailableToken)
			{
				base.ItemName = "Oops! Please select a folder to search".Localize();
				base.IsViewHelperItem = true;
				base.EnableSlideInActions = false;
			}
			this.libraryProvider = libraryProvider;
			ItemIndex = itemIndex;
			CreateGuiElements();
			AddLoadingProgressBar();
			libraryProvider.RegisterForProgress(itemIndex, new ReportProgressRatio(ReportProgressRatio));
		}

		public async Task<PrintItemWrapper> GetPrintItemWrapperAsync()
		{
			return await libraryProvider.GetPrintItemWrapperAsync(ItemIndex);
		}

		private void ReportProgressRatio(double progress0To1, string processingState, out bool continueProcessing)
		{
			continueProcessing = true;
			if (progress0To1 == 0.0)
			{
				((GuiWidget)processingProgressControl).set_Visible(false);
			}
			else
			{
				((GuiWidget)processingProgressControl).set_Visible(true);
			}
			processingProgressControl.set_RatioComplete(progress0To1);
			processingProgressControl.set_ProcessType(processingState);
		}

		private void AddLoadingProgressBar()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			ProgressControl val = new ProgressControl("Downloading...".Localize(), RGBA_Bytes.Black, ActiveTheme.get_Instance().get_SecondaryAccentColor(), (int)(100.0 * GuiWidget.get_DeviceScale()), 5, 0);
			val.set_PointSize(8.0);
			processingProgressControl = val;
			((GuiWidget)processingProgressControl).set_VAnchor((VAnchor)1);
			((GuiWidget)processingProgressControl).set_HAnchor((HAnchor)1);
			((GuiWidget)processingProgressControl).set_Margin(new BorderDouble(0.0));
			((GuiWidget)processingProgressControl).set_Visible(false);
			middleColumn.AddChild((GuiWidget)(object)processingProgressControl, -1);
		}

		public override async void AddToQueue()
		{
			PrintItemWrapper printItemWrapper = await MakeCopyForQueue();
			if (printItemWrapper != null)
			{
				QueueData.Instance.AddItem(printItemWrapper);
			}
		}

		private async Task<PrintItemWrapper> MakeCopyForQueue()
		{
			PrintItemWrapper printItemWrapper = await GetPrintItemWrapperAsync();
			if (!File.Exists(printItemWrapper.FileLocation))
			{
				return null;
			}
			PrintItem printItem = printItemWrapper.PrintItem;
			string path = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(printItem.FileLocation));
			string text = Path.Combine(ApplicationDataStorage.Instance.ApplicationLibraryDataPath, path);
			try
			{
				File.Copy(printItem.FileLocation, text);
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Unable to duplicate file for queue: {printItem.FileLocation}\r\n{ex.Message}");
				return null;
			}
			return new PrintItemWrapper(new PrintItem(printItem.Name, text)
			{
				Protected = printItem.Protected
			});
		}

		public override void Edit()
		{
			OpenPartViewWindow(View3DWidget.OpenMode.Editing);
		}

		public override async void Export()
		{
			OpenExportWindow(await GetPrintItemWrapperAsync());
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			if (libraryDataView.EditMode)
			{
				selectionCheckBoxContainer.set_Visible(true);
				((GuiWidget)rightButtonOverlay).set_Visible(false);
			}
			else
			{
				selectionCheckBoxContainer.set_Visible(false);
			}
			base.OnDraw(graphics2D);
			if (base.IsSelectedItem)
			{
				((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
				partLabel.set_TextColor(RGBA_Bytes.White);
				selectionCheckBox.set_TextColor(RGBA_Bytes.White);
			}
			else if (base.IsHoverItem)
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

		public async void OpenPartViewWindow(View3DWidget.OpenMode openMode = View3DWidget.OpenMode.Viewing, PrintItemWrapper printItemWrapper = null)
		{
			if (viewingWindow == null)
			{
				if (printItemWrapper == null)
				{
					printItemWrapper = await GetPrintItemWrapperAsync();
				}
				viewingWindow = new PartPreviewMainWindow(printItemWrapper, View3DWidget.AutoRotate.Enabled, openMode);
				((GuiWidget)viewingWindow).add_Closed((EventHandler<ClosedEventArgs>)PartPreviewMainWindow_Closed);
			}
			else
			{
				((GuiWidget)viewingWindow).BringToFront();
			}
		}

		public override void RemoveFromCollection()
		{
			libraryDataView.CurrentLibraryProvider.RemoveItem(ItemIndex);
		}

		protected override SlideWidget GetItemActionButtons()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Expected O, but got Unknown
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Expected O, but got Unknown
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Expected O, but got Unknown
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Expected O, but got Unknown
			SlideWidget buttonContainer = new SlideWidget();
			((GuiWidget)buttonContainer).set_VAnchor((VAnchor)5);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			TextWidget val2 = new TextWidget("Print".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(RGBA_Bytes.White);
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			((GuiWidget)val2).set_HAnchor((HAnchor)2);
			FatFlatClickWidget fatFlatClickWidget = new FatFlatClickWidget(val2);
			((GuiWidget)fatFlatClickWidget).set_Name("Row Item " + ((GuiWidget)partLabel).get_Text() + " Print Button");
			((GuiWidget)fatFlatClickWidget).set_VAnchor((VAnchor)5);
			((GuiWidget)fatFlatClickWidget).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			((GuiWidget)fatFlatClickWidget).set_Width(100.0);
			fatFlatClickWidget.Click += printButton_Click;
			fatFlatClickWidget.Click += delegate
			{
				buttonContainer.SlideOut();
			};
			TextWidget val3 = new TextWidget("Enqueue".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val3.set_TextColor(RGBA_Bytes.White);
			((GuiWidget)val3).set_VAnchor((VAnchor)2);
			((GuiWidget)val3).set_HAnchor((HAnchor)2);
			FatFlatClickWidget fatFlatClickWidget2 = new FatFlatClickWidget(val3);
			((GuiWidget)fatFlatClickWidget2).set_Name("Row Item " + ((GuiWidget)partLabel).get_Text() + " Enqueue Button");
			((GuiWidget)fatFlatClickWidget2).set_VAnchor((VAnchor)5);
			((GuiWidget)fatFlatClickWidget2).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			((GuiWidget)fatFlatClickWidget2).set_Width(100.0);
			fatFlatClickWidget2.Click += async delegate
			{
				PrintItemWrapper item = await MakeCopyForQueue();
				QueueData.Instance.AddItem(item);
				((GuiWidget)this).Invalidate();
			};
			TextWidget val4 = new TextWidget("View".Localize(), 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val4.set_TextColor(RGBA_Bytes.White);
			((GuiWidget)val4).set_VAnchor((VAnchor)2);
			((GuiWidget)val4).set_HAnchor((HAnchor)2);
			FatFlatClickWidget fatFlatClickWidget3 = new FatFlatClickWidget(val4);
			((GuiWidget)fatFlatClickWidget3).set_Name("Row Item " + ((GuiWidget)partLabel).get_Text() + " View Button");
			((GuiWidget)fatFlatClickWidget3).set_VAnchor((VAnchor)5);
			((GuiWidget)fatFlatClickWidget3).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
			((GuiWidget)fatFlatClickWidget3).set_Width(100.0);
			fatFlatClickWidget3.Click += onViewPartClick;
			((GuiWidget)val).AddChild((GuiWidget)(object)fatFlatClickWidget3, -1);
			GuiWidget val5 = new GuiWidget(2.0, 2.0, (SizeLimitsToSet)1);
			val5.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			val5.set_VAnchor((VAnchor)5);
			((GuiWidget)val).AddChild(val5, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)fatFlatClickWidget2, -1);
			GuiWidget val6 = new GuiWidget(2.0, 2.0, (SizeLimitsToSet)1);
			val6.set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryAccentColor());
			val6.set_VAnchor((VAnchor)5);
			((GuiWidget)val).AddChild(val6, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)fatFlatClickWidget, -1);
			((GuiWidget)buttonContainer).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)buttonContainer).set_Width(304.0);
			return buttonContainer;
		}

		private async void printButton_Click(object sender, EventArgs e)
		{
			PrintItemWrapper item = await MakeCopyForQueue();
			if (!PrinterConnectionAndCommunication.Instance.PrintIsActive)
			{
				QueueData.Instance.AddItem(item, 0);
				QueueData.Instance.SelectedIndex = 0;
				PrinterConnectionAndCommunication.Instance.PrintActivePartIfPossible();
			}
			else
			{
				QueueData.Instance.AddItem(item);
			}
			((GuiWidget)this).Invalidate();
		}

		protected override void RemoveThisFromPrintLibrary()
		{
			libraryDataView.CurrentLibraryProvider.RemoveItem(ItemIndex);
		}

		private void ExportQueueItemWindow_Closed(object sender, ClosedEventArgs e)
		{
			exportingWindow = null;
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

		private void onLibraryItemClick(object sender, EventArgs e)
		{
			if (libraryDataView.EditMode)
			{
				if (!base.IsSelectedItem)
				{
					selectionCheckBox.set_Checked(true);
					libraryDataView.SelectedItems.Add(this);
				}
				else
				{
					selectionCheckBox.set_Checked(false);
					libraryDataView.SelectedItems.Remove(this);
				}
			}
		}

		private void onOpenPartViewClick(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				openPartView();
			});
		}

		private void onRemoveLinkClick(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)RemoveThisFromPrintLibrary);
		}

		private void onThemeChanged(object sender, EventArgs e)
		{
			((GuiWidget)this).Invalidate();
		}

		private void onViewPartClick(object sender, EventArgs e)
		{
			if (!base.IsViewHelperItem)
			{
				UiThread.RunOnIdle((Action)delegate
				{
					rightButtonOverlay.SlideOut();
					openPartView();
				});
			}
		}

		private async void OpenExportWindow()
		{
			if (exportingWindow == null)
			{
				exportingWindow = new ExportPrintItemWindow(await GetPrintItemWrapperAsync());
				((GuiWidget)exportingWindow).add_Closed((EventHandler<ClosedEventArgs>)ExportQueueItemWindow_Closed);
				((SystemWindow)exportingWindow).ShowAsSystemWindow();
			}
			else
			{
				((GuiWidget)exportingWindow).BringToFront();
			}
		}

		private void OpenExportWindow(PrintItemWrapper printItem)
		{
			if (exportingWindow == null)
			{
				exportingWindow = new ExportPrintItemWindow(printItem);
				((GuiWidget)exportingWindow).add_Closed((EventHandler<ClosedEventArgs>)ExportQueueItemWindow_Closed);
				((SystemWindow)exportingWindow).ShowAsSystemWindow();
			}
			else
			{
				((GuiWidget)exportingWindow).BringToFront();
			}
		}

		private async void openPartView(View3DWidget.OpenMode openMode = View3DWidget.OpenMode.Viewing)
		{
			PrintItemWrapper printItemWrapper = await GetPrintItemWrapperAsync();
			if (printItemWrapper != null)
			{
				string fileLocation = printItemWrapper.FileLocation;
				if (File.Exists(fileLocation))
				{
					OpenPartViewWindow(openMode, printItemWrapper);
					return;
				}
				string message = $"Cannot find\n'{fileLocation}'.\nWould you like to remove it from the library?";
				StyledMessageBox.ShowMessageBox(null, message, "Item not found", StyledMessageBox.MessageType.YES_NO, "Remove".Localize(), "Cancel".Localize());
			}
		}

		private void PartPreviewMainWindow_Closed(object sender, ClosedEventArgs e)
		{
			viewingWindow = null;
		}

		private void selectionCheckBox_CheckedStateChanged(object sender, EventArgs e)
		{
			if (selectionCheckBox.get_Checked())
			{
				libraryDataView.SelectedItems.Add(this);
			}
			else
			{
				libraryDataView.SelectedItems.Remove(this);
			}
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
