using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSO.Exceptions
{
    public class InvalidDataFrameException : Exception
    {
        public InvalidDataFrameException() : base() { }
        public InvalidDataFrameException(string message) : base(message) { }
        public InvalidDataFrameException(string message, System.Exception inner) : base(message, inner) { }
    }
}
