using System;

namespace BLL.Exceptions
{
    public class ForbiddenException : ApplicationException
    {
        public ForbiddenException() : base("Access denied.") { }

        public ForbiddenException(string message) : base(message) { }

        public ForbiddenException(string message, Exception inner) : base(message, inner) { }
    }
}
