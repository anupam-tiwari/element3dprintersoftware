using System;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.EeProm
{
	public class CloseOnDisconnectWindow : SystemWindow
	{
		private EventHandler unregisterEvents;

		public CloseOnDisconnectWindow(double width, double height)
			: this(width, height)
		{
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent((EventHandler)delegate
			{
				if (!PrinterConnectionAndCommunication.Instance.PrinterIsConnected)
				{
					((GuiWidget)this).CloseOnIdle();
				}
			}, ref unregisterEvents);
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((SystemWindow)this).OnClosed(e);
		}
	}
}
