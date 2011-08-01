namespace Microsoft.Web.Mvc {
    using System;
    using System.Threading;

    internal static class SynchronizationContextHelper {

        public static SynchronizationContext GetSynchronizationContext() {
            // In a runtime environment, SynchronizationContext.Current will be set to an instance
            // of AspNetSynchronizationContext. In a unit test environment, the Current property
            // won't be set and we have to create one on the fly.
            return SynchronizationContext.Current ?? new SynchronizationContext();
        }

        public static T Sync<T>(this SynchronizationContext syncContext, Func<T> func) {
            T theValue = default(T);
            Exception thrownException = null;

            syncContext.Send(o => {
                try {
                    theValue = func();
                }
                catch (Exception ex) {
                    // by default, the AspNetSynchronizationContext type will swallow thrown exceptions,
                    // so we need to save and propagate them
                    thrownException = ex;
                }
            }, null);

            if (thrownException != null) {
                throw thrownException;
            }
            return theValue;
        }

        public static void Sync(this SynchronizationContext syncContext, Action action) {
            Sync<object>(syncContext, () => {
                action();
                return null;
            });
        }

    }
}
