using System.Collections.Generic;

namespace MatterHackers.MatterControl.CreatorPlugins
{
	public class RegisteredCreators
	{
		private static RegisteredCreators instance;

		public List<CreatorInformation> Creators = new List<CreatorInformation>();

		public static RegisteredCreators Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new RegisteredCreators();
				}
				return instance;
			}
		}

		private RegisteredCreators()
		{
		}

		public void RegisterLaunchFunction(CreatorInformation creatorInformation)
		{
			Creators.Add(creatorInformation);
		}
	}
}
