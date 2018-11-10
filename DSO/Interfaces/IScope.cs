using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using DSO.Interfaces;
using DSO.Parameters;

namespace DSO.Interfaces
{
    public interface IScope
    {
        event EventHandler NewDataInBuffer;
        bool Destroy();
        bool Connect();
        bool Disconnect();
        bool StartCapture();
        bool StopCapture();
        string ScopeName { get; }
        IParameter<float> CurrentVoltageLimit { get; }
        IParameter<int> DataSamplesPerDiv { get; }

        List<IParameter<Config.Timebase>> AvailableTimebaseSettings { get; }
        List<IParameter<Config.Coupling>> AvailableCoupleSettings { get; }
        List<IParameter<Config.Slope>> AvailableTriggerSlopeSettings { get; }
        List<IParameter<Config.VerticalSensitivity>> AvailableSenitivitySettings { get; }
        List<IParameter<Config.TriggerMode>> AvailableTriggerModeSettings { get; }
        List<IParameter<Config.RecordLength>> AvailableRecordLength { get; }


        IParameter<Config.Timebase> TimeBase { get; set; }
        IParameter<Config.Coupling> Couple { get; set; }
        IParameter<Config.Slope> TriggerSlope { get; set; }
        IParameter<Config.VerticalSensitivity> Sensitivity { get; set; }
        IParameter<Config.TriggerMode> TriggerMode { get; set; }
        IParameter<Config.RecordLength> RecordLength { get; set; }
        int TriggerPosition { get; set; }
        float TriggerLevel { get; set; }
    }
}
