using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSO.Exceptions
{
    public class ScopeNotRecognizedException : Exception
    {
        public ScopeNotRecognizedException() : base() { }
        public ScopeNotRecognizedException(string message) : base(message) { }
        public ScopeNotRecognizedException(string message, System.Exception inner) : base(message, inner) { }
    }

}
