using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO
{
    public interface ISetCurrentParam
    {
        void SetVSensitivity(Util.VerticalSensitivity VSensitivity);
        void SetCouple(Util.Coupling Couple);
        void SetVPosition(int VPos);
        void SetTBase(Util.Timebase TBase);
        void SetTriggerMode(Util.TriggerMode Trigger);
        void SetTriggerSlope(Util.Slope Slope);
        void SetTriggerLevel(int TrigLevel);
        void SetTriggerPosition(int TrigPos);
        void SetRecordLength(int RecordLength);
    }
}
