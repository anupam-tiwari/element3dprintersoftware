using System;
using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.PolygonMesh;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class MeshGroupUndoCommand : IUndoRedoCommand
	{
		private View3DWidget view3DWidget;

		private Tuple<List<int>, List<MeshGroup>, List<Matrix4X4>, List<PlatingMeshGroupData>> doData;

		private Tuple<List<int>, List<MeshGroup>, List<Matrix4X4>, List<PlatingMeshGroupData>> undoData;

		public MeshGroupUndoCommand(View3DWidget view3DWidget, Tuple<List<int>, List<MeshGroup>, List<Matrix4X4>, List<PlatingMeshGroupData>> undoData)
		{
			this.view3DWidget = view3DWidget;
			this.undoData = undoData;
			doData = this.view3DWidget.MakeCopyOfAllMeshData();
		}

		public void Do()
		{
			DoWithData(doData);
		}

		public void Undo()
		{
			DoWithData(undoData);
		}

		private void DoWithData(Tuple<List<int>, List<MeshGroup>, List<Matrix4X4>, List<PlatingMeshGroupData>> data)
		{
			view3DWidget.MeshGroups.Clear();
			view3DWidget.MeshGroupTransforms.Clear();
			view3DWidget.MeshGroupExtraData.Clear();
			view3DWidget.SelectedMeshGroupIndices.Clear();
			view3DWidget.MeshGroups.AddRange(data.Item2);
			view3DWidget.MeshGroupTransforms.AddRange(data.Item3);
			view3DWidget.MeshGroupExtraData.AddRange(data.Item4);
			view3DWidget.SelectedMeshGroupIndices.AddRange(data.Item1);
			view3DWidget.PartHasBeenChanged();
			((GuiWidget)view3DWidget).Invalidate();
		}
	}
}
