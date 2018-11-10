using DSO.DataFrames;
using DSO.DataFrames.DSO068;
using DSO.Interfaces;
using DSO.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO
{
    public class DSO068 : JyeScope
    {
        private Dictionary<object, string> _AvailableRecordLength = new Dictionary<object, string>();
        private Dictionary<object, string> _AvailableSensitivitySettings  = new Dictionary<object, string>();
        private Dictionary<object, string> _AvailableTimeBaseSettings  = new Dictionary<object, string>();

        public DSO068(SerialPortAdapter port) : base(port)
        {
            
        }
   
        public override ICurrentConfig GetCurrentConfig()
        {
            var conf = (CurrConfigDataFrame)new AcknowledgedFrame().GetAcknowledgedFrame
                        (typeof(DataFrames.ScopeControlDataFrames.GetConfig), typeof(CurrConfigDataFrame), this);
            return conf;
        }
      
        protected override bool ChangeParamAcknowledged()
        {
            var curParam = Get_CurParamDataFrame_From_Current_Object_State();
            CurrParamDataFrame curParam2 = null;

            try
            {
                curParam2 = (CurrParamDataFrame)new AcknowledgedFrame().GetAcknowledgedFrame
                (typeof(DataFrames.ScopeControlDataFrames.GetParameters), typeof(CurrParamDataFrame), this);
            }catch (DSO.Exceptions.FrameNotAcknowledgedException)
            {
                return false;
            }
         
            if (!curParam.Equals(curParam2))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
