using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PartPreviewWindow;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class WidescreenPanel : FlowLayoutWidget
	{
		private static readonly int ColumnOneFixedWidth = 590;

		private int lastNumberOfVisiblePanels;

		private TextImageButtonFactory advancedControlsButtonFactory = new TextImageButtonFactory();

		private RGBA_Bytes unselectedTextColor = ActiveTheme.get_Instance().get_TabLabelUnselected();

		private FlowLayoutWidget ColumnOne;

		private FlowLayoutWidget ColumnTwo;

		private double Force1PanelWidth = 990.0 * GuiWidget.get_DeviceScale();

		private double Force2PanelWidth = 1590.0 * GuiWidget.get_DeviceScale();

		private GuiWidget leftBorderLine;

		private EventHandler unregisterEvents;

		public static RootedObjectEventHandler PreChangePanels = new RootedObjectEventHandler();

		private QueueDataView queueDataView;

		private CompactSlidePanel compactSlidePanel;

		private int VisiblePanelCount
		{
			get
			{
				if (!(((GuiWidget)this).get_Width() < Force1PanelWidth))
				{
					return 2;
				}
				return 1;
			}
		}

		public WidescreenPanel()
			: this((FlowDirection)0)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Name("WidescreenPanel");
			((GuiWidget)this).AnchorAll();
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)this).set_Padding(new BorderDouble(4.0));
			PrinterConnectionAndCommunication.Instance.ActivePrintItemChanged.RegisterEvent((EventHandler)onActivePrintItemChanged, ref unregisterEvents);
			ApplicationController.Instance.AdvancedControlsPanelReloading.RegisterEvent((EventHandler)delegate
			{
				UiThread.RunOnIdle((Action)ReloadAdvancedControlsPanel);
			}, ref unregisterEvents);
		}

		public override void OnBoundsChanged(EventArgs e)
		{
			if (VisiblePanelCount != lastNumberOfVisiblePanels)
			{
				RecreateAllPanels();
			}
			((GuiWidget)this).OnBoundsChanged(e);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}

		private void onActivePrintItemChanged(object sender, EventArgs e)
		{
			if (VisiblePanelCount > 1)
			{
				UiThread.RunOnIdle((Action)LoadColumnTwo);
			}
		}

		private void LoadCompactView()
		{
			queueDataView = new QueueDataView();
			((GuiWidget)ColumnOne).RemoveAllChildren();
			((GuiWidget)ColumnOne).AddChild((GuiWidget)(object)new ActionBarPlus(queueDataView), -1);
			compactSlidePanel = new CompactSlidePanel(queueDataView);
			((GuiWidget)ColumnOne).AddChild((GuiWidget)(object)compactSlidePanel, -1);
			((GuiWidget)ColumnOne).AnchorAll();
		}

		private void LoadColumnTwo()
		{
			PopOutManager.SaveIfClosed = false;
			((GuiWidget)ColumnTwo).CloseAllChildren();
			PopOutManager.SaveIfClosed = true;
			PartPreviewContent partPreviewContent = new PartPreviewContent(PrinterConnectionAndCommunication.Instance.ActivePrintItem, View3DWidget.WindowMode.Embeded, View3DWidget.AutoRotate.Disabled);
			((GuiWidget)partPreviewContent).AnchorAll();
			((GuiWidget)ColumnTwo).AddChild((GuiWidget)(object)partPreviewContent, -1);
			((GuiWidget)ColumnTwo).AnchorAll();
		}

		public void RecreateAllPanels(object state = null)
		{
			if (((GuiWidget)this).get_Width() != 0.0)
			{
				int visiblePanelCount = VisiblePanelCount;
				PreChangePanels.CallEvents((object)this, (EventArgs)null);
				RemovePanelsAndCreateEmpties();
				LoadCompactView();
				if (visiblePanelCount == 2)
				{
					LoadColumnTwo();
				}
				SetColumnVisibility();
				lastNumberOfVisiblePanels = visiblePanelCount;
			}
		}

		private void SetColumnVisibility(object state = null)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			switch (VisiblePanelCount)
			{
			case 1:
				((GuiWidget)ColumnTwo).set_Visible(false);
				((GuiWidget)ColumnOne).set_Visible(true);
				((GuiWidget)this).set_Padding(new BorderDouble(0.0));
				leftBorderLine.set_Visible(false);
				break;
			case 2:
				((GuiWidget)this).set_Padding(new BorderDouble(4.0));
				((GuiWidget)ColumnOne).set_Visible(true);
				((GuiWidget)ColumnTwo).set_Visible(true);
				((GuiWidget)ColumnOne).set_HAnchor((HAnchor)0);
				((GuiWidget)ColumnOne).set_Width((double)ColumnOneFixedWidth);
				((GuiWidget)ColumnOne).set_MinimumSize(new Vector2(Math.Max(compactSlidePanel.TabBarWidth, ColumnOneFixedWidth), 0.0));
				break;
			}
		}

		private void RemovePanelsAndCreateEmpties()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			((GuiWidget)this).CloseAllChildren();
			ColumnOne = new FlowLayoutWidget((FlowDirection)3);
			ColumnTwo = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)this).AddChild((GuiWidget)(object)ColumnOne, -1);
			GuiWidget val = new GuiWidget();
			val.set_VAnchor((VAnchor)5);
			leftBorderLine = val;
			leftBorderLine.set_Width(15.0);
			leftBorderLine.add_BeforeDraw((EventHandler<DrawEventArgs>)delegate(object widget, DrawEventArgs graphics2D)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				RectangleDouble localBounds = ((GuiWidget)widget).get_LocalBounds();
				localBounds.Left += 3.0;
				localBounds.Right -= 8.0;
				graphics2D.get_graphics2D().FillRectangle(localBounds, (IColorType)(object)new RGBA_Bytes(160, 160, 160));
			});
			((GuiWidget)this).AddChild(leftBorderLine, -1);
			((GuiWidget)this).AddChild((GuiWidget)(object)ColumnTwo, -1);
		}

		public void ReloadAdvancedControlsPanel()
		{
			PreChangePanels.CallEvents((object)this, (EventArgs)null);
		}
	}
}
