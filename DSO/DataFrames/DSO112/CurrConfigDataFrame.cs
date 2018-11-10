using DSO.Exceptions;
using DSO.Interfaces;
using DSO.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.DataFrames.DSO112
{
    public class CurrConfigDataFrame : DataFrame, ICurrentConfig
    {
        public CurrConfigDataFrame(byte[] data) : base(data, 192, 48) //readonly
        {
            if (FrameID != 192 || FrameSubID != 48 || data.Length < 55)
            {
                throw new InvalidDataFrameException("Wrong CurrConfigDataFrame - invalid FrameID or data length");
            }
        }

        public bool VerticalPositionChangeableByHost
        {
            get
            {
                return true;
            }
        }

        public bool Ch1Present
        {
            get
            {
                return true;
            }
        }
        public bool Ch2Present
        {
            get
            {
                return false;
            }
        }
        public bool SensitivityChangeableByHost
        {
            get
            {
                return true;
            }
        }
        public bool CoupleChangeableByHost
        {
            get
            {
                return true;
            }
        }
        public Config.VerticalSensitivity MaxVerticalSensitivity
        {
            get
            {
                return (Config.VerticalSensitivity)Data[5];
            }
        }
        public Config.VerticalSensitivity MinVerticalSensitivity
        {
            get
            {
                return (Config.VerticalSensitivity)Data[6];
            }
        }
        public Config.Coupling MaxCoupleSetting
        {
            get
            {
                return (Config.Coupling)Data[7];
            }
        }
        public Config.Coupling MinCoupleSetting
        {
            get
            {
                return (Config.Coupling)Data[8];
            }
        }
        public int MaxVerticalPosition
        {
            get
            {
                byte[] bytes = { Data[9], Data[10] };
                return bytes.ToInt();
            }
        }
        public int MinVerticalPosition
        {
            get
            {
                byte[] bytes = { Data[11], Data[12] };
                return bytes.ToInt();
            }
        }
        public Config.Timebase MaxTimebaseSetting
        {
            get
            {
                return (Config.Timebase)Data[21];
            }
        }
        public Config.Timebase MinTimebaseSetting
        {
            get
            {
                return (Config.Timebase)Data[22];
            }
        }
        public Config.TriggerMode MaxTriggerModeSetting
        {
            get
            {
                return (Config.TriggerMode)Data[27];
            }
        }
        public Config.TriggerMode MinTriggerModeSetting
        {
            get
            {
                return (Config.TriggerMode)Data[28];
            }
        }
        public Config.Slope MaxSlopeModeSetting
        {
            get
            {
                return (Config.Slope)Data[29];
            }
        }
        public Config.Slope MinSlopeModeSetting
        {
            get
            {
                return (Config.Slope)Data[30];
            }
        }
        public int MaxTriggerLevel
        {
            get
            {
                byte[] bytes = { Data[31], Data[32] };
                return bytes.ToInt();
            }
        }
        public int MinTriggerLevel
        {
            get
            {
                byte[] bytes = { Data[33], Data[34] };
                return bytes.ToInt();
            }
        }
        public int MaxTriggerPosition
        {
            get
            {
                return Data[35];
            }
        }
        public int MinTriggerPosition
        {
            get
            {
                return Data[36];
            }
        }
        public int MaxRecordLength
        {
            get
            {
                byte[] bytes = { Data[43], Data[44], Data[45], Data[46] };
                return bytes.ToInt();
            }
        }
        public int MinRecordLength
        {
            get
            {
                byte[] bytes = { Data[47], Data[48], Data[49], Data[50] };
                return bytes.ToInt();
            }
        }

        public int PointsPerDiv
        {
            get
            {
                return 25;
            }
        }
    }
}
