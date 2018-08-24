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
        public CurrParamDataFrame(Util.Slope Slope, Util.Timebase TBase, Util.TriggerMode Trigger, int trigLevel, byte trigPos, int recLength)
        {
            byte[] data = new byte[37];
            data[0] = 0xFE; data[1] = 0xC0; data[2] = 0x24; data[4] = 0x22;
            data[13] = (byte)TBase; data[17] = (byte)Trigger; data[18] = (byte)Slope;
            byte[] bytes = BitConverter.GetBytes(trigLevel);
            data[19] = bytes[0]; data[20] = bytes[1];
            data[21] = trigPos;
            bytes = BitConverter.GetBytes(recLength);
            data[25] = bytes[0]; data[26] = bytes[1]; data[27] = bytes[2]; data[28] = bytes[3];
            base.Generate(data);
        }
       
        public Util.VerticalSensitivity VSensitivity
        {
            get { return (Util.VerticalSensitivity)Data[5]; }
        }
        public Util.Coupling Couple
        {
            get { return (Util.Coupling)Data[6]; }
        }
        public int VPosition
        {
            get
            {
                byte[] bytes = { Data[7], Data[8] };
                return bytes.ToInt();  // return ((Data[3] << 8) + Data[2]);
            }
        }
        public Util.Timebase TBase
        {
            get { return (Util.Timebase)Data[13]; }
        }
        public Util.TriggerMode TriggerMode
        {
            get { return (Util.TriggerMode)Data[17]; }
        }
        public Util.Slope TriggerSlope
        {
            get { return (Util.Slope)Data[18]; }
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
    }
}

