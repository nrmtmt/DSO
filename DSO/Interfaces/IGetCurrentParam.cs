using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO
{
    public interface IGetCurrentParam
    {
       Util.VerticalSensitivity GetVSensitivity();
       Util.Coupling GetCouple();
       int GetVPosition();
       Util.Timebase GetTBase();
       Util.TriggerMode GetTriggerMode();
       Util.Slope GetTriggerSlope();
       int GetTriggerLevel();
       int GetTriggerPosition();
       int GetRecordLength();
    }
}
