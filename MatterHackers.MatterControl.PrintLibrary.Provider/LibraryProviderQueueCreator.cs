using System;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public class LibraryProviderQueueCreator : ILibraryCreator
	{
		public string ProviderKey => LibraryProviderQueue.StaticProviderKey;

		public virtual LibraryProvider CreateLibraryProvider(LibraryProvider parentLibraryProvider, Action<LibraryProvider> setCurrentLibraryProvider)
		{
			return new LibraryProviderQueue(null, setCurrentLibraryProvider, parentLibraryProvider);
		}
	}
}
