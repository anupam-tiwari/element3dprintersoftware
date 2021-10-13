using System;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public class LibraryProviderFileSystemCreator : ILibraryCreator
	{
		private string rootPath;

		private bool useIncrementedNameDuringTypeChange;

		public string Description
		{
			get;
			set;
		}

		public string ProviderKey => "FileSystem_" + rootPath + "_Key";

		public LibraryProviderFileSystemCreator(string rootPath, string description, bool useIncrementedNameDuringTypeChange = false)
		{
			this.rootPath = rootPath;
			Description = description;
			this.useIncrementedNameDuringTypeChange = useIncrementedNameDuringTypeChange;
		}

		public virtual LibraryProvider CreateLibraryProvider(LibraryProvider parentLibraryProvider, Action<LibraryProvider> setCurrentLibraryProvider)
		{
			return new LibraryProviderFileSystem(rootPath, Description, parentLibraryProvider, setCurrentLibraryProvider, useIncrementedNameDuringTypeChange);
		}
	}
}
