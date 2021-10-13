using System;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl
{
	public class ManualPrinterControls : GuiWidget
	{
		public static RootedObjectEventHandler AddPluginControls = new RootedObjectEventHandler();

		private static bool pluginsQueuedToAdd = false;

		public void AddPlugins()
		{
			AddPluginControls.CallEvents((object)this, (EventArgs)null);
			pluginsQueuedToAdd = false;
		}

		public ManualPrinterControls()
			: this()
		{
			((GuiWidget)this).AnchorAll();
			if (UserSettings.Instance.IsTouchScreen)
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)new ManualPrinterControlsTouchScreen(), -1);
			}
			else
			{
				((GuiWidget)this).AddChild((GuiWidget)(object)new ManualPrinterControlsDesktop(), -1);
			}
			if (!pluginsQueuedToAdd && ActiveSliceSettings.Instance.GetValue("include_firmware_updater") == "Simple Arduino")
			{
				UiThread.RunOnIdle((Action)AddPlugins);
				pluginsQueuedToAdd = true;
			}
		}
	}
}
