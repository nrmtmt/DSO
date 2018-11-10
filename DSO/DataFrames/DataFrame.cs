using DSO.Exceptions;
using DSO.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.DataFrames
{
    public abstract class DataFrame
    {
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

                            if (frameSize > 3 && frameSize < 16384) //to avoid blank or corrupted frames //need to change to something more sophisticated
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
    }
   

}
