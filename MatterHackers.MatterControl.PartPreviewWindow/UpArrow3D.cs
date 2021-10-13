using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.PolygonMesh;
using MatterHackers.PolygonMesh.Processors;
using MatterHackers.RayTracer;
using MatterHackers.RenderOpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class UpArrow3D : InteractionVolume
	{
		internal HeightValueDisplay heightDisplay;

		private PlaneShape hitPlane;

		private Vector3 lastMoveDelta;

		private List<Matrix4X4> transformsOnMouseDown = new List<Matrix4X4>();

		private Mesh upArrow;

		private View3DWidget view3DWidget;

		private double zHitHeight;

		public UpArrow3D(View3DWidget view3DWidget)
			: base(null, view3DWidget.meshViewerWidget)
		{
			heightDisplay = new HeightValueDisplay(view3DWidget);
			((GuiWidget)heightDisplay).set_Visible(false);
			base.DrawOnTop = true;
			this.view3DWidget = view3DWidget;
			string text = Path.Combine("Icons", "3D Icons", "up_pointer.stl");
			if (!StaticData.get_Instance().FileExists(text))
			{
				return;
			}
			using Stream stream = StaticData.get_Instance().OpenSteam(text);
			using MemoryStream memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream, 65536);
			List<MeshGroup> list = MeshFileIo.Load((Stream)memoryStream, Path.GetExtension(text), (ReportProgressRatio)null);
			upArrow = list[0].get_Meshes()[0];
			base.CollisionVolume = PlatingHelper.CreateTraceDataForMesh(upArrow);
		}

		public override void DrawGlContent(EventArgs e)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			bool flag = true;
			if (base.MeshViewerToDrawWith.SelectedInteractionVolume != null && !(base.MeshViewerToDrawWith.SelectedInteractionVolume is UpArrow3D))
			{
				flag = false;
			}
			if (base.MeshViewerToDrawWith.HaveSelection && flag)
			{
				if (base.MouseOver)
				{
					RenderMeshToGl.Render(upArrow, (IColorType)(object)RGBA_Bytes.Red, TotalTransform, (RenderTypes)1);
				}
				else
				{
					RenderMeshToGl.Render(upArrow, (IColorType)(object)RGBA_Bytes.Black, TotalTransform, (RenderTypes)1);
				}
			}
			base.DrawGlContent(e);
		}

		public override void OnMouseDown(MouseEvent3DArgs mouseEvent3D)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			zHitHeight = mouseEvent3D.info.hitPosition.z;
			lastMoveDelta = default(Vector3);
			double num = Vector3.Dot(mouseEvent3D.info.hitPosition, mouseEvent3D.MouseRay.directionNormal);
			hitPlane = new PlaneShape(mouseEvent3D.MouseRay.directionNormal, num, (MaterialAbstract)null);
			IntersectInfo closestIntersection = ((BaseShape)hitPlane).GetClosestIntersection(mouseEvent3D.MouseRay);
			zHitHeight = closestIntersection.hitPosition.z;
			transformsOnMouseDown = base.MeshViewerToDrawWith.SelectedMeshGroupTransforms;
			base.OnMouseDown(mouseEvent3D);
		}

		public override void OnMouseMove(MouseEvent3DArgs mouseEvent3D)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			IntersectInfo closestIntersection = ((BaseShape)hitPlane).GetClosestIntersection(mouseEvent3D.MouseRay);
			if (closestIntersection != null && base.MeshViewerToDrawWith.HaveSelection)
			{
				Vector3 val = default(Vector3);
				((Vector3)(ref val))._002Ector(0.0, 0.0, closestIntersection.hitPosition.z - zHitHeight);
				foreach (int selectedMeshGroupIndex in base.MeshViewerToDrawWith.SelectedMeshGroupIndices)
				{
					List<Matrix4X4> meshGroupTransforms = base.MeshViewerToDrawWith.MeshGroupTransforms;
					int index = selectedMeshGroupIndex;
					meshGroupTransforms[index] *= Matrix4X4.CreateTranslation(new Vector3(-lastMoveDelta));
				}
				if (base.MeshViewerToDrawWith.SnapGridDistance > 0.0)
				{
					double snapGridDistance = base.MeshViewerToDrawWith.SnapGridDistance;
					AxisAlignedBoundingBox boundsForSelection = base.MeshViewerToDrawWith.GetBoundsForSelection();
					double num = Math.Round((boundsForSelection.minXYZ.z + val.z) / snapGridDistance) * snapGridDistance;
					val.z = num - boundsForSelection.minXYZ.z;
				}
				foreach (int selectedMeshGroupIndex2 in base.MeshViewerToDrawWith.SelectedMeshGroupIndices)
				{
					List<Matrix4X4> meshGroupTransforms = base.MeshViewerToDrawWith.MeshGroupTransforms;
					int index = selectedMeshGroupIndex2;
					meshGroupTransforms[index] *= Matrix4X4.CreateTranslation(new Vector3(val));
				}
				lastMoveDelta = val;
				view3DWidget.PartHasBeenChanged();
				Invalidate();
			}
			base.OnMouseMove(mouseEvent3D);
		}

		public override void OnMouseUp(MouseEvent3DArgs mouseEvent3D)
		{
			view3DWidget.AddUndoForSelectedMeshGroupTransforms(transformsOnMouseDown);
			base.OnMouseUp(mouseEvent3D);
		}

		public override void SetPosition()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			AxisAlignedBoundingBox boundsForSelection = base.MeshViewerToDrawWith.GetBoundsForSelection();
			Vector3 center = boundsForSelection.get_Center();
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(center.x, center.y, boundsForSelection.maxXYZ.z);
			base.MeshViewerToDrawWith.TrackballTumbleWidget.GetScreenPosition(val);
			double worldUnitsPerScreenPixelAtPosition = base.MeshViewerToDrawWith.TrackballTumbleWidget.GetWorldUnitsPerScreenPixelAtPosition(val);
			Matrix4X4 val2 = Matrix4X4.CreateTranslation(new Vector3(val.x, val.y, val.z + 20.0 * worldUnitsPerScreenPixelAtPosition));
			val2 = (TotalTransform = Matrix4X4.CreateScale(worldUnitsPerScreenPixelAtPosition) * val2);
			if (base.MouseOver || MouseDownOnControl)
			{
				((GuiWidget)heightDisplay).set_Visible(true);
			}
			else if (!view3DWidget.DisplayAllValueData)
			{
				((GuiWidget)heightDisplay).set_Visible(false);
			}
		}
	}
}
