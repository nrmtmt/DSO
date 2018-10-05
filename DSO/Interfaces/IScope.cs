using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DSO
{
    public interface IScope
    {
        event EventHandler NewDataInBuffer;
        IScope Create();
        bool Destroy();
        bool Connect();
        bool Disconnect();
        bool StartCapture();
        bool StopCapture();
        long[] GetScaledData();
        Dictionary<int, string> AvailableTimebaseSettings { get; }
        Dictionary<int, string> AvailableCoupleSettings { get; }
        Dictionary<int, string> AvailableTriggerSlopeSettings { get; }
        Dictionary<int, string> AvailableSenitivitySettings { get; }
        Dictionary<int, string> AvailableTriggerModeSettings { get; }
        Dictionary<int, string> AvailableRecordLength { get; }
        string ScopeName { get; }

        int ReadDelay { get; set; }
        int TimeBase { get; set; }
        int TriggerPos { get; set; }
        float TriggerLevel { get; set; }
        int Sensitivity { get; set; }
        int TriggerMode { get; set; }
        int Couple { get; set; }
        int TriggerSlope { get; set; }
        int RecordLength { get; set; }
        int VerticalPosition { get; set; }
    }
}
