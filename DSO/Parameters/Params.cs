using DSO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSO.Parameters
{
    public class Parameter<T> : IParameter<T>
    {
        public Parameter(String ParamName, String ParamValue, String ParamUnit, bool ReadOnly, T ParamObject)
        {
            GetParameter = ParamObject;
            ParameterName = ParamName;
            ParameterValue = ParamValue;
            ParameterUnit = ParamUnit;
            IsReadOnly = ReadOnly;
        }

        public T GetParameter { get; }
        public bool IsReadOnly { get; }
        public string ParameterValue { get; }
        public string ParameterName { get; }
        public string ParameterUnit { get; }

        public override string ToString()
        {
            return ParameterValue + " " + ParameterUnit;
        }
    }

}

