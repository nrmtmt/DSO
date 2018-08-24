using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO
{
    public class CurrConfigDataFrame : DataFrame
    {
        public CurrConfigDataFrame(byte[] data) : base(data, 192, 48) //readonly
        {
            if (FrameID != 192 || FrameSubID != 48 || data.Length < 55)
            {
                throw new InvalidDataFrameException("Wrong CurrConfigDataFrame - invalid FrameID or data length");
            }
        }

        public bool Ch1Present
        {
            get
            {
                BitArray bit = new BitArray(BitConverter.GetBytes(Data[5]).ToArray());
                return bit[0];
            }
        }
        public bool Ch2Present
        {
            get
            {
                BitArray bit = new BitArray(BitConverter.GetBytes(Data[5]).ToArray());
                return bit[1];
            }
        }
        public bool SensitivityChangeableByHost
        {
            get
            {
                BitArray bit = new BitArray(BitConverter.GetBytes(Data[6]).ToArray());
                return bit[0];
            }
        }
        public bool CoupleChangeableByHost
        {
            get
            {
                BitArray bit = new BitArray(BitConverter.GetBytes(Data[6]).ToArray());
                return bit[1];
            }
        }
        public Util.VerticalSensitivity MaxVerticalSensitivity
        {
            get
            {
                return (Util.VerticalSensitivity)Data[9];
            }
        }
        public Util.VerticalSensitivity MinVerticalSensitivity
        {
            get
            {
                return (Util.VerticalSensitivity)Data[10];
            }
        }
        public Util.Coupling MaxCoupleSetting //?
        {
            get
            {
                return (Util.Coupling)Data[11];
            }
        }
        public Util.Coupling MinCoupleSetting //?
        {
            get
            {
                return (Util.Coupling)Data[12];
            }
        }
        public int MaxVerticalPosition
        {
            get
            {
                byte[] bytes = { Data[13], Data[14] };
                return bytes.ToInt();
            }
        }
        public int MinVerticalPosition
        {
            get
            {
                byte[] bytes = { Data[15], Data[16] };
                return bytes.ToInt();
            }
        }
        public Util.Timebase MaxTimebaseSetting
        {
            get
            {
                return (Util.Timebase)Data[25];
            }
        }
        public Util.Timebase MinTimebaseSetting
        {
            get
            {
                return (Util.Timebase)Data[26];
            }
        }
        public Util.TriggerMode MaxTriggerModeSetting
        {
            get
            {
                return (Util.TriggerMode)Data[31];
            }
        }
        public Util.TriggerMode MinTriggerModeSetting
        {
            get
            {
                return (Util.TriggerMode)Data[32];
            }
        }
        public Util.Slope MaxSlopeModeSetting
        {
            get
            {
                return (Util.Slope)Data[33];
            }
        }
        public Util.Slope MinSlopeModeSetting
        {
            get
            {
                return (Util.Slope)Data[34];
            }
        }
        public int MaxTriggerLevel
        {
            get
            {
                byte[] bytes = { Data[35], Data[36] };
                return bytes.ToInt();
            }
        }
        public int MinTriggerLevel
        {
            get
            {
                byte[] bytes = { Data[37], Data[38] };
                return bytes.ToInt();
            }
        }
        public int MaxTriggerPosition
        {
            get
            {
                return Data[39];
            }
        }
        public int MinTriggerPosition
        {
            get
            {
                return Data[40];
            }
        }
        public int MaxRecordLength
        {
            get
            {
                byte[] bytes = { Data[47], Data[48], Data[49], Data[50] };
                return bytes.ToInt();
            }
        }
        public int MinRecordLength
        {
            get
            {
                byte[] bytes = { Data[51], Data[52], Data[53], Data[54] };
                return bytes.ToInt();
            }
        }

    }
}
