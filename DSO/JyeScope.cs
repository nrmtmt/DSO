using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using DSO.Utilities;

namespace DSO
{
    public abstract class JyeScope : IScope
    {
        //interface event
        public event System.EventHandler NewDataInBuffer = delegate { };
        //info event (for debug)
        public event System.EventHandler Info = delegate { };
        public delegate void NewDataInBufferEventHandler();
        public delegate void InfoEventHandler();
        private int timeoutTime = 6000; //time in with TimeoutException will be thrown
        //back fields
        private Dictionary<int, string> _AvailableTriggerModeSettings = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableTriggerSlopeSettings = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableCoupleSettings = new Dictionary<int, string>();
        private bool _startCapture = false;
        private int _readDelay = 30;
        private int _recordLength = 256;
        private int _timeBase = 3;
        private int _triggerPos = 50;
        private int _triggerLevel = 100;
        private int _sensitivity = 7;
        private int _triggerMode = 1;
        private int _couple = 1;
        private int _triggerSlope = 1;

        private bool _stopCapture = false;

        private List<byte> LongBuffer = new List<byte>();
        private byte[] CurrentBuffer = null;

        public IStreamResource SerialPort
        {
            get;
            private set;
        }

        public JyeScope(IStreamResource port)
        {
            SerialPort = port;
            port.DataReceivedEvent += Port_DataReceivedEvent;

            foreach (var couple in (int[])Enum.GetValues(typeof(DSO.Config.Coupling)))
            {
                _AvailableCoupleSettings.Add(couple, Enum.GetName(typeof(DSO.Config.Coupling), couple));
            }
            foreach (var mode in (int[])Enum.GetValues(typeof(DSO.Config.TriggerMode)))
            {
                _AvailableTriggerModeSettings.Add(mode, Enum.GetName(typeof(DSO.Config.TriggerMode), mode));
            }
            foreach (var slope in (int[])Enum.GetValues(typeof(DSO.Config.Slope)))
            {
                _AvailableTriggerSlopeSettings.Add(slope, Enum.GetName(typeof(DSO.Config.Slope), slope));
            }
        }

        private void Port_DataReceivedEvent(object sender, EventArgs e)
        {
            //populate buffer
            Info(sender, null);
            CurrentBuffer = ((byte[])sender);
            LongBuffer.AddRange(CurrentBuffer);

            if (LongBuffer.Count() > _recordLength)
            {
                GenerateFrame(LongBuffer.ToArray());
                LongBuffer.Clear(); ;
            }
        }

        private void GenerateFrame(byte[] data)
        {
            try
            {
                var DataFrame = new DataBlockDataFrame(data);
                if (DataFrame != null)
                {
                    byte[] rawData = new byte[DataFrame.Data.Count() - 14]; //4 reserved
                    for (int i = 5; i < DataFrame.Data.Count() - 9; i++) //[syncChar][frameID][frameSize][frameSize][frameFunc][data1]...[dataN][8][0][0][0][0][0][0][0][0]
                    {
                        rawData[i - 5] = DataFrame.Data[i];
                    }
                    NewDataInBuffer(rawData, null);
                }
            }
            catch (InvalidDataFrameException ex)
            {
                try
                {
                    var DataFrame = new DataSampleDataFrame(data);
                    if (DataFrame != null)
                    {
                        byte[] rawData = new byte[DataFrame.Data.Count() - 13]; //3 reserved
                        for (int i = 5; i < DataFrame.Data.Count() - 9; i++)
                        {
                            rawData[i - 5] = DataFrame.Data[i];
                        }
                        NewDataInBuffer(rawData, null);
                    }
                }
                catch (InvalidDataFrameException ex2)
                {

                }
            }
        }

        protected bool WriteFrame(DataFrame frame)
        {
            SerialPort.Write(frame.Data, 0, frame.Data.Count());
            return true;
        }


        public CurrConfigDataFrame GetCurrentConfig() //seems to be same in each jye scope
        {
            try
            {
                if (WriteFrame(new ScopeControlFrames.GetConfig()))
                {
                    var conf = (CurrConfigDataFrame)GetAcknowledgedFrame.Get(typeof(CurrConfigDataFrame), ref CurrentBuffer, ref timeoutTime);
                  
                    return conf;
                }
            }
            catch (TimeoutException)
            {
            }
            return GetCurrentConfig();
        }

        public CurrParamDataFrame GetCurrentParameters()
        {
            try
            {
                if (WriteFrame(new ScopeControlFrames.GetParam()))
                {
                    var param = (CurrParamDataFrame)GetAcknowledgedFrame.Get(typeof(CurrParamDataFrame), ref CurrentBuffer, ref timeoutTime);
                    _recordLength = param.RecordLength;
                    _timeBase = (int)param.TBase;
                    _triggerPos = param.TriggerPosition;
                    _triggerLevel = param.TriggerLevel;
                    _sensitivity = (int)param.VSensitivity;
                    _triggerMode = (int)param.TriggerMode;
                    _couple = (int)param.Couple;
                    _triggerSlope = (int)param.TriggerSlope;
                    return param;
                }
            }
            catch (TimeoutException)
            {
            }
            return GetCurrentParameters();
        }

        private bool SetConfig()
        {
            return true;
        }

        public bool SetCurrentParameters()
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutTime) 
            {
                var curParam = new DSO.CurrParamDataFrame((DSO.Config.VerticalSensitivity)_sensitivity,
                                                             (DSO.Config.Timebase)_timeBase,
                                                             (DSO.Config.Slope)_triggerSlope,
                                                             (DSO.Config.TriggerMode)_triggerMode,
                                                             (byte)_triggerLevel, 
                                                             (byte)_triggerPos, 
                                                             DSO.Config.RecLength[Array.IndexOf(DSO.Config.RecLength, _recordLength)]);
                WriteFrame(curParam);
                Thread.Sleep(timeoutTime);
                var curParam2 = GetCurrentParameters();
                if (!curParam.Equals(curParam2))
                {
                    //do it again
                }else
                {
                    return true;
                }
            } 
            throw new TimeoutException("Timeout while waiting for acknowledge");
        }

        private void ReadBuffer()
        {
            SerialPort.DiscardInBuffer();
            while (!_stopCapture)
            {
                byte[] buffer = InstReadBuffer();
                Port_DataReceivedEvent(buffer, null);
                Thread.Sleep(_readDelay);
            }
            CurrentBuffer = null;
        }

        private byte[] InstReadBuffer()
        {
            int bufferSize = SerialPort.BytesToRead;
            byte[] buffer = new byte[bufferSize];
            SerialPort.Read(buffer, 0, bufferSize);
            return buffer;
        }

        protected byte[] GetBuffer()
        {
            return CurrentBuffer;
        }

        protected byte[] GetLongBuffer()
        {
            return LongBuffer.ToArray();
        }

        //Interface implementation
        public bool Connect()
        {
            WriteFrame(new ScopeControlFrames.EnterUSBScopeMode());
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutTime)
            {
                try
                {
                    CurrentBuffer = InstReadBuffer();
                    if (new ScopeControlFrames.ScopeReady(CurrentBuffer) != null)
                    {
                        Thread BackgroundReader = new Thread(ReadBuffer);
                        BackgroundReader.IsBackground = true;
                        BackgroundReader.Start();
                        GetCurrentConfig();
                        GetCurrentParameters();
                        return true;
                    }
                }
                catch (InvalidDataFrameException ex)
                {
                    WriteFrame(new ScopeControlFrames.ExitUSBScopeMode());
                    Thread.Sleep(100);
                    WriteFrame(new ScopeControlFrames.EnterUSBScopeMode());
                }
            }
            throw new TimeoutException("Timeout while waiting for ScopeReady acknowledge");
        }

        public bool Disconnect()
        {
            WriteFrame(new ScopeControlFrames.ExitUSBScopeMode());
            _stopCapture = true;
            return true;
        }

        public IScope Create()
        {
            return this;
        }

        public bool Destroy()
        {
            SerialPort.Dispose();
            return true;
        }

        public bool StartCapture()
        {
            return true;
        }

        public bool StopCapture()
        {
            while(CurrentBuffer!= null)
            {
                _stopCapture = true;
            }
            return true;
        }

        public long[] GetScaledData()
        {
            throw new NotImplementedException();
        }

        public abstract Dictionary<int, string> AvailableTimebaseSettings { get; }

        public Dictionary<int, string> AvailableCoupleSettings
        {
            get
            {
                return _AvailableCoupleSettings;
            }
        }

        public Dictionary<int, string> AvailableTriggerSlopeSettings
        {
            get
            {
                return _AvailableTriggerSlopeSettings;
            }
        }

        public abstract Dictionary<int, string> AvailableSenitivitySettings { get; }

        public Dictionary<int, string> AvailableTriggerModeSettings
        {
            get
            {
                return _AvailableTriggerModeSettings;
            }
        }

        public int ReadDelay
        {
            get
            {
                return _readDelay;
            }

            set
            {
                _readDelay = value;
            }
        }

        public int TimeBase
        {
            get
            {
                return _timeBase;
            }

            set
            {
                _timeBase = value;
            }
        }

        public int TriggerPos
        {
            get
            {
                return _triggerPos;
            }

            set
            {
                _triggerPos = value;
            }
        }

        public int TriggerLevel
        {
            get
            {
                return _triggerLevel;
            }

            set
            {
                _triggerLevel = value;
            }
        }

        public int Sensitivity
        {
            get
            {
                return _sensitivity;
            }

            set
            {
                _sensitivity = value;
            }
        }

        public int TriggerMode
        {
            get
            {
                return _triggerMode;
            }

            set
            {
                _triggerMode = value;
            }
        }

        public int Couple
        {
            get
            {
                return _couple;
            }

            set
            {
                _couple = value;
            }
        }

        public int TriggerSlope
        {
            get
            {
                return _triggerSlope;
            }

            set
            {
                _triggerSlope = value;
            }
        }

        public abstract Dictionary<int, string> AvailableRecordLength { get; }

        public int RecordLength
        {
            get
            {
                return _recordLength;
            }

            set
            {
                _recordLength = value;
            }
        }
    }
}

