using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.DataFrames
{
    public class CurrParamDataFrame : DataFrame
    {
        public CurrParamDataFrame(byte[] data) : base(data, 192, 49)
        {
            if (FrameID != 192 || FrameSubID != 49 || data.Length < 32)
            {
                throw new InvalidDataFrameException("Wrong CurrParamDataFrame - invalid FrameID or data length");
            }
        }
        /// <summary>
        /// <param name="Slope">Slope</param>
        /// <param name="Sensitivity">Sensitivity</param>
        /// <param name="TBase">Time base</param>
        /// <param name="Trigger">Trigger mode</param>
        /// <param name="trigLevel">Trigger level (0 - 100%)</param>
        /// <param name="trigPos">Trigger position (0 - 255)</param>
        /// <param name="recLength">Record length</param>
        /// <param name="Couple">Couple</param> vPos
        /// <param name="vPos">Vertical position</param> vPos
        /// </summary>
        public CurrParamDataFrame(Config.VerticalSensitivity VSensitivity, Config.Timebase TBase, Config.Slope Slope, Config.TriggerMode Trigger, Config.Coupling Couple, int trigLevel, byte trigPos, int recLength, int vPos)
        {
            byte[] data = new byte[53];
            data[0] = 0xFE; data[1] = 0xC0; data[2] = 0x34; data[4] = 0x22;
            data[5] = (byte)VSensitivity;
            data[6] = (byte)Couple;

            byte[] bytes = BitConverter.GetBytes(vPos);
            data[7] = bytes[0]; data[8] = bytes[1];

            data[13] = (byte)TBase;
            data[17] = (byte)Trigger;
            data[18] = (byte)Slope;

            bytes = BitConverter.GetBytes(trigLevel);
            data[19] = bytes[0]; data[20] = bytes[1];

            data[21] = trigPos;

            bytes = BitConverter.GetBytes(recLength);
            data[25] = bytes[0]; data[26] = bytes[1];

            //data[27] = bytes[2]; data[28] = bytes[3];
            base.Generate(data);
        }

        public Config.VerticalSensitivity VSensitivity
        {
            get { return (Config.VerticalSensitivity)Data[5]; }
        }
        public Config.Coupling Couple
        {
            get { return (Config.Coupling)Data[6]; }
        }
        public int VPosition
        {
            get
            {
                byte[] bytes = { Data[7], Data[8] };
                return bytes.ToInt();  // return ((Data[3] << 8) + Data[2]);
            }
        }
        public Config.Timebase TBase
        {
            get { return (Config.Timebase)Data[13]; }
        }
        public Config.TriggerMode TriggerMode
        {
            get { return (Config.TriggerMode)Data[17]; }
        }
        public Config.Slope TriggerSlope
        {
            get { return (Config.Slope)Data[18]; }
        }
        public int TriggerLevel
        {
            get
            {
                byte[] bytes = { Data[19], Data[20] };
                return bytes.ToInt();
            }
        }
        public float VoltagePerDiv
        {
            get
            {
                byte curV = Data[5];
                switch (curV)
                {
                    case 0:
                        return 0;       //divCustom = 0,
                    case 1:
                        return 100;     //div100v = 1,
                    case 2:
                        return 50;      //div50v = 2,
                    case 3:
                        return 20;      //div20v = 3,
                    case 4:
                        return 10;      //div10v = 4,
                    case 5:
                        return 5;       //div5V = 5,
                    case 6:
                        return 2;       //div2V = 6,
                    case 7:
                        return 1;       //div1V = 7,
                    case 8:
                        return 0.5F;    //div500mV = 8,
                    case 9:
                        return 0.2F;    //div200mV = 9,
                    case 10:
                        return 0.1F;    //div100mV = 10,
                    case 11:
                        return 0.05F;   //div50mV = 11,
                    case 12:
                        return 0.02F;   //div20mV = 12,
                    case 13:
                        return 0.01F;   //div10mV = 13,
                    case 14:
                        return 0.005F;  //div5mV = 14,
                    case 15:
                        return 0.002F;  //div2mV = 15,
                    default:
                        return 0;
                }
            }
        }
        public byte TriggerPosition
        {
            get { return Data[21]; }
        }
        public int RecordLength
        {
            get
            {
                byte[] bytes = { Data[25], Data[26], Data[27], Data[28] };
                return bytes.ToInt();
            }
        }
        public override bool Equals(object obj)
        {
            try
            {
                CurrParamDataFrame data = (CurrParamDataFrame)obj;
                if (this.Couple == data.Couple 
                    && this.TBase == data.TBase
                    && this.TriggerLevel == data.TriggerLevel
                    && this.TriggerMode == data.TriggerMode
                    && this.TriggerPosition == data.TriggerPosition
                    && this.TriggerSlope == data.TriggerSlope
                    && this.VPosition == data.VPosition
                    && this.VSensitivity == data.VSensitivity)
                {     
                        return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}

