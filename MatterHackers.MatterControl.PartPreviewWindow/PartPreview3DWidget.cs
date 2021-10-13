using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.MatterControl.Utilities;
using MatterHackers.PolygonMesh;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public abstract class PartPreview3DWidget : PartPreviewWidget
	{
		protected static readonly int DefaultScrollBarWidth = 120;

		protected bool autoRotating;

		protected bool allowAutoRotate;

		public MeshViewerWidget meshViewerWidget;

		private EventHandler unregisterEvents;

		protected ViewControls3D viewControls3D;

		private bool needToRecretaeBed;

		private static ImageBuffer wattermarkImage = null;

		public bool HaveSelection => meshViewerWidget.HaveSelection;

		public List<MeshGroup> SelectedMeshGroups => meshViewerWidget.SelectedMeshGroups;

		public EventList<int> SelectedMeshGroupIndices => meshViewerWidget.SelectedMeshGroupIndices;

		public List<Matrix4X4> SelectedMeshGroupTransforms => meshViewerWidget.SelectedMeshGroupTransforms;

		public Matrix4X4 SelectedMeshTransform
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return meshViewerWidget.SelectedMeshGroupTransform;
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				meshViewerWidget.SelectedMeshGroupTransform = value;
			}
		}

		public MeshGroup SelectedMeshGroup => meshViewerWidget.SelectedMeshGroup;

		public virtual int SelectedMeshGroupIndex
		{
			get
			{
				return meshViewerWidget.SelectedMeshGroupIndex;
			}
			set
			{
				meshViewerWidget.SelectedMeshGroupIndex = value;
			}
		}

		public List<MeshGroup> MeshGroups => meshViewerWidget.MeshGroups;

		public List<Matrix4X4> MeshGroupTransforms => meshViewerWidget.MeshGroupTransforms;

		public virtual bool InEditMode => false;

		public PartPreview3DWidget()
		{
			ActiveSliceSettings.SettingChanged.RegisterEvent((EventHandler)CheckSettingChanged, ref unregisterEvents);
			ApplicationController.Instance.AdvancedControlsPanelReloading.RegisterEvent((EventHandler)CheckSettingChanged, ref unregisterEvents);
		}

		private void CheckSettingChanged(object sender, EventArgs e)
		{
			StringEventArgs val = e as StringEventArgs;
			if (val != null && (val.get_Data() == "bed_size" || val.get_Data() == "print_center" || val.get_Data() == "build_height" || val.get_Data() == "bed_shape" || val.get_Data() == "center_part_on_bed"))
			{
				needToRecretaeBed = true;
			}
		}

		private void RecreateBed()
		{
			double buildHeight = ActiveSliceSettings.Instance.GetValue<double>("build_height");
			UiThread.RunOnIdle((Action)delegate
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_012f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0134: Unknown result type (might be due to invalid IL or missing references)
				//IL_0139: Unknown result type (might be due to invalid IL or missing references)
				meshViewerWidget.CreatePrintBed(new Vector3(ActiveSliceSettings.Instance.GetValue<Vector2>("bed_size"), buildHeight), ActiveSliceSettings.Instance.GetValue<Vector2>("print_center"), ActiveSliceSettings.Instance.GetValue<BedShape>("bed_shape"));
				PutOemImageOnBed();
				Vector2 value = ActiveSliceSettings.Instance.GetValue<Vector2>("print_center");
				if (ActiveSliceSettings.Instance.GetValue<bool>("center_part_on_bed") && !InEditMode && meshViewerWidget.MeshGroups.Count > 0)
				{
					AxisAlignedBoundingBox axisAlignedBoundingBox = meshViewerWidget.MeshGroups[0].GetAxisAlignedBoundingBox();
					Vector3 val = (axisAlignedBoundingBox.maxXYZ + axisAlignedBoundingBox.minXYZ) / 2.0;
					for (int i = 0; i < meshViewerWidget.MeshGroups.Count; i++)
					{
						meshViewerWidget.MeshGroupTransforms[i] = Matrix4X4.CreateTranslation(-val + new Vector3(0.0, 0.0, axisAlignedBoundingBox.get_ZSize() / 2.0) + new Vector3(value, 0.0));
					}
				}
			});
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			if (needToRecretaeBed)
			{
				needToRecretaeBed = false;
				RecreateBed();
			}
			((GuiWidget)this).OnDraw(graphics2D);
		}

		protected void PutOemImageOnBed()
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			string text = Path.Combine("OEMSettings", "bedimage.png");
			if (allowAutoRotate && StaticData.get_Instance().FileExists(text))
			{
				if (wattermarkImage == (ImageBuffer)null)
				{
					wattermarkImage = StaticData.get_Instance().LoadImage(text);
				}
				ImageBuffer bedImage = MeshViewerWidget.BedImage;
				bedImage.NewGraphics2D().Render((IImageByte)(object)wattermarkImage, new Vector2((double)((bedImage.get_Width() - wattermarkImage.get_Width()) / 2), (double)((bedImage.get_Height() - wattermarkImage.get_Height()) / 2)));
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

		protected static SolidSlider InsertUiForSlider(FlowLayoutWidget wordOptionContainer, string header, double min = 0.0, double max = 0.5)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			double thumbWidth = 10.0;
			if (UserSettings.Instance.IsTouchScreen)
			{
				thumbWidth = 20.0;
			}
			TextWidget val = new TextWidget(header, 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			((GuiWidget)val).set_Margin(new BorderDouble(10.0, 3.0, 3.0, 5.0));
			((GuiWidget)val).set_HAnchor((HAnchor)1);
			((GuiWidget)wordOptionContainer).AddChild((GuiWidget)(object)val, -1);
			SolidSlider solidSlider = new SolidSlider(default(Vector2), thumbWidth, 0.0, 1.0, (Orientation)0);
			solidSlider.TotalWidthInPixels = DefaultScrollBarWidth;
			solidSlider.Minimum = min;
			solidSlider.Maximum = max;
			((GuiWidget)solidSlider).set_Margin(new BorderDouble(3.0, 5.0, 3.0, 3.0));
			((GuiWidget)solidSlider).set_HAnchor((HAnchor)2);
			solidSlider.View.BackgroundColor = default(RGBA_Bytes);
			((GuiWidget)wordOptionContainer).AddChild((GuiWidget)(object)solidSlider, -1);
			return solidSlider;
		}
	}
}
