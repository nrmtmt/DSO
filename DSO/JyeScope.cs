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
using DSO.DataFrames;
using DSO.Interfaces;
using DSO.DataFrames.DSO068;

namespace DSO
{
    public abstract class JyeScope : IScope
    {
        //interface event. New measurements are measured by device. At this moment this delegate is generic.
        public event System.EventHandler NewDataInBuffer = delegate { };
        //info event (for debug), also generic delegate.
        public event System.EventHandler Info = delegate { };
        public delegate void NewDataInBufferEventHandler();
        public delegate void InfoEventHandler();

        protected int timeoutTime = 500; //time in with TimeoutException will be thrown. Should be 10 times more than readDelay (to look nice :))
        //back fields
        private Dictionary<int, string> _AvailableTriggerModeSettings = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableTriggerSlopeSettings = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableCoupleSettings = new Dictionary<int, string>();
        private bool _startCapture = false;
        protected Config.ScopeType _scopeType;
        private int _readDelay = 50; //Delay between write and read from serial port. DSO068 allows less readDelay than DSO112, both should work in this settings. Raise in case of errors.
            //Cold start parameters. Shoud be overwritten at first start.
        private int _recordLength = 512;
        private int _timeBase = 13;
        private int _triggerPos = 50;
        private int _triggerLevel = 150;
        private int _sensitivity = 4;
        private int _triggerMode = 0;
        private int _couple = 1;
        private int _triggerSlope = 1;
        private int _verticalPosition = 0;
        //End cold start parameters.
        private ICurrentConfig ScopeConfig;
        private bool _stopCapture = false;
        private float _voltPerDiv;
       

        private Queue<byte> _DataBuffer = new Queue<byte>();
        protected Queue<byte> _CurrentBuffer = new Queue<byte>();

        public IStreamResource SerialPort
        {
            get;
            private set;
        }

        public float MaxVoltage
        {
            get
            {
                return _voltPerDiv * 6;
            }
        }

        public byte[] ShortBuffer
        {
          get
            {
                return _CurrentBuffer.ToArray(); ;
            }
        }
        public byte[] LongBuffer
        {
            get
            {
                return _DataBuffer.ToArray(); ;
            }
        }
        public int TimeoutTime
        {
            get
            {
                return timeoutTime;
            }
            set
            {
                timeoutTime = value;
            }
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
        //event raised when buffer contains more than 5 elements. RaisedBy ReadBuffer() method
        private void Port_DataReceivedEvent(object sender, EventArgs e)
        {
            Info(_CurrentBuffer.ToArray(), null);
            foreach (byte data in _CurrentBuffer)
            {
                _DataBuffer.Enqueue(data);
            }

            if (_DataBuffer.Count() > _recordLength * 2 && ScopeConfig != null)
            {
                var measurements = Measurements.GetFromBuffer(_DataBuffer.ToArray(), _voltPerDiv, ScopeConfig.PointsPerDiv, _recordLength);
                if ( measurements!= null)
                {
                    NewDataInBuffer(measurements, null);
                    _GetCurrentParameters();
                }

                foreach (byte data in _CurrentBuffer)
                {
                    _DataBuffer.Dequeue();
                }
            }
        }

        protected bool WriteFrame(DataFrame frame)
        {
            SerialPort.Write(frame.Data, 0, frame.Data.Count());
            return true;
        }
        public abstract ICurrentConfig GetCurrentConfig();

        private void _GetCurrentParameters()
        {
          
            var param = (CurrParamDataFrame)new AcknowledgedFrame().GetAcknowledgedFrame
               (typeof(ScopeControlFrames.GetParameters), typeof(CurrParamDataFrame), this);
            if(param != null)
            {
                _recordLength = param.RecordLength;
                _timeBase = (int)param.TBase;
                _triggerPos = param.TriggerPosition;
                _triggerLevel = param.TriggerLevel;
                _sensitivity = (int)param.VSensitivity;
                _triggerMode = (int)param.TriggerMode;
                _couple = (int)param.Couple;
                _triggerSlope = (int)param.TriggerSlope;
                _verticalPosition = (int)param.VPosition;
                _voltPerDiv = param.VoltagePerDiv;
            }
           
        }

        public CurrParamDataFrame GetCurrentParameters()
        {
            var paramx = new CurrParamDataFrame((Config.VerticalSensitivity)_sensitivity, (Config.Timebase)_timeBase, (Config.Slope)_triggerSlope, (Config.TriggerMode)_triggerMode, (Config.Coupling)_couple, _triggerLevel, (byte)_triggerPos, _recordLength, _verticalPosition);
            return paramx;
        }

        private bool SetConfig()
        {
            return true;
        }

        private bool SetCurrentParameters()
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutTime) 
            {
                var curParam = new CurrParamDataFrame((DSO.Config.VerticalSensitivity)_sensitivity,
                                                             (DSO.Config.Timebase)_timeBase,
                                                             (DSO.Config.Slope)_triggerSlope,
                                                             (DSO.Config.TriggerMode)_triggerMode, 
                                                             (DSO.Config.Coupling)_couple,
                                                             (byte)_triggerLevel, 
                                                             (byte)_triggerPos, 
                                                             DSO.Config.RecLength[Array.IndexOf(DSO.Config.RecLength, _recordLength)], 
                                                             _verticalPosition);
                WriteFrame(curParam);
                //System.Threading.Thread.Sleep(_readDelay);
                //if (!FrameAcknowledged())
                //{
                //    //do it again
                //}
                //else
                //{
                //    return true;
                //}

                var curParam2 = GetCurrentParameters();  //old version of acknowledge
                if (!curParam.Equals(curParam2))
                {
                    //do it again
                }
                else
                {
                    return true;
                }
            }
            throw new TimeoutException("Timeout while waiting for acknowledge");
        }
        private bool FrameAcknowledged()
        {
            string output = "";
            var tempBuff = _CurrentBuffer;
            bool found = false;
            int zeroCount = 0;
            foreach (byte val in tempBuff)
            {
                if (val > 39 && val < 42)
                {
                    output += Convert.ToString(val);
                    found = true;
                    continue;
                }
                if (val == 0 && found == true)
                {
                    output += Convert.ToString(val);
                    zeroCount++;

                        if(zeroCount > 2)
                        {
                            return true; // 41, 0, 0, 0, 0 - DSO112a ack after sending params
                                         // 40, 0, 0, 0     - DSO068 ack after sending params
                        }
                }
                else
                {
                    found = false;
                }
            }
            return false;
        }

        private void ReadBuffer()
        {
            SerialPort.DiscardInBuffer();
            while (!_stopCapture)
            {
                int bufferSize = SerialPort.BytesToRead;
                byte[] buffer = new byte[bufferSize];
                if(bufferSize > 5)
                {
                    _CurrentBuffer.Clear();
                    //Monitor.Enter(SerialPort);
                    SerialPort.Read(buffer, 0, bufferSize);
                    //Monitor.Exit(SerialPort);
                    foreach (var item in buffer)
                    {
                        _CurrentBuffer.Enqueue(item);
                    }
                    Port_DataReceivedEvent(buffer, null);
                }
                Thread.Sleep(_readDelay);
            }
           _CurrentBuffer = null;
        }

        public byte[] InstReadBuffer()
        {   
            int bufferSize = SerialPort.BytesToRead;
            byte[] buffer = new byte[bufferSize];
            Monitor.Enter(SerialPort);
            SerialPort.Read(buffer, 0, bufferSize);
            Monitor.Exit(SerialPort);
            return buffer;
        }

        protected byte[] GetBuffer()
        {
            return _CurrentBuffer.ToArray();
        }

        //Interface implementation
        public bool Connect()
        {
            Thread BackgroundReader = new Thread(ReadBuffer);
            BackgroundReader.IsBackground = true;
            BackgroundReader.Start();

          
                var Ready = (ScopeControlFrames.ScopeReady)new AcknowledgedFrame().GetAcknowledgedFrame
                            (typeof(ScopeControlFrames.EnterUSBScopeMode), typeof(ScopeControlFrames.ScopeReady), this);

                     ScopeConfig = GetCurrentConfig();
                     _GetCurrentParameters();
                    _scopeType = Ready.ScopeType;
                return true;
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
            while(_CurrentBuffer!= null)
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
                timeoutTime = _readDelay * 10;
                SetCurrentParameters();
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
                SetCurrentParameters();
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
                SetCurrentParameters();
            }
        }

        public float TriggerLevel
        {
            get
            {
                return Measurements.GetScaledData((byte)_triggerLevel, _voltPerDiv, ScopeConfig.PointsPerDiv);
            }

            set
            {
                _triggerLevel = Measurements.GetRawData(value, _voltPerDiv, ScopeConfig.PointsPerDiv);
                SetCurrentParameters();
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
                SetCurrentParameters();
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
                SetCurrentParameters();
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
                SetCurrentParameters();
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
                SetCurrentParameters();
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
                SetCurrentParameters();
            }
        }
      
        public int VerticalPosition
        {
            get
            {
                return _verticalPosition;
            }

            set
            {
                _verticalPosition = value;
                SetCurrentParameters();
            }
        }

        public string ScopeName
        {
            get
            {
                return Convert.ToString(_scopeType);
            }
        }
    }
}

