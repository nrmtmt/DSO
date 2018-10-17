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
        private Dictionary<int, string> _AvailableRecordLength = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableSensitivitySettings  = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableTimeBaseSettings  = new Dictionary<int, string>();

        public DSO068(SerialPortAdapter port) : base(port)
        {
            foreach (var value in DSO.Config.RecLength)
            {
                _AvailableRecordLength.Add(value, Convert.ToString(value));
            }
            foreach (var sensitivity in (int[])Enum.GetValues(typeof(DSO.Config.VerticalSensitivity)))
            {
                _AvailableSensitivitySettings.Add((int)sensitivity, Enum.GetName(typeof(DSO.Config.VerticalSensitivity), sensitivity));
            }
            foreach (var timeBase in (int[])Enum.GetValues(typeof(DSO.Config.Timebase)))
            {
                _AvailableTimeBaseSettings.Add((int)timeBase, Enum.GetName(typeof(DSO.Config.Timebase), timeBase));
            }

        }
   
        public override ICurrentConfig GetCurrentConfig()
        {
            var conf = (CurrConfigDataFrame)new AcknowledgedFrame().GetAcknowledgedFrame
                        (typeof(ScopeControlFrames.GetConfig), typeof(CurrConfigDataFrame), this);
            return conf;
        }

        protected override bool FrameAcknowledged()
        {
            //40, 0, 0, 0
           var curParam = new CurrParamDataFrame((DSO.Config.VerticalSensitivity)_sensitivity,
                                                          (DSO.Config.Timebase)_timeBase,
                                                          (DSO.Config.Slope)_triggerSlope,
                                                          (DSO.Config.TriggerMode)_triggerMode,
                                                          (DSO.Config.Coupling)_couple,
                                                          (byte)_triggerLevel,
                                                          (byte)_triggerPos,
                                                          DSO.Config.RecLength[Array.IndexOf(DSO.Config.RecLength, _recordLength)],
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

        public override Dictionary<int, string> AvailableRecordLength
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Dictionary<int, string> AvailableSenitivitySettings
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Dictionary<int, string> AvailableTimebaseSettings
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
