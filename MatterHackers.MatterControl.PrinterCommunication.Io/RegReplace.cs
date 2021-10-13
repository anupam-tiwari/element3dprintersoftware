using System.Text.RegularExpressions;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class RegReplace
	{
		public Regex Regex
		{
			get;
			set;
		}

		public string Replacement
		{
			get;
			set;
		}
	}
}
