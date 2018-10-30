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
            try
            {
                var conf = (CurrConfigDataFrame)new AcknowledgedFrame().GetAcknowledgedFrame
                       (typeof(DataFrames.ScopeControlDataFrames.GetConfig), typeof(CurrConfigDataFrame), this);

                return conf;
            }
            catch (FrameNotAcknowledgedException)
            {
                return GetCurrentConfig();
            }
               
        }

        public override float TriggerLevel
        {
            get
            {
                  var output = Measurements.GetScaledData(_triggerLevel + 128, base._voltPerDiv, ScopeConfig.PointsPerDiv, _verticalPosition, base.ScopeConfig.VerticalPositionChangeableByHost);
                  return output;
            }

            set
            {
                _triggerLevel = (Measurements.GetRawData(value, _voltPerDiv, ScopeConfig.PointsPerDiv))-128;
                SetCurrentParameters();
            }
        }

        protected override bool ChangeParamAcknowledged()
        {
            Queue<byte> tempBuff = new Queue<byte>();
            tempBuff = _CurrentBuffer;
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
