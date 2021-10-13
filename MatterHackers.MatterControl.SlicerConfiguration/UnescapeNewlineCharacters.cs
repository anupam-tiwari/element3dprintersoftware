namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class UnescapeNewlineCharacters : MappedSetting
	{
		public override string Value => base.Value.Replace("\\n", "\n");

		public UnescapeNewlineCharacters(string canonicalSettingsName, string exportedName)
			: base(canonicalSettingsName, exportedName)
		{
		}
	}
}
