using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public static class ProfileMigrations
	{
		public static string MigrateDocument(string filePath, int fromVersion = -1)
		{
			JObject val = JObject.Parse(File.ReadAllText(filePath));
			if (fromVersion < 201606271)
			{
				JToken obj = val.get_Item("OemProfile");
				JObject val2 = obj as JObject;
				if (val2 != null)
				{
					((JToken)val.Property("OemProfile")).Remove();
					val.set_Item("OemLayer", val2.get_Item("OemLayer"));
				}
				val.set_Item("DocumentVersion", JToken.op_Implicit(201606271));
			}
			File.WriteAllText(filePath, JsonConvert.SerializeObject((object)val, (Formatting)1));
			return filePath;
		}
	}
}
