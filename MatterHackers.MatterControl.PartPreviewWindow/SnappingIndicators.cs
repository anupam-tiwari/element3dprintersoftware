using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class SnappingIndicators : InteractionVolume
	{
		private double distToStart = 10.0;

		private double lineLength = 15.0;

		private Vector2[] lines = (Vector2[])(object)new Vector2[4];

		private View3DWidget view3DWidget;

		public SnappingIndicators(View3DWidget view3DWidget)
			: base(null, view3DWidget.meshViewerWidget)
		{
			this.view3DWidget = view3DWidget;
			base.DrawOnTop = true;
			((GuiWidget)base.MeshViewerToDrawWith).add_AfterDraw((EventHandler<DrawEventArgs>)MeshViewerToDrawWith_Draw);
		}

		public override void SetPosition()
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			if (base.MeshViewerToDrawWith.HaveSelection)
			{
				AxisAlignedBoundingBox boundsForSelection = base.MeshViewerToDrawWith.GetBoundsForSelection();
				switch (view3DWidget.CurrentSelectInfo.HitQuadrant)
				{
				case HitQuadrant.LB:
				{
					Vector3 val4 = default(Vector3);
					((Vector3)(ref val4))._002Ector(boundsForSelection.minXYZ.x, boundsForSelection.minXYZ.y, 0.0);
					double worldUnitsPerScreenPixelAtPosition4 = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetWorldUnitsPerScreenPixelAtPosition(val4);
					lines[0] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val4 - new Vector3(distToStart * worldUnitsPerScreenPixelAtPosition4, 0.0, 0.0));
					lines[1] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val4 - new Vector3((distToStart + lineLength) * worldUnitsPerScreenPixelAtPosition4, 0.0, 0.0));
					lines[2] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val4 - new Vector3(0.0, distToStart * worldUnitsPerScreenPixelAtPosition4, 0.0));
					lines[3] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val4 - new Vector3(0.0, (distToStart + lineLength) * worldUnitsPerScreenPixelAtPosition4, 0.0));
					break;
				}
				case HitQuadrant.LT:
				{
					Vector3 val3 = default(Vector3);
					((Vector3)(ref val3))._002Ector(boundsForSelection.minXYZ.x, boundsForSelection.maxXYZ.y, 0.0);
					double worldUnitsPerScreenPixelAtPosition3 = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetWorldUnitsPerScreenPixelAtPosition(val3);
					lines[0] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val3 - new Vector3(distToStart * worldUnitsPerScreenPixelAtPosition3, 0.0, 0.0));
					lines[1] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val3 - new Vector3((distToStart + lineLength) * worldUnitsPerScreenPixelAtPosition3, 0.0, 0.0));
					lines[2] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val3 + new Vector3(0.0, distToStart * worldUnitsPerScreenPixelAtPosition3, 0.0));
					lines[3] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val3 + new Vector3(0.0, (distToStart + lineLength) * worldUnitsPerScreenPixelAtPosition3, 0.0));
					break;
				}
				case HitQuadrant.RB:
				{
					Vector3 val2 = default(Vector3);
					((Vector3)(ref val2))._002Ector(boundsForSelection.maxXYZ.x, boundsForSelection.minXYZ.y, 0.0);
					double worldUnitsPerScreenPixelAtPosition2 = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetWorldUnitsPerScreenPixelAtPosition(val2);
					lines[0] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val2 + new Vector3(distToStart * worldUnitsPerScreenPixelAtPosition2, 0.0, 0.0));
					lines[1] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val2 + new Vector3((distToStart + lineLength) * worldUnitsPerScreenPixelAtPosition2, 0.0, 0.0));
					lines[2] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val2 - new Vector3(0.0, distToStart * worldUnitsPerScreenPixelAtPosition2, 0.0));
					lines[3] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val2 - new Vector3(0.0, (distToStart + lineLength) * worldUnitsPerScreenPixelAtPosition2, 0.0));
					break;
				}
				case HitQuadrant.RT:
				{
					Vector3 val = default(Vector3);
					((Vector3)(ref val))._002Ector(boundsForSelection.maxXYZ.x, boundsForSelection.maxXYZ.y, 0.0);
					double worldUnitsPerScreenPixelAtPosition = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetWorldUnitsPerScreenPixelAtPosition(val);
					lines[0] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val + new Vector3(distToStart * worldUnitsPerScreenPixelAtPosition, 0.0, 0.0));
					lines[1] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val + new Vector3((distToStart + lineLength) * worldUnitsPerScreenPixelAtPosition, 0.0, 0.0));
					lines[2] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val + new Vector3(0.0, distToStart * worldUnitsPerScreenPixelAtPosition, 0.0));
					lines[3] = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val + new Vector3(0.0, (distToStart + lineLength) * worldUnitsPerScreenPixelAtPosition, 0.0));
					break;
				}
				}
			}
		}

		private void MeshViewerToDrawWith_Draw(object drawingWidget, DrawEventArgs drawEvent)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			if (base.MeshViewerToDrawWith.HaveSelection && view3DWidget.meshViewerWidget.SnapGridDistance > 0.0 && view3DWidget.CurrentSelectInfo.DownOnPart && drawEvent != null)
			{
				drawEvent.get_graphics2D().Line(lines[0], lines[1], RGBA_Bytes.Red, 1.0);
				drawEvent.get_graphics2D().Line(lines[2], lines[3], RGBA_Bytes.Red, 1.0);
			}
		}
	}
}
