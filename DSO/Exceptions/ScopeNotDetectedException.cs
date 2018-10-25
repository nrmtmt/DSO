using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSO.Exceptions
{
    public class ScopeNotDetectedException : Exception
    {
        public ScopeNotDetectedException() : base() { }
        public ScopeNotDetectedException(string message) : base(message) { }
        public ScopeNotDetectedException(string message, System.Exception inner) : base(message, inner) { }
    }
}
