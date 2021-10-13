using System;
using System.Collections.ObjectModel;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.MatterControl.WellPlate;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class PartPreviewContent : GuiWidget
	{
		private EventHandler unregisterEvents;

		private View3DWidget partPreviewView;

		private ViewGcodeBasic viewGcodeBasic;

		private TabControl layer3DTabControl;

		private Tab layerViewTab;

		private TabControl well3DTabControl;

		private Tab threeDViewTab;

		private View3DWidget.AutoRotate autoRotate3DView;

		private View3DWidget.OpenMode openMode;

		private View3DWidget.WindowMode windowMode;

		public PartPreviewContent(PrintItemWrapper printItem, View3DWidget.WindowMode windowMode, View3DWidget.AutoRotate autoRotate3DView, View3DWidget.OpenMode openMode = View3DWidget.OpenMode.Viewing)
			: this()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			this.openMode = openMode;
			this.autoRotate3DView = autoRotate3DView;
			this.windowMode = windowMode;
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)this).AnchorAll();
			Load(printItem);
			if (printItem != null && Path.GetExtension(printItem.FileLocation)!.ToUpper() == ".GCODE")
			{
				SwitchToGcodeView();
			}
		}

		public void Reload(PrintItemWrapper printItem)
		{
			((GuiWidget)this).CloseAllChildren();
			Load(printItem);
		}

		private void Load(PrintItemWrapper printItem)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Expected O, but got Unknown
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Expected O, but got Unknown
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Expected O, but got Unknown
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Expected O, but got Unknown
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Expected O, but got Unknown
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Expected O, but got Unknown
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Expected O, but got Unknown
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Expected O, but got Unknown
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Expected O, but got Unknown
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			int num = 16;
			layer3DTabControl = new TabControl((Orientation)0);
			layer3DTabControl.get_TabBar().set_BorderColor(new RGBA_Bytes(0, 0, 0, 0));
			FlowLayoutWidget well3DTopDown = new FlowLayoutWidget((FlowDirection)3);
			((GuiWidget)well3DTopDown).AnchorAll();
			((GuiWidget)well3DTopDown).add_ParentChanged((EventHandler)delegate
			{
				if (!(((GuiWidget)well3DTopDown).get_Parent() is SystemWindow))
				{
					((Collection<GuiWidget>)(object)((GuiWidget)well3DTopDown).get_Children())[0].set_Visible(true);
				}
				else
				{
					((Collection<GuiWidget>)(object)((GuiWidget)well3DTopDown).get_Children())[0].set_Visible(false);
				}
			});
			GuiWidget val = new GuiWidget();
			val.set_BackgroundColor(new RGBA_Bytes(200, 200, 200));
			val.set_Height(2.0);
			val.set_HAnchor((HAnchor)5);
			val.set_Margin(new BorderDouble(3.0));
			GuiWidget val2 = val;
			((GuiWidget)well3DTopDown).AddChild(val2, -1);
			well3DTabControl = new TabControl((Orientation)0);
			well3DTabControl.get_TabBar().set_BorderColor(new RGBA_Bytes(0, 0, 0, 0));
			((GuiWidget)well3DTopDown).AddChild((GuiWidget)(object)well3DTabControl, -1);
			((GuiWidget)layer3DTabControl.get_TabBar()).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			((GuiWidget)well3DTabControl.get_TabBar()).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			RGBA_Bytes tabLabelSelected = ActiveTheme.get_Instance().get_TabLabelSelected();
			double value = ActiveSliceSettings.Instance.GetValue<double>("build_height");
			partPreviewView = new View3DWidget(printItem, new Vector3(ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size"), value), ActiveSliceSettings.Instance.GetValue<Vector2>("print_center"), ActiveSliceSettings.Instance.GetValue<BedShape>("bed_shape"), this.windowMode, autoRotate3DView, openMode);
			TabPage val3 = new TabPage((GuiWidget)(object)partPreviewView, string.Format("3D {0} ", "View".Localize()).ToUpper());
			threeDViewTab = (Tab)new SimpleTextTabWidget(val3, "3D View Tab", (double)num, tabLabelSelected, default(RGBA_Bytes), ActiveTheme.get_Instance().get_TabLabelUnselected(), default(RGBA_Bytes));
			well3DTabControl.AddTab(threeDViewTab);
			TabPage val4 = new TabPage((GuiWidget)(object)new WellPlateWidget(partPreviewView.MeshViewer), "cultureware".Localize().ToUpper());
			Tab wellPlateViewTab = (Tab)new SimpleTextTabWidget(val4, "WellPlate View Tab", (double)num, tabLabelSelected, default(RGBA_Bytes), ActiveTheme.get_Instance().get_TabLabelUnselected(), default(RGBA_Bytes));
			well3DTabControl.AddTab(wellPlateViewTab);
			TabPage val5 = new TabPage((GuiWidget)(object)well3DTopDown, "Model".Localize().ToUpper());
			ViewGcodeBasic.WindowMode windowMode = ViewGcodeBasic.WindowMode.Embeded;
			if (this.windowMode == View3DWidget.WindowMode.StandAlone)
			{
				windowMode = ViewGcodeBasic.WindowMode.StandAlone;
			}
			viewGcodeBasic = new ViewGcodeBasic(printItem, new Vector3(ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size"), value), ActiveSliceSettings.Instance.GetValue<Vector2>("print_center"), ActiveSliceSettings.Instance.GetValue<BedShape>("bed_shape"), windowMode);
			if (this.windowMode == View3DWidget.WindowMode.StandAlone)
			{
				((GuiWidget)partPreviewView).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					((GuiWidget)this).Close();
				});
				((GuiWidget)viewGcodeBasic).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					((GuiWidget)this).Close();
				});
			}
			TabPage val6 = new TabPage((GuiWidget)(object)viewGcodeBasic, "Layer View".Localize().ToUpper());
			Tab val7;
			if (this.windowMode == View3DWidget.WindowMode.StandAlone || UserSettings.Instance.IsTouchScreen)
			{
				val7 = (Tab)new SimpleTextTabWidget(val5, "WellPlate3D View Tab", (double)num, tabLabelSelected, default(RGBA_Bytes), ActiveTheme.get_Instance().get_TabLabelUnselected(), default(RGBA_Bytes));
				layer3DTabControl.AddTab(val7);
				layerViewTab = (Tab)new SimpleTextTabWidget(val6, "Layer View Tab", (double)num, tabLabelSelected, default(RGBA_Bytes), ActiveTheme.get_Instance().get_TabLabelUnselected(), default(RGBA_Bytes));
				layer3DTabControl.AddTab(layerViewTab);
			}
			else
			{
				val7 = (Tab)(object)new PopOutTextTabWidget(val5, "WellPlate3D View Tab", new Vector2(590.0, 400.0), num);
				layer3DTabControl.AddTab(val7);
				layerViewTab = (Tab)(object)new PopOutTextTabWidget(val6, "Layer View Tab", new Vector2(590.0, 400.0), num);
				layer3DTabControl.AddTab(layerViewTab);
			}
			((GuiWidget)val7).set_ToolTipText("Preview Model, Configure Well Plate".Localize());
			((GuiWidget)threeDViewTab).set_ToolTipText("Preview 3D Design".Localize());
			((GuiWidget)wellPlateViewTab).set_ToolTipText("Cultureware Configuration".Localize());
			((GuiWidget)layerViewTab).set_ToolTipText("Preview layer Tool Paths".Localize());
			((GuiWidget)wellPlateViewTab).set_Visible(false);
			partPreviewView.MeshViewer.LoadDone += delegate
			{
				((GuiWidget)wellPlateViewTab).set_Visible(!partPreviewView.ActivePrintItem.PrintItem.WellPlatePrint);
			};
			((GuiWidget)this).AddChild((GuiWidget)(object)layer3DTabControl, -1);
		}

		public void SwitchToGcodeView()
		{
			layer3DTabControl.get_TabBar().SelectTab(layerViewTab);
			((GuiWidget)viewGcodeBasic).Focus();
		}

		public void SwitchTo3dView()
		{
			well3DTabControl.get_TabBar().SelectTab(threeDViewTab);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			unregisterEvents?.Invoke(this, null);
			((GuiWidget)this).OnClosed(e);
		}
	}
}
