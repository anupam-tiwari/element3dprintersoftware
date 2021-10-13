using System;
using System.Collections.Generic;
using MatterHackers.MatterControl.DataStorage;

namespace MatterHackers.MatterControl
{
	internal class ManifestFile
	{
		private List<PrintItem> projectFiles;

		private string projectName = "Test Project";

		private string projectDateCreated;

		public List<PrintItem> ProjectFiles
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

		public ManifestFile()
		{
			projectDateCreated = DateTime.Now.ToString("s");
		}
	}
}
