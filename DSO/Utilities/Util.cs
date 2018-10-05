using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO
{
    public static class ByteExtension
    {
        public static string ToHex(this Byte value)
        {
            return value.ToString("X");
        }
    }

    public static class ArrayExtension
    {
        public static int ToInt(this Byte[] value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToInt16(value, 0);
            }
            else
            {
                value.Reverse();
                return BitConverter.ToInt16(value, 0);
            }
        }
        public static string ToCommaDelimitedString(this Byte[] value)
        {
            string Data = "";
            foreach (var data in value)
            {
                Data += data + ",";
            }
            Data.Remove(Data.Length - 1);
            return Data;
        }
    }
    

    public static class BitArrayExtension
    {
        public static byte ToByte(this BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }
    }

    public class Config
    {
        public enum ScopeType
        {
            DSO068 = 4,
            DSO112A = 5,
        }

        public enum Coupling
        {
            DC = 0,
            AC = 1,
            GND = 2,
        }

        public static  int[] RecLength = new int[] { 256, 512, 1024 };
       
        public enum Slope
        {
            Falling = 0,
            Rising = 1,
        }

        public enum TriggerMode
        {
            Auto = 0,
            Normal = 1,
            Single = 2,
        }

        public enum Timebase
        {
            divCustom = 0,
            div50min = 1,
            div20min = 2,
            div10min = 3,
            div5min = 4,
            div2min = 5,
            div1min = 6,
            div50s = 7,
            div20s = 8,
            div10s = 9,
            div5s = 10,//0x0A,
            div2s = 11,//0x0B,
            div1s = 12,//0x0c,
            div500ms = 13,//0x0D,
            div200ms = 14,//0x0E,
            div100ms = 15,//0x0F,
            div50ms = 16,//0x10,
            div20ms = 17,//0x11,
            div10ms = 18,//0x12,
            div5ms = 19,//0x13,
            div2ms = 20,//0x14,
            div1ms = 21,//0x15,
            div500us = 22,//0x16,
            div200us = 23,//0x17,
            div100us = 24,// 0x18,
            div50us = 25,//0x19,
            div20us = 26,//0x1A,
            div10us = 27,//0x1B,
            div5us = 28, //0x1C,
            div2us = 29, //0x1D,
            div1us = 30, //0x1E
            div500ns = 31, //0x1f
            div200ns = 32,
            div100ns = 33,
            div50ns = 34,
            div20ns = 35,
            div10ns = 36,
            div5ns = 37
        }
        public enum VerticalSensitivity
        {
            divCustom = 0,
            div100v = 1,
            div50v = 2,
            div20v = 3,
            div10v = 4,
            div5V = 5,
            div2V = 6,
            div1V = 7,
            div500mV = 8,
            div200mV = 9,
            div100mV = 10,
            div50mV = 11,
            div20mV = 12,
            div10mV = 13,
            div5mV = 14,
            div2mV = 15,
        }

        public enum ScopeState
        {
            AutoState = 0,
            ManualState = 1,
        }
        public enum DataLoggerRefVoltage
        {
            AREF = 0,
            AVCC = 1,
            INTERNAL = 2      
        }
        public enum DataLoggerDataAdjustment
        {
            RightAdjustment = 0,
            LeftAdjustment = 1
        }
    }
}
