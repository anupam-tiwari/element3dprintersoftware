using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MatterHackers.MatterControl.DataStorage;

namespace SQLiteWin32
{
	public class TableQuery<T> : BaseTableQuery, IEnumerable<T>, IEnumerable, ITableQuery<T>
	{
		private class CompileResult
		{
			public string CommandText
			{
				get;
				set;
			}

			public object Value
			{
				get;
				set;
			}
		}

		private Expression _where;

		private List<Ordering> _orderBys;

		private int? _limit;

		private int? _offset;

		private BaseTableQuery _joinInner;

		private Expression _joinInnerKeySelector;

		private BaseTableQuery _joinOuter;

		private Expression _joinOuterKeySelector;

		private Expression _joinSelector;

		private Expression _selector;

		private bool _deferred;

		public SQLiteConnection Connection
		{
			get;
			private set;
		}

		public TableMapping Table
		{
			get;
			private set;
		}

		private TableQuery(SQLiteConnection conn, TableMapping table)
		{
			Connection = conn;
			Table = table;
		}

		public TableQuery(SQLiteConnection conn)
		{
			Connection = conn;
			Table = Connection.GetMapping(typeof(T));
		}

		public ITableQuery<U> Clone<U>()
		{
			TableQuery<U> tableQuery = new TableQuery<U>(Connection, Table);
			tableQuery._where = _where;
			tableQuery._deferred = _deferred;
			if (_orderBys != null)
			{
				tableQuery._orderBys = new List<Ordering>(_orderBys);
			}
			tableQuery._limit = _limit;
			tableQuery._offset = _offset;
			tableQuery._joinInner = _joinInner;
			tableQuery._joinInnerKeySelector = _joinInnerKeySelector;
			tableQuery._joinOuter = _joinOuter;
			tableQuery._joinOuterKeySelector = _joinOuterKeySelector;
			tableQuery._joinSelector = _joinSelector;
			tableQuery._selector = _selector;
			return tableQuery;
		}

		public ITableQuery<T> Where(Expression<Func<T, bool>> predExpr)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			if ((int)((Expression)predExpr).get_NodeType() == 18)
			{
				Expression body = ((LambdaExpression)predExpr).get_Body();
				ITableQuery<T> tableQuery = Clone<T>();
				((TableQuery<T>)tableQuery).AddWhere(body);
				return tableQuery;
			}
			throw new NotSupportedException("Must be a predicate");
		}

		public ITableQuery<T> Take(int n)
		{
			ITableQuery<T> tableQuery = Clone<T>();
			((TableQuery<T>)tableQuery)._limit = n;
			return tableQuery;
		}

		public ITableQuery<T> Skip(int n)
		{
			ITableQuery<T> tableQuery = Clone<T>();
			((TableQuery<T>)tableQuery)._offset = n;
			return tableQuery;
		}

		public T ElementAt(int index)
		{
			return Skip(index).Take(1).First();
		}

		public ITableQuery<T> Deferred()
		{
			ITableQuery<T> tableQuery = Clone<T>();
			((TableQuery<T>)tableQuery)._deferred = true;
			return tableQuery;
		}

		public ITableQuery<T> OrderBy<U>(Expression<Func<T, U>> orderExpr)
		{
			return AddOrderBy<U>(orderExpr, asc: true);
		}

		public ITableQuery<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr)
		{
			return AddOrderBy<U>(orderExpr, asc: false);
		}

		private TableQuery<T> AddOrderBy<U>(Expression<Func<T, U>> orderExpr, bool asc)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Invalid comparison between Unknown and I4
			if ((int)((Expression)orderExpr).get_NodeType() == 18)
			{
				MemberExpression val = null;
				Expression body = ((LambdaExpression)orderExpr).get_Body();
				UnaryExpression val2 = body as UnaryExpression;
				val = ((val2 == null || (int)((Expression)val2).get_NodeType() != 10) ? (((LambdaExpression)orderExpr).get_Body() as MemberExpression) : (val2.get_Operand() as MemberExpression));
				if (val != null && (int)val.get_Expression().get_NodeType() == 38)
				{
					ITableQuery<T> tableQuery = Clone<T>();
					if (((TableQuery<T>)tableQuery)._orderBys == null)
					{
						((TableQuery<T>)tableQuery)._orderBys = new List<Ordering>();
					}
					((TableQuery<T>)tableQuery)._orderBys.Add(new Ordering
					{
						ColumnName = Table.FindColumnWithPropertyName(val.get_Member().Name).Name,
						Ascending = asc
					});
					return (TableQuery<T>)tableQuery;
				}
				throw new NotSupportedException("Order By does not support: " + orderExpr);
			}
			throw new NotSupportedException("Must be a predicate");
		}

		private void AddWhere(Expression pred)
		{
			if (_where == null)
			{
				_where = pred;
			}
			else
			{
				_where = (Expression)(object)Expression.AndAlso(_where, pred);
			}
		}

		public ITableQuery<TResult> Join<TInner, TKey, TResult>(ITableQuery<TInner> inner, Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
		{
			return new TableQuery<TResult>(Connection, Connection.GetMapping(typeof(TResult)))
			{
				_joinOuter = this,
				_joinOuterKeySelector = (Expression)(object)outerKeySelector,
				_joinInner = (TableQuery<T>)inner,
				_joinInnerKeySelector = (Expression)(object)innerKeySelector,
				_joinSelector = (Expression)(object)resultSelector
			};
		}

		public ITableQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
		{
			ITableQuery<TResult> tableQuery = Clone<TResult>();
			((TableQuery<T>)tableQuery)._selector = (Expression)(object)selector;
			return tableQuery;
		}

		private SQLiteCommand GenerateCommand(string selectionList)
		{
			if (_joinInner != null && _joinOuter != null)
			{
				throw new NotSupportedException("Joins are not supported.");
			}
			string text = "select " + selectionList + " from \"" + Table.TableName + "\"";
			List<object> list = new List<object>();
			if (_where != null)
			{
				CompileResult compileResult = CompileExpr(_where, list);
				text = text + " where " + compileResult.CommandText;
			}
			if (_orderBys != null && _orderBys.Count > 0)
			{
				string str = string.Join(", ", Enumerable.ToArray<string>(Enumerable.Select<Ordering, string>((IEnumerable<Ordering>)_orderBys, (Func<Ordering, string>)((Ordering o) => "\"" + o.ColumnName + "\"" + (o.Ascending ? "" : " desc")))));
				text = text + " order by " + str;
			}
			if (_limit.HasValue)
			{
				text = text + " limit " + _limit.Value;
			}
			if (_offset.HasValue)
			{
				if (!_limit.HasValue)
				{
					text += " limit -1 ";
				}
				text = text + " offset " + _offset.Value;
			}
			return Connection.CreateCommand(text, list.ToArray());
		}

		private CompileResult CompileExpr(Expression expr, List<object> queryArgs)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Invalid comparison between Unknown and I4
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Expected O, but got Unknown
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Invalid comparison between Unknown and I4
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Expected O, but got Unknown
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Invalid comparison between Unknown and I4
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Expected O, but got Unknown
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Invalid comparison between Unknown and I4
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Expected O, but got Unknown
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Invalid comparison between Unknown and I4
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			if (expr == null)
			{
				throw new NotSupportedException("Expression is NULL");
			}
			if (expr is BinaryExpression)
			{
				BinaryExpression val = (BinaryExpression)expr;
				CompileResult compileResult = CompileExpr(val.get_Left(), queryArgs);
				CompileResult compileResult2 = CompileExpr(val.get_Right(), queryArgs);
				string commandText = ((compileResult.CommandText == "?" && compileResult.Value == null) ? CompileNullBinaryExpression(val, compileResult2) : ((!(compileResult2.CommandText == "?") || compileResult2.Value != null) ? ("(" + compileResult.CommandText + " " + GetSqlName((Expression)(object)val) + " " + compileResult2.CommandText + ")") : CompileNullBinaryExpression(val, compileResult)));
				return new CompileResult
				{
					CommandText = commandText
				};
			}
			if ((int)expr.get_NodeType() == 6)
			{
				MethodCallExpression val2 = (MethodCallExpression)expr;
				CompileResult[] array = new CompileResult[val2.get_Arguments().Count];
				CompileResult compileResult3 = ((val2.get_Object() != null) ? CompileExpr(val2.get_Object(), queryArgs) : null);
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = CompileExpr(val2.get_Arguments()[i], queryArgs);
				}
				string text = "";
				text = ((val2.get_Method().Name == "Like" && array.Length == 2) ? ("(" + array[0].CommandText + " like " + array[1].CommandText + ")") : ((val2.get_Method().Name == "Contains" && array.Length == 2) ? ("(" + array[1].CommandText + " in " + array[0].CommandText + ")") : ((val2.get_Method().Name == "Contains" && array.Length == 1) ? ((val2.get_Object() == null || !(val2.get_Object().get_Type() == typeof(string))) ? ("(" + array[0].CommandText + " in " + compileResult3.CommandText + ")") : ("(" + compileResult3.CommandText + " like ('%' || " + array[0].CommandText + " || '%'))")) : ((val2.get_Method().Name == "StartsWith" && array.Length == 1) ? ("(" + compileResult3.CommandText + " like (" + array[0].CommandText + " || '%'))") : ((!(val2.get_Method().Name == "EndsWith") || array.Length != 1) ? (val2.get_Method().Name.ToLower() + "(" + string.Join(",", Enumerable.ToArray<string>(Enumerable.Select<CompileResult, string>((IEnumerable<CompileResult>)array, (Func<CompileResult, string>)((CompileResult a) => a.CommandText)))) + ")") : ("(" + compileResult3.CommandText + " like ('%' || " + array[0].CommandText + "))"))))));
				return new CompileResult
				{
					CommandText = text
				};
			}
			if ((int)expr.get_NodeType() == 9)
			{
				ConstantExpression val3 = (ConstantExpression)expr;
				queryArgs.Add(val3.get_Value());
				return new CompileResult
				{
					CommandText = "?",
					Value = val3.get_Value()
				};
			}
			if ((int)expr.get_NodeType() == 10)
			{
				UnaryExpression val4 = (UnaryExpression)expr;
				Type type = ((Expression)val4).get_Type();
				CompileResult compileResult4 = CompileExpr(val4.get_Operand(), queryArgs);
				return new CompileResult
				{
					CommandText = compileResult4.CommandText,
					Value = ((compileResult4.Value != null) ? ConvertTo(compileResult4.Value, type) : null)
				};
			}
			if ((int)expr.get_NodeType() == 23)
			{
				MemberExpression val5 = (MemberExpression)expr;
				if ((int)val5.get_Expression().get_NodeType() == 38)
				{
					string name = Table.FindColumnWithPropertyName(val5.get_Member().Name).Name;
					return new CompileResult
					{
						CommandText = "\"" + name + "\""
					};
				}
				object obj = null;
				if (val5.get_Expression() != null)
				{
					CompileResult compileResult5 = CompileExpr(val5.get_Expression(), queryArgs);
					if (compileResult5.Value == null)
					{
						throw new NotSupportedException("Member access failed to compile expression");
					}
					if (compileResult5.CommandText == "?")
					{
						queryArgs.RemoveAt(queryArgs.Count - 1);
					}
					obj = compileResult5.Value;
				}
				object obj2 = null;
				if (val5.get_Member().MemberType == MemberTypes.Property)
				{
					obj2 = ((PropertyInfo)val5.get_Member()).GetValue(obj, null);
				}
				else
				{
					if (val5.get_Member().MemberType != MemberTypes.Field)
					{
						throw new NotSupportedException("MemberExpr: " + val5.get_Member().MemberType);
					}
					obj2 = ((FieldInfo)val5.get_Member()).GetValue(obj);
				}
				if (obj2 != null && obj2 is IEnumerable && !(obj2 is string))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("(");
					string value = "";
					foreach (object? item in (IEnumerable)obj2)
					{
						queryArgs.Add(item);
						stringBuilder.Append(value);
						stringBuilder.Append("?");
						value = ",";
					}
					stringBuilder.Append(")");
					return new CompileResult
					{
						CommandText = stringBuilder.ToString(),
						Value = obj2
					};
				}
				queryArgs.Add(obj2);
				return new CompileResult
				{
					CommandText = "?",
					Value = obj2
				};
			}
			ExpressionType nodeType = expr.get_NodeType();
			throw new NotSupportedException("Cannot compile: " + ((object)(ExpressionType)(ref nodeType)).ToString());
		}

		private static object ConvertTo(object obj, Type t)
		{
			Type underlyingType = Nullable.GetUnderlyingType(t);
			if (underlyingType != null)
			{
				if (obj == null)
				{
					return null;
				}
				return Convert.ChangeType(obj, underlyingType);
			}
			return Convert.ChangeType(obj, t);
		}

		private string CompileNullBinaryExpression(BinaryExpression expression, CompileResult parameter)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if ((int)((Expression)expression).get_NodeType() == 13)
			{
				return "(" + parameter.CommandText + " is ?)";
			}
			if ((int)((Expression)expression).get_NodeType() == 35)
			{
				return "(" + parameter.CommandText + " is not ?)";
			}
			ExpressionType nodeType = ((Expression)expression).get_NodeType();
			throw new NotSupportedException("Cannot compile Null-BinaryExpression with type " + ((object)(ExpressionType)(ref nodeType)).ToString());
		}

		private string GetSqlName(Expression expr)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Invalid comparison between Unknown and I4
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Invalid comparison between Unknown and I4
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Invalid comparison between Unknown and I4
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Invalid comparison between Unknown and I4
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Invalid comparison between Unknown and I4
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Invalid comparison between Unknown and I4
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Invalid comparison between Unknown and I4
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Invalid comparison between Unknown and I4
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Invalid comparison between Unknown and I4
			ExpressionType nodeType = expr.get_NodeType();
			if ((int)nodeType == 15)
			{
				return ">";
			}
			if ((int)nodeType == 16)
			{
				return ">=";
			}
			if ((int)nodeType == 20)
			{
				return "<";
			}
			if ((int)nodeType == 21)
			{
				return "<=";
			}
			if ((int)nodeType == 2)
			{
				return "&";
			}
			if ((int)nodeType == 3)
			{
				return "and";
			}
			if ((int)nodeType == 36)
			{
				return "|";
			}
			if ((int)nodeType == 37)
			{
				return "or";
			}
			if ((int)nodeType == 13)
			{
				return "=";
			}
			if ((int)nodeType == 35)
			{
				return "!=";
			}
			throw new NotSupportedException("Cannot get SQL for: " + ((object)(ExpressionType)(ref nodeType)).ToString());
		}

		public int Count()
		{
			return GenerateCommand("count(*)").ExecuteScalar<int>();
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (!_deferred)
			{
				return GenerateCommand("*").ExecuteQuery<T>().GetEnumerator();
			}
			return GenerateCommand("*").ExecuteDeferredQuery<T>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T First()
		{
			return Enumerable.First<T>((IEnumerable<T>)Enumerable.ToList<T>((IEnumerable<T>)(TableQuery<T>)Take(1)));
		}

		public T FirstOrDefault()
		{
			return Enumerable.FirstOrDefault<T>((IEnumerable<T>)Enumerable.ToList<T>((IEnumerable<T>)(TableQuery<T>)Take(1)));
		}
	}
}
