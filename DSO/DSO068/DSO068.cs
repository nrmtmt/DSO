using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSO.Interfaces;

namespace DSO.DSO068
{
    public class DSO068 : JyeScope
    {
        public DSO068(SerialPortAdapter port) : base(port)
        {

        }
        public override CurrParamDataFrame GetCurrentParameters()
        {
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    if (WriteFrame(new ScopeControlFrames.GetParam()))
                    {
                        CurrParamDataFrame CurrParam = new CurrParamDataFrame(base.GetBuffer());
                        return CurrParam;
                    }
                }
                catch (InvalidDataFrameException ex)
                {
                    //System.Threading.Thread.Sleep(i);
                   
                }
            }
            return null;
        }

        public override DataFrame GetData()
        {
         
            throw new NotImplementedException();
        }

        public override CurrParamDataFrame SetCurrentParameters()
        {
            throw new NotImplementedException();
        }
    }
}
