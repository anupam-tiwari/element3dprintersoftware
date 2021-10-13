using System;

namespace MatterHackers.MatterControl.GCodeVisualizer
{
	[Flags]
	public enum RenderType
	{
		None = 0x0,
		Extrusions = 0x1,
		Moves = 0x2,
		Retractions = 0x4,
		SpeedColors = 0x8,
		SimulateExtrusion = 0x10,
		HideExtruderOffsets = 0x20,
		TransparentExtrusion = 0x40
	}
}
