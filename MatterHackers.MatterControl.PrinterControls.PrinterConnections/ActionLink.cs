using System;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl.PrinterControls.PrinterConnections
{
	public class ActionLink : TextWidget
	{
		private bool isUnderlined = true;

		public ActionLink(string text, int fontSize = 10)
			: this(text, 0.0, 0.0, (double)fontSize, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_Selectable(true);
			((GuiWidget)this).set_Margin(new BorderDouble(3.0, 0.0, 3.0, 0.0));
			((GuiWidget)this).set_VAnchor((VAnchor)5);
			((GuiWidget)this).add_MouseEnter((EventHandler)onMouse_Enter);
			((GuiWidget)this).add_MouseLeave((EventHandler)onMouse_Leave);
			((GuiWidget)this).set_Cursor((Cursors)3);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			((TextWidget)this).OnDraw(graphics2D);
			if (isUnderlined)
			{
				RectangleDouble val = default(RectangleDouble);
				((RectangleDouble)(ref val))._002Ector(((GuiWidget)this).get_LocalBounds().Left, ((GuiWidget)this).get_LocalBounds().Bottom, ((GuiWidget)this).get_LocalBounds().Right, ((GuiWidget)this).get_LocalBounds().Bottom);
				graphics2D.Rectangle(val, ((TextWidget)this).get_TextColor(), 1.0);
			}
		}

		private void onMouse_Enter(object sender, EventArgs args)
		{
			isUnderlined = false;
			((GuiWidget)this).Invalidate();
		}

		private void onMouse_Leave(object sender, EventArgs args)
		{
			isUnderlined = true;
			((GuiWidget)this).Invalidate();
		}
	}
}
