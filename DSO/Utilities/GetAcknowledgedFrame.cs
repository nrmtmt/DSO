using DSO.DataFrames;
using DSO.ScopeControlFrames;
using System;
using System.Diagnostics;
using System.Linq;

namespace DSO.Utilities
{
    public class GetAcknowledgedFrame
    {
        byte[] WritedData;
        string lastEx;
        string stringData;

        public DataFrame WriteAcknowledged(Type SendType, Type ReturnType, JyeScope scope)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Restart();
            while (stopwatch.ElapsedMilliseconds < scope.TimeoutTime)
            {
                try
                {
                    if (SendType == typeof(GetParameters))
                    {
                        WriteFrame(new ScopeControlFrames.GetParameters(), scope.SerialPort);
                    }
                    else if(SendType == typeof(GetConfig))
                    {
                        WriteFrame(new ScopeControlFrames.GetConfig(), scope.SerialPort);
                    }
                    else if (SendType == typeof(EnterUSBScopeMode))
                    {
                        WriteFrame(new ScopeControlFrames.EnterUSBScopeMode(), scope.SerialPort);
                    }
                    return ReturnFrame(ReturnType, scope.Buffer, scope.TimeoutTime);
                }
                catch (InvalidDataFrameException ex)
                {
                    lastEx = ex.Message;
                    System.Threading.Thread.Sleep(10);
                }
            }
            stringData = "";
            foreach (var data in scope.Buffer)
            {
                stringData += data + ",";
            }
            stringData.Remove(stringData.Length - 1);
            throw new TimeoutException($"Timeout while waiting for frame acknowledge: " + SendType.ToString() + ", " + ReturnType.ToString() + Environment.NewLine+ "Add. err: "+lastEx);
        }


        private DataFrame ReturnFrame(Type FrameType, byte[] buffer, int timeoutTime)
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
            else if (FrameType == typeof(ScopeControlFrames.ScopeReady))
            {
                ScopeControlFrames.ScopeReady ready = new ScopeControlFrames.ScopeReady(buffer);
                return ready;
            }
            else
            {
                throw new InvalidOperationException("Wrong object type");
            }
        }

        private bool WriteFrame(DataFrame frame, IStreamResource port)
        {
            WritedData = frame.Data;
            port.Write(frame.Data, 0, frame.Data.Count());
            return true;
        }
    }
}
