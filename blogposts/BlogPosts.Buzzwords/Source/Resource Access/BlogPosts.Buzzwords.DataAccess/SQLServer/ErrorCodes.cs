using System;
using System.Collections.Generic;
using System.Text;

namespace BlogPosts.Buzzwords.DataAccess.SQLServer
{
    internal static class ErrorCodes
    {
        public const byte ValidationError = 1;
        public const byte ConcurrencyViolationError = 2;
        public const int SqlUserRaisedError = 50000;
    }
}

