using MatterHackers.Agg.Transform;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	public class GCodeRenderInfo
	{
		private Vector2[] extruderOffsets;

		public int startLayerIndex;

		private int endLayerIndex;

		private Affine transform;

		private double layerScale;

		private RenderType currentRenderType;

		private double featureToStartOnRatio0To1;

		private double featureToEndOnRatio0To1;

		private GCodeSelectionInfo gCodeSelectionInfo;

		public int StartLayerIndex => startLayerIndex;

		public int EndLayerIndex => endLayerIndex;

		public Affine Transform => transform;

		public double LayerScale => layerScale;

		public RenderType CurrentRenderType => currentRenderType;

		public double FeatureToStartOnRatio0To1 => featureToStartOnRatio0To1;

		public double FeatureToEndOnRatio0To1 => featureToEndOnRatio0To1;

		public GCodeSelectionInfo GCodeSelectionInfo => gCodeSelectionInfo;

		public Vector2 GetExtruderOffset(int index)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (extruderOffsets != null && extruderOffsets.Length > index)
			{
				return extruderOffsets[index];
			}
			return Vector2.Zero;
		}

		public GCodeRenderInfo()
		{
		}

		public GCodeRenderInfo(int startLayerIndex, int endLayerIndex, Affine transform, double layerScale, RenderType renderType, double featureToStartOnRatio0To1, double featureToEndOnRatio0To1, Vector2[] extruderOffsets, GCodeSelectionInfo gCodeSelectionInfo = null)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			this.startLayerIndex = startLayerIndex;
			this.endLayerIndex = endLayerIndex;
			this.transform = transform;
			this.layerScale = layerScale;
			currentRenderType = renderType;
			this.featureToStartOnRatio0To1 = featureToStartOnRatio0To1;
			this.featureToEndOnRatio0To1 = featureToEndOnRatio0To1;
			this.extruderOffsets = extruderOffsets;
			this.gCodeSelectionInfo = gCodeSelectionInfo ?? new GCodeSelectionInfo();
		}
	}
}
