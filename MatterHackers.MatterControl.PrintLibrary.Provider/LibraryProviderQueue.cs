using System;
using System.Threading.Tasks;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintQueue;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public class LibraryProviderQueue : LibraryProvider
	{
		private static LibraryProviderQueue instance;

		private EventHandler unregisterEvent;

		public static LibraryProvider Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new LibraryProviderQueue(null, null, null);
				}
				return instance;
			}
		}

		public static string StaticProviderKey => "LibraryProviderQueueKey";

		public override bool CanShare => false;

		public override int CollectionCount => 0;

		public override int ItemCount => QueueData.Instance.ItemCount;

		public override string ProviderKey => StaticProviderKey;

		public LibraryProviderQueue(PrintItemCollection baseLibraryCollection, Action<LibraryProvider> setCurrentLibraryProvider, LibraryProvider parentLibraryProvider)
			: base(parentLibraryProvider, setCurrentLibraryProvider)
		{
			QueueData.Instance.ItemAdded.RegisterEvent((EventHandler)delegate
			{
				OnDataReloaded(null);
			}, ref unregisterEvent);
			base.Name = "Print Queue";
		}

		public override int GetCollectionItemCount(int collectionIndex)
		{
			return base.GetCollectionItemCount(collectionIndex);
		}

		public override PrintItemCollection GetCollectionItem(int collectionIndex)
		{
			throw new NotImplementedException();
		}

		public override string GetPrintItemName(int itemIndex)
		{
			return QueueData.Instance.GetItemName(itemIndex);
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
			QueueData.Instance.AddItem(itemToAdd);
		}

		public void AddItem(PrintItemWrapper item, int indexToInsert = -1)
		{
			QueueData.Instance.AddItem(item, indexToInsert);
		}

		public override Task<PrintItemWrapper> GetPrintItemWrapperAsync(int index)
		{
			return Task.FromResult(QueueData.Instance.GetPrintItemWrapper(index));
		}

		public override LibraryProvider GetProviderForCollection(PrintItemCollection collection)
		{
			return new LibraryProviderQueue(collection, base.SetCurrentLibraryProvider, this);
		}

		public override void RemoveCollection(int collectionIndexToRemove)
		{
		}

		public override void RemoveItem(int itemToRemoveIndex)
		{
			QueueData.Instance.RemoveAt(itemToRemoveIndex);
			OnDataReloaded(null);
		}
	}
}
