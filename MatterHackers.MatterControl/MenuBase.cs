using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public abstract class MenuBase : GuiWidget
	{
		public DropDownMenu MenuDropList;

		private List<MenuItemAction> menuActions;

		protected abstract IEnumerable<MenuItemAction> GetMenuActions();

		public MenuBase(string menuName)
			: this()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			MenuDropList = new DropDownMenu(menuName.ToUpper(), (Direction)1, 10.0);
			MenuDropList.TextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			((GuiWidget)MenuDropList).set_Margin(new BorderDouble(0.0));
			((GuiWidget)MenuDropList).set_Padding(new BorderDouble(4.0, 4.0, 0.0, 4.0));
			MenuDropList.MenuItemsPadding = new BorderDouble(8.0, 4.0);
			MenuDropList.DrawDirectionalArrow = false;
			MenuDropList.MenuAsWideAsItems = false;
			menuActions = new List<MenuItemAction>(GetMenuActions());
			foreach (MenuItemAction menuAction in menuActions)
			{
				if (menuAction.Title.StartsWith("-----"))
				{
					((Menu)MenuDropList).AddHorizontalLine();
					continue;
				}
				MenuItem val = MenuDropList.AddItem(menuAction.Title, null, 11.0);
				if (menuAction.Action == null)
				{
					((GuiWidget)val).set_Enabled(false);
				}
			}
			((GuiWidget)this).AddChild((GuiWidget)(object)MenuDropList, -1);
			RectangleDouble childrenBoundsIncludingMargins = ((GuiWidget)this).GetChildrenBoundsIncludingMargins(false, (Func<GuiWidget, GuiWidget, bool>)null);
			((GuiWidget)this).set_Width(((RectangleDouble)(ref childrenBoundsIncludingMargins)).get_Width());
			((GuiWidget)this).set_Height(22.0 * GuiWidget.get_DeviceScale());
			((GuiWidget)this).set_Margin(new BorderDouble(0.0));
			((GuiWidget)this).set_Padding(new BorderDouble(0.0));
			((GuiWidget)this).set_VAnchor((VAnchor)2);
			MenuDropList.SelectionChanged += MenuDropList_SelectionChanged;
			((Menu)MenuDropList).set_OpenOffset(new Vector2(0.0, 0.0));
		}

		private void MenuDropList_SelectionChanged(object sender, EventArgs e)
		{
			string selectedValue = ((DropDownMenu)sender).SelectedValue;
			foreach (MenuItemAction menuAction in menuActions)
			{
				if (menuAction.Title == selectedValue && menuAction.Action != null)
				{
					UiThread.RunOnIdle(menuAction.Action);
				}
			}
		}
	}
}
