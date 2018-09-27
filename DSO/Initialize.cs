using DSO.Utilities;
using System;
using System.Linq;

namespace DSO
{
    public class Initialize
    {
        public static JyeScope GetScope(SerialPortAdapter port)
        {
            WriteFrame(new ScopeControlFrames.EnterUSBScopeMode(), port);
            System.Threading.Thread.Sleep(50);
            ScopeControlFrames.ScopeReady Ready = new ScopeControlFrames.ScopeReady(InstReadBuffer(port));
            Config.ScopeType properScope = Ready.ScopeType;
            switch (properScope)
            {
                case Config.ScopeType.DSO068:
                    return new DSO068.DSO068(port);
                case Config.ScopeType.DSO112A:
                    return new DSO112.DSO112(port);
                case Config.ScopeType.Unspecified:
                    throw new ScopeNotDetectedException($"Scope not detected. Returned scope type: {(int)Ready.ScopeType}.");
                default:
                    throw new NullReferenceException();
            }
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
            //Monitor.Enter(port);
            port.Write(frame.Data, 0, frame.Data.Count());
            //Monitor.Exit(port);
            return true;
        }
    }
    public class ScopeNotDetectedException : Exception
    {
        public ScopeNotDetectedException() : base() { }
        public ScopeNotDetectedException(string message) : base(message) { }
        public ScopeNotDetectedException(string message, System.Exception inner) : base(message, inner) { }
    }
}
