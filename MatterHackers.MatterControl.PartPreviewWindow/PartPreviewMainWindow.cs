using System;
using System.IO;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{
	public class PartPreviewMainWindow : SystemWindow
	{
		private EventHandler unregisterEvents;

		private PartPreviewContent partPreviewWidget;

		public PartPreviewMainWindow(PrintItemWrapper printItem, View3DWidget.AutoRotate autoRotate3DView, View3DWidget.OpenMode openMode = View3DWidget.OpenMode.Viewing)
			: this(750.0, 550.0)
		{
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			((SystemWindow)this).set_UseOpenGL(true);
			string arg = "Element".Localize();
			((SystemWindow)this).set_Title($"{arg}: " + Path.GetFileName(printItem.Name));
			((GuiWidget)this).set_Name("Part Preview Window");
			partPreviewWidget = new PartPreviewContent(printItem, View3DWidget.WindowMode.StandAlone, autoRotate3DView, openMode);
			((GuiWidget)partPreviewWidget).add_Closed((EventHandler<ClosedEventArgs>)delegate
			{
				((GuiWidget)this).Close();
			});
			((GuiWidget)this).AddChild((GuiWidget)(object)partPreviewWidget, -1);
			AddHandlers();
			((GuiWidget)this).set_Width(750.0);
			((GuiWidget)this).set_Height(550.0);
			((GuiWidget)this).set_MinimumSize(new Vector2(400.0, 300.0));
			((SystemWindow)this).ShowAsSystemWindow();
		}

		private void AddHandlers()
		{
			ActiveTheme.ThemeChanged.RegisterEvent((EventHandler)ThemeChanged, ref unregisterEvents);
		}

		public void ThemeChanged(object sender, EventArgs e)
		{
			((GuiWidget)this).Invalidate();
		}

		public override void OnClosed(ClosedEventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			((SystemWindow)this).OnClosed(e);
		}
	}
}
