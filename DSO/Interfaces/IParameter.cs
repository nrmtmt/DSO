using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSO.Interfaces
{
   public interface IParameter<out T>
    {
        string ParameterName { get; }
        string ParameterValue { get; }
        string ParameterUnit { get; }
        bool IsReadOnly { get; }
        T GetParameter { get; }
    }
}
