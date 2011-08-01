namespace Microsoft.Web.Mvc {
    using System.Threading;

    public sealed class NoAsyncTimeoutAttribute : AsyncTimeoutAttribute {

        public NoAsyncTimeoutAttribute()
            : base(Timeout.Infinite) {
        }

    }
}
