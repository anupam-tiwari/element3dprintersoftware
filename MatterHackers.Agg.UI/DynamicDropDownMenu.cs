using System;
using MatterHackers.MatterControl;

namespace MatterHackers.Agg.UI
{
	public class DynamicDropDownMenu : DropDownMenu
	{
		private TupleList<string, Func<bool>> menuItems;

		public DynamicDropDownMenu(GuiWidget buttonView, Direction direction = 1, double pointSize = 12.0)
			: base(buttonView, direction, pointSize)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			menuItems = new TupleList<string, Func<bool>>();
			base.TextColor = RGBA_Bytes.Black;
			base.NormalArrowColor = RGBA_Bytes.Black;
			base.HoverArrowColor = RGBA_Bytes.Black;
			base.BorderWidth = 1;
			base.BorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			base.SelectionChanged += AltChoices_SelectionChanged;
		}

		public DynamicDropDownMenu(string topMenuText, Direction direction = 1, double pointSize = 12.0)
			: base(topMenuText, direction, pointSize)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			menuItems = new TupleList<string, Func<bool>>();
			base.NormalColor = RGBA_Bytes.White;
			base.TextColor = RGBA_Bytes.Black;
			base.NormalArrowColor = RGBA_Bytes.Black;
			base.HoverArrowColor = RGBA_Bytes.Black;
			base.HoverColor = new RGBA_Bytes(255, 255, 255, 200);
			base.BorderWidth = 1;
			base.BorderColor = new RGBA_Bytes(ActiveTheme.get_Instance().get_PrimaryTextColor(), 200);
			base.SelectionChanged += AltChoices_SelectionChanged;
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			base.OnDraw(graphics2D);
		}

		public void addItem(string name, Func<bool> clickFunction)
		{
			AddItem(name);
			menuItems.Add(name, clickFunction);
		}

		private void AltChoices_SelectionChanged(object sender, EventArgs e)
		{
			menuItems[((DropDownMenu)sender).SelectedIndex].Item2();
		}
	}
}
