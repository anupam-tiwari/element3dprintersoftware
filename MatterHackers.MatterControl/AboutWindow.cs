using System;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl
{
	public class AboutWindow : SystemWindow
	{
		private static AboutWindow aboutWindow;

		private TextImageButtonFactory textImageButtonFactory = new TextImageButtonFactory();

		public AboutWindow()
			: this(500.0, 340.0)
		{
			GuiWidget val = (GuiWidget)(object)new AboutWidget();
			val.AnchorAll();
			((GuiWidget)this).AddChild(val, -1);
			Button val2 = textImageButtonFactory.Generate("Close".Localize());
			((GuiWidget)val2).add_Click((EventHandler<MouseEventArgs>)delegate
			{
				CancelButton_Click();
			});
			((GuiWidget)val2).set_HAnchor((HAnchor)4);
			((GuiWidget)this).AddChild((GuiWidget)(object)val2, -1);
			((SystemWindow)this).set_Title("About Element".Localize());
			((SystemWindow)this).set_AlwaysOnTopOfMain(true);
			((SystemWindow)this).ShowAsSystemWindow();
		}

		public static void Show()
		{
			if (aboutWindow == null)
			{
				aboutWindow = new AboutWindow();
				((GuiWidget)aboutWindow).add_Closed((EventHandler<ClosedEventArgs>)delegate
				{
					aboutWindow = null;
				});
			}
			else
			{
				((GuiWidget)aboutWindow).BringToFront();
			}
		}

		private void CancelButton_Click()
		{
			UiThread.RunOnIdle((Action)((GuiWidget)aboutWindow).Close);
		}
	}
}
