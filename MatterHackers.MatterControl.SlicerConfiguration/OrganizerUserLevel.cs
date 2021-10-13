using System.Collections.Generic;

namespace MatterHackers.MatterControl.SlicerConfiguration
{
	public class OrganizerUserLevel
	{
		public List<OrganizerCategory> CategoriesList = new List<OrganizerCategory>();

		public string Name
		{
			get;
			set;
		}

		public OrganizerUserLevel(string userLevelName)
		{
			Name = userLevelName;
		}
	}
}
