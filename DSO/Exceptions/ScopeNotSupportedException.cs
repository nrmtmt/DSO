using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSO.Exceptions
{
    public class ScopeNotSupportedException : Exception
    {
            public ScopeNotSupportedException() : base() { }
            public ScopeNotSupportedException(string message) : base(message) { }
            public ScopeNotSupportedException(string message, System.Exception inner) : base(message, inner) { } 
    }
}
