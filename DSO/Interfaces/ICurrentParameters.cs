using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.Interfaces
{
    public interface ICurrentParameters
    {
        Config.VerticalSensitivity VSensitivity { get; set; }

        Config.Coupling Couple { get; set; }

        int VPosition { get; set; }

        Config.Timebase TBase { get; set; }

        Config.TriggerMode TriggerMode { get; set; }

        Config.Slope TriggerSlope { get; set; }

        int TriggerLevel { get; set; }

        byte TriggerPosition { get; set; }

        int RecordLength { get; set; }
    }
}
