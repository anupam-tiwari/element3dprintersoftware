using System;
using System.Collections.Generic;

namespace MatterHackers.MatterControl
{
	internal class Project
	{
		private List<ManifestItem> projectFiles;

		private string projectName = "Test Project";

		private string projectDateCreated;

		public List<ManifestItem> ProjectFiles
		{
			get
			{
				return projectFiles;
			}
			set
			{
				projectFiles = value;
			}
		}

		public string ProjectName
		{
			get
			{
				return projectName;
			}
			set
			{
				projectName = value;
			}
		}

		public string ProjectDateCreated
		{
			get
			{
				return projectDateCreated;
			}
			set
			{
				projectDateCreated = value;
			}
		}

		public Project()
		{
			projectDateCreated = DateTime.Now.ToString("s");
		}
	}
}
