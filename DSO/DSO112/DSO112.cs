using DSO.DataFrames.DSO112;
using DSO.Interfaces;
using DSO.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.DSO112
{
    class DSO112 :JyeScope
    {
        private Dictionary<int, string> _AvailableRecordLength = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableSensitivitySettings = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableTimeBaseSettings = new Dictionary<int, string>();

        public DSO112(SerialPortAdapter port) : base(port)
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
        public override ICurrentConfig GetCurrentConfig() //seems to be same in each jye scope
        {
            try
            {
                    var conf = (CurrConfigDataFrame)new GetAcknowledgedFrame().WriteAcknowledged
                         (typeof(ScopeControlFrames.GetConfig), typeof(CurrConfigDataFrame), this);
                    return conf;
            }
            catch (TimeoutException)
            {
            }
            return GetCurrentConfig();
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
