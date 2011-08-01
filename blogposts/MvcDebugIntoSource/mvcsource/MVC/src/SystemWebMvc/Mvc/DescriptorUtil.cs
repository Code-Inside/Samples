namespace System.Web.Mvc {
    using System;
    using System.Linq;
    using System.Threading;

    internal static class DescriptorUtil {

        public static TDescriptor[] LazilyFetchOrCreateDescriptors<TReflection, TDescriptor>(ref TDescriptor[] cacheLocation, Func<TReflection[]> initializer, Func<TReflection, TDescriptor> converter) {
            // did we already calculate this once?
            TDescriptor[] existingCache = Interlocked.CompareExchange(ref cacheLocation, null, null);
            if (existingCache != null) {
                return existingCache;
            }

            TReflection[] memberInfos = initializer();
            TDescriptor[] descriptors = memberInfos.Select(converter).Where(descriptor => descriptor != null).ToArray();
            TDescriptor[] updatedCache = Interlocked.CompareExchange(ref cacheLocation, descriptors, null);
            return updatedCache ?? descriptors;
        }

    }
}
