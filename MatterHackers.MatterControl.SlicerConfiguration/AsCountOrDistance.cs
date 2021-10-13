namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class AsCountOrDistance : MappedSetting
	{
		private string keyToUseAsDenominatorForCount;

		public override string Value
		{
			get
			{
				if (base.Value.Contains("mm"))
				{
					string textValue = base.Value.Replace("mm", "");
					string value = ActiveSliceSettings.Instance.GetValue(keyToUseAsDenominatorForCount);
					double num = ParseDouble(value, 1.0);
					return ((int)(ParseDouble(textValue) / num + 0.5)).ToString();
				}
				return base.Value;
			}
		}

		public AsCountOrDistance(string canonicalSettingsName, string exportedName, string keyToUseAsDenominatorForCount)
			: base(canonicalSettingsName, exportedName)
		{
			this.keyToUseAsDenominatorForCount = keyToUseAsDenominatorForCount;
		}
	}
}
