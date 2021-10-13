using MatterHackers.MatterControl.MeshVisualizer;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public interface IInteractionVolumeCreator
	{
		InteractionVolume CreateInteractionVolume(View3DWidget widget);
	}
}
