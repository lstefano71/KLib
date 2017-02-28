using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KLib.Extensions
{
	static class Extensions {
		public static IEnumerable<T> Use<T>(this T obj) where T : IDisposable
		{
			try {
				yield return obj;
			} finally {
				if (obj != null)
					obj.Dispose();
			}
		}

		public static IEnumerable<Tuple<Int32, X>> WithIndex<X>(this IEnumerable<X> lhs)
		{
			return lhs.WithIndex(0);
		}

		public static IEnumerable<Tuple<Int32, X>> WithIndex<X>(this IEnumerable<X> lhs, Int32 initial)
		{
			Int32 index = initial - 1;

			return lhs.Select(x => new Tuple<Int32, X>(++index, x));
		}

		public static IEnumerable<Tuple<int,string>> FromReader(this TextReader input)
		{
			string s;
			int line = 0;
			while((s=input.ReadLine()) != null) {
				yield return Tuple.Create(++line,s);
			}
		}
	}
}
