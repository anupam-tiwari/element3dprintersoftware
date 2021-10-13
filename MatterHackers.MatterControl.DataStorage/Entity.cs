using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace MatterHackers.MatterControl.DataStorage
{
	public class Entity
	{
		protected int hashCode;

		protected bool isSaved;

		private static readonly int maxRetries = 20;

		private IEnumerable<PropertyInfo> properties;

		private int retryCount;

		[PrimaryKey]
		[AutoIncrement]
		public int Id
		{
			get;
			set;
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			[CompilerGenerated]
			add
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				PropertyChangedEventHandler val = this.PropertyChanged;
				PropertyChangedEventHandler val2;
				do
				{
					val2 = val;
					PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
					val = Interlocked.CompareExchange(ref System.Runtime.CompilerServices.Unsafe.As<PropertyChangedEventHandler, PropertyChangedEventHandler>(ref this.PropertyChanged), value2, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				PropertyChangedEventHandler val = this.PropertyChanged;
				PropertyChangedEventHandler val2;
				do
				{
					val2 = val;
					PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
					val = Interlocked.CompareExchange(ref System.Runtime.CompilerServices.Unsafe.As<PropertyChangedEventHandler, PropertyChangedEventHandler>(ref this.PropertyChanged), value2, val2);
				}
				while (val != val2);
			}
		}

		public Entity()
		{
			isSaved = false;
		}

		public virtual void Commit()
		{
			if (Id == 0)
			{
				TryHandleInsert();
			}
			else
			{
				TryHandleUpdate();
			}
		}

		public void Delete()
		{
			Datastore.Instance.dbSQLite.Delete(this);
		}

		public override int GetHashCode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (hashCode == 0)
			{
				properties = Enumerable.Where<PropertyInfo>(Enumerable.Where<PropertyInfo>(Enumerable.Where<PropertyInfo>((IEnumerable<PropertyInfo>)GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public), (Func<PropertyInfo, bool>)((PropertyInfo prop) => prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int))), (Func<PropertyInfo, bool>)((PropertyInfo prop) => prop.GetIndexParameters().Length == 0)), (Func<PropertyInfo, bool>)((PropertyInfo prop) => prop.CanWrite && prop.CanRead));
				foreach (PropertyInfo property in properties)
				{
					object value = property.GetValue(this, null);
					if (value != null)
					{
						string text = value.ToString();
						if (text != null)
						{
							stringBuilder.Append(property.Name);
							stringBuilder.Append(text);
						}
					}
				}
				hashCode = stringBuilder.ToString().GetHashCode();
			}
			return hashCode;
		}

		protected void OnPropertyChanged(string name)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			hashCode = 0;
			if (propertyChanged != null)
			{
				propertyChanged.Invoke((object)this, new PropertyChangedEventArgs(name));
			}
		}

		private void TryHandleInsert()
		{
			retryCount++;
			try
			{
				if (retryCount < maxRetries)
				{
					Datastore.Instance.dbSQLite.Insert(this);
				}
			}
			catch (Exception)
			{
				Thread.Sleep(100);
				TryHandleInsert();
			}
			retryCount = 0;
		}

		private void TryHandleUpdate()
		{
			retryCount++;
			try
			{
				if (retryCount < maxRetries)
				{
					Datastore.Instance.dbSQLite.Update(this);
				}
			}
			catch (Exception)
			{
				Thread.Sleep(100);
				TryHandleUpdate();
			}
			retryCount = 0;
		}
	}
}
