using System.Globalization;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.PrinterCommunication
{
	public static class ExtensionMethods
	{
		private static TextInfo textInfo = new CultureInfo("en-US", useUserOverride: false).TextInfo;

		public static string GetFriendlyName(this PrintItemWrapper printItemWrapper)
		{
			if (printItemWrapper == null || printItemWrapper.Name == null)
			{
				return "";
			}
			return textInfo?.ToTitleCase(printItemWrapper.Name.Replace('_', ' '));
		}
	}
}
