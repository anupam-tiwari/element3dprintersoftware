namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class MapFirstValue : MappedSetting
	{
		public override string Value
		{
			get
			{
				if (!base.Value.Contains(","))
				{
					return base.Value;
				}
				return base.Value.Split(new char[1]
				{
					','
				})[0];
			}
		}

		public MapFirstValue(string canonicalSettingsName, string exportedName)
			: base(canonicalSettingsName, exportedName)
		{
		}
	}
}
