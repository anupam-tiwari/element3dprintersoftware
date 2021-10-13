using System.Collections.Generic;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	internal class ArangeUndoCommand : IUndoRedoCommand
	{
		private List<TransformUndoCommand> allUndoTransforms = new List<TransformUndoCommand>();

		public ArangeUndoCommand(View3DWidget view3DWidget, List<Matrix4X4> preArrangeTarnsforms, List<Matrix4X4> postArrangeTarnsforms)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < preArrangeTarnsforms.Count; i++)
			{
				allUndoTransforms.Add(new TransformUndoCommand(view3DWidget, i, preArrangeTarnsforms[i], postArrangeTarnsforms[i]));
			}
		}

		public void Do()
		{
			for (int i = 0; i < allUndoTransforms.Count; i++)
			{
				allUndoTransforms[i].Do();
			}
		}

		public void Undo()
		{
			for (int i = 0; i < allUndoTransforms.Count; i++)
			{
				allUndoTransforms[i].Undo();
			}
		}
	}
}
