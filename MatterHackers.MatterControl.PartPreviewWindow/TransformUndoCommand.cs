using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	internal class TransformUndoCommand : IUndoRedoCommand
	{
		private List<int> meshGroupIndices = new List<int>();

		private List<Matrix4X4> redoTransforms = new List<Matrix4X4>();

		private List<Matrix4X4> undoTransforms = new List<Matrix4X4>();

		private View3DWidget view3DWidget;

		public TransformUndoCommand(View3DWidget view3DWidget, int meshGroupIndex, Matrix4X4 undoTransform, Matrix4X4 redoTransform)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			this.view3DWidget = view3DWidget;
			meshGroupIndices.Add(meshGroupIndex);
			undoTransforms.Add(undoTransform);
			redoTransforms.Add(redoTransform);
		}

		public TransformUndoCommand(View3DWidget view3DWidget, List<int> meshGroupIndices, List<Matrix4X4> undoTransforms, List<Matrix4X4> redoTransforms)
		{
			this.view3DWidget = view3DWidget;
			this.meshGroupIndices.AddRange(meshGroupIndices);
			this.undoTransforms.AddRange(undoTransforms);
			this.redoTransforms.AddRange(redoTransforms);
		}

		public void Do()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < meshGroupIndices.Count; i++)
			{
				view3DWidget.MeshGroupTransforms[meshGroupIndices[i]] = redoTransforms[i];
			}
			view3DWidget.PartHasBeenChanged();
		}

		public void Undo()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < meshGroupIndices.Count; i++)
			{
				view3DWidget.MeshGroupTransforms[meshGroupIndices[i]] = undoTransforms[i];
			}
			view3DWidget.PartHasBeenChanged();
		}
	}
}
