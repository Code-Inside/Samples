using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentInterfaces.Data.Filters
{
    public static class PersonFilters
    {
        public static IQueryable<Person> WithAge(this IQueryable<Person> qry, int age)
        {
            return (from x in qry
                    where x.Age == age
                    select x);
        }

        public static IQueryable<Person> NameStartsWith(this IQueryable<Person> qry, string start)
        {
            return (from x in qry
                    where x.Name.StartsWith(start)
                    select x);
        }
    }
}
