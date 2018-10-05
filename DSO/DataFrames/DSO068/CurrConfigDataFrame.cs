using DSO.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.DataFrames.DSO068
{
    public class CurrConfigDataFrame : DataFrame , ICurrentConfig
    {
        public CurrConfigDataFrame(byte[] data) : base(data, 192, 48) //readonly
        {
            if (FrameID != 192 || FrameSubID != 48 || Data.Length < 55)
            {
                throw new InvalidDataFrameException($"Wrong CurrConfigDataFrame - invalid FrameID or data length : FrameID {FrameID}, Frame subID {FrameSubID}, DataLength {data.Length} ");
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
        public Config.VerticalSensitivity MaxVerticalSensitivity
        {
            get
            {
                return (Config.VerticalSensitivity)Data[9];
            }
        }
        public Config.VerticalSensitivity MinVerticalSensitivity
        {
            get
            {
                return (Config.VerticalSensitivity)Data[10];
            }
        }
        public Config.Coupling MaxCoupleSetting //?
        {
            get
            {
                return (Config.Coupling)Data[11];
            }
        }
        public Config.Coupling MinCoupleSetting //?
        {
            get
            {
                return (Config.Coupling)Data[12];
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
        public Config.Timebase MaxTimebaseSetting
        {
            get
            {
                return (Config.Timebase)Data[25];
            }
        }
        public Config.Timebase MinTimebaseSetting
        {
            get
            {
                return (Config.Timebase)Data[26];
            }
        }
        public Config.TriggerMode MaxTriggerModeSetting
        {
            get
            {
                return (Config.TriggerMode)Data[31];
            }
        }
        public Config.TriggerMode MinTriggerModeSetting
        {
            get
            {
                return (Config.TriggerMode)Data[32];
            }
        }
        public Config.Slope MaxSlopeModeSetting
        {
            get
            {
                return (Config.Slope)Data[33];
            }
        }
        public Config.Slope MinSlopeModeSetting
        {
            get
            {
                return (Config.Slope)Data[34];
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

        public int PointsPerDiv
        {
            get
            {
                return 10;
            }
        }
    }
}
