using System;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class ActionLinkFactory
	{
		public ActionLink Generate(string linkText, int fontSize, EventHandler<MouseEventArgs> clickEvent)
		{
			ActionLink actionLink = new ActionLink(linkText, fontSize);
			if (clickEvent != null)
			{
				((GuiWidget)actionLink).add_MouseUp(clickEvent);
			}
			return actionLink;
		}
	}
}
