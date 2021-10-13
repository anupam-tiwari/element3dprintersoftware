using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrintHistory;
using MatterHackers.MatterControl.PrintLibrary;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl
{
	public class FirstPanelTabView : TabControl
	{
		public static int firstPanelCurrentTab;

		private TabPage QueueTabPage;

		private TabPage LibraryTabPage;

		private TabPage HistoryTabPage;

		private RGBA_Bytes unselectedTextColor = ActiveTheme.get_Instance().get_TabLabelUnselected();

		private QueueDataView queueDataView;

		private EventHandler unregisterEvents;

		public FirstPanelTabView(QueueDataView queueDataView)
			: this((Orientation)0)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Expected O, but got Unknown
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Expected O, but got Unknown
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Expected O, but got Unknown
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Expected O, but got Unknown
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Expected O, but got Unknown
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Expected O, but got Unknown
			this.queueDataView = queueDataView;
			((GuiWidget)((TabControl)this).get_TabBar()).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((TabControl)this).get_TabBar().set_BorderColor(new RGBA_Bytes(0, 0, 0, 0));
			((GuiWidget)((TabControl)this).get_TabBar()).set_Margin(new BorderDouble(0.0, 0.0));
			((GuiWidget)((TabControl)this).get_TabBar()).set_Padding(new BorderDouble(0.0, 2.0));
			((GuiWidget)this).set_Margin(new BorderDouble(0.0, 0.0, 0.0, 4.0));
			QueueTabPage = new TabPage((GuiWidget)(object)new QueueDataWidget(queueDataView), "Queue".Localize().ToUpper());
			((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(QueueTabPage, "Queue Tab", 15.0, ActiveTheme.get_Instance().get_TabLabelSelected(), default(RGBA_Bytes), unselectedTextColor, default(RGBA_Bytes)));
			LibraryTabPage = new TabPage((GuiWidget)(object)new PrintLibraryWidget(), "Library".Localize().ToUpper());
			((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(LibraryTabPage, "Library Tab", 15.0, ActiveTheme.get_Instance().get_TabLabelSelected(), default(RGBA_Bytes), unselectedTextColor, default(RGBA_Bytes)));
			HistoryTabPage = new TabPage((GuiWidget)(object)new PrintHistoryWidget(), "History".Localize().ToUpper());
			((TabControl)this).AddTab((Tab)new SimpleTextTabWidget(HistoryTabPage, "History Tab", 15.0, ActiveTheme.get_Instance().get_TabLabelSelected(), default(RGBA_Bytes), unselectedTextColor, default(RGBA_Bytes)));
			NumQueueItemsChanged(this, null);
			QueueData.Instance.ItemAdded.RegisterEvent((EventHandler)NumQueueItemsChanged, ref unregisterEvents);
			QueueData.Instance.ItemRemoved.RegisterEvent((EventHandler)NumQueueItemsChanged, ref unregisterEvents);
			WidescreenPanel.PreChangePanels.RegisterEvent((EventHandler)SaveCurrentTab, ref unregisterEvents);
			((TabControl)this).set_SelectedTabIndex(firstPanelCurrentTab);
		}

		private void NumQueueItemsChanged(object sender, EventArgs widgetEvent)
		{
			string arg = "Queue".Localize().ToUpper();
			string format = string.Format("{1} ({0})", QueueData.Instance.ItemCount, arg);
			((GuiWidget)QueueTabPage).set_Text(string.Format(format, QueueData.Instance.ItemCount));
		}

		private void SaveCurrentTab(object sender, EventArgs e)
		{
			firstPanelCurrentTab = ((TabControl)this).get_SelectedTabIndex();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
		}
	}
}
