using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocks.Data.Filters
{
    public static class PersonFilter
    {
        public static IQueryable<Person> WithAge(this IQueryable<Person> qry, int age)
        {
            return (from x in qry
                    where x.Age == age
                    select x);
        }
    }
}
