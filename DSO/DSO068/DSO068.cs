using DSO.DataFrames;
using DSO.DataFrames.DSO068;
using DSO.Interfaces;
using DSO.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.DSO068
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
                        (typeof(ScopeControlFrames.GetConfig), typeof(CurrConfigDataFrame), this);
            return conf;
        }

        protected override bool ChangeParamAcknowledged()
        {

            var curParam = new CurrParamDataFrame((DSO.Config.VerticalSensitivity)_sensitivity,
                                                            (DSO.Config.Timebase)_timeBase,
                                                            (DSO.Config.Slope)_triggerSlope,
                                                            (DSO.Config.TriggerMode)_triggerMode,
                                                            (DSO.Config.Coupling)_couple,
                                                            (byte)_triggerLevel,
                                                            (byte)_triggerPos,
                                                            (DSO.Config.RecordLength)_recordLength,
                                                            _verticalPosition);
            var curParam2 = GetCurrentParameters();
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
