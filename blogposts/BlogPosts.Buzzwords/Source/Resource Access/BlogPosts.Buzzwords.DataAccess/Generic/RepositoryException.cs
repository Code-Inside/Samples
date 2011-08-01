using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BlogPosts.Buzzwords.DataAccess
{
    /// <summary>
    /// This class is the base class for all exceptions from our
    /// repositories.
    /// </summary>
    [Serializable]
    public class RepositoryException : Exception
    {
        public RepositoryException()
        {
        }

        public RepositoryException(string message)
            : base(message)
        {
        }

        public RepositoryException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected RepositoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
