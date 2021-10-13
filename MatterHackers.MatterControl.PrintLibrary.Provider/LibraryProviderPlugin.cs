using System;
using MatterHackers.Agg.Image;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public class LibraryProviderPlugin : ILibraryCreator
	{
		public virtual string ProviderKey
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual LibraryProvider CreateLibraryProvider(LibraryProvider parentLibraryProvider, Action<LibraryProvider> setCurrentLibraryProvider)
		{
			throw new NotImplementedException();
		}

		public virtual void ForceVisible()
		{
		}

		public virtual bool ShouldBeShown()
		{
			return true;
		}

		public virtual ImageBuffer GetFolderImage()
		{
			return LibraryProvider.NormalFolderImage;
		}
	}
}
