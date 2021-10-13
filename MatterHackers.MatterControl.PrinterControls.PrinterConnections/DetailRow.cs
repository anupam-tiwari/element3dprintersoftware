using System;
using System.Collections.Generic;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class DetailRow : FlowLayoutWidget
	{
		public List<GuiWidget> showOnHoverChildren = new List<GuiWidget>();

		public DetailRow()
			: this((FlowDirection)0)
		{
			((GuiWidget)this).add_MouseEnter((EventHandler)onMouse_Enter);
			((GuiWidget)this).add_MouseLeave((EventHandler)onMouse_Leave);
		}

		private void onMouse_Enter(object sender, EventArgs args)
		{
			foreach (GuiWidget showOnHoverChild in showOnHoverChildren)
			{
				showOnHoverChild.set_Visible(true);
			}
			((GuiWidget)this).Invalidate();
		}

		private void onMouse_Leave(object sender, EventArgs args)
		{
			foreach (GuiWidget showOnHoverChild in showOnHoverChildren)
			{
				showOnHoverChild.set_Visible(false);
			}
			((GuiWidget)this).Invalidate();
		}
	}
}
