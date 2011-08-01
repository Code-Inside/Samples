using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class ServiceResponse<T>
    {
        public ServiceResult Result { get; set; }
        public ErrorCodes ErrorCodes { get; set; }
        public Exception Exception { get; set; }
        public T Value { get; set; }
    }

    /// <summary>
    /// Usererror Codes?
    /// </summary>
    public enum ErrorCodes
    {
        InvalidName,
        NothingFound
    }

    /// <summary>
    /// Succeeded = Everything worked
    /// Failed = Userinput incorrect or business logic has detected an error
    /// Fatal = exception occured (e.g. db down)
    /// </summary>
    public enum ServiceResult
    {
        Succeeded,
        Failed,
        Fatal
    }
}
