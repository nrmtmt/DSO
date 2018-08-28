using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO
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
        /// </summary>
        public CurrParamDataFrame(Config.VerticalSensitivity Sensitivity, Config.Timebase TBase, Config.Slope Slope, Config.TriggerMode Trigger, int trigLevel, byte trigPos, int recLength)
        {
            byte[] data = new byte[37];
            data[0] = 0xFE; data[1] = 0xC0; data[2] = 0x24; data[4] = 0x22;
            data[5] = (byte)Sensitivity;
            data[13] = (byte)TBase; data[17] = (byte)Trigger; data[18] = (byte)Slope;
            byte[] bytes = BitConverter.GetBytes(trigLevel);
            data[19] = bytes[0]; data[20] = bytes[1];
            data[21] = trigPos;
            bytes = BitConverter.GetBytes(recLength);
            data[25] = bytes[0]; data[26] = bytes[1]; data[27] = bytes[2]; data[28] = bytes[3];
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

