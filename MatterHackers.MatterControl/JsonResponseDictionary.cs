using System.Collections.Generic;

namespace MatterHackers.MatterControl
{
	public class JsonResponseDictionary : Dictionary<string, string>
	{
		public string get(string key)
		{
			TryGetValue(key, out var value);
			return value;
		}
	}
}
