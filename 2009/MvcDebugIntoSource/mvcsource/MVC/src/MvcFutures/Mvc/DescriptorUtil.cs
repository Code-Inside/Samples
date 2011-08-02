namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    internal static class DescriptorUtil {

        public static TResult[] LazilyFetchOrCreate<TSource, TResult>(ref TResult[] cacheLocation, Func<IEnumerable<TSource>> initializer, Func<TSource, TResult> converter) {
            // did we already calculate this once?
            TResult[] existingCache = Interlocked.CompareExchange(ref cacheLocation, null, null);
            if (existingCache != null) {
                return existingCache;
            }

            IEnumerable<TSource> sources = initializer();
            TResult[] converted = sources.Select(converter).Where(result => result != null).ToArray();
            TResult[] updatedCache = Interlocked.CompareExchange(ref cacheLocation, converted, null);
            return updatedCache ?? converted;
        }

    }
}
