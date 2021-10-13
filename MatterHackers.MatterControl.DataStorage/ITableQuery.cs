using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MatterHackers.MatterControl.DataStorage
{
	public interface ITableQuery<T>
	{
		ITableQuery<T> Where(Expression<Func<T, bool>> predExpr);

		ITableQuery<U> Clone<U>();

		int Count();

		ITableQuery<T> Deferred();

		T ElementAt(int index);

		T First();

		T FirstOrDefault();

		IEnumerator<T> GetEnumerator();

		ITableQuery<TResult> Join<TInner, TKey, TResult>(ITableQuery<TInner> inner, Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector);

		ITableQuery<T> OrderBy<U>(Expression<Func<T, U>> orderExpr);

		ITableQuery<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr);

		ITableQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);

		ITableQuery<T> Skip(int n);

		ITableQuery<T> Take(int n);
	}
}
