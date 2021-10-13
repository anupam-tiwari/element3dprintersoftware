using MatterHackers.MatterControl;

namespace MatterHackers.Localizations
{
	public static class LocalizedString
	{
		private static TranslationMap MatterControlTranslationMap;

		private static readonly object syncRoot;

		static LocalizedString()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			syncRoot = new object();
			lock (syncRoot)
			{
				if (MatterControlTranslationMap == null)
				{
					MatterControlTranslationMap = new TranslationMap("Translations", UserSettings.Instance.Language);
				}
			}
		}

		public static string Get(string englishText)
		{
			return MatterControlTranslationMap.Translate(englishText);
		}

		public static void ResetTranslationMap()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			MatterControlTranslationMap = new TranslationMap("Translations", UserSettings.Instance.Language);
		}

		public static string Localize(this string englishString)
		{
			return Get(englishString);
		}
	}
}
