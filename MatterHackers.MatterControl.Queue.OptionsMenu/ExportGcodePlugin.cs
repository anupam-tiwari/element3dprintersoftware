using MatterHackers.MatterControl.PluginSystem;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.Queue.OptionsMenu
{
	public class ExportGcodePlugin : MatterControlPlugin
	{
		public virtual string GetButtonText()
		{
			return "";
		}

		public virtual string GetFileExtension()
		{
			return "";
		}

		public virtual string GetExtensionFilter()
		{
			return "";
		}

		public virtual void Generate(string gcodeInputPath, string x3gOutputPath)
		{
		}

		public virtual bool EnabledForCurrentPart(PrintItemWrapper printItemWrapper)
		{
			return true;
		}

		public ExportGcodePlugin()
			: this()
		{
		}
	}
}
