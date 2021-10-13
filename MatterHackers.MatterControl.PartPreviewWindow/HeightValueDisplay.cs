using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class HeightValueDisplay : GuiWidget
	{
		private static readonly int HorizontalLineLength = 30;

		private View3DWidget view3DWidget;

		private ValueDisplayInfo heightValueDisplayInfo = new ValueDisplayInfo();

		private MeshViewerWidget MeshViewerToDrawWith => view3DWidget.meshViewerWidget;

		public HeightValueDisplay(View3DWidget view3DWidget)
			: this()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_BackgroundColor(new RGBA_Bytes(RGBA_Bytes.White, 150));
			this.view3DWidget = view3DWidget;
			((GuiWidget)view3DWidget.meshViewerWidget).AddChild((GuiWidget)(object)this, -1);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)MeshViewerToDrawWith).add_AfterDraw((EventHandler<DrawEventArgs>)MeshViewerToDrawWith_Draw);
		}

		private void MeshViewerToDrawWith_Draw(object drawingWidget, DrawEventArgs drawEvent)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			if (!((GuiWidget)this).get_Visible() || drawEvent == null)
			{
				return;
			}
			Vector2 val = Vector2.Zero;
			Vector2 val2 = Vector2.Zero;
			Vector2 widthDisplayCenter = Vector2.Zero;
			if (!MeshViewerToDrawWith.HaveSelection)
			{
				return;
			}
			AxisAlignedBoundingBox boundsForSelection = MeshViewerToDrawWith.GetBoundsForSelection();
			Vector2 val3 = default(Vector2);
			((Vector2)(ref val3))._002Ector(-100.0, 0.0);
			Vector3[] array = (Vector3[])(object)new Vector3[4]
			{
				new Vector3(boundsForSelection.minXYZ.x, boundsForSelection.minXYZ.y, boundsForSelection.minXYZ.z),
				new Vector3(boundsForSelection.minXYZ.x, boundsForSelection.maxXYZ.y, boundsForSelection.minXYZ.z),
				new Vector3(boundsForSelection.maxXYZ.x, boundsForSelection.minXYZ.y, boundsForSelection.minXYZ.z),
				new Vector3(boundsForSelection.maxXYZ.x, boundsForSelection.maxXYZ.y, boundsForSelection.minXYZ.z)
			};
			for (int i = 0; i < 4; i++)
			{
				Vector2 screenPosition = MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(array[i]);
				if (screenPosition.x > val3.x)
				{
					val2 = screenPosition;
					val = MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(array[i] + new Vector3(0.0, 0.0, 0.0 - array[i].z));
					widthDisplayCenter = MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(array[i] + new Vector3(0.0, 0.0, (0.0 - array[i].z) / 2.0));
					val3 = screenPosition + new Vector2((double)HorizontalLineLength, 0.0);
				}
			}
			heightValueDisplayInfo.DisplaySizeInfo(drawEvent.get_graphics2D(), widthDisplayCenter, boundsForSelection.minXYZ.z);
			((GuiWidget)this).set_OriginRelativeParent(val3);
			double num = Math.Round(val.y) + 0.5;
			drawEvent.get_graphics2D().Line(val.x, num, val.x + (double)HorizontalLineLength - 5.0, num, RGBA_Bytes.Black, 1.0);
			double num2 = Math.Round(val2.y) + 0.5;
			drawEvent.get_graphics2D().Line(val2.x, num2, val2.x + (double)HorizontalLineLength - 5.0, num2, RGBA_Bytes.Black, 1.0);
			Vector2 lineStart = default(Vector2);
			((Vector2)(ref lineStart))._002Ector(val.x + (double)(HorizontalLineLength / 2), num);
			Vector2 lineEnd = default(Vector2);
			((Vector2)(ref lineEnd))._002Ector(val2.x + (double)(HorizontalLineLength / 2), num2);
			InteractionVolume.DrawMeasureLine(drawEvent.get_graphics2D(), lineStart, lineEnd, RGBA_Bytes.Black, InteractionVolume.LineArrows.End);
		}
	}
}
