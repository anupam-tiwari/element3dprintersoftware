using System;
using System.Text.RegularExpressions;
using MatterHackers.MatterControl.PrinterCommunication;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class GCodeMacro
	{
		public string Name
		{
			get;
			set;
		}

		public string GCode
		{
			get;
			set;
		}

		public bool ActionGroup
		{
			get;
			set;
		}

		public DateTime LastModified
		{
			get;
			set;
		}

		public static string FixMacroName(string input)
		{
			int num = 24;
			string text = Regex.Replace(input, "\\r\\n?|\\n", " ");
			if (text.Length > num)
			{
				text = text.Substring(0, num) + "...";
			}
			return text;
		}

		public void Run()
		{
			if (PrinterConnectionAndCommunication.Instance.PrinterIsConnected)
			{
				PrinterConnectionAndCommunication.Instance.MacroStart();
				SendCommandToPrinter(GCode);
				if (GCode.Contains("; host."))
				{
					SendCommandToPrinter("\n; host.close()");
				}
			}
		}

		protected void SendCommandToPrinter(string command)
		{
			PrinterConnectionAndCommunication.Instance.SendLineToPrinterNow(command);
		}
	}
}
