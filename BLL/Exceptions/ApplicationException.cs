using System;

namespace BLL.Exceptions
{

    public abstract class ApplicationException : Exception
    {

        protected ApplicationException(string message) : base(message)
        {
        }

        protected ApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}