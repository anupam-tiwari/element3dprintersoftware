using System;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.CustomWidgets;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class AdvancedControlsPanel : GuiWidget
	{
		private static readonly string ThirdPanelTabView_AdvancedControls_CurrentTab = "ThirdPanelTabView_AdvancedControls_CurrentTab";

		private EventHandler unregisterEvents;

		private Button backButton;

		private GuiWidget sliceSettingsWidget;

		private TabControl advancedTab;

		public static string SliceSettingsTabName
		{
			get;
		} = "Slice Settings Tab";


		public static string ControlsTabName
		{
			get;
		} = "Controls Tab";


		public event EventHandler BackClicked;

		public AdvancedControlsPanel()
			: this()
		{
			advancedTab = CreateAdvancedControlsTab();
			((GuiWidget)this).AddChild((GuiWidget)(object)advancedTab, -1);
			((GuiWidget)this).AnchorAll();
			ApplicationController.Instance.AdvancedControlsPanelReloading.RegisterEvent((EventHandler)delegate
			{
				UiThread.RunOnIdle((Action)ReloadSliceSettings);
			}, ref unregisterEvents);
		}

		public void ReloadSliceSettings()
		{
			WidescreenPanel.PreChangePanels.CallEvents((object)null, (EventArgs)null);
			if (!((GuiWidget)advancedTab).get_HasBeenClosed())
			{
				PopOutManager.SaveIfClosed = false;
				int childIndex = ((GuiWidget)this).GetChildIndex((GuiWidget)(object)advancedTab);
				((GuiWidget)this).RemoveChild(childIndex);
				((GuiWidget)advancedTab).Close();
				advancedTab = CreateAdvancedControlsTab();
				((GuiWidget)this).AddChild((GuiWidget)(object)advancedTab, childIndex);
				PopOutManager.SaveIfClosed = true;
			}
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private TabControl CreateAdvancedControlsTab()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Expected O, but got Unknown
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Expected O, but got Unknown
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Expected O, but got Unknown
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Expected O, but got Unknown
			TabControl advancedControls = new TabControl((Orientation)0);
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			advancedControls.get_TabBar().set_BorderColor(ActiveTheme.get_Instance().get_SecondaryTextColor());
			((GuiWidget)advancedControls.get_TabBar()).set_Margin(new BorderDouble(0.0, 0.0));
			((GuiWidget)advancedControls.get_TabBar()).set_Padding(new BorderDouble(0.0, 2.0));
			int num = 16;
			num = 14;
			TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();
			textImageButtonFactory.fontSize = 14.0;
			textImageButtonFactory.invertImageLocation = false;
			backButton = textImageButtonFactory.Generate("Back".Localize(), StaticData.get_Instance().LoadIcon("icon_arrow_left_32x32.png", 32, 32));
			((GuiWidget)backButton).set_ToolTipText("Switch to Queue, Library and History".Localize());
			((GuiWidget)backButton).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
			((GuiWidget)backButton).set_VAnchor((VAnchor)1);
			((GuiWidget)backButton).set_Cursor((Cursors)3);
			((GuiWidget)backButton).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				this.BackClicked?.Invoke(this, null);
			});
			((GuiWidget)advancedControls.get_TabBar()).AddChild((GuiWidget)(object)backButton, -1);
			((GuiWidget)advancedControls.get_TabBar()).AddChild((GuiWidget)(object)new HorizontalSpacer(), -1);
			RGBA_Bytes tabLabelUnselected = ActiveTheme.get_Instance().get_TabLabelUnselected();
			if (ActiveSliceSettings.Instance.PrinterSelected)
			{
				sliceSettingsWidget = (GuiWidget)(object)new SliceSettingsWidget();
			}
			else
			{
				sliceSettingsWidget = (GuiWidget)(object)new NoSettingsWidget();
			}
			PopOutTextTabWidget popOutTextTabWidget = new PopOutTextTabWidget(new TabPage(sliceSettingsWidget, "Settings".Localize().ToUpper()), SliceSettingsTabName, new Vector2(590.0, 400.0), num);
			advancedControls.AddTab((Tab)(object)popOutTextTabWidget);
			PopOutTextTabWidget popOutTextTabWidget2 = new PopOutTextTabWidget(new TabPage((GuiWidget)(object)new ManualPrinterControls(), "Controls".Localize().ToUpper()), ControlsTabName, new Vector2(400.0, 300.0), num);
			advancedControls.AddTab((Tab)(object)popOutTextTabWidget2);
			if (!UserSettings.Instance.IsTouchScreen)
			{
				MenuOptionSettings.sliceSettingsPopOut = popOutTextTabWidget;
				MenuOptionSettings.controlsPopOut = popOutTextTabWidget2;
			}
			PrinterConfigurationScrollWidget printerConfigurationScrollWidget = new PrinterConfigurationScrollWidget();
			advancedControls.AddTab((Tab)new SimpleTextTabWidget(new TabPage((GuiWidget)(object)printerConfigurationScrollWidget, "Options".Localize().ToUpper()), "Options Tab", (double)num, ActiveTheme.get_Instance().get_PrimaryTextColor(), default(RGBA_Bytes), tabLabelUnselected, default(RGBA_Bytes)));
			string text = UserSettings.Instance.get(ThirdPanelTabView_AdvancedControls_CurrentTab);
			advancedControls.SelectTab(text);
			advancedControls.get_TabBar().add_TabIndexChanged((EventHandler)delegate
			{
				string selectedTabName = advancedControls.get_TabBar().get_SelectedTabName();
				if (!string.IsNullOrEmpty(selectedTabName))
				{
					UserSettings.Instance.set(ThirdPanelTabView_AdvancedControls_CurrentTab, selectedTabName);
				}
			});
			return advancedControls;
		}
	}
}
