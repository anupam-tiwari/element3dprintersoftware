using System.Collections.Generic;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class GCodeSelectionInfo
	{
		private HashSet<int> selectedFeatureInstructionIndices = new HashSet<int>();

		public int StartLineIndex = -1;

		public int EndLineIndex = -1;

		public Vector2 StartBedPosition = Vector2.Zero;

		public Vector2 EndBedPosition = Vector2.Zero;

		public HashSet<int> SelectedFeatureInstructionIndices => selectedFeatureInstructionIndices;

		public void ClearSelection()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			selectedFeatureInstructionIndices.Clear();
			StartLineIndex = -1;
			EndLineIndex = -1;
			StartBedPosition = Vector2.Zero;
			EndBedPosition = Vector2.Zero;
		}
	}
}
