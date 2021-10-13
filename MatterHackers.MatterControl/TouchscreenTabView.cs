using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PartPreviewWindow;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintHistory;
using MatterHackers.MatterControl.PrintLibrary;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	internal class TouchscreenTabView : TabControl
	{
		public static int firstPanelCurrentTab = 0;

		private static readonly string CompactTabView_CurrentTab = "CompactTabView_CurrentTab";

		private static readonly string CompactTabView_Options_ScrollPosition = "CompactTabView_Options_ScrollPosition";

		private static int lastAdvanceControlsIndex = 0;

		private GuiWidget addedUpdateMark;

		private PartPreviewContent partPreviewContainer;

		private QueueDataView queueDataView;

		private TabPage QueueTabPage;

		private bool simpleMode;

		private GuiWidget sliceSettingsWidget;

		private int TabTextSize;

		private EventHandler unregisterEvents;

		public bool TouchScreenIsTall
		{
			get
			{
				foreach (SystemWindow item in ExtensionMethods.Parents<SystemWindow>((GuiWidget)(object)this))
				{
					if (((GuiWidget)item).get_Height() < 610.0)
					{
						return false;
					}
				}
				return true;
			}
		}

		public TouchscreenTabView(QueueDataView queueDataView)
			: this((Orientation)1)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			TouchscreenTabView touchscreenTabView = this;
			this.queueDataView = queueDataView;
			((GuiWidget)((TabControl)this).get_TabBar()).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((TabControl)this).get_TabBar().set_BorderColor(new RGBA_Bytes(0, 0, 0, 0));
			((GuiWidget)((TabControl)this).get_TabBar()).set_Margin(new BorderDouble(4.0, 0.0, 0.0, 0.0));
			((GuiWidget)((TabControl)this).get_TabBar()).set_Padding(new BorderDouble(0.0, 8.0));
			((GuiWidget)this).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 0.0));
			TabTextSize = 18;
			string text = UserSettings.Instance.get("IsSimpleMode");
			if (text == null)
			{
				simpleMode = true;
				UserSettings.Instance.set("IsSimpleMode", "true");
			}
			else
			{
				simpleMode = Convert.ToBoolean(text);
			}
			AddTab("Part Preview Tab", "Preview".Localize().ToUpper(), delegate
			{
				touchscreenTabView.partPreviewContainer = new PartPreviewContent(PrinterConnectionAndCommunication.Instance.ActivePrintItem, View3DWidget.WindowMode.Embeded, View3DWidget.AutoRotate.Enabled);
				return (GuiWidget)(object)touchscreenTabView.partPreviewContainer;
			});
			AddTab("Slice Settings Tab", "Settings".Localize().ToUpper(), delegate
			{
				if (ActiveSliceSettings.Instance.PrinterSelected)
				{
					touchscreenTabView.sliceSettingsWidget = (GuiWidget)(object)new SliceSettingsWidget();
				}
				else
				{
					touchscreenTabView.sliceSettingsWidget = (GuiWidget)(object)new NoSettingsWidget();
				}
				return touchscreenTabView.sliceSettingsWidget;
			});
			BorderDouble margin = default(BorderDouble);
			((BorderDouble)(ref margin))._002Ector(4.0, 10.0);
			TabBar tabBar = ((TabControl)this).get_TabBar();
			HorizontalLine horizontalLine = new HorizontalLine();
			((GuiWidget)horizontalLine).set_Margin(margin);
			((GuiWidget)tabBar).AddChild((GuiWidget)(object)horizontalLine, -1);
			AddTab("Controls Tab", "Controls".Localize().ToUpper(), () => (GuiWidget)(object)new ManualPrinterControls());
			TabBar tabBar2 = ((TabControl)this).get_TabBar();
			HorizontalLine horizontalLine2 = new HorizontalLine();
			((GuiWidget)horizontalLine2).set_Margin(margin);
			((GuiWidget)tabBar2).AddChild((GuiWidget)(object)horizontalLine2, -1);
			AddTab("Queue Tab", "Queue".Localize().ToUpper(), () => (GuiWidget)(object)new QueueDataWidget(queueDataView));
			QueueTabPage = ((TabControl)this).GetTabPage("Queue Tab");
			AddTab("Library Tab", "Library".Localize().ToUpper(), () => (GuiWidget)(object)new PrintLibraryWidget());
			if (!simpleMode)
			{
				AddTab("History Tab", "History".Localize().ToUpper(), () => (GuiWidget)(object)new PrintHistoryWidget());
			}
			TabBar tabBar3 = ((TabControl)this).get_TabBar();
			HorizontalLine horizontalLine3 = new HorizontalLine();
			((GuiWidget)horizontalLine3).set_Margin(margin);
			((GuiWidget)tabBar3).AddChild((GuiWidget)(object)horizontalLine3, -1);
			((GuiWidget)this).add_Load((EventHandler)delegate
			{
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				if (!touchscreenTabView.simpleMode && !touchscreenTabView.TouchScreenIsTall)
				{
					foreach (HorizontalLine item in ExtensionMethods.Children<HorizontalLine>((GuiWidget)(object)((TabControl)touchscreenTabView).get_TabBar()))
					{
						((GuiWidget)item).set_Margin(new BorderDouble(4.0, 5.0));
					}
				}
			});
			AddTab("Options Tab", "Options".Localize().ToUpper(), () => (GuiWidget)(object)new PrinterConfigurationScrollWidget());
			AddTab("About Tab", "About".Localize().ToUpper(), () => (GuiWidget)(object)new AboutWidget());
			NumQueueItemsChanged(this, null);
			SetUpdateNotification(this, null);
			QueueData.Instance.ItemAdded.RegisterEvent((EventHandler)NumQueueItemsChanged, ref unregisterEvents);
			QueueData.Instance.ItemRemoved.RegisterEvent((EventHandler)NumQueueItemsChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.ActivePrintItemChanged.RegisterEvent((EventHandler)delegate
			{
				UiThread.RunOnIdle((Action)delegate
				{
					touchscreenTabView.partPreviewContainer?.Reload(PrinterConnectionAndCommunication.Instance.ActivePrintItem);
				}, 1.0);
			}, ref unregisterEvents);
			ApplicationController.Instance.AdvancedControlsPanelReloading.RegisterEvent((EventHandler)delegate
			{
				UiThread.RunOnIdle((Action)touchscreenTabView.ReloadAdvancedControls);
			}, ref unregisterEvents);
			UpdateControlData.Instance.UpdateStatusChanged.RegisterEvent((EventHandler)SetUpdateNotification, ref unregisterEvents);
			string text2 = UserSettings.Instance.get(CompactTabView_CurrentTab);
			((TabControl)this).SelectTab(text2);
			((TabControl)this).get_TabBar().add_TabIndexChanged((EventHandler)delegate
			{
				string selectedTabName = ((TabControl)touchscreenTabView).get_TabBar().get_SelectedTabName();
				if (!string.IsNullOrEmpty(selectedTabName))
				{
					UserSettings.Instance.set(CompactTabView_CurrentTab, selectedTabName);
				}
			});
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
		}

		public void SetUpdateNotification(object sender, EventArgs widgetEvent)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			switch (UpdateControlData.Instance.UpdateStatus)
			{
			case UpdateControlData.UpdateStatusStates.MayBeAvailable:
			case UpdateControlData.UpdateStatusStates.UpdateAvailable:
			case UpdateControlData.UpdateStatusStates.UpdateDownloading:
			case UpdateControlData.UpdateStatusStates.ReadyToInstall:
				if (addedUpdateMark == null)
				{
					addedUpdateMark = (GuiWidget)(object)new UpdateNotificationMark();
					GuiWidget obj = ((GuiWidget)((TabControl)this).get_TabBar()).FindNamedChildRecursive("About Tab");
					SimpleTextTabWidget val = obj as SimpleTextTabWidget;
					addedUpdateMark.set_OriginRelativeParent(new Vector2(((GuiWidget)val.tabTitle).get_Width() + 3.0, 7.0 * GuiWidget.get_DeviceScale()));
					((GuiWidget)val).AddChild(addedUpdateMark, -1);
				}
				addedUpdateMark.set_Visible(true);
				break;
			case UpdateControlData.UpdateStatusStates.CheckingForUpdate:
			case UpdateControlData.UpdateStatusStates.UpToDate:
				if (addedUpdateMark != null)
				{
					addedUpdateMark.set_Visible(false);
				}
				break;
			default:
				throw new NotImplementedException();
			}
		}

		private void ReloadAdvancedControls()
		{
			TabPage tabPage = ((TabControl)this).GetTabPage("Controls Tab");
			(tabPage as LazyTabPage).Reload();
			TabPage tabPage2 = ((TabControl)this).GetTabPage("Options Tab");
			(tabPage2 as LazyTabPage).Reload();
			TabPage tabPage3 = ((TabControl)this).GetTabPage("Slice Settings Tab");
			(tabPage3 as LazyTabPage).Reload();
			((GuiWidget)this).Invalidate();
		}

		private void NumQueueItemsChanged(object sender, EventArgs widgetEvent)
		{
			((GuiWidget)QueueTabPage).set_Text(string.Format("{0} ({1})", "Queue".Localize().ToUpper(), QueueData.Instance.ItemCount));
		}

		private void AddTab(string name, string tabTitle, Func<GuiWidget> generator)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			LazyTabPage val = new LazyTabPage(tabTitle);
			val.set_Generator(generator);
			TabPage val2 = (TabPage)val;
			((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(val2, name, (double)TabTextSize, ActiveTheme.get_Instance().get_SecondaryAccentColor(), RGBA_Bytes.Transparent, ActiveTheme.get_Instance().get_TabLabelUnselected(), RGBA_Bytes.Transparent));
		}
	}
}
