using System;
using System.Collections.Generic;

namespace MatterHackers.MatterControl.Utilities
{
	public class EventList<T> : List<T>
	{
		public EventHandler ListChanged;

		public new void Add(T item)
		{
			base.Add(item);
			ListChanged?.Invoke(this, null);
		}

		public new void AddRange(IEnumerable<T> collection)
		{
			base.AddRange(collection);
			ListChanged?.Invoke(this, null);
		}

		public new bool Remove(T item)
		{
			bool result = base.Remove(item);
			EventHandler listChanged = ListChanged;
			if (listChanged != null)
			{
				listChanged(this, null);
				return result;
			}
			return result;
		}

		public new int RemoveAll(Predicate<T> match)
		{
			int result = base.RemoveAll(match);
			EventHandler listChanged = ListChanged;
			if (listChanged != null)
			{
				listChanged(this, null);
				return result;
			}
			return result;
		}

		public new void RemoveAt(int index)
		{
			base.RemoveAt(index);
			ListChanged?.Invoke(this, null);
		}

		public new void RemoveRange(int index, int count)
		{
			base.RemoveRange(index, count);
			ListChanged?.Invoke(this, null);
		}

		public new void Clear()
		{
			base.Clear();
			ListChanged?.Invoke(this, null);
		}
	}
}
