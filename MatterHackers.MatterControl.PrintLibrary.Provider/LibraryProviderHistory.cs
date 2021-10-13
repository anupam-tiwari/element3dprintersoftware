using System;
using System.Threading.Tasks;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public class LibraryProviderHistory : LibraryProvider
	{
		private static LibraryProviderHistory instance;

		public static LibraryProvider Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new LibraryProviderHistory(null, null, null);
				}
				return instance;
			}
		}

		public static string StaticProviderKey => "LibraryProviderHistoryKey";

		public override int CollectionCount => 0;

		public override bool CanShare => false;

		public override int ItemCount => 10;

		public override string ProviderKey => StaticProviderKey;

		public LibraryProviderHistory(PrintItemCollection baseLibraryCollection, LibraryProvider parentLibraryProvider, Action<LibraryProvider> setCurrentLibraryProvider)
			: base(parentLibraryProvider, setCurrentLibraryProvider)
		{
			base.Name = "Print History";
		}

		public override string GetPrintItemName(int itemIndex)
		{
			return "item";
		}

		public override void RenameCollection(int collectionIndexToRename, string newName)
		{
			throw new NotImplementedException();
		}

		public override void RenameItem(int itemIndexToRename, string newName)
		{
			throw new NotImplementedException();
		}

		public override void ShareItem(int itemIndexToShare)
		{
		}

		public override void AddCollectionToLibrary(string collectionName)
		{
		}

		public override void AddItem(PrintItemWrapper itemToAdd)
		{
			throw new NotImplementedException();
		}

		public void AddItem(PrintItemWrapper item, int indexToInsert = -1)
		{
			throw new NotImplementedException();
		}

		public override PrintItemCollection GetCollectionItem(int collectionIndex)
		{
			throw new NotImplementedException();
		}

		public override Task<PrintItemWrapper> GetPrintItemWrapperAsync(int index)
		{
			throw new NotImplementedException();
		}

		public override LibraryProvider GetProviderForCollection(PrintItemCollection collection)
		{
			return new LibraryProviderHistory(collection, this, base.SetCurrentLibraryProvider);
		}

		public override void RemoveCollection(int collectionIndexToRemove)
		{
		}

		public override void RemoveItem(int itemToRemoveIndex)
		{
			throw new NotImplementedException();
		}
	}
}
