using System.Collections.Generic;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class OrganizerCategory
	{
		public string Name
		{
			get;
			set;
		}

		public List<OrganizerGroup> GroupsList
		{
			get;
			set;
		} = new List<OrganizerGroup>();


		public OrganizerCategory(string categoryName)
		{
			Name = categoryName;
		}
	}
}
