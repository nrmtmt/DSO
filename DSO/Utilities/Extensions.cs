using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.Utilities
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
}
