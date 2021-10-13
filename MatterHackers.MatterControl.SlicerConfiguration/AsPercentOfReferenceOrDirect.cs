namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class AsPercentOfReferenceOrDirect : MappedSetting
	{
		private string originalReference;

		private double scale;

		public override string Value
		{
			get
			{
				double num = 0.0;
				if (base.Value.Contains("%"))
				{
					string textValue = base.Value.Replace("%", "");
					double num2 = ParseDouble(textValue) / 100.0;
					string value = ActiveSliceSettings.Instance.GetValue(originalReference);
					num = ParseDouble(value) * num2;
				}
				else
				{
					num = ParseDouble(base.Value);
				}
				if (num == 0.0)
				{
					num = ParseDouble(ActiveSliceSettings.Instance.GetValue(originalReference));
				}
				return (num * scale).ToString();
			}
		}

		public AsPercentOfReferenceOrDirect(string canonicalSettingsName, string exportedName, string originalReference, double scale = 1.0)
			: base(canonicalSettingsName, exportedName)
		{
			this.scale = scale;
			this.originalReference = originalReference;
		}
	}
}
