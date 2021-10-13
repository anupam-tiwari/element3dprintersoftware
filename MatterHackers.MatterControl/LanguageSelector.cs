using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl
{
	public class LanguageSelector : DropDownList
	{
		private Dictionary<string, string> languageDict;

		public LanguageSelector()
			: this("Default", (Direction)1, 0.0, false)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			RectangleDouble localBounds = ((GuiWidget)this).get_LocalBounds();
			double width = ((RectangleDouble)(ref localBounds)).get_Width();
			localBounds = ((GuiWidget)this).get_LocalBounds();
			((GuiWidget)this).set_MinimumSize(new Vector2(width, ((RectangleDouble)(ref localBounds)).get_Height()));
			CreateLanguageDict();
			string a = UserSettings.Instance.get("Language");
			foreach (KeyValuePair<string, string> item in languageDict)
			{
				((DropDownList)this).AddItem(item.Key, item.Value, 12.0);
			}
			foreach (KeyValuePair<string, string> item2 in languageDict)
			{
				if (a == item2.Value)
				{
					((DropDownList)this).set_SelectedLabel(item2.Key);
					break;
				}
			}
		}

		private void CreateLanguageDict()
		{
			languageDict = new Dictionary<string, string>
			{
				["Default"] = "EN",
				["English"] = "EN",
				["Čeština"] = "CS",
				["Deutsch"] = "DE",
				["Dansk"] = "DA",
				["Español"] = "ES",
				["Français"] = "FR",
				["Italiano"] = "IT",
				["ελληνικά"] = "EL",
				["Norsk"] = "NO",
				["Polski"] = "PL",
				["Русский"] = "RU",
				["Română"] = "RO",
				["Türkçe"] = "TR",
				["Vlaams"] = "NL"
			};
		}
	}
}
