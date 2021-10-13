using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.PolygonMesh;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	internal class CopyUndoCommand : IUndoRedoCommand
	{
		private int lastNewItemIndex;

		private List<int> selectedIndices = new List<int>();

		private View3DWidget view3DWidget;

		private List<Matrix4X4> newItemTransforms = new List<Matrix4X4>();

		private List<PlatingMeshGroupData> newItemPlatingDatas = new List<PlatingMeshGroupData>();

		private List<MeshGroup> newItemMeshGroups = new List<MeshGroup>();

		public CopyUndoCommand(View3DWidget view3DWidget, int lastNewItemIndex, List<int> selectedIndices)
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			this.view3DWidget = view3DWidget;
			this.lastNewItemIndex = lastNewItemIndex;
			this.selectedIndices.AddRange(selectedIndices);
			for (int num = this.lastNewItemIndex; num > this.lastNewItemIndex - this.selectedIndices.Count; num--)
			{
				newItemMeshGroups.Add(view3DWidget.MeshGroups[num]);
				newItemTransforms.Add(view3DWidget.MeshGroupTransforms[num]);
				newItemPlatingDatas.Add(view3DWidget.MeshGroupExtraData[num]);
			}
		}

		public void Undo()
		{
			for (int num = lastNewItemIndex; num > lastNewItemIndex - selectedIndices.Count; num--)
			{
				view3DWidget.MeshGroups.RemoveAt(num);
				view3DWidget.MeshGroupExtraData.RemoveAt(num);
				view3DWidget.MeshGroupTransforms.RemoveAt(num);
			}
			view3DWidget.SelectedMeshGroupIndices.Clear();
			view3DWidget.SelectedMeshGroupIndices.AddRange(selectedIndices);
			view3DWidget.PartHasBeenChanged();
		}

		public void Do()
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			view3DWidget.SelectedMeshGroupIndices.Clear();
			int num = lastNewItemIndex - selectedIndices.Count + 1;
			for (int i = num; i <= lastNewItemIndex; i++)
			{
				view3DWidget.MeshGroups.Insert(i, newItemMeshGroups[i - num]);
				view3DWidget.MeshGroupTransforms.Insert(i, newItemTransforms[i - num]);
				view3DWidget.MeshGroupExtraData.Insert(i, newItemPlatingDatas[i - num]);
				view3DWidget.SelectedMeshGroupIndices.Add(i);
			}
			view3DWidget.PartHasBeenChanged();
			((GuiWidget)view3DWidget).Invalidate();
		}
	}
}
