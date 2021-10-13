using System;
using MatterHackers.Localizations;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public class LibraryProviderSQLiteCreator : ILibraryCreator
	{
		public string ProviderKey => LibraryProviderSQLite.StaticProviderKey;

		public virtual LibraryProvider CreateLibraryProvider(LibraryProvider parentLibraryProvider, Action<LibraryProvider> setCurrentLibraryProvider)
		{
			return new LibraryProviderSQLite(null, setCurrentLibraryProvider, parentLibraryProvider, "Local Library".Localize());
		}
	}
}
