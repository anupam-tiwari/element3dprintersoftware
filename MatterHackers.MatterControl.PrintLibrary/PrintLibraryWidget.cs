using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.CustomWidgets.LibrarySelector;
using MatterHackers.MatterControl.PrintLibrary.Provider;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrintLibrary
{
	public class PrintLibraryWidget : GuiWidget
	{
		private static CreateFolderWindow createFolderWindow;

		private static RenameItemWindow renameItemWindow;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private TextImageButtonFactory editButtonFactory = new TextImageButtonFactory();

		private TextWidget navigationLabel;

		private FlowLayoutWidget breadCrumbAndActionBar;

		private FolderBreadCrumbWidget breadCrumbWidget;

		private List<MenuEnableData> actionMenuEnableData = new List<MenuEnableData>();

		private Button addToLibraryButton;

		private Button createFolderButton;

		private Button enterEditModeButton;

		private Button leaveEditModeButton;

		private FlowLayoutWidget buttonPanel;

		private MHTextEditWidget searchInput;

		private LibraryDataView libraryDataView;

		private GuiWidget providerMessageContainer;

		private TextWidget providerMessageWidget;

		private static PrintLibraryWidget currentPrintLibraryWidget;

		private List<PrintItemAction> menuItems = new List<PrintItemAction>();

		private EventHandler unregisterEvents;

		public string ProviderMessage
		{
			get
			{
				return ((GuiWidget)providerMessageWidget).get_Text();
			}
			set
			{
				if (value != "")
				{
					((GuiWidget)providerMessageWidget).set_Text(value);
					providerMessageContainer.set_Visible(true);
				}
				else
				{
					providerMessageContainer.set_Visible(false);
				}
			}
		}

		public static void Reload()
		{
			if (currentPrintLibraryWidget.libraryDataView != null)
			{
				currentPrintLibraryWidget.libraryDataView.SelectedItems.OnAdd -= currentPrintLibraryWidget.onLibraryItemsSelected;
				currentPrintLibraryWidget.libraryDataView.SelectedItems.OnRemove -= currentPrintLibraryWidget.onLibraryItemsSelected;
				((GuiWidget)currentPrintLibraryWidget).CloseAllChildren();
			}
			currentPrintLibraryWidget.LoadContent();
			currentPrintLibraryWidget.libraryDataView.SelectedItems.OnAdd += currentPrintLibraryWidget.onLibraryItemsSelected;
			currentPrintLibraryWidget.libraryDataView.SelectedItems.OnRemove += currentPrintLibraryWidget.onLibraryItemsSelected;
		}

		public PrintLibraryWidget()
			: this()
		{
			currentPrintLibraryWidget = this;
			Reload();
		}

		private void LoadContent()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Expected O, but got Unknown
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Expected O, but got Unknown
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Expected O, but got Unknown
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Expected O, but got Unknown
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Expected O, but got Unknown
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Expected O, but got Unknown
			((GuiWidget)this).set_Padding(new BorderDouble(3.0));
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)this).AnchorAll();
			textImageButtonFactory.borderWidth = 0.0;
			editButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			editButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			editButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_TabLabelUnselected();
			editButtonFactory.disabledFillColor = default(RGBA_Bytes);
			editButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			editButtonFactory.borderWidth = 0.0;
			editButtonFactory.Margin = new BorderDouble(10.0, 0.0);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			enterEditModeButton = editButtonFactory.Generate("Edit".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)enterEditModeButton).add_Click((EventHandler<MouseEventArgs>)enterEditModeButtonClick);
			leaveEditModeButton = editButtonFactory.Generate("Done".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)leaveEditModeButton).add_Click((EventHandler<MouseEventArgs>)leaveEditModeButtonClick);
			if (((GuiWidget)leaveEditModeButton).get_Width() < ((GuiWidget)enterEditModeButton).get_Width())
			{
				editButtonFactory.FixedWidth = ((GuiWidget)enterEditModeButton).get_Width();
				leaveEditModeButton = editButtonFactory.Generate("Done".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
				((GuiWidget)leaveEditModeButton).add_Click((EventHandler<MouseEventArgs>)leaveEditModeButtonClick);
			}
			else
			{
				editButtonFactory.FixedWidth = ((GuiWidget)leaveEditModeButton).get_Width();
				enterEditModeButton = editButtonFactory.Generate("Edit".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
				((GuiWidget)enterEditModeButton).add_Click((EventHandler<MouseEventArgs>)enterEditModeButtonClick);
			}
			((GuiWidget)enterEditModeButton).set_Name("Library Edit Button");
			((GuiWidget)leaveEditModeButton).set_Visible(false);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0));
			((GuiWidget)val2).set_BackgroundColor(ActiveTheme.get_Instance().get_TransparentLightOverlay());
			navigationLabel = new TextWidget("My Library".Localize(), 0.0, 0.0, 14.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)navigationLabel).set_VAnchor((VAnchor)2);
			navigationLabel.set_TextColor(ActiveTheme.get_Instance().get_PrimaryTextColor());
			((GuiWidget)val2).AddChild(new GuiWidget(50.0, 0.0, (SizeLimitsToSet)1), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)navigationLabel, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			buttonPanel = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)buttonPanel).set_HAnchor((HAnchor)5);
			((GuiWidget)buttonPanel).set_Padding(new BorderDouble(0.0, 3.0));
			((GuiWidget)buttonPanel).set_MinimumSize(new Vector2(0.0, 46.0));
			AddLibraryButtonElements();
			((GuiWidget)val).AddChild(CreateSearchPannel(), -1);
			libraryDataView = new LibraryDataView();
			breadCrumbWidget = new FolderBreadCrumbWidget(libraryDataView.SetCurrentLibraryProvider, libraryDataView.CurrentLibraryProvider);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			FlowLayoutWidget val4 = val3;
			((GuiWidget)val4).AddChild((GuiWidget)(object)breadCrumbWidget, -1);
			libraryDataView.ChangedCurrentLibraryProvider += breadCrumbWidget.SetBreadCrumbs;
			libraryDataView.ChangedCurrentLibraryProvider += LibraryProviderChanged;
			FlowLayoutWidget val5 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val5).set_HAnchor((HAnchor)5);
			breadCrumbAndActionBar = val5;
			((GuiWidget)breadCrumbAndActionBar).AddChild((GuiWidget)(object)val4, -1);
			((GuiWidget)breadCrumbAndActionBar).AddChild(CreateActionsMenu(), -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)breadCrumbAndActionBar, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)libraryDataView, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)buttonPanel, -1);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
		}

		private GuiWidget CreateSearchPannel()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Expected O, but got Unknown
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Expected O, but got Unknown
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val).set_BackgroundColor(ActiveTheme.get_Instance().get_TransparentDarkOverlay());
			((GuiWidget)val).set_HAnchor((HAnchor)5);
			((GuiWidget)val).set_Padding(new BorderDouble(0.0));
			searchInput = new MHTextEditWidget("", 0.0, 0.0, 12.0, 0.0, 0.0, multiLine: false, 0, "Search Library".Localize());
			((GuiWidget)searchInput).set_Name("Search Library Edit");
			((GuiWidget)searchInput).set_Margin(new BorderDouble(0.0, 3.0, 0.0, 0.0));
			((GuiWidget)searchInput).set_HAnchor((HAnchor)5);
			((GuiWidget)searchInput).set_VAnchor((VAnchor)2);
			searchInput.ActualTextEditWidget.add_EnterPressed(new KeyEventHandler(searchInputEnterPressed));
			double fixedWidth = editButtonFactory.FixedWidth;
			editButtonFactory.FixedWidth = 0.0;
			Button val2 = editButtonFactory.Generate("Search".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)val2).set_Name("Search Library Button");
			((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)searchButtonClick);
			editButtonFactory.FixedWidth = fixedWidth;
			((GuiWidget)val).AddChild((GuiWidget)(object)enterEditModeButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)leaveEditModeButton, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)searchInput, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			return (GuiWidget)val;
		}

		private GuiWidget CreateActionsMenu()
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			DropDownMenu dropDownMenu = new DropDownMenu("Actions".Localize() + "... ", (Direction)1);
			((Menu)dropDownMenu).set_AlignToRightEdge(true);
			dropDownMenu.NormalColor = default(RGBA_Bytes);
			dropDownMenu.BorderWidth = 1;
			dropDownMenu.BorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_SecondaryTextColor(), 100);
			dropDownMenu.MenuAsWideAsItems = false;
			((GuiWidget)dropDownMenu).set_VAnchor((VAnchor)5);
			((GuiWidget)dropDownMenu).set_Margin(new BorderDouble(3.0));
			((GuiWidget)dropDownMenu).set_Padding(new BorderDouble(10.0));
			((GuiWidget)dropDownMenu).set_Name("LibraryActionMenu");
			CreateActionMenuItems(dropDownMenu);
			return (GuiWidget)(object)dropDownMenu;
		}

		private void LibraryProviderChanged(LibraryProvider previousLibraryProvider, LibraryProvider currentLibraryProvider)
		{
			if (currentLibraryProvider.IsProtected())
			{
				((GuiWidget)addToLibraryButton).set_Enabled(false);
				((GuiWidget)createFolderButton).set_Enabled(false);
				DoLeaveEditMode();
			}
			else
			{
				((GuiWidget)addToLibraryButton).set_Enabled(true);
				((GuiWidget)createFolderButton).set_Enabled(true);
			}
			if (previousLibraryProvider != null)
			{
				previousLibraryProvider.KeywordFilter = "";
				previousLibraryProvider.DataReloaded -= UpdateStatus;
			}
			((GuiWidget)searchInput).set_Text(currentLibraryProvider.KeywordFilter);
			breadCrumbWidget.SetBreadCrumbs(null, libraryDataView.CurrentLibraryProvider);
			currentLibraryProvider.DataReloaded += UpdateStatus;
			UpdateStatus(null, null);
		}

		private void UpdateStatus(object sender, EventArgs e)
		{
			if (libraryDataView.CurrentLibraryProvider != null)
			{
				ProviderMessage = libraryDataView.CurrentLibraryProvider.StatusMessage;
			}
		}

		private void AddLibraryButtonElements()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Expected O, but got Unknown
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Expected O, but got Unknown
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_TabLabelUnselected();
			textImageButtonFactory.disabledFillColor = default(RGBA_Bytes);
			((GuiWidget)buttonPanel).RemoveAllChildren();
			addToLibraryButton = textImageButtonFactory.Generate("Add".Localize(), "icon_circle_plus.png");
			((GuiWidget)addToLibraryButton).set_Enabled(false);
			((GuiWidget)addToLibraryButton).set_ToolTipText("Add an .stl, .amf, .gcode or .zip file to the Library".Localize());
			((GuiWidget)addToLibraryButton).set_Name("Library Add Button");
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)addToLibraryButton, -1);
			((GuiWidget)addToLibraryButton).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
			((GuiWidget)addToLibraryButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				UiThread.RunOnIdle((Action)importToLibraryloadFile_ClickOnIdle);
			});
			createFolderButton = textImageButtonFactory.Generate("Create Folder".Localize());
			((GuiWidget)createFolderButton).set_Enabled(false);
			((GuiWidget)createFolderButton).set_Name("Create Folder From Library Button");
			((GuiWidget)buttonPanel).AddChild((GuiWidget)(object)createFolderButton, -1);
			((GuiWidget)createFolderButton).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
			((GuiWidget)createFolderButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				if (createFolderWindow == null)
				{
					createFolderWindow = new CreateFolderWindow(delegate(CreateFolderWindow.CreateFolderReturnInfo returnInfo)
					{
						libraryDataView.CurrentLibraryProvider.AddCollectionToLibrary(returnInfo.newName);
					});
					((GuiWidget)createFolderWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
					{
						createFolderWindow = null;
					});
				}
				else
				{
					((GuiWidget)createFolderWindow).BringToFront();
				}
			});
			TextWidget val = new TextWidget("", 0.0, 0.0, 12.0, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			val.set_PointSize(8.0);
			((GuiWidget)val).set_HAnchor((HAnchor)4);
			((GuiWidget)val).set_VAnchor((VAnchor)1);
			val.set_TextColor(ActiveTheme.get_Instance().get_SecondaryTextColor());
			((GuiWidget)val).set_Margin(new BorderDouble(6.0));
			val.set_AutoExpandBoundsToText(true);
			providerMessageWidget = val;
			GuiWidget val2 = new GuiWidget();
			val2.set_VAnchor((VAnchor)12);
			val2.set_HAnchor((HAnchor)5);
			val2.set_Visible(false);
			providerMessageContainer = val2;
			providerMessageContainer.AddChild((GuiWidget)(object)providerMessageWidget, -1);
			((GuiWidget)buttonPanel).AddChild(providerMessageContainer, -1);
		}

		private void CreateActionMenuItems(DropDownMenu actionMenu)
		{
			actionMenu.SelectionChanged += delegate(object sender, EventArgs e)
			{
				string selectedValue = ((DropDownMenu)sender).SelectedValue;
				foreach (PrintItemAction menuItem in menuItems)
				{
					if (menuItem.Title == selectedValue)
					{
						menuItem.Action?.Invoke(null, null);
					}
				}
			};
			menuItems.Add(new PrintItemAction
			{
				Title = "Edit".Localize(),
				Action = delegate(IEnumerable<QueueRowItem> s, QueueDataWidget e)
				{
					editButton_Click(s, null);
				}
			});
			actionMenuEnableData.Add(new MenuEnableData(actionMenu.AddItem(menuItems[menuItems.Count - 1].Title), multipleItems: false, protectedItems: false, collectionItems: false));
			((Menu)actionMenu).AddHorizontalLine();
			menuItems.Add(new PrintItemAction
			{
				Title = "Rename".Localize(),
				Action = delegate(IEnumerable<QueueRowItem> s, QueueDataWidget e)
				{
					renameFromLibraryButton_Click(s, null);
				}
			});
			actionMenuEnableData.Add(new MenuEnableData(actionMenu.AddItem(menuItems[menuItems.Count - 1].Title), multipleItems: false, protectedItems: false, collectionItems: true));
			menuItems.Add(new PrintItemAction
			{
				Title = "Move".Localize(),
				Action = delegate(IEnumerable<QueueRowItem> s, QueueDataWidget e)
				{
					moveInLibraryButton_Click(s, null);
				}
			});
			menuItems.Add(new PrintItemAction
			{
				Title = "Remove".Localize(),
				Action = delegate(IEnumerable<QueueRowItem> s, QueueDataWidget e)
				{
					deleteFromLibraryButton_Click(s, null);
				}
			});
			actionMenuEnableData.Add(new MenuEnableData(actionMenu.AddItem(menuItems[menuItems.Count - 1].Title), multipleItems: true, protectedItems: false, collectionItems: true));
			((Menu)actionMenu).AddHorizontalLine();
			menuItems.Add(new PrintItemAction
			{
				Title = "Add to Queue".Localize(),
				Action = delegate(IEnumerable<QueueRowItem> s, QueueDataWidget e)
				{
					addToQueueButton_Click(s, null);
				}
			});
			actionMenuEnableData.Add(new MenuEnableData(actionMenu.AddItem(menuItems[menuItems.Count - 1].Title), multipleItems: true, protectedItems: true, collectionItems: false));
			menuItems.Add(new PrintItemAction
			{
				Title = "Export".Localize(),
				Action = delegate(IEnumerable<QueueRowItem> s, QueueDataWidget e)
				{
					exportButton_Click(s, null);
				}
			});
			actionMenuEnableData.Add(new MenuEnableData(actionMenu.AddItem(menuItems[menuItems.Count - 1].Title), multipleItems: false, protectedItems: true, collectionItems: false));
			menuItems.Add(new PrintItemAction
			{
				Title = "Share".Localize(),
				Action = delegate(IEnumerable<QueueRowItem> s, QueueDataWidget e)
				{
					shareFromLibraryButton_Click(s, null);
				}
			});
			actionMenuEnableData.Add(new MenuEnableData(actionMenu.AddItem(menuItems[menuItems.Count - 1].Title), multipleItems: false, protectedItems: false, collectionItems: false, shareItems: true));
			SetActionMenuStates();
		}

		private void renameFromLibraryButton_Click(IEnumerable<QueueRowItem> s, object p)
		{
			if (libraryDataView.SelectedItems.Count != 1)
			{
				return;
			}
			if (renameItemWindow == null)
			{
				LibraryRowItem libraryRowItem = libraryDataView.SelectedItems[0];
				LibraryRowItemPart partItem = libraryRowItem as LibraryRowItemPart;
				LibraryRowItemCollection collectionItem = libraryRowItem as LibraryRowItemCollection;
				renameItemWindow = new RenameItemWindow(libraryDataView.SelectedItems[0].ItemName, delegate(RenameItemWindow.RenameItemReturnInfo returnInfo)
				{
					if (partItem != null)
					{
						libraryDataView.CurrentLibraryProvider.RenameItem(partItem.ItemIndex, returnInfo.newName);
					}
					else if (collectionItem != null)
					{
						libraryDataView.CurrentLibraryProvider.RenameCollection(collectionItem.CollectionIndex, returnInfo.newName);
					}
					libraryDataView.ClearSelectedItems();
				});
				((GuiWidget)renameItemWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					renameItemWindow = null;
				});
			}
			else
			{
				((GuiWidget)renameItemWindow).BringToFront();
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (libraryDataView != null && libraryDataView.CurrentLibraryProvider != null)
			{
				libraryDataView.CurrentLibraryProvider.DataReloaded -= UpdateStatus;
			}
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		private void searchInputEnterPressed(object sender, KeyEventArgs keyEvent)
		{
			searchButtonClick(null, null);
		}

		private void enterEditModeButtonClick(object sender, EventArgs e)
		{
			((GuiWidget)breadCrumbWidget).set_Visible(false);
			((GuiWidget)enterEditModeButton).set_Visible(false);
			((GuiWidget)leaveEditModeButton).set_Visible(true);
			libraryDataView.EditMode = true;
		}

		private void leaveEditModeButtonClick(object sender, EventArgs e)
		{
			DoLeaveEditMode();
		}

		private void DoLeaveEditMode()
		{
			((GuiWidget)breadCrumbWidget).set_Visible(true);
			((GuiWidget)enterEditModeButton).set_Visible(true);
			((GuiWidget)leaveEditModeButton).set_Visible(false);
			libraryDataView.EditMode = false;
		}

		private void searchButtonClick(object sender, EventArgs e)
		{
			UiThread.RunOnIdle((Action)delegate
			{
				string keywordFilter = ((GuiWidget)searchInput).get_Text().Trim();
				libraryDataView.CurrentLibraryProvider.KeywordFilter = keywordFilter;
				breadCrumbWidget.SetBreadCrumbs(null, libraryDataView.CurrentLibraryProvider);
			});
		}

		private void addToQueueButton_Click(object sender, EventArgs e)
		{
			foreach (LibraryRowItem selectedItem in libraryDataView.SelectedItems)
			{
				selectedItem.AddToQueue();
			}
			libraryDataView.ClearSelectedItems();
		}

		private void onLibraryItemsSelected(object sender, EventArgs e)
		{
			SetActionMenuStates();
		}

		private void SetActionMenuStates()
		{
			int count = libraryDataView.SelectedItems.Count;
			for (int i = 0; i < actionMenuEnableData.Count; i++)
			{
				bool enabled = count > 0;
				if (count > 1 && !actionMenuEnableData[i].multipleItems)
				{
					enabled = false;
				}
				else
				{
					if (!actionMenuEnableData[i].protectedItems)
					{
						for (int j = 0; j < libraryDataView.SelectedItems.Count; j++)
						{
							if (libraryDataView.SelectedItems[j].Protected)
							{
								enabled = false;
							}
						}
					}
					if (!actionMenuEnableData[i].collectionItems)
					{
						for (int k = 0; k < libraryDataView.SelectedItems.Count; k++)
						{
							if (libraryDataView.SelectedItems[k] is LibraryRowItemCollection)
							{
								enabled = false;
							}
						}
					}
				}
				if (actionMenuEnableData[i].shareItems && libraryDataView?.CurrentLibraryProvider != null && !libraryDataView.CurrentLibraryProvider.CanShare)
				{
					enabled = false;
				}
				((GuiWidget)actionMenuEnableData[i].menuItemToChange).set_Enabled(enabled);
			}
		}

		public int SortRowItemsOnIndex(LibraryRowItem x, LibraryRowItem y)
		{
			int itemIndex = libraryDataView.GetItemIndex(x);
			int itemIndex2 = libraryDataView.GetItemIndex(y);
			return itemIndex.CompareTo(itemIndex2);
		}

		private void deleteFromLibraryButton_Click(object sender, EventArgs e)
		{
			libraryDataView.SelectedItems.Sort(new Comparison<LibraryRowItem>(SortRowItemsOnIndex));
			IEnumerable<LibraryRowItem> enumerable = Enumerable.Where<LibraryRowItem>((IEnumerable<LibraryRowItem>)libraryDataView.SelectedItems, (Func<LibraryRowItem, bool>)((LibraryRowItem item) => item is LibraryRowItemPart));
			if (Enumerable.Count<LibraryRowItem>(enumerable) == libraryDataView.SelectedItems.Count)
			{
				int[] indexes = Enumerable.ToArray<int>(Enumerable.Select<LibraryRowItemPart, int>(Enumerable.Cast<LibraryRowItemPart>((IEnumerable)enumerable), (Func<LibraryRowItemPart, int>)((LibraryRowItemPart l) => l.ItemIndex)));
				libraryDataView.CurrentLibraryProvider.RemoveItems(indexes);
			}
			else
			{
				for (int num = libraryDataView.SelectedItems.Count - 1; num >= 0; num--)
				{
					libraryDataView.SelectedItems[num].RemoveFromCollection();
				}
			}
			libraryDataView.ClearSelectedItems();
		}

		private void moveInLibraryButton_Click(object sender, EventArgs e)
		{
			libraryDataView.SelectedItems.Sort(new Comparison<LibraryRowItem>(SortRowItemsOnIndex));
			IEnumerable<LibraryRowItem> enumerable = Enumerable.Where<LibraryRowItem>((IEnumerable<LibraryRowItem>)libraryDataView.SelectedItems, (Func<LibraryRowItem, bool>)((LibraryRowItem item) => item is LibraryRowItemPart));
			if (Enumerable.Count<LibraryRowItem>(enumerable) > 0)
			{
				int[] indexes = Enumerable.ToArray<int>(Enumerable.Select<LibraryRowItemPart, int>(Enumerable.Cast<LibraryRowItemPart>((IEnumerable)enumerable), (Func<LibraryRowItemPart, int>)((LibraryRowItemPart l) => l.ItemIndex)));
				libraryDataView.CurrentLibraryProvider.MoveItems(indexes);
			}
			libraryDataView.ClearSelectedItems();
		}

		private void shareFromLibraryButton_Click(object sender, EventArgs e)
		{
			if (libraryDataView.SelectedItems.Count == 1)
			{
				LibraryRowItemPart libraryRowItemPart = libraryDataView.SelectedItems[0] as LibraryRowItemPart;
				if (libraryRowItemPart != null)
				{
					libraryDataView.CurrentLibraryProvider.ShareItem(libraryRowItemPart.ItemIndex);
				}
			}
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			if (libraryDataView.SelectedItems.Count == 1)
			{
				libraryDataView.SelectedItems[0].Export();
			}
		}

		private void editButton_Click(object sender, EventArgs e)
		{
			if (libraryDataView.SelectedItems.Count == 1)
			{
				libraryDataView.SelectedItems[0].Edit();
			}
		}

		public override void OnDragEnter(FileDropEventArgs fileDropEventArgs)
		{
			if (libraryDataView != null && libraryDataView.CurrentLibraryProvider != null && !libraryDataView.CurrentLibraryProvider.IsProtected())
			{
				foreach (string droppedFile in fileDropEventArgs.DroppedFiles)
				{
					string text = Path.GetExtension(droppedFile)!.ToUpper();
					if ((text != "" && MeshFileIo.ValidFileExtensions().Contains(text)) || text == ".GCODE" || text == ".ZIP")
					{
						fileDropEventArgs.set_AcceptDrop(true);
					}
				}
			}
			((GuiWidget)this).OnDragEnter(fileDropEventArgs);
		}

		public override void OnDragOver(FileDropEventArgs fileDropEventArgs)
		{
			if (libraryDataView != null && libraryDataView.CurrentLibraryProvider != null && !libraryDataView.CurrentLibraryProvider.IsProtected())
			{
				foreach (string droppedFile in fileDropEventArgs.DroppedFiles)
				{
					string text = Path.GetExtension(droppedFile)!.ToUpper();
					if ((text != "" && MeshFileIo.ValidFileExtensions().Contains(text)) || text == ".GCODE" || text == ".ZIP")
					{
						fileDropEventArgs.set_AcceptDrop(true);
						break;
					}
				}
			}
			((GuiWidget)this).OnDragOver(fileDropEventArgs);
		}

		public override void OnDragDrop(FileDropEventArgs fileDropEventArgs)
		{
			if (libraryDataView != null && libraryDataView.CurrentLibraryProvider != null && !libraryDataView.CurrentLibraryProvider.IsProtected())
			{
				libraryDataView.CurrentLibraryProvider.AddFilesToLibrary(fileDropEventArgs.DroppedFiles);
			}
			((GuiWidget)this).OnDragDrop(fileDropEventArgs);
		}

		private void importToLibraryloadFile_ClickOnIdle()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			FileDialog.OpenFileDialog(new OpenFileDialogParams(ApplicationSettings.OpenPrintableFileParams, "", true, "", ""), (Action<OpenFileDialogParams>)onLibraryLoadFileSelected);
		}

		private void onLibraryLoadFileSelected(OpenFileDialogParams openParams)
		{
			if (((FileDialogParams)openParams).get_FileNames() != null)
			{
				libraryDataView.CurrentLibraryProvider.AddFilesToLibrary(((FileDialogParams)openParams).get_FileNames());
			}
		}
	}
}
