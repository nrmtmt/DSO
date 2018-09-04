using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO
{
    public abstract class DataFrame
    {
      
        private static int ByteSearch(byte[] searchIn, byte[] searchBytes, int start = 0)
        {
            int found = -1;
            bool matched = false;
            //only look at this if we have a populated search array and search bytes with a sensible start
            if (searchIn.Length > 0 && searchBytes.Length > 0 && start <= (searchIn.Length - searchBytes.Length) && searchIn.Length >= searchBytes.Length)
            {
                //iterate through the array to be searched
                for (int i = start; i <= searchIn.Length - searchBytes.Length; i++)
                {
                    //if the start bytes match we will start comparing all other bytes
                    if (searchIn[i] == searchBytes[0])
                    {
                        if (searchIn.Length > 1)
                        {
                            //multiple bytes to be searched we have to compare byte by byte
                            matched = true;
                            for (int y = 1; y <= searchBytes.Length - 1; y++)
                            {
                                if (searchIn[i + y] != searchBytes[y])
                                {
                                    matched = false;
                                    break;
                                }
                            }
                            //everything matched up
                            if (matched)
                            {
                                found = i;
                                break;
                            }

                        }
                        else
                        {
                            //search byte is only one bit nothing else to do
                            found = i;
                            break; //stop the loop
                        }

                    }
                }

            }
            return found;
        }
        private static readonly byte SyncChar = 254;

        public DataFrame()
        {
           
        }
        public DataFrame(byte[] data)
        {
            Generate(data);
        }

        public DataFrame(byte[] data, int frameID, int frameSubID)
        {
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    try
                    {
                        if (data[i] == frameSubID && data[i - 3] == frameID & i > 3)
                        {
                            var frameSize = (ushort)((data[i - 1] << 8) + data[i - 2]);

                            if (frameSize > 3 && frameSize < 1096) //to avoid blank or corrupted frames //need to change to something more sophisticated
                            {
                                byte[] frame = new byte[frameSize + 1];
                                for (int z = 0; z <= frameSize; z++)
                                {
                                    try
                                    {
                                        frame[z] = data[i - 4 + z];
                                    }
                                    catch (IndexOutOfRangeException ex)
                                    {
                                        frame[z] = SyncChar; //sometimes sync char is missing
                                    }
                                }
                                Generate(frame);
                            }
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                   
                }

            }
        }

        public override bool Equals(object obj)
        {
            try
            {
                DataFrame data = (DataFrame)obj;
                if (this.FrameID == data.FrameID && this.FrameSize == data.FrameSize && this.FrameSubID == data.FrameSubID)
                {
                    if (this.Data.SequenceEqual(data.Data))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected void Generate(byte[] data)
        {
            if(data != null && data.Count() > 4)
            {
                var _frameSize = (ushort)((data[3] << 8) + data[2]);
                var _count = data.Count();
                if (_count >= _frameSize)
                {
                    SyncCharacter = data[0];
                    FrameID = data[1];
                    FrameSize = _frameSize; //little endian
                    FrameSubID = data[4];
                    if (SyncCharacter != SyncChar)
                    {
                        throw new InvalidDataFrameException("Wrong DataFrame - invalid SyncCharacter");
                    }
                    if (FrameSize < 3 || FrameSize > 1096)
                    {
                        throw new InvalidDataFrameException("Wrong DataFrame - invalid FrameSize");
                    }
                    byte[] DataInFrame = new byte[FrameSize + 1];

                    string[] DataInFrameHex = new string[FrameSize + 1];
                    for (int i = 0; i <= FrameSize; i++)
                    {
                        DataInFrame[i] = data[i];
                        DataInFrameHex[i] = data[i].ToHex();
                    }
                    Data = DataInFrame;
                    HexData = DataInFrameHex;
                } 
            }
        }
        public byte SyncCharacter
        {
            get;
            private set;
        }
        public byte FrameID
        {
            get;
            private set;
        }
        public int FrameSize
        {
            get;
            private set;
        }
        public byte FrameSubID
        {
            get;
            private set;
        }

        public byte[] Data
        {
            get;
            private set;
        }
        public string[] HexData
        {
            get;
            private set;
        }
    }
    public class InvalidDataFrameException : Exception
    {
        public InvalidDataFrameException() : base() { }
        public InvalidDataFrameException(string message) : base(message) { }
        public InvalidDataFrameException(string message, System.Exception inner) : base(message, inner) { }
    }

}
