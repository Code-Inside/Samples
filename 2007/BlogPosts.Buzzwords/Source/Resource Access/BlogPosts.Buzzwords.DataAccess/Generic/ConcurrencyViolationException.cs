using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BlogPosts.Buzzwords.DataAccess
{
    /// <summary>
    /// This exception is thrown when a concurrency
    /// violation is detected in the database.
    /// </summary>
    [Serializable]
    public class ConcurrencyViolationException : RepositoryException
    {
        public ConcurrencyViolationException()
        {
        }

        public ConcurrencyViolationException(string message)
            : base(message)
        {
        }

        public ConcurrencyViolationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ConcurrencyViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

