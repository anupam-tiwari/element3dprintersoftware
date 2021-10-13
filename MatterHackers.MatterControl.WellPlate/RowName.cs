using System;
using System.Collections.Generic;

namespace MatterHackers.MatterControl.WellPlate
{
	public static class RowName
	{
		private static readonly string startChar = "A";

		private static readonly string endChar = "Z";

		private static Dictionary<int, string> indexToNameMap = new Dictionary<int, string>();

		private static string IncrementName(string nameToIncrement)
		{
			Tuple<string, string> tuple = IncrementNameRecursive(nameToIncrement);
			if (!string.IsNullOrEmpty(tuple.Item2))
			{
				return tuple.Item2 + tuple.Item1;
			}
			return tuple.Item1;
		}

		private static Tuple<string, string> IncrementNameRecursive(string nameToIncrement)
		{
			if (nameToIncrement.Length == 1)
			{
				if (nameToIncrement == endChar)
				{
					return Tuple.Create(startChar, startChar);
				}
				return Tuple.Create(((char)(nameToIncrement[0] + 1)).ToString(), "");
			}
			Tuple<string, string> tuple = IncrementNameRecursive(nameToIncrement.Substring(1));
			if (!string.IsNullOrEmpty(tuple.Item2))
			{
				Tuple<string, string> tuple2 = IncrementNameRecursive(nameToIncrement[0].ToString());
				if (!string.IsNullOrEmpty(tuple2.Item2))
				{
					return Tuple.Create(tuple2.Item1 + tuple.Item1, tuple2.Item2);
				}
				return Tuple.Create(tuple2.Item1 + tuple.Item1, "");
			}
			return Tuple.Create(nameToIncrement[0] + tuple.Item1, "");
		}

		public static string RowIndexToName(int index)
		{
			lock (indexToNameMap)
			{
				for (int i = indexToNameMap.Count; i <= index; i++)
				{
					if (i == 0)
					{
						indexToNameMap.Add(i, startChar);
					}
					else
					{
						indexToNameMap.Add(i, IncrementName(indexToNameMap[i - 1]));
					}
				}
			}
			return indexToNameMap[index];
		}
	}
}
