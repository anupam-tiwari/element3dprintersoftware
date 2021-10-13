using System;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl
{
	public class CompactSlidePanel : SlidePanel
	{
		private EventHandler unregisterEvents;

		private TabControl mainControlsTabControl;

		public TabPage QueueTabPage;

		public TabPage AboutTabPage;

		private QueueDataView queueDataView;

		private const int StandardControlsPanelIndex = 0;

		private const int AdvancedControlsPanelIndex = 1;

		private static int lastPanelIndexBeforeReload;

		private GuiWidget LeftPanel => GetPanel(0);

		private GuiWidget RightPanel => GetPanel(1);

		public double TabBarWidth => ((GuiWidget)mainControlsTabControl).get_Width();

		public CompactSlidePanel(QueueDataView queueDataView)
			: base(2)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			this.queueDataView = queueDataView;
			LeftPanel.AddChild((GuiWidget)(object)new PrintProgressBar(), -1);
			mainControlsTabControl = (TabControl)(object)new FirstPanelTabView(queueDataView);
			Button val = new TextImageButtonFactory
			{
				normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor(),
				fontSize = 10.0,
				disabledTextColor = RGBA_Bytes.LightGray,
				disabledFillColor = ActiveTheme.get_Instance().get_PrimaryBackgroundColor(),
				disabledBorderColor = ActiveTheme.get_Instance().get_PrimaryBackgroundColor(),
				invertImageLocation = true
			}.Generate("Settings\n& Controls".Localize(), StaticData.get_Instance().LoadIcon("icon_arrow_right_32x32.png", 32, 32));
			((GuiWidget)val).set_Name("SettingsAndControls");
			((GuiWidget)val).set_ToolTipText("Switch to Settings, Controls and Options".Localize());
			((GuiWidget)val).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
			((GuiWidget)val).set_VAnchor((VAnchor)1);
			((GuiWidget)val).set_Cursor((Cursors)3);
			((GuiWidget)val).add_Click((EventHandler<MouseEventArgs>)ToggleActivePanel_Click);
			((GuiWidget)mainControlsTabControl.get_TabBar()).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			((GuiWidget)mainControlsTabControl.get_TabBar()).AddChild((GuiWidget)(object)val, -1);
			((GuiWidget)mainControlsTabControl.get_TabBar()).set_HAnchor((HAnchor)13);
			((GuiWidget)mainControlsTabControl).set_HAnchor((HAnchor)13);
			LeftPanel.AddChild((GuiWidget)(object)mainControlsTabControl, -1);
			RightPanel.AddChild((GuiWidget)(object)new PrintProgressBar(), -1);
			AdvancedControlsPanel advancedControlsPanel = new AdvancedControlsPanel();
			((GuiWidget)advancedControlsPanel).set_Name("For - CompactSlidePanel");
			AdvancedControlsPanel advancedControlsPanel2 = advancedControlsPanel;
			advancedControlsPanel2.BackClicked += ToggleActivePanel_Click;
			RightPanel.AddChild((GuiWidget)(object)advancedControlsPanel2, -1);
			WidescreenPanel.PreChangePanels.RegisterEvent((EventHandler)SaveCurrentPanelIndex, ref unregisterEvents);
			SetPanelIndexImmediate(lastPanelIndexBeforeReload);
		}

		private void SaveCurrentPanelIndex(object sender, EventArgs e)
		{
			lastPanelIndexBeforeReload = base.PanelIndex;
		}

		private void ToggleActivePanel_Click(object sender, EventArgs mouseEvent)
		{
			if (base.PanelIndex == 0)
			{
				base.PanelIndex = 1;
			}
			else
			{
				base.PanelIndex = 0;
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
