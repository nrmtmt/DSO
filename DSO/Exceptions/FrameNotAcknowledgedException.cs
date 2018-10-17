using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSO.Exceptions
{
    public class FrameNotAcknowledgedException : Exception
    {
        public FrameNotAcknowledgedException() : base() { }
        public FrameNotAcknowledgedException(string message) : base(message) { }
        public FrameNotAcknowledgedException(string message, System.Exception inner) : base(message, inner) { }
    }
}
