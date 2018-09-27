using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO
{
    public class DataSampleDataFrame : DataFrame
    {
        public DataSampleDataFrame(byte[] data) : base(data, 192, 51)
        {
            if (FrameID != 192 || FrameSubID != 51)
            {
                throw new InvalidDataFrameException("Wrong DataSampleDataFrame - invalid FrameID");
            }
            else if (this.Data.Count() < 13)
            {
                throw new InvalidDataFrameException("Wrong DataBlockDataFrame - not enough data");
            }
        }
    }
}
