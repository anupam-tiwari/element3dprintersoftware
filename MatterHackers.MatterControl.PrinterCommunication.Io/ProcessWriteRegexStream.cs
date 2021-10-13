using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MatterHackers.MatterControl.SlicerConfiguration;

namespace MatterHackers.MatterControl.PrinterCommunication.Io
{
	public class ProcessWriteRegexStream : GCodeStreamProxy
	{
		private static Regex getQuotedParts = new Regex("([\"'])(\\\\?.)*?\\1", (RegexOptions)8);

		private QueuedCommandsStream queueStream;

		private static string write_regex = "";

		private static List<RegReplace> WriteLineReplacements = new List<RegReplace>();

		public ProcessWriteRegexStream(GCodeStream internalStream, QueuedCommandsStream queueStream)
			: base(internalStream)
		{
			this.queueStream = queueStream;
		}

		public override string ReadLine()
		{
			string text = base.ReadLine();
			if (text == null)
			{
				return null;
			}
			List<string> list = ProcessWriteRegEx(text);
			for (int i = 1; i < list.Count; i++)
			{
				queueStream.Add(list[i]);
			}
			return list[0];
		}

		public static List<string> ProcessWriteRegEx(string lineToWrite)
		{
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Expected O, but got Unknown
			lock (WriteLineReplacements)
			{
				if (write_regex != ActiveSliceSettings.Instance.GetValue("write_regex"))
				{
					WriteLineReplacements.Clear();
					string text = "\\n";
					write_regex = ActiveSliceSettings.Instance.GetValue("write_regex");
					string[] array = write_regex.Split(new string[1]
					{
						text
					}, StringSplitOptions.RemoveEmptyEntries);
					foreach (string text2 in array)
					{
						MatchCollection val = getQuotedParts.Matches(text2);
						if (val.get_Count() == 2)
						{
							string text3 = ((Capture)val.get_Item(0)).get_Value().Substring(1, ((Capture)val.get_Item(0)).get_Value().Length - 2);
							string replacement = ((Capture)val.get_Item(1)).get_Value().Substring(1, ((Capture)val.get_Item(1)).get_Value().Length - 2);
							WriteLineReplacements.Add(new RegReplace
							{
								Regex = new Regex(text3, (RegexOptions)8),
								Replacement = replacement
							});
						}
					}
				}
			}
			List<string> list = new List<string>();
			list.Add(lineToWrite);
			List<string> list2 = new List<string>();
			for (int j = 0; j < list.Count; j++)
			{
				foreach (RegReplace writeLineReplacement in WriteLineReplacements)
				{
					string[] array2 = writeLineReplacement.Replacement.Split(new char[1]
					{
						','
					});
					if (array2.Length != 0 && writeLineReplacement.Regex.IsMatch(lineToWrite))
					{
						string text5 = (list[j] = writeLineReplacement.Regex.Replace(lineToWrite, array2[0]));
						for (int k = 1; k < array2.Length; k++)
						{
							list2.Add(array2[k]);
						}
					}
				}
			}
			list.AddRange(list2);
			return list;
		}
	}
}
