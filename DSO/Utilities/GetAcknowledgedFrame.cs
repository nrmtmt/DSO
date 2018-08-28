using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.Utilities
{
    public static class GetAcknowledgedFrame
    {
        public static DataFrame Get(Type FrameType, byte[] buffer, ref int timeoutTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < timeoutTime)
            {
                try
                {
                    if (FrameType == typeof(CurrConfigDataFrame))
                    {
                        CurrConfigDataFrame CurrConfig = new CurrConfigDataFrame(buffer);
                        return CurrConfig;
                    }
                    else if (FrameType == typeof(CurrParamDataFrame))
                    {
                        CurrParamDataFrame CurrParam = new CurrParamDataFrame(buffer);
                        return CurrParam;
                    }
                    else if (FrameType == typeof(DataBlockDataFrame))
                    {
                        DataBlockDataFrame CurrData = new DataBlockDataFrame(buffer);
                        return CurrData;
                    }
                    else if (FrameType == typeof(DataSampleDataFrame))
                    {
                        DataSampleDataFrame CurrData = new DataSampleDataFrame(buffer);
                        return CurrData;
                    }
                    else
                    {
                        throw new InvalidOperationException("Wrong object type");
                    }
                }
                catch (InvalidDataFrameException ex)
                {
                    //do it again
                }
                throw new TimeoutException("Timeout while waiting for frame acknowledge");
            }
            return null;
        }
    }
}
