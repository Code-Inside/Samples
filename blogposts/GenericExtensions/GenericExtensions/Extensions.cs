using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace GenericExtensions
{
    public static class Extensions
    {
        public static ICollection<T> Add<T>(this ICollection<T> src, ICollection<T> addingElements)
        {
            foreach (T element in addingElements)
            {
                src.Add(element);
            }
            return src;
        }
    }
}
