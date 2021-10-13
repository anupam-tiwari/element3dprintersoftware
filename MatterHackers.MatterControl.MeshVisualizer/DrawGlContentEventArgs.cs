using System;

namespace MatterHackers.MatterControl.MeshVisualizer
{
	public class DrawGlContentEventArgs : EventArgs
	{
		public bool ZBuffered
		{
			get;
		}

		public DrawGlContentEventArgs(bool zBuffered)
		{
			ZBuffered = zBuffered;
		}
	}
}
