using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.Interfaces
{
    public interface IGetCurrentConfig
    {
        bool Ch1Present();
        bool Ch2Present();
        bool SensitivityChangeableByHost();
        bool CoupleChangeableByHost();
        Util.VerticalSensitivity MaxVerticalSensitivity();
        Util.VerticalSensitivity MinVerticalSensitivity();
        Util.Coupling MaxCoupleSetting();
        Util.Coupling MinCoupleSetting();
        int MaxVerticalPosition();
        int MinVerticalPosition();
        Util.Timebase MaxTimebaseSetting();
        Util.Timebase MinTimebaseSetting();
        Util.TriggerMode MaxTriggerModeSetting();
        Util.TriggerMode MinTriggerModeSetting();
        Util.Slope MaxSlopeModeSetting();
        Util.Slope MinSlopeModeSetting();
        int MaxTriggerLevel();
        int MinTriggerLevel();
        int MaxTriggerPosition();
        int MinTriggerPosition();
        int MaxRecordLength();
        int MinRecordLength();  
    }
}
