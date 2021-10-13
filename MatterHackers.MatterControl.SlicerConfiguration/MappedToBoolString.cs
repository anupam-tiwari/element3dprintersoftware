namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class MappedToBoolString : MappedSetting
	{
		public override string Value
		{
			get
			{
				if (!(base.Value == "1"))
				{
					return "False";
				}
				return "True";
			}
		}

		public MappedToBoolString(string canonicalSettingsName, string exportedName)
			: base(canonicalSettingsName, exportedName)
		{
		}
	}
}
