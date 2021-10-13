using System;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class EditMacrosWindow : SystemWindow
	{
		public GCodeMacro ActiveMacro;

		private static EditMacrosWindow editMacrosWindow;

		public EditMacrosWindow()
			: this(560.0, 420.0)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((SystemWindow)this).set_Title("Macro Editor".Localize());
			((GuiWidget)this).set_BackgroundColor(ActiveTheme.get_Instance().get_PrimaryBackgroundColor());
			ChangeToMacroList();
			((SystemWindow)this).ShowAsSystemWindow();
			((GuiWidget)this).set_MinimumSize(new Vector2(360.0, 420.0));
		}

		public static void Show()
		{
			if (editMacrosWindow == null)
			{
				editMacrosWindow = new EditMacrosWindow();
				((GuiWidget)editMacrosWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					editMacrosWindow = null;
				});
			}
			else
			{
				((GuiWidget)editMacrosWindow).BringToFront();
			}
		}

		public void ChangeToMacroDetail(GCodeMacro macro)
		{
			ActiveMacro = macro;
			UiThread.RunOnIdle((Action)delegate
			{
				((GuiWidget)this).RemoveAllChildren();
				((GuiWidget)this).AddChild((GuiWidget)(object)new MacroDetailWidget(this), -1);
				((GuiWidget)this).Invalidate();
			});
		}

		public void ChangeToMacroList()
		{
			ActiveMacro = null;
			UiThread.RunOnIdle((Action)DoChangeToMacroList);
		}

		public void RefreshMacros()
		{
			ActiveSliceSettings.Instance.Save();
			ApplicationController.Instance.ReloadAll();
		}

		private void DoChangeToMacroList()
		{
			GuiWidget val = (GuiWidget)(object)new MacroListWidget(this);
			((GuiWidget)this).RemoveAllChildren();
			((GuiWidget)this).AddChild(val, -1);
			((GuiWidget)this).Invalidate();
		}
	}
}
