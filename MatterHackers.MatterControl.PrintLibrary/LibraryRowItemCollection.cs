using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintLibrary.Provider;

namespace MatterHackers.MatterControl.PrintLibrary
{
	public class LibraryRowItemCollection : LibraryRowItem
	{
		private LibraryProvider parentProvider;

		private LibraryProvider currentProvider;

		private PrintItemCollection printItemCollection;

		private string openButtonText;

		private static readonly string collectionNotEmtyMessage = "The folder '{0}' is not empty.\n\nWould you like to delete it anyway?".Localize();

		private static readonly string collectionNotEmtyTitle = "Delete folder?".Localize();

		private static readonly string deleteNow = "Delete".Localize();

		private static readonly string doNotDelete = "Cancel".Localize();

		private ConditionalClickWidget primaryClickContainer;

		public int CollectionIndex
		{
			get;
			private set;
		}

		public PrintItemCollection PrintItemCollection => printItemCollection;

		public override bool Protected => currentProvider.IsProtected();

		public LibraryRowItemCollection(PrintItemCollection collection, LibraryProvider currentProvider, int collectionIndex, LibraryDataView libraryDataView, LibraryProvider parentProvider, GuiWidget thumbnailWidget, string openButtonText)
			: base(libraryDataView, thumbnailWidget)
		{
			this.openButtonText = openButtonText;
			this.currentProvider = currentProvider;
			CollectionIndex = collectionIndex;
			this.parentProvider = parentProvider;
			printItemCollection = collection;
			base.ItemName = printItemCollection.Name;
			((GuiWidget)this).set_Name(base.ItemName + " Row Item Collection");
			if (collection.Key == LibraryRowItem.LoadingPlaceholderToken)
			{
				base.EnableSlideInActions = false;
				base.IsViewHelperItem = true;
			}
			CreateGuiElements();
		}

		public override void Export()
		{
			throw new NotImplementedException();
		}

		public override void Edit()
		{
			throw new NotImplementedException();
		}

		public override void RemoveFromCollection()
		{
			int collectionChildCollectionCount = currentProvider.GetCollectionChildCollectionCount(CollectionIndex);
			int collectionItemCount = currentProvider.GetCollectionItemCount(CollectionIndex);
			if (collectionChildCollectionCount > 0 || collectionItemCount > 0)
			{
				string message = StringHelper.FormatWith(collectionNotEmtyMessage, new object[1]
				{
					currentProvider.GetCollectionItem(CollectionIndex).Name
				});
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(ProcessDialogResponse, message, collectionNotEmtyTitle, StyledMessageBox.MessageType.YES_NO, deleteNow, doNotDelete);
				});
			}
			else
			{
				currentProvider.RemoveCollection(CollectionIndex);
			}
		}

		private void ProcessDialogResponse(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				currentProvider.RemoveCollection(CollectionIndex);
			}
		}

		public override void AddToQueue()
		{
			throw new NotImplementedException();
		}

		protected override SlideWidget GetItemActionButtons()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			SlideWidget slideWidget = new SlideWidget();
			((GuiWidget)slideWidget).set_VAnchor((VAnchor)5);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_VAnchor((VAnchor)5);
			TextWidget val2 = new TextWidget(openButtonText, 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val2.set_TextColor(RGBA_Bytes.White);
			((GuiWidget)val2).set_VAnchor((VAnchor)2);
			((GuiWidget)val2).set_HAnchor((HAnchor)2);
			FatFlatClickWidget fatFlatClickWidget = new FatFlatClickWidget(val2);
			((GuiWidget)fatFlatClickWidget).set_Name("Open Collection");
			((GuiWidget)fatFlatClickWidget).set_Cursor((Cursors)3);
			((GuiWidget)fatFlatClickWidget).set_VAnchor((VAnchor)5);
			((GuiWidget)fatFlatClickWidget).set_BackgroundColor(ActiveTheme.get_Instance().get_SecondaryAccentColor());
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

		private void ChangeCollection()
		{
			if (parentProvider == null)
			{
				libraryDataView.CurrentLibraryProvider = currentProvider.GetProviderForCollection(printItemCollection);
			}
			else
			{
				libraryDataView.CurrentLibraryProvider = parentProvider;
			}
		}

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			if (((GuiWidget)this).IsDoubleClick(mouseEvent) && base.EnableSlideInActions)
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

		private void onAddLinkClick(object sender, EventArgs e)
		{
		}

		protected override void RemoveThisFromPrintLibrary()
		{
			throw new NotImplementedException();
		}

		private void onRemoveLinkClick(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)RemoveThisFromPrintLibrary);
		}

		private void onThemeChanged(object sender, EventArgs e)
		{
			((GuiWidget)this).Invalidate();
		}
	}
}
