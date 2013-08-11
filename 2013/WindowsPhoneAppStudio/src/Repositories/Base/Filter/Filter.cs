using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WPAppStudio.Shared;

namespace WPAppStudio.Repositories.Base
{
    public class Filter<T>
    {
        public static ObservableCollection<T> FilterCollection(FilterSpecification filter, IEnumerable<T> data)
        {
			IQueryable<T> query = data.AsQueryable();				
			foreach (var predicate in filter.Predicates)
			{
				Func<T, bool> expression;
				var predicateAux = predicate;
				switch (predicate.Operator)
				{
					case ColumnOperatorEnum.Contains:
						expression = x => predicateAux.GetFieldValue(x).ToLower().Contains(predicateAux.Value.ToString().ToLower());
						break;
					case ColumnOperatorEnum.StartsWith:
						expression = x => predicateAux.GetFieldValue(x).ToLower().StartsWith(predicateAux.Value.ToString().ToLower());
						break;
					case ColumnOperatorEnum.GreaterThan:
						expression = x => String.Compare(predicateAux.GetFieldValue(x).ToLower(), predicateAux.Value.ToString().ToLower(), StringComparison.Ordinal) > 0;
						break;
					case ColumnOperatorEnum.LessThan:
						expression = x => String.Compare(predicateAux.GetFieldValue(x).ToLower(), predicateAux.Value.ToString().ToLower(), StringComparison.Ordinal) < 0;
						break;
					case ColumnOperatorEnum.NotEquals:
						expression = x => !predicateAux.GetFieldValue(x).Equals(predicateAux.Value.ToString(), StringComparison.InvariantCultureIgnoreCase);
						break;
					default:
						expression = x => predicateAux.GetFieldValue(x).Equals(predicateAux.Value.ToString(), StringComparison.InvariantCultureIgnoreCase);
						break;
				}
				query = query.Where(expression).AsQueryable();
			}
            return new ObservableCollection<T>(query);
        }
    }
}
