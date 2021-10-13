using System.Collections.Generic;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class OrganizerSubGroup
	{
		public string Name
		{
			get;
		}

		public List<SliceSettingData> SettingDataList
		{
			get;
			private set;
		} = new List<SliceSettingData>();


		public OrganizerSubGroup(string groupName)
		{
			Name = groupName;
		}
	}
}
