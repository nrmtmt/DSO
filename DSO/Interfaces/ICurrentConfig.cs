using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.Interfaces
{
     public interface ICurrentConfig
    {
        bool Ch1Present { get; }

        bool Ch2Present { get; }

        bool SensitivityChangeableByHost { get; }

        bool CoupleChangeableByHost { get; }

        bool VerticalPositionChangeableByHost { get; }

        Config.VerticalSensitivity MaxVerticalSensitivity { get; }

        Config.VerticalSensitivity MinVerticalSensitivity { get; }

        Config.Coupling MaxCoupleSetting { get; }

        Config.Coupling MinCoupleSetting { get; }

        int MaxVerticalPosition { get; }

        int MinVerticalPosition { get; }

        Config.Timebase MaxTimebaseSetting { get; }

        Config.Timebase MinTimebaseSetting { get; }

        Config.TriggerMode MaxTriggerModeSetting { get; }

        Config.TriggerMode MinTriggerModeSetting { get; }

        Config.Slope MaxSlopeModeSetting { get; }

        Config.Slope MinSlopeModeSetting { get; }

        int MaxTriggerLevel { get; }

        int MinTriggerLevel { get; }

        int MaxTriggerPosition { get; }

        int MinTriggerPosition { get; }

        int MaxRecordLength { get; }

        int MinRecordLength { get; }

        int PointsPerDiv { get; }

    }
}
