namespace Earthshine
{
	using System;
	using System.Linq;
	using System.Collections.Generic;

	public static class Extensions
	{
		public static string DictListJoin<T, TU>(
			this Dictionary<T, List<TU>> dict,
			string listSeparator,
			string elementSeparator,
			Func<TU, object> valueSelector)
		{
			// this should be illegal, I love it :')
			return string.Join(listSeparator,
				dict.Keys.Select(key =>
					string.Join(elementSeparator,
						dict[key].Select(valueSelector))));
		}

		public static string DictListJoin<T, TU>(
			this Dictionary<T, List<TU>> dict,
			string listSeparator,
			string elementSeparator,
			Func<TU, float> orderBy,
			Func<TU, object> valueSelector)
		{
			// this should be illegal, I love it :')
			return string.Join(listSeparator,
				dict.Keys.Select(key =>
					string.Join(elementSeparator,
						dict[key]
							.OrderBy(orderBy)
							.Select(valueSelector))));
		}
	}
}