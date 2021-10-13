using System.Collections.Generic;
using System.Collections.ObjectModel;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.MatterControl
{
	public class BoundDropList : DropDownList
	{
		private List<KeyValuePair<string, string>> listSource;

		public List<KeyValuePair<string, string>> ListSource
		{
			get
			{
				return listSource;
			}
			set
			{
				if (listSource == value)
				{
					return;
				}
				((Collection<MenuItem>)(object)base.MenuItems).Clear();
				((DropDownList)this).set_SelectedIndex(-1);
				listSource = value;
				foreach (KeyValuePair<string, string> item in listSource)
				{
					((DropDownList)this).AddItem(item.Key, item.Value, 12.0);
				}
				((GuiWidget)this).Invalidate();
			}
		}

		public BoundDropList(string noSelectionString, int maxHeight = 0)
			: this(noSelectionString, (Direction)1, (double)maxHeight, false)
		{
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			((DropDownList)this).OnDraw(graphics2D);
			if (((GuiWidget)this).get_Focused())
			{
				graphics2D.Rectangle(((GuiWidget)this).get_LocalBounds(), RGBA_Bytes.Orange, 1.0);
			}
		}
	}
}
