using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrintLibrary.Provider;

namespace MatterHackers.MatterControl.CustomWidgets.LibrarySelector
{
	public class FolderBreadCrumbWidget : FlowLayoutWidget
	{
		private static TextImageButtonFactory navigationButtonFactory = new TextImageButtonFactory();

		private Action<LibraryProvider> SwitchToLibraryProvider;

		public FolderBreadCrumbWidget(Action<LibraryProvider> SwitchToLibraryProvider, LibraryProvider currentLibraryProvider)
			: this((FlowDirection)0)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			FolderBreadCrumbWidget folderBreadCrumbWidget = this;
			((GuiWidget)this).set_Name("FolderBreadCrumbWidget");
			this.SwitchToLibraryProvider = SwitchToLibraryProvider;
			UiThread.RunOnIdle((Action)delegate
			{
				folderBreadCrumbWidget.SetBreadCrumbs(null, currentLibraryProvider);
			});
			navigationButtonFactory.normalTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			navigationButtonFactory.hoverTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			navigationButtonFactory.pressedTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			navigationButtonFactory.disabledTextColor = ActiveTheme.get_Instance().get_PrimaryTextColor();
			navigationButtonFactory.disabledFillColor = navigationButtonFactory.normalFillColor;
			navigationButtonFactory.Margin = new BorderDouble(10.0, 0.0);
			navigationButtonFactory.borderWidth = 0.0;
			((GuiWidget)this).set_HAnchor((HAnchor)5);
		}

		public void SetBreadCrumbs(LibraryProvider previousLibraryProvider, LibraryProvider currentLibraryProvider)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Expected O, but got Unknown
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Expected O, but got Unknown
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			LibraryProvider libraryProvider = currentLibraryProvider;
			((GuiWidget)this).CloseAllChildren();
			List<LibraryProvider> list = new List<LibraryProvider>();
			while (currentLibraryProvider != null)
			{
				list.Add(currentLibraryProvider);
				currentLibraryProvider = currentLibraryProvider.ParentLibraryProvider;
			}
			bool flag = !string.IsNullOrEmpty(libraryProvider.KeywordFilter);
			bool flag2 = true;
			for (int num = list.Count - 1; num >= 0; num--)
			{
				LibraryProvider parentLibraryProvider = list[num];
				if (!flag2)
				{
					GuiWidget val = (GuiWidget)new TextWidget(">", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
					val.set_VAnchor((VAnchor)2);
					val.set_Margin(new BorderDouble(0.0));
					((GuiWidget)this).AddChild(val, -1);
				}
				Button val2 = navigationButtonFactory.Generate(parentLibraryProvider.Name);
				((GuiWidget)val2).set_Name("Bread Crumb Button " + parentLibraryProvider.Name);
				if (flag2)
				{
					((GuiWidget)val2).set_Margin(new BorderDouble(0.0, 0.0, 3.0, 0.0));
				}
				else
				{
					((GuiWidget)val2).set_Margin(new BorderDouble(3.0, 0.0));
				}
				((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)delegate
				{
					UiThread.RunOnIdle((Action)delegate
					{
						SwitchToLibraryProvider(parentLibraryProvider);
					});
				});
				((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
				flag2 = false;
			}
			if (flag)
			{
				GuiWidget val3 = (GuiWidget)new TextWidget(">", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
				val3.set_VAnchor((VAnchor)2);
				val3.set_Margin(new BorderDouble(0.0));
				((GuiWidget)this).AddChild(val3, -1);
				Button val4 = null;
				val4 = ((!UserSettings.Instance.IsTouchScreen) ? navigationButtonFactory.Generate("Search Results".Localize(), "icon_search_24x24.png") : navigationButtonFactory.Generate("Search Results".Localize(), "icon_search_32x32.png"));
				((GuiWidget)val4).set_Name("Bread Crumb Button Search Results");
				((GuiWidget)val4).set_Margin(new BorderDouble(3.0, 0.0));
				((GuiWidget)this).AddChild((GuiWidget)(object)val4, -1);
			}
			if (((GuiWidget)this).get_Parent() == null || !(((GuiWidget)this).get_Width() > 0.0) || ((Collection<GuiWidget>)(object)((GuiWidget)this).get_Children()).Count <= 4)
			{
				return;
			}
			RectangleDouble childrenBoundsIncludingMargins = ((GuiWidget)this).GetChildrenBoundsIncludingMargins(false, (Func<GuiWidget, GuiWidget, bool>)null);
			if (!(((RectangleDouble)(ref childrenBoundsIncludingMargins)).get_Width() > ((GuiWidget)this).get_Width() - 20.0))
			{
				return;
			}
			((GuiWidget)this).RemoveChild(1);
			GuiWidget val5 = (GuiWidget)new TextWidget("...", 0.0, 0.0, 12.0, (Justification)0, ActiveTheme.get_Instance().get_PrimaryTextColor(), true, false, default(RGBA_Bytes), (TypeFace)null);
			val5.set_VAnchor((VAnchor)2);
			val5.set_Margin(new BorderDouble(3.0, 0.0));
			((GuiWidget)this).AddChild(val5, 1);
			while (true)
			{
				childrenBoundsIncludingMargins = ((GuiWidget)this).GetChildrenBoundsIncludingMargins(false, (Func<GuiWidget, GuiWidget, bool>)null);
				if (((RectangleDouble)(ref childrenBoundsIncludingMargins)).get_Width() > ((GuiWidget)this).get_Width() - 20.0 && ((Collection<GuiWidget>)(object)((GuiWidget)this).get_Children()).Count > 4)
				{
					((GuiWidget)this).RemoveChild(3);
					((GuiWidget)this).RemoveChild(2);
					continue;
				}
				break;
			}
		}
	}
}
