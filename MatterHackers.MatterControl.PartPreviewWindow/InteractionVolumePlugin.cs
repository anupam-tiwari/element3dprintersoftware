using MatterHackers.MatterControl.MeshVisualizer;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class InteractionVolumePlugin : IInteractionVolumeCreator
	{
		public virtual InteractionVolume CreateInteractionVolume(View3DWidget widget)
		{
			return null;
		}
	}
}
