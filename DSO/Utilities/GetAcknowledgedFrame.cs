using DSO.DataFrames;
using System;
using System.Diagnostics;

namespace DSO.Utilities
{
    public static class GetAcknowledgedFrame
    {
        public static DataFrame Get(Type FrameType, ref byte[] buffer, ref int timeoutTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Restart();
            while (stopwatch.ElapsedMilliseconds < timeoutTime)
            {
                try
                {
                    if (FrameType == typeof(DataFrames.DSO068.CurrConfigDataFrame))
                    {
                        DataFrames.DSO068.CurrConfigDataFrame CurrConfig = new DataFrames.DSO068.CurrConfigDataFrame(buffer);
                        return CurrConfig;
                    }
                    else if (FrameType == typeof(DataFrames.DSO112.CurrConfigDataFrame))
                    {
                        DataFrames.DSO112.CurrConfigDataFrame CurrParam = new DataFrames.DSO112.CurrConfigDataFrame(buffer);
                        return CurrParam;
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
