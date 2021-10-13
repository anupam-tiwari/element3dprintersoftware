using System.Collections.Generic;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class OrganizerGroup
	{
		public string Name
		{
			get;
		}

		public List<OrganizerSubGroup> SubGroupsList
		{
			get;
			set;
		} = new List<OrganizerSubGroup>();


		public OrganizerGroup(string displayName)
		{
			Name = displayName;
		}
	}
}
