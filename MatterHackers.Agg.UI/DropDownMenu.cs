using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg.UI
{
	public class DropDownMenu : Menu
	{
		private GuiWidget mainControlWidget;

		private RGBA_Bytes textColor = RGBA_Bytes.Black;

		private int selectedIndex = -1;

		public BorderDouble MenuItemsPadding
		{
			get;
			set;
		}

		public bool MenuAsWideAsItems
		{
			get;
			set;
		} = true;


		public bool DrawDirectionalArrow
		{
			get;
			set;
		} = true;


		public int BorderWidth
		{
			get;
			set;
		} = 1;


		public RGBA_Bytes BorderColor
		{
			get;
			set;
		}

		public RGBA_Bytes NormalArrowColor
		{
			get;
			set;
		}

		public RGBA_Bytes HoverArrowColor
		{
			get;
			set;
		}

		public RGBA_Bytes NormalColor
		{
			get;
			set;
		}

		public RGBA_Bytes HoverColor
		{
			get;
			set;
		}

		public RGBA_Bytes TextColor
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return textColor;
			}
			set
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				if (value != textColor)
				{
					textColor = value;
					this.TextColorChanged?.Invoke(this, null);
				}
			}
		}

		public int SelectedIndex
		{
			get
			{
				return selectedIndex;
			}
			set
			{
				selectedIndex = value;
				OnSelectionChanged(null);
				((GuiWidget)this).Invalidate();
			}
		}

		public string SelectedValue => GetValue(SelectedIndex);

		public event EventHandler SelectionChanged;

		public event EventHandler TextColorChanged;

		public DropDownMenu(string topMenuText, Direction direction = 1, double pointSize = 12.0)
			: this(direction, 0.0)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			TextWidget textWidget = new TextWidget(topMenuText, 0.0, 0.0, pointSize, (Justification)0, default(RGBA_Bytes), true, false, default(RGBA_Bytes), (TypeFace)null);
			textWidget.set_TextColor(TextColor);
			TextColorChanged += delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				textWidget.set_TextColor(TextColor);
			};
			textWidget.set_AutoExpandBoundsToText(true);
			((GuiWidget)this).set_Name(topMenuText + " Menu");
			SetStates((GuiWidget)(object)textWidget);
		}

		public DropDownMenu(GuiWidget topMenuContent, Direction direction = 1, double pointSize = 12.0)
			: this(direction, 0.0)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			SetStates(topMenuContent);
		}

		public string GetValue(int itemIndex)
		{
			return ((Collection<MenuItem>)(object)base.MenuItems)[SelectedIndex].get_Value();
		}

		private void SetStates(GuiWidget topMenuContent)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			SetDisplayAttributes();
			base.MenuItems.add_CollectionChanged(new NotifyCollectionChangedEventHandler(MenuItems_CollectionChanged));
			mainControlWidget = topMenuContent;
			mainControlWidget.set_VAnchor((VAnchor)2);
			mainControlWidget.set_HAnchor((HAnchor)1);
			((GuiWidget)this).AddChild(mainControlWidget, -1);
			((GuiWidget)this).set_HAnchor((HAnchor)8);
			((GuiWidget)this).set_VAnchor((VAnchor)8);
			((GuiWidget)this).add_MouseEnter((EventHandler)DropDownList_MouseEnter);
			((GuiWidget)this).add_MouseLeave((EventHandler)DropDownList_MouseLeave);
			NormalArrowColor = new RGBA_Bytes(255, 255, 255, 0);
			HoverArrowColor = TextColor;
		}

		protected override void DropListItems_Closed(object sender, ClosedEventArgs e)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_BackgroundColor(NormalColor);
			((Menu)this).DropListItems_Closed(sender, e);
		}

		private void DropDownList_MouseLeave(object sender, EventArgs e)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			if (!((Menu)this).get_IsOpen())
			{
				((GuiWidget)this).set_BackgroundColor(NormalColor);
			}
		}

		private void DropDownList_MouseEnter(object sender, EventArgs e)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			((GuiWidget)this).set_BackgroundColor(HoverColor);
		}

		private void OnSelectionChanged(EventArgs e)
		{
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, e);
			}
		}

		private void MenuItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Expected O, but got Unknown
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			double width = ((RectangleDouble)(ref localBounds)).get_Width();
			localBounds = ((GuiWidget)this).get_LocalBounds();
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(width, ((RectangleDouble)(ref localBounds)).get_Height());
			foreach (MenuItem item in (Collection<MenuItem>)(object)base.MenuItems)
			{
				val.x = Math.Max(val.x, ((GuiWidget)item).get_Width());
			}
			string text = mainControlWidget.get_Text();
			foreach (MenuItem item2 in (Collection<MenuItem>)(object)base.MenuItems)
			{
				mainControlWidget.set_Text(((GuiWidget)item2).get_Text());
				double x = val.x;
				localBounds = ((GuiWidget)this).get_LocalBounds();
				val.x = Math.Max(x, ((RectangleDouble)(ref localBounds)).get_Width());
				double y = val.y;
				localBounds = ((GuiWidget)this).get_LocalBounds();
				val.y = Math.Max(y, ((RectangleDouble)(ref localBounds)).get_Height());
			}
			mainControlWidget.set_Text(text);
			if (MenuAsWideAsItems)
			{
				((GuiWidget)this).set_MinimumSize(val);
			}
			foreach (MenuItem newItem in e.get_NewItems())
			{
				MenuItem val2 = newItem;
				((GuiWidget)val2).set_MinimumSize(new Vector2(val.x, ((GuiWidget)val2).get_MinimumSize().y));
				val2.remove_Selected((EventHandler)item_Selected);
				val2.add_Selected((EventHandler)item_Selected);
			}
		}

		private void item_Selected(object sender, EventArgs e)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			using (IEnumerator<MenuItem> enumerator = ((Collection<MenuItem>)(object)base.MenuItems).GetEnumerator())
			{
				while (enumerator.MoveNext() && enumerator.Current != sender)
				{
					num++;
				}
			}
			SelectedIndex = num;
			((GuiWidget)this).set_BackgroundColor(NormalColor);
		}

		private void SetDisplayAttributes()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			TextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			NormalColor = ActiveTheme.get_Instance().get_PrimaryBackgroundColor();
			HoverColor = RGBA_Bytes.Gray;
			((Menu)this).set_MenuItemsBorderWidth(1);
			((Menu)this).set_MenuItemsBackgroundColor(RGBA_Bytes.White);
			((Menu)this).set_MenuItemsBorderColor(RGBA_Bytes.Gray);
			MenuItemsPadding = new BorderDouble(10.0, 10.0, 10.0, 10.0);
		}

		public override void OnDraw(Graphics2D graphics2D)
		{
			((GuiWidget)this).OnDraw(graphics2D);
			DrawBorder(graphics2D);
			if (DrawDirectionalArrow)
			{
				DoDrawDirectionalArrow(graphics2D);
			}
		}

		protected virtual void DoDrawDirectionalArrow(Graphics2D graphics2D)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			PathStorage val = new PathStorage();
			if ((int)((Menu)this).get_MenuDirection() == 1)
			{
				val.MoveTo(-4.0, 0.0);
				val.LineTo(4.0, 0.0);
				val.LineTo(0.0, -5.0);
			}
			else
			{
				if ((int)((Menu)this).get_MenuDirection() != 0)
				{
					throw new NotImplementedException("Pulldown direction has not been implemented");
				}
				val.MoveTo(-4.0, -5.0);
				val.LineTo(4.0, -5.0);
				val.LineTo(0.0, 0.0);
			}
			if ((int)((GuiWidget)this).get_UnderMouseState() != 0)
			{
				graphics2D.Render((IVertexSource)(object)val, ((GuiWidget)this).get_LocalBounds().Right - 10.0, ((GuiWidget)this).get_LocalBounds().Bottom + ((GuiWidget)this).get_Height() - 4.0, (IColorType)(object)NormalArrowColor);
			}
			else
			{
				graphics2D.Render((IVertexSource)(object)val, ((GuiWidget)this).get_LocalBounds().Right - 10.0, ((GuiWidget)this).get_LocalBounds().Bottom + ((GuiWidget)this).get_Height() - 4.0, (IColorType)(object)HoverArrowColor);
			}
		}

		private void DrawBorder(Graphics2D graphics2D)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			if (BorderWidth > 0)
			{
				if (BorderWidth == 1)
				{
					graphics2D.Rectangle(localBounds, BorderColor, 1.0);
					return;
				}
				Stroke val = new Stroke((IVertexSource)new RoundedRect(((GuiWidget)this).get_LocalBounds(), 0.0), (double)BorderWidth);
				graphics2D.Render((IVertexSource)(object)val, (IColorType)(object)BorderColor);
			}
		}

		public MenuItem AddItem(string name, string value = null, double pointSize = 12.0)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			if (value == null)
			{
				value = name;
			}
			if (mainControlWidget.get_Text() != "")
			{
				mainControlWidget.set_Margin(MenuItemsPadding);
			}
			MenuItemColorStatesView val = new MenuItemColorStatesView(name);
			val.set_NormalBackgroundColor(((Menu)this).get_MenuItemsBackgroundColor());
			val.set_OverBackgroundColor(((Menu)this).get_MenuItemsBackgroundHoverColor());
			val.set_NormalTextColor(((Menu)this).get_MenuItemsTextColor());
			val.set_OverTextColor(((Menu)this).get_MenuItemsTextHoverColor());
			val.set_DisabledTextColor(RGBA_Bytes.Gray);
			val.set_PointSize(pointSize);
			((GuiWidget)val).set_Padding(MenuItemsPadding);
			MenuItem val2 = new MenuItem((GuiWidget)val, value);
			((GuiWidget)val2).set_Text(name);
			((GuiWidget)val2).set_Name(name + " Menu Item");
			((Collection<MenuItem>)(object)base.MenuItems).Add(val2);
			return val2;
		}
	}
}
