using DSO.Exceptions;
using DSO.Utilities;
using System;
using System.Diagnostics;
using System.Linq;

namespace DSO
{
    public static class Initialization
    {
        ///<summary>
        ///Returns JyeScope object. If scope is not recognized, throws ScopeNotRecognizedException with information about returned scope type. If no scope is found, throws ScopeNotDetectedException. 
        ///<param name="port">IStreamResource serial port</param>
        ///</summary>
        ///
        public static JyeScope GetScope(SerialPortAdapter port)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Restart();
            while (stopwatch.ElapsedMilliseconds < 2000)
            {
                WriteFrame(new DataFrames.ScopeControlDataFrames.EnterUSBScopeMode(), port);
                System.Threading.Thread.Sleep(100);
                try
                {
                    DataFrames.ScopeControlDataFrames.ScopeReady Ready = new DataFrames.ScopeControlDataFrames.ScopeReady(InstReadBuffer(port));
                    Config.ScopeType properScope = Ready.ScopeType;
                    switch (properScope)
                    {
                        case Config.ScopeType.DSO082:
                            throw new ScopeNotSupportedException($"Scope recognized but not supported. Returned scope type: {(int)Ready.ScopeType}.");
                        case Config.ScopeType.DSO094:
                            throw new ScopeNotSupportedException($"Scope recognized but not supported. Returned scope type: {(int)Ready.ScopeType}.");
                        case Config.ScopeType.DSO068:
                            return new DSO068.DSO068(port);
                        case Config.ScopeType.DSO112A:
                            return new DSO112.DSO112(port);
                        default:
                            throw new ScopeNotRecognizedException($"Scope not recognized. Returned scope type: {(int)Ready.ScopeType}.");
                    }
                }
                catch (InvalidDataFrameException ex)
                {
                    //try again
                }
            }
            throw new ScopeNotDetectedException("Scope not detected.");
        }
        private static byte[] InstReadBuffer(IStreamResource port)
        {
            int bufferSize = port.BytesToRead;
            byte[] buffer = new byte[bufferSize];
            port.Read(buffer, 0, bufferSize);
            return buffer;
        }
        private static bool WriteFrame(DataFrame frame, IStreamResource port)
        {
            port.Write(frame.Data, 0, frame.Data.Count());
            return true;
        }
    }
}
