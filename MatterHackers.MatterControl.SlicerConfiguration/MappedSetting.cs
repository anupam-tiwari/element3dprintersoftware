namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class MappedSetting
	{
		public string ExportedName
		{
			get;
		}

		public string CanonicalSettingsName
		{
			get;
		}

		public virtual string Value => ActiveSliceSettings.Instance.GetValue(CanonicalSettingsName);

		public MappedSetting(string canonicalSettingsName, string exportedName)
		{
			CanonicalSettingsName = canonicalSettingsName;
			ExportedName = exportedName;
		}

		public double ParseDouble(string textValue, double valueOnError = 0.0)
		{
			if (!double.TryParse(textValue, out var result))
			{
				return valueOnError;
			}
			return result;
		}

		public double ParseDoubleFromRawValue(string canonicalSettingsName, double valueOnError = 0.0)
		{
			return ParseDouble(ActiveSliceSettings.Instance.GetValue(canonicalSettingsName), valueOnError);
		}
	}
}
