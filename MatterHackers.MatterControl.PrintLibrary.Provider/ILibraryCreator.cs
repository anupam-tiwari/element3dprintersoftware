using System;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public interface ILibraryCreator
	{
		string ProviderKey
		{
			get;
		}

		LibraryProvider CreateLibraryProvider(LibraryProvider parentLibraryProvider, Action<LibraryProvider> setCurrentLibraryProvider);
	}
}
