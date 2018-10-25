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
using DSO.Exceptions;

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
        private Dictionary<string, object> _AvailableTriggerModeSettings = new Dictionary<string, object>();
        private Dictionary<string, object> _AvailableTriggerSlopeSettings = new Dictionary<string, object>();
        private Dictionary<string, object> _AvailableCoupleSettings = new Dictionary<string, object>();

        private Dictionary<string, object> _AvailableRecordLength = new Dictionary<string, object>();
        private Dictionary<string, object> _AvailableSensitivitySettings = new Dictionary<string, object>();
        private Dictionary<string, object> _AvailableTimebaseSettings = new Dictionary<string, object>();



        private bool _startCapture = false;
        protected Config.ScopeType _scopeType;
        protected int _readDelay = 50; //Delay between write and read from serial port. DSO068 allows less readDelay than DSO112, both should work in this settings. Raise in case of errors.
                                      //Cold start parameters. Shoud be overwritten at first start.
        protected int _recordLength = 512;
        protected int _timeBase = 13;
        protected int _triggerPos = 50;
        protected int _triggerLevel = 150;
        protected int _sensitivity = 4;
        protected int _triggerMode = 0;
        protected int _couple = 1;
        protected int _triggerSlope = 1;
        protected int _verticalPosition = 0;
        //End cold start parameters.
        protected ICurrentConfig ScopeConfig;
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
                var measurements = Measurements.GetFromBuffer(_DataBuffer.ToArray(), _voltPerDiv, ScopeConfig.PointsPerDiv, _recordLength, _verticalPosition);
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

        //abstract methods

        public abstract ICurrentConfig GetCurrentConfig();
        protected abstract bool ChangeParamAcknowledged();

        //end of abstract methods


        protected bool WriteFrame(DataFrame frame)
        {
            SerialPort.Write(frame.Data, 0, frame.Data.Count());
            return true;
        }

       

        private void _GetCurrentParameters()
        {
          
            var param = (CurrParamDataFrame)new AcknowledgedFrame().GetAcknowledgedFrame
               (typeof(ScopeControlFrames.GetParameters), typeof(CurrParamDataFrame), this);
            if(param != null)
            {
                _recordLength = (int)param.RecordLength;
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
            var paramx = new CurrParamDataFrame((Config.VerticalSensitivity)_sensitivity, (Config.Timebase)_timeBase, (Config.Slope)_triggerSlope, (Config.TriggerMode)_triggerMode, (Config.Coupling)_couple, _triggerLevel, (byte)_triggerPos, (Config.RecordLength)_recordLength, _verticalPosition);
            return paramx;
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
                CurrParamDataFrame curParam = null;
                try
                {
                    curParam = new CurrParamDataFrame((DSO.Config.VerticalSensitivity)_sensitivity,
                                                           (DSO.Config.Timebase)_timeBase,
                                                           (DSO.Config.Slope)_triggerSlope,
                                                           (DSO.Config.TriggerMode)_triggerMode,
                                                           (DSO.Config.Coupling)_couple,
                                                           (byte)_triggerLevel,
                                                           (byte)_triggerPos,
                                                           (DSO.Config.RecordLength)_recordLength,                                                        
                                                           _verticalPosition);
                    //DSO.Config.RecLength[Array.IndexOf(DSO.Config.RecLength, _recordLength)],

                    WriteFrame(curParam);
                } catch (Exception ex)
                {
                    throw new ParametersNotSetException("Cannot set parameters. Error while writing or creating DataFrame: " + ex.Message);
                }

                if (!ChangeParamAcknowledged())
                {
                    //do it again
                }
                else
                {
                    return true;
                }
            }
            throw new ParametersNotSetException("Cannot set parameters. Timeout while waiting for acknowledge.");
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

        private void PopulateConfigDictionaries()
        {
            _AvailableCoupleSettings.Clear();
            _AvailableRecordLength.Clear();
            _AvailableSensitivitySettings.Clear();
            _AvailableTimebaseSettings.Clear();
            _AvailableTriggerModeSettings.Clear();
            _AvailableTriggerSlopeSettings.Clear();

            foreach (var length in Enum.GetValues(typeof(DSO.Config.RecordLength)))
            {
                if (((int)length >= ScopeConfig.MinRecordLength && (int)length <= ScopeConfig.MaxRecordLength))
                {
                    _AvailableRecordLength.Add(Convert.ToString((int)length), (DSO.Config.RecordLength)length);
                }
            }

            foreach (var couple in Enum.GetValues(typeof(DSO.Config.Coupling)))
            {
                if (((int)couple >= (int)ScopeConfig.MinCoupleSetting && (int)couple <= (int)ScopeConfig.MaxCoupleSetting))
                {
                    _AvailableCoupleSettings.Add((Convert.ToString(couple)), (DSO.Config.Coupling)couple);
                }
            }

            foreach (var sensitivity in Enum.GetValues(typeof(DSO.Config.VerticalSensitivity)))
            {
                if ((int)sensitivity >= (int)ScopeConfig.MinVerticalSensitivity && (int)sensitivity <= (int)ScopeConfig.MaxVerticalSensitivity)
                {
                    _AvailableSensitivitySettings.Add(Convert.ToString(sensitivity), (DSO.Config.VerticalSensitivity)sensitivity);
                }
            }

            foreach (var timebase in Enum.GetValues(typeof(DSO.Config.Timebase)))
            {
                if ((int)timebase >= (int)ScopeConfig.MinTimebaseSetting && (int)timebase <= (int)ScopeConfig.MaxTimebaseSetting)
                {
                    _AvailableTimebaseSettings.Add(Convert.ToString(timebase), (DSO.Config.Timebase)timebase);
                }
            }

            foreach (var mode in Enum.GetValues(typeof(DSO.Config.TriggerMode)))
            {
                if ((int)mode >= (int)ScopeConfig.MinTriggerModeSetting && (int)mode <= (int)ScopeConfig.MaxTriggerModeSetting)
                {
                    _AvailableTriggerModeSettings.Add(Convert.ToString(mode), (DSO.Config.TriggerMode)mode);
                }
            }

            foreach (var slope in Enum.GetValues(typeof(DSO.Config.Slope)))
            {
                if ((int)slope >= (int)ScopeConfig.MinSlopeModeSetting && (int)slope <= (int)ScopeConfig.MaxSlopeModeSetting)
                {
                    _AvailableTriggerSlopeSettings.Add(Convert.ToString(slope), (DSO.Config.Slope)slope);
                }
            }
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
                    PopulateConfigDictionaries();
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


        public Dictionary<string, object> AvailableTimebaseSettings
        {
            get
            {
                return _AvailableTimebaseSettings;
            }

        }

        public Dictionary<string, object> AvailableCoupleSettings
        {
            get
            {
                return _AvailableCoupleSettings;
            }
        }

        public Dictionary<string, object> AvailableTriggerSlopeSettings
        {
            get
            {
                return _AvailableTriggerSlopeSettings;
            }
        }

        public Dictionary<string, object> AvailableSenitivitySettings
        {
            get
            {
                return _AvailableSensitivitySettings;
            }

        }

        public Dictionary<string, object> AvailableTriggerModeSettings
        {
            get
            {
                return _AvailableTriggerModeSettings;
            }
        }
        public Dictionary<string, object> AvailableRecordLength
        {
            get
            {
                return _AvailableRecordLength;
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
                return Measurements.GetScaledData((byte)_triggerLevel, _voltPerDiv, ScopeConfig.PointsPerDiv, _verticalPosition);
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

