using DSO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSO.DataFrames.DSO112
{
    public class CommandAcknowledgedDataFrame : DataFrame
    {
        public CommandAcknowledgedDataFrame(byte[] data) : base(data, 230, 0)
        {
            if (FrameID != 230 || FrameSubID != 0 )
            {
                throw new InvalidDataFrameException($"CommandAcknowledgedDataFrame - invalid FrameID or FrameSubID : FrameID {FrameID}, Frame subID {FrameSubID}, DataLength {data.Length} ");
            }

        }
    }
}

