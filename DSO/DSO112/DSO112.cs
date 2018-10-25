using DSO.DataFrames.DSO112;
using DSO.Exceptions;
using DSO.Interfaces;
using DSO.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.DSO112
{
    public class DSO112 :JyeScope
    {
     
        public DSO112(SerialPortAdapter port) : base(port)
        {
          
        }

        public override ICurrentConfig GetCurrentConfig()
        {
                var conf = (CurrConfigDataFrame)new AcknowledgedFrame().GetAcknowledgedFrame
                        (typeof(ScopeControlFrames.GetConfig), typeof(CurrConfigDataFrame), this);

                return conf;
        }

        protected override bool ChangeParamAcknowledged()
        {
            var tempBuff = _CurrentBuffer;
            try
            {
                var cmd = new DataFrames.DSO112.CommandAcknowledgedDataFrame(tempBuff.ToArray());
                return true;
            }
            catch (InvalidDataFrameException)
            {
                return false;
            }

        }
    }
}
