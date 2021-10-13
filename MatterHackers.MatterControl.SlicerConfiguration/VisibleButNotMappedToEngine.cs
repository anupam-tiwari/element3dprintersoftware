namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class VisibleButNotMappedToEngine : MappedSetting
	{
		public override string Value => null;

		public VisibleButNotMappedToEngine(string canonicalSettingsName)
			: base(canonicalSettingsName, "")
		{
		}
	}
}
