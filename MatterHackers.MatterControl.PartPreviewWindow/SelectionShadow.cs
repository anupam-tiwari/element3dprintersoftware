using System;
using MatterHackers.Agg;
using MatterHackers.MatterControl.MeshVisualizer;
using MatterHackers.PolygonMesh;
using MatterHackers.RenderOpenGl;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class SelectionShadow : InteractionVolume
	{
		private View3DWidget view3DWidget;

		public SelectionShadow(View3DWidget view3DWidget)
			: base(null, view3DWidget.meshViewerWidget)
		{
			this.view3DWidget = view3DWidget;
		}

		public override void SetPosition()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			Vector3 center = base.MeshViewerToDrawWith.GetBoundsForSelection().get_Center();
			TotalTransform = Matrix4X4.CreateTranslation(new Vector3(center.x, center.y, 0.1));
		}

		public override void DrawGlContent(EventArgs e)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			if (base.MeshViewerToDrawWith.HaveSelection)
			{
				AxisAlignedBoundingBox boundsForSelection = base.MeshViewerToDrawWith.GetBoundsForSelection();
				RenderMeshToGl.Render(PlatonicSolids.CreateCube(boundsForSelection.get_XSize(), boundsForSelection.get_YSize(), 0.1), (IColorType)(object)new RGBA_Bytes(22, 80, 220, 30), TotalTransform, (RenderTypes)1);
			}
			base.DrawGlContent(e);
		}
	}
}
