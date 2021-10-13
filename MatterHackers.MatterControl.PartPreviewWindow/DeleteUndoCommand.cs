using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.PolygonMesh;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	internal class DeleteUndoCommand : IUndoRedoCommand
	{
		private List<int> deletedIndices = new List<int>();

		private View3DWidget view3DWidget;

		private List<Matrix4X4> deletedTransforms = new List<Matrix4X4>();

		private List<PlatingMeshGroupData> deletedPlatingDatas = new List<PlatingMeshGroupData>();

		private List<MeshGroup> meshGroupsThatWereDeleted = new List<MeshGroup>();

		public DeleteUndoCommand(View3DWidget view3DWidget, List<int> deletedIndices)
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			this.view3DWidget = view3DWidget;
			this.deletedIndices.AddRange(deletedIndices);
			foreach (int deletedIndex in this.deletedIndices)
			{
				meshGroupsThatWereDeleted.Add(view3DWidget.MeshGroups[deletedIndex]);
				deletedTransforms.Add(view3DWidget.MeshGroupTransforms[deletedIndex]);
				deletedPlatingDatas.Add(view3DWidget.MeshGroupExtraData[deletedIndex]);
			}
		}

		public void Do()
		{
			foreach (int deletedIndex in deletedIndices)
			{
				view3DWidget.MeshGroups.RemoveAt(deletedIndex);
				view3DWidget.MeshGroupExtraData.RemoveAt(deletedIndex);
				view3DWidget.MeshGroupTransforms.RemoveAt(deletedIndex);
			}
			view3DWidget.SelectedMeshGroupIndices.Clear();
			view3DWidget.PartHasBeenChanged();
		}

		public void Undo()
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			for (int num = deletedIndices.Count - 1; num >= 0; num--)
			{
				view3DWidget.MeshGroups.Insert(deletedIndices[num], meshGroupsThatWereDeleted[num]);
				view3DWidget.MeshGroupTransforms.Insert(deletedIndices[num], deletedTransforms[num]);
				view3DWidget.MeshGroupExtraData.Insert(deletedIndices[num], deletedPlatingDatas[num]);
			}
			((GuiWidget)view3DWidget).Invalidate();
			view3DWidget.SelectedMeshGroupIndices.AddRange(deletedIndices);
			view3DWidget.PartHasBeenChanged();
		}
	}
}
