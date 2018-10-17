using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSO.Exceptions
{
    public class ParametersNotSetException : Exception
    {
        public ParametersNotSetException() : base() { }
        public ParametersNotSetException(string message) : base(message) { }
        public ParametersNotSetException(string message, System.Exception inner) : base(message, inner) { }
    }
}
