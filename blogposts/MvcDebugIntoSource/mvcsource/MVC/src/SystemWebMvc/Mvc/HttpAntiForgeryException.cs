namespace System.Web.Mvc {
    using System;
    using System.Runtime.Serialization;
    using System.Web;

    [Serializable]
    public sealed class HttpAntiForgeryException : HttpException {

        public HttpAntiForgeryException() {
        }

        private HttpAntiForgeryException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public HttpAntiForgeryException(string message)
            : base(message) {
        }

        public HttpAntiForgeryException(string message, Exception innerException)
            : base(message, innerException) {
        }

    }
}
