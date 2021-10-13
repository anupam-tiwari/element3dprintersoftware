using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CreatorPlugins;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintLibrary.Provider;
using MatterHackers.MatterControl.Queue.OptionsMenu;
using MatterHackers.MatterControl.SettingsManagement;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PrintQueue
{
	public class QueueDataWidget : GuiWidget
	{
		public delegate void SendButtonAction(object state, List<PrintItemWrapper> sendItems);

		private class PartToAddToQueue
		{
			internal PrintItem PrintItem
			{
				get;
				set;
			}

			internal int InsertAfterIndex
			{
				get;
				set;
			}
		}

		public static SendButtonAction sendButtonFunction;

		private static Button shopButton;

		private Button addToQueueButton;

		private Button createButton;

		private TextImageButtonFactory editButtonFactory = new TextImageButtonFactory();

		private FlowLayoutWidget itemOperationButtons;

		private List<ButtonEnableData> editButtonsEnableData = new List<ButtonEnableData>();

		private Button enterEditModeButton;

		private ExportPrintItemWindow exportingWindow;

		private bool exportingWindowIsOpen;

		private Button leaveEditModeButton;

		private Button completeEditModeButton;

		private Action<IEnumerable<QueueRowItem>, QueueDataWidget> mergeAction;

		private List<PrintItemAction> menuItems;

		private HashSet<string> singleSelectionMenuItems = new HashSet<string>();

		private HashSet<string> multiSelectionMenuItems = new HashSet<string>();

		private PluginChooserWindow pluginChooserWindow;

		private QueueDataView queueDataView;

		private QueueOptionsMenu queueMenu;

		private FlowLayoutWidget queueMenuContainer;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		private EventHandler unregisterEvents;

		private string pleaseSelectPrinterMessage = "Before you can export printable files, you must select a printer.";

		private string pleaseSelectPrinterTitle = "Please select a printer";

		public QueueDataWidget(QueueDataView queueDataView)
			: this()
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Expected O, but got Unknown
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Expected O, but got Unknown
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Expected O, but got Unknown
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Expected O, but got Unknown
			this.queueDataView = queueDataView;
			MyMenuExtension myMenuExtension = new MyMenuExtension();
			mergeAction = Enumerable.First<PrintItemAction>(myMenuExtension.GetMenuItems()).Action;
			SetDisplayAttributes();
			textImageButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			textImageButtonFactory.borderWidth = 0.0;
			editButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			editButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			editButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_TabLabelUnselected();
			editButtonFactory.disabledFillColor = default(RGBA_Bytes);
			editButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			editButtonFactory.borderWidth = 0.0;
			editButtonFactory.Margin = new BorderDouble(10.0, 0.0);
			FlowLayoutWidget val = new FlowLayoutWidget((FlowDirection)3);
			enterEditModeButton = editButtonFactory.Generate("Combine".Localize() + "...", (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)enterEditModeButton).set_ToolTipText("Enter merge mode".Localize());
			((GuiWidget)enterEditModeButton).add_Click((EventHandler<MouseEventArgs>)enterEditModeButtonClick);
			leaveEditModeButton = editButtonFactory.Generate("Cancel".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)leaveEditModeButton).set_Name("Queue Done Button");
			((GuiWidget)leaveEditModeButton).add_Click((EventHandler<MouseEventArgs>)leaveEditModeButtonClick);
			completeEditModeButton = editButtonFactory.Generate("Complete".Localize(), (string)null, (string)null, (string)null, (string)null, centerText: true);
			((GuiWidget)completeEditModeButton).set_Name("Edit Complete Merge Button");
			((GuiWidget)completeEditModeButton).add_Click((EventHandler<MouseEventArgs>)editModeCompleteMergeButtonClick);
			multiSelectionMenuItems.Add("Merge".Localize() + "...");
			CreateEditBarButtons();
			((GuiWidget)leaveEditModeButton).set_Visible(false);
			FlowLayoutWidget val2 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val2).set_BackgroundColor(ActiveTheme.get_Instance().get_TransparentDarkOverlay());
			((GuiWidget)val2).set_HAnchor((HAnchor)5);
			((GuiWidget)val2).set_Padding(new BorderDouble(0.0));
			((GuiWidget)val2).AddChild((GuiWidget)(object)enterEditModeButton, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)leaveEditModeButton, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)completeEditModeButton, -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)val2).AddChild((GuiWidget)(object)itemOperationButtons, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val2, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)queueDataView, -1);
			FlowLayoutWidget val3 = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)val3).set_HAnchor((HAnchor)5);
			((GuiWidget)val3).set_Padding(new BorderDouble(0.0, 3.0));
			((GuiWidget)val3).set_MinimumSize(new Vector2(0.0, 46.0));
			addToQueueButton = textImageButtonFactory.Generate("Add".Localize(), StaticData.get_Instance().LoadIcon("icon_plus.png", 32, 32));
			((GuiWidget)addToQueueButton).set_ToolTipText("Add an .stl, .amf, .gcode or .zip file to the Queue".Localize());
			((GuiWidget)val3).AddChild((GuiWidget)(object)addToQueueButton, -1);
			((GuiWidget)addToQueueButton).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
			((GuiWidget)addToQueueButton).add_Click((EventHandler<MouseEventArgs>)addToQueueButton_Click);
			((GuiWidget)addToQueueButton).set_Name("Queue Add Button");
			createButton = textImageButtonFactory.Generate("Create".Localize(), StaticData.get_Instance().LoadIcon("icon_creator.png", 32, 32));
			((GuiWidget)createButton).set_ToolTipText("Choose a Create Tool to generate custom designs".Localize());
			((GuiWidget)createButton).set_Name("Design Tool Button");
			((GuiWidget)val3).AddChild((GuiWidget)(object)createButton, -1);
			((GuiWidget)createButton).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
			((GuiWidget)createButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				OpenPluginChooserWindow();
			});
			_ = UserSettings.Instance.IsTouchScreen;
			if (OemSettings.Instance.ShowShopButton)
			{
				shopButton = textImageButtonFactory.Generate("Buy Materials".Localize(), StaticData.get_Instance().LoadIcon("icon_shopping_cart_32x32.png", 32, 32));
				((GuiWidget)shopButton).set_ToolTipText("Shop online for printing materials".Localize());
				((GuiWidget)shopButton).set_Name("Buy Materials Button");
				((GuiWidget)val3).AddChild((GuiWidget)(object)shopButton, -1);
				((GuiWidget)shopButton).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
				((GuiWidget)shopButton).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					double num = 0.0;
					if (ActiveSliceSettings.Instance.PrinterSelected)
					{
						num = 3.0;
						if (ActiveSliceSettings.Instance.GetValue<double>("filament_diameter") < 2.0)
						{
							num = 1.75;
						}
					}
					MatterControlApplication.Instance.LaunchBrowser(StringHelper.FormatWith("http://www.matterhackers.com/mc/store/redirect?d={0}&clk=mcs&a={1}", new object[2]
					{
						num,
						OemSettings.Instance.AffiliateCode
					}));
				});
			}
			((GuiWidget)val3).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			queueMenuContainer = new FlowLayoutWidget((FlowDirection)0);
			((GuiWidget)queueMenuContainer).set_VAnchor((VAnchor)5);
			queueMenu = new QueueOptionsMenu();
			((GuiWidget)queueMenuContainer).AddChild((GuiWidget)(object)queueMenu.MenuDropList, -1);
			((GuiWidget)val3).AddChild((GuiWidget)(object)queueMenuContainer, -1);
			((GuiWidget)val).AddChild((GuiWidget)(object)val3, -1);
			((GuiWidget)val).AnchorAll();
			((GuiWidget)this).AddChild((GuiWidget)(object)val, -1);
			QueueData.Instance.SelectedIndexChanged.RegisterEvent((EventHandler)delegate
			{
				SetEditButtonsStates();
			}, ref unregisterEvents);
			SetEditButtonsStates();
		}

		private void CreateEditBarButtons()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			itemOperationButtons = new FlowLayoutWidget((FlowDirection)0);
			double fixedWidth = editButtonFactory.FixedWidth;
			editButtonFactory.FixedWidth = 0.0;
			Button val = editButtonFactory.Generate("Export".Localize());
			((GuiWidget)val).set_Name("Queue Export Button");
			((GuiWidget)val).set_Margin(new BorderDouble(3.0, 0.0));
			((GuiWidget)val).add_Click((EventHandler<MouseEventArgs>)exportButton_Click);
			editButtonsEnableData.Add(new ButtonEnableData(multipleItems: false, protectedItems: false, collectionItems: false));
			((GuiWidget)itemOperationButtons).AddChild((GuiWidget)(object)val, -1);
			Button val2 = editButtonFactory.Generate("Copy".Localize());
			((GuiWidget)val2).set_Name("Queue Copy Button");
			((GuiWidget)val2).set_Margin(new BorderDouble(3.0, 0.0));
			((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)copyButton_Click);
			editButtonsEnableData.Add(new ButtonEnableData(multipleItems: false, protectedItems: true, collectionItems: false));
			((GuiWidget)itemOperationButtons).AddChild((GuiWidget)(object)val2, -1);
			Button val3 = editButtonFactory.Generate("Remove".Localize());
			((GuiWidget)val3).set_Name("Queue Remove Button");
			((GuiWidget)val3).set_Margin(new BorderDouble(3.0, 0.0));
			((GuiWidget)val3).add_Click((EventHandler<MouseEventArgs>)removeButton_Click);
			editButtonsEnableData.Add(new ButtonEnableData(multipleItems: true, protectedItems: true, collectionItems: true));
			((GuiWidget)itemOperationButtons).AddChild((GuiWidget)(object)val3, -1);
			Button val4 = editButtonFactory.Generate("Add to Library".Localize());
			((GuiWidget)val4).set_Name("Library Add Button");
			((GuiWidget)val4).set_Margin(new BorderDouble(3.0, 0.0));
			((GuiWidget)val4).add_Click((EventHandler<MouseEventArgs>)addToLibraryButton_Click);
			editButtonsEnableData.Add(new ButtonEnableData(multipleItems: true, protectedItems: true, collectionItems: true));
			((GuiWidget)itemOperationButtons).AddChild((GuiWidget)(object)val4, -1);
			editButtonFactory.FixedWidth = fixedWidth;
		}

		public void CreateCopyInQueue()
		{
			if (QueueData.Instance.SelectedCount != 1)
			{
				return;
			}
			QueueRowItem queueRowItem = queueDataView.GetQueueRowItem(QueueData.Instance.SelectedIndex);
			if (queueRowItem == null)
			{
				return;
			}
			PrintItemWrapper printItemWrapper = queueRowItem.PrintItemWrapper;
			int index = QueueData.Instance.GetIndex(printItemWrapper);
			if (index != -1 && File.Exists(printItemWrapper.FileLocation))
			{
				string applicationLibraryDataPath = ApplicationDataStorage.Instance.ApplicationLibraryDataPath;
				if (!Directory.Exists(applicationLibraryDataPath))
				{
					Directory.CreateDirectory(applicationLibraryDataPath);
				}
				int num = 0;
				string path;
				do
				{
					path = Path.Combine(applicationLibraryDataPath, Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(printItemWrapper.FileLocation)));
					path = Path.GetFullPath(path);
					num++;
				}
				while (File.Exists(path) && num < 100);
				File.Copy(printItemWrapper.FileLocation, path);
				string name = printItemWrapper.Name;
				name = (name.Contains(" - copy") ? (name[..name.LastIndexOf(" - copy")] + " - copy") : (name + " - copy"));
				int num2 = 2;
				string text = name;
				string[] itemNames = QueueData.Instance.GetItemNames();
				while (Enumerable.Contains<string>((IEnumerable<string>)itemNames, text))
				{
					text = StringHelper.FormatWith("{0} {1}", new object[2]
					{
						name,
						num2
					});
					num2++;
				}
				name = text;
				PrintItem printItem = new PrintItem();
				printItem.Name = name;
				printItem.FileLocation = path;
				printItem.ReadOnly = printItemWrapper.PrintItem.ReadOnly;
				printItem.Protected = printItemWrapper.PrintItem.Protected;
				UiThread.RunOnIdle((Action<object>)AddPartCopyToQueue, (object)new PartToAddToQueue
				{
					PrintItem = printItem,
					InsertAfterIndex = index + 1
				}, 0.0);
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((GuiWidget)this).OnClosed(e);
		}

		public override void OnDragDrop(FileDropEventArgs fileDropEventArgs)
		{
			DoAddFiles(fileDropEventArgs.DroppedFiles);
			((GuiWidget)this).OnDragDrop(fileDropEventArgs);
		}

		public static void DoAddFiles(List<string> files)
		{
			int itemCount = QueueData.Instance.ItemCount;
			foreach (string file in files)
			{
				string text = Path.GetExtension(file)!.ToUpper();
				if ((text != "" && MeshFileIo.ValidFileExtensions().Contains(text)) || text == ".GCODE")
				{
					QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(Path.GetFileNameWithoutExtension(file), Path.GetFullPath(file))));
				}
				else
				{
					if (!(text == ".ZIP"))
					{
						continue;
					}
					List<PrintItem> list = new ProjectFileHandler(null).ImportFromProjectArchive(file);
					if (list == null)
					{
						continue;
					}
					foreach (PrintItem item in list)
					{
						QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(item.Name, item.FileLocation)));
					}
				}
			}
			if (QueueData.Instance.ItemCount != itemCount)
			{
				QueueData.Instance.SelectedIndex = QueueData.Instance.ItemCount - 1;
			}
		}

		public override void OnDragEnter(FileDropEventArgs fileDropEventArgs)
		{
			foreach (string droppedFile in fileDropEventArgs.DroppedFiles)
			{
				string text = Path.GetExtension(droppedFile)!.ToUpper();
				if ((text != "" && MeshFileIo.ValidFileExtensions().Contains(text)) || text == ".GCODE" || text == ".ZIP")
				{
					fileDropEventArgs.set_AcceptDrop(true);
				}
			}
			((GuiWidget)this).OnDragEnter(fileDropEventArgs);
		}

		public override void OnDragOver(FileDropEventArgs fileDropEventArgs)
		{
			foreach (string droppedFile in fileDropEventArgs.DroppedFiles)
			{
				string text = Path.GetExtension(droppedFile)!.ToUpper();
				if ((text != "" && MeshFileIo.ValidFileExtensions().Contains(text)) || text == ".GCODE" || text == ".ZIP")
				{
					fileDropEventArgs.set_AcceptDrop(true);
				}
			}
			((GuiWidget)this).OnDragOver(fileDropEventArgs);
		}

		private void AddItemsToQueue()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			OpenFileDialogParams val = new OpenFileDialogParams(ApplicationSettings.OpenPrintableFileParams, "", false, "", "");
			val.set_MultiSelect(true);
			((FileDialogParams)val).set_ActionButtonLabel("Add to Queue");
			((FileDialogParams)val).set_Title("Element: Select A File");
			FileDialog.OpenFileDialog(val, (Action<OpenFileDialogParams>)delegate(OpenFileDialogParams openParams)
			{
				if (((FileDialogParams)openParams).get_FileNames() != null)
				{
					int itemCount = QueueData.Instance.ItemCount;
					string[] fileNames = ((FileDialogParams)openParams).get_FileNames();
					foreach (string text in fileNames)
					{
						string text2 = Path.GetExtension(text)!.ToUpper();
						if (text2 == ".ZIP")
						{
							List<PrintItem> list = new ProjectFileHandler(null).ImportFromProjectArchive(text);
							if (list != null)
							{
								foreach (PrintItem item in list)
								{
									QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(item.Name, item.FileLocation)));
								}
							}
						}
						else if (text2 != "" && ApplicationSettings.OpenDesignFileParams.Contains(text2.ToLower()))
						{
							QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(Path.GetFileNameWithoutExtension(text), Path.GetFullPath(text))));
						}
					}
					if (QueueData.Instance.ItemCount != itemCount)
					{
						QueueData.Instance.SelectedIndex = QueueData.Instance.ItemCount - 1;
					}
				}
			});
		}

		private void AddPartCopyToQueue(object state)
		{
			PartToAddToQueue partToAddToQueue = state as PartToAddToQueue;
			QueueData.Instance.AddItem(new PrintItemWrapper(partToAddToQueue.PrintItem), partToAddToQueue.InsertAfterIndex, QueueData.ValidateSizeOn32BitSystems.Skip);
		}

		private void DoAddToSpecificLibrary(SaveAsWindow.SaveAsReturnInfo returnInfo)
		{
			if (returnInfo == null)
			{
				return;
			}
			LibraryProvider destinationLibraryProvider = returnInfo.destinationLibraryProvider;
			if (destinationLibraryProvider == null)
			{
				return;
			}
			foreach (int selectedIndex in QueueData.Instance.SelectedIndexes)
			{
				QueueRowItem queueRowItem = queueDataView.GetQueueRowItem(selectedIndex);
				if (queueRowItem != null && File.Exists(queueRowItem.PrintItemWrapper.FileLocation))
				{
					PrintItemWrapper itemToAdd = new PrintItemWrapper(new PrintItem(queueRowItem.PrintItemWrapper.PrintItem.Name, queueRowItem.PrintItemWrapper.FileLocation), returnInfo.destinationLibraryProvider.GetProviderLocator());
					destinationLibraryProvider.AddItem(itemToAdd);
				}
			}
			destinationLibraryProvider.Dispose();
		}

		private void addToLibraryButton_Click(object sender, EventArgs mouseEvent)
		{
			new SaveAsWindow(DoAddToSpecificLibrary, null, showQueue: false, getNewName: false);
		}

		private void addToQueueButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle((Action)AddItemsToQueue);
		}

		private void clearAllButton_Click(object sender, EventArgs mouseEvent)
		{
			QueueData.Instance.RemoveAll();
			LeaveEditMode();
		}

		private void copyButton_Click(object sender, EventArgs mouseEvent)
		{
			CreateCopyInQueue();
		}

		private void deleteAllFromQueueButton_Click(object sender, EventArgs mouseEvent)
		{
			QueueData.Instance.RemoveAll();
		}

		private void enterEditModeButtonClick(object sender, EventArgs mouseEvent)
		{
			((GuiWidget)enterEditModeButton).set_Visible(false);
			((GuiWidget)leaveEditModeButton).set_Visible(true);
			((GuiWidget)completeEditModeButton).set_Visible(true);
			queueDataView.EditMode = true;
			SetEditButtonsStates();
		}

		private void MustSelectPrinterMessage()
		{
			StyledMessageBox.ShowMessageBox(null, pleaseSelectPrinterMessage, pleaseSelectPrinterTitle);
		}

		private void exportButton_Click(object sender, EventArgs mouseEvent)
		{
			if (!ActiveSliceSettings.Instance.PrinterSelected)
			{
				UiThread.RunOnIdle((Action)MustSelectPrinterMessage);
			}
			else if (QueueData.Instance.SelectedCount == 1)
			{
				QueueRowItem queueRowItem = queueDataView.GetQueueRowItem(QueueData.Instance.SelectedIndex);
				if (queueRowItem != null)
				{
					OpenExportWindow(queueRowItem.PrintItemWrapper);
				}
			}
		}

		private void exportQueueButton_Click(object sender, EventArgs mouseEvent)
		{
			new ProjectFileHandler(QueueData.Instance.CreateReadOnlyPartList(includeProtectedItems: false)).SaveAs();
		}

		private void exportToSDProcess_UpdateRemainingItems(object sender, EventArgs e)
		{
			_ = (ExportToFolderProcess)sender;
		}

		private void importQueueButton_Click(object sender, EventArgs mouseEvent)
		{
			new ProjectFileHandler(null);
			throw new NotImplementedException();
		}

		private void ItemMenu_SelectionChanged(object sender, EventArgs e)
		{
			string selectedValue = ((DropDownMenu)sender).SelectedValue;
			foreach (PrintItemAction menuItem in menuItems)
			{
				if (menuItem.Title == selectedValue)
				{
					menuItem.Action?.Invoke(queueDataView.GetSelectedItems(), this);
				}
			}
		}

		public void LeaveEditMode()
		{
			if (queueDataView.EditMode)
			{
				((GuiWidget)enterEditModeButton).set_Visible(true);
				((GuiWidget)leaveEditModeButton).set_Visible(false);
				queueDataView.EditMode = false;
			}
		}

		private void leaveEditModeButtonClick(object sender, EventArgs mouseEvent)
		{
			LeaveEditMode();
			SetEditButtonsStates();
		}

		private void editModeCompleteMergeButtonClick(object sender, EventArgs eventArgs)
		{
			mergeAction(queueDataView.GetSelectedItems(), this);
			leaveEditModeButtonClick(this, null);
		}

		private void OpenExportWindow(PrintItemWrapper printItem)
		{
			if (!exportingWindowIsOpen)
			{
				exportingWindow = new ExportPrintItemWindow(printItem);
				exportingWindowIsOpen = true;
				((GuiWidget)exportingWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					exportingWindowIsOpen = false;
				});
				((SystemWindow)exportingWindow).ShowAsSystemWindow();
			}
			else if (exportingWindow != null)
			{
				((GuiWidget)exportingWindow).BringToFront();
			}
		}

		private void OpenPluginChooserWindow()
		{
			if (pluginChooserWindow == null)
			{
				pluginChooserWindow = new PluginChooserWindow();
				((GuiWidget)pluginChooserWindow).set_Name("Plugin Chooser Window");
				((GuiWidget)pluginChooserWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					pluginChooserWindow = null;
				});
			}
			else
			{
				((GuiWidget)pluginChooserWindow).BringToFront();
			}
		}

		private void removeButton_Click(object sender, EventArgs mouseEvent)
		{
			QueueData.Instance.RemoveSelected();
		}

		private void sendButton_Click(object sender, EventArgs mouseEvent)
		{
			if (sendButtonFunction != null)
			{
				List<PrintItemWrapper> itemList = Enumerable.ToList<PrintItemWrapper>(Enumerable.Select<QueueRowItem, PrintItemWrapper>((IEnumerable<QueueRowItem>)queueDataView.GetSelectedItems(), (Func<QueueRowItem, PrintItemWrapper>)((QueueRowItem item) => item.PrintItemWrapper)));
				UiThread.RunOnIdle((Action)delegate
				{
					sendButtonFunction(null, itemList);
				});
			}
			else
			{
				UiThread.RunOnIdle((Action)delegate
				{
					StyledMessageBox.ShowMessageBox(null, "Oops! Send is currently disabled.", "Send Print");
				});
			}
		}

		private void SetDisplayAttributes()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Padding(new BorderDouble(3.0));
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)this).AnchorAll();
		}

		private void SetMenuItems(DropDownMenu dropDownMenu)
		{
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			menuItems = new List<PrintItemAction>();
			menuItems.Add(new PrintItemAction
			{
				Title = "Send".Localize(),
				Action = delegate
				{
					sendButton_Click(null, null);
				}
			});
			menuItems.Add(new PrintItemAction
			{
				Title = "Add to Library".Localize(),
				Action = delegate
				{
					addToLibraryButton_Click(null, null);
				}
			});
			foreach (PrintItemMenuExtension plugin in new PluginFinder<PrintItemMenuExtension>((string)null, (IComparer<PrintItemMenuExtension>)null).Plugins)
			{
				foreach (PrintItemAction menuItem in plugin.GetMenuItems())
				{
					menuItems.Add(menuItem);
				}
			}
			BorderDouble menuItemsPadding = dropDownMenu.MenuItemsPadding;
			foreach (PrintItemAction menuItem2 in menuItems)
			{
				if (menuItem2.Action == null)
				{
					dropDownMenu.MenuItemsPadding = new BorderDouble(5.0, 0.0, menuItemsPadding.Right, 3.0);
				}
				else
				{
					if (menuItem2.SingleItemOnly)
					{
						singleSelectionMenuItems.Add(menuItem2.Title);
					}
					dropDownMenu.MenuItemsPadding = new BorderDouble(10.0, 5.0, menuItemsPadding.Right, 5.0);
				}
				dropDownMenu.AddItem(menuItem2.Title);
			}
			((GuiWidget)dropDownMenu).set_Padding(menuItemsPadding);
		}

		private void SetEditButtonsStates()
		{
			int selectedCount = QueueData.Instance.SelectedCount;
			for (int i = 0; i < ((Collection<GuiWidget>)(object)((GuiWidget)itemOperationButtons).get_Children()).Count; i++)
			{
				bool enabled = selectedCount > 0;
				GuiWidget obj = ((Collection<GuiWidget>)(object)((GuiWidget)itemOperationButtons).get_Children())[i];
				Button val = obj as Button;
				if (val != null)
				{
					if (selectedCount > 1 && !editButtonsEnableData[i].multipleItems)
					{
						enabled = false;
					}
					((GuiWidget)val).set_Enabled(enabled);
				}
			}
			((GuiWidget)completeEditModeButton).set_Visible(queueDataView.EditMode);
			((GuiWidget)completeEditModeButton).set_Enabled(selectedCount > 1);
		}
	}
}
