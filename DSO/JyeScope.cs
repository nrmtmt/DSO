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
using DSO.Parameters;
using System.Text.RegularExpressions;

namespace DSO
{
    public abstract class JyeScope : DSO.IScope
    {
        //Interface implementation

        //interface event. New measurements are measured by device. At this moment this delegate is generic.
        public event System.EventHandler NewDataInBuffer = delegate { };
        public bool Connect()
        {

            Thread BackgroundReader = new Thread(ReadBuffer);
            BackgroundReader.IsBackground = true;
            BackgroundReader.Start();


            var Ready = (DataFrames.ScopeControlDataFrames.ScopeReady)new AcknowledgedFrame().GetAcknowledgedFrame
                        (typeof(DataFrames.ScopeControlDataFrames.EnterUSBScopeMode), typeof(DataFrames.ScopeControlDataFrames.ScopeReady), this);

            ScopeConfig = GetCurrentConfig();
            PopulateConfigDictionaries();
            _GetCurrentParameters();
            _scopeType = Ready.ScopeType;
            return true;
        }

        public bool Disconnect()
        {
            WriteFrame(new DataFrames.ScopeControlDataFrames.ExitUSBScopeMode());
            _stopCapture = true;
            return true;
        }

        public bool Destroy()
        {
            Disconnect();
            SerialPort.Dispose();
            return true;
        }

        public bool StartCapture()
        {
            WriteFrame(new DSO.DataFrames.ScopeControlDataFrames.EnterUSBScopeMode());
            return true;
        }

        public bool StopCapture()
        {
            WriteFrame(new DSO.DataFrames.ScopeControlDataFrames.ExitUSBScopeMode());
            return true;
        }

        public List<IParameter<Config.Timebase>> AvailableTimebaseSettings
        {
            get
            {
                return _AvailableTimebaseSettings;
            }
        }

        public List<IParameter<Config.Coupling>> AvailableCoupleSettings
        {
            get
            {
                return _AvailableCoupleSettings;
            }
        }

        public List<IParameter<Config.Slope>> AvailableTriggerSlopeSettings
        {
            get
            {
                return _AvailableTriggerSlopeSettings;
            }
        }

        public List<IParameter<Config.VerticalSensitivity>> AvailableSenitivitySettings
        {
            get
            {
                return _AvailableSensitivitySettings;
            }
        }

        public List<IParameter<Config.TriggerMode>> AvailableTriggerModeSettings
        {
            get
            {
                return _AvailableTriggerModeSettings;
            }
        }
        public List<IParameter<Config.RecordLength>> AvailableRecordLength
        {
            get
            {
                return _AvailableRecordLength;
            }
        }

        public IParameter<Config.Timebase> TimeBase
        {
            get
            {
                return _TimeBase;
            }

            set
            {
                _TimeBase = value;
                SetCurrentParameters();
            }
        }

        public int TriggerPosition
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

        public virtual float TriggerLevel
        {
            get
            {
                var output = Measurements.GetScaledData(_triggerLevel, _voltPerDiv, ScopeConfig.PointsPerDiv, _verticalPosition, ScopeConfig.VerticalPositionChangeableByHost);
                return output;
            }

            set
            {
                _triggerLevel = Measurements.GetRawData((float)value, _voltPerDiv, ScopeConfig.PointsPerDiv);
                SetCurrentParameters();
            }
        }

        public IParameter<Config.VerticalSensitivity> Sensitivity
        {
            get
            {
                return _Sensitivity;
            }

            set
            {
                _Sensitivity = value;
                SetCurrentParameters();
            }
        }

        public IParameter<Config.TriggerMode> TriggerMode
        {
            get
            {
                return _TriggerMode;
            }

            set
            {
                _TriggerMode = value;
                SetCurrentParameters();
            }
        }

        public IParameter<Config.Coupling> Couple
        {
            get
            {
                return _Couple;
            }

            set
            {
                _Couple = value;
                SetCurrentParameters();
            }
        }

        public IParameter<Config.Slope> TriggerSlope
        {
            get
            {
                return _TriggerSlope;
            }

            set
            {
                _TriggerSlope = value;
                SetCurrentParameters();
            }
        }

        public IParameter<Config.RecordLength> RecordLength
        {
            get
            {
                return _RecordLength;
            }

            set
            {
                _RecordLength = value;
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

        public IParameter<float> CurrentVoltageLimit
        {
            get
            {
                return new Parameter<float>("Voltage limit", Convert.ToString(_voltPerDiv * 6), "V", true, _voltPerDiv * 6);
            }
        }
        //End of interface implementaion


        //info event (for debug), also generic delegate.
        public event System.EventHandler Info = delegate { };
        public delegate void NewDataInBufferEventHandler();
        public delegate void InfoEventHandler();

        protected int timeoutTime = 500; //time in with TimeoutException will be thrown
        //back fields

        private IParameter<Config.Timebase> _TimeBase;
        private IParameter<Config.VerticalSensitivity> _Sensitivity;
        private IParameter<Config.TriggerMode> _TriggerMode;
        private IParameter<Config.Coupling> _Couple;
        private IParameter<Config.Slope> _TriggerSlope;
        private IParameter<Config.RecordLength> _RecordLength = new Parameter<Config.RecordLength>("Record Length", "512", "Points", false, (DSO.Config.RecordLength)512);


        private List<IParameter<Config.TriggerMode>> _AvailableTriggerModeSettings = new List<IParameter<Config.TriggerMode>>();
        private List<IParameter<Config.Slope>> _AvailableTriggerSlopeSettings = new List<IParameter<Config.Slope>>();
        private List<IParameter<Config.Coupling>> _AvailableCoupleSettings = new List<IParameter<Config.Coupling>>();

        private List<IParameter<Config.RecordLength>> _AvailableRecordLength = new List<IParameter<Config.RecordLength>>();
        private List<IParameter<Config.VerticalSensitivity>> _AvailableSensitivitySettings = new List<IParameter<Config.VerticalSensitivity>>();
        private List<IParameter<Config.Timebase>> _AvailableTimebaseSettings = new List<IParameter<Config.Timebase>>();


        private byte[] _ShortBufferArray; //Queue is constanly changing, so it's not safe to use .ToArray() extension
        private byte[] _LongBufferArray;

        private bool _startCapture = false;
        protected Config.ScopeType _scopeType;
        protected int _readDelay = 100; //Delay between write and read from serial port. DSO068 allows less readDelay than DSO112, both should work in this settings. Raise in case of errors.
                                      //Cold start parameters. Shoud be overwritten at first start.
       
        protected int _triggerPos = 50;
        protected int _triggerLevel = 127;
        protected int _verticalPosition = 0;

        //End cold start parameters.
        protected ICurrentConfig ScopeConfig;
        private bool _stopCapture = false;
        protected float _voltPerDiv;
       

        private Queue<byte> _DataBuffer = new Queue<byte>();
        protected Queue<byte> _CurrentBuffer = new Queue<byte>();

        public IStreamResource SerialPort
        {
            get;
            private set;
        }

        public byte[] ShortBuffer
        {
          get
            {
                return _ShortBufferArray;
            }
        }

        public byte[] LongBuffer
        {
            get
            {
                return _LongBufferArray;
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

        public int ReadDelay
        {
            get
            {
                return _readDelay;
            }

            set
            {
                _readDelay = value;
                timeoutTime = _readDelay * 5;
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

        public JyeScope(IStreamResource port)
        {
            SerialPort = port;
            port.DataReceivedEvent += Port_DataReceivedEvent;
        }

        //event raised when buffer contains more than 5 elements. RaisedBy ReadBuffer() method
        private void Port_DataReceivedEvent(object sender, EventArgs e)
        {
            _ShortBufferArray = _CurrentBuffer.ToArray();
            Info(_ShortBufferArray, null);
            foreach (byte data in _CurrentBuffer)
            {
                _DataBuffer.Enqueue(data);
            }

            if (_DataBuffer.Count() > (int)_RecordLength.GetParameter*2 && ScopeConfig != null)
            {
                var measurements = Measurements.GetFromBuffer(_DataBuffer.ToArray(), _voltPerDiv, ScopeConfig.PointsPerDiv, (int)_RecordLength.GetParameter, _verticalPosition, ScopeConfig.VerticalPositionChangeableByHost);
                if ( measurements!= null)
                {
                    NewDataInBuffer(measurements, null);
                }
                _GetCurrentParameters();
                foreach (byte data in _CurrentBuffer)
                {
                    _DataBuffer.Dequeue();
                }
            }
            _LongBufferArray = _DataBuffer.ToArray();
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

        ///<summary>
        ///Retrieve current parameters from device and set it to JyeScope parameters fields. Returns CurrParamDataFrame with actual settings in success, returns null otherwise.
        ///</summary>
        ///
        protected CurrParamDataFrame _GetCurrentParameters()
        {
          try
            {
                var param = (CurrParamDataFrame)new AcknowledgedFrame().GetAcknowledgedFrame
             (typeof(DataFrames.ScopeControlDataFrames.GetParameters), typeof(CurrParamDataFrame), this);

                _RecordLength = _AvailableRecordLength.Where(x => x.GetParameter == param.RecordLength).First();

                _TimeBase = _AvailableTimebaseSettings.Where(x => x.GetParameter == param.TBase).First();

                _Sensitivity = _AvailableSensitivitySettings.Where(x => x.GetParameter == param.VSensitivity).First();

                _TriggerMode = _AvailableTriggerModeSettings.Where(x => x.GetParameter == param.TriggerMode).First();

                _Couple = _AvailableCoupleSettings.Where(x => x.GetParameter == param.Couple).First();

                _TriggerSlope = _AvailableTriggerSlopeSettings.Where(x => x.GetParameter == param.TriggerSlope).First();

                _verticalPosition = param.VPosition;
                _voltPerDiv = param.VoltagePerDiv;
                _triggerPos = param.TriggerPosition;
                _triggerLevel = param.TriggerLevel;

                return param;

               
            } catch (FrameNotAcknowledgedException ex)
            {
                return null;
            }
        }

        ///<summary>
        /// Returns CurrParamDataFrame from JyeScope parameters.
        ///</summary>
        ///
        public CurrParamDataFrame Get_CurParamDataFrame_From_Current_Object_State()
        {

            var curParam = new CurrParamDataFrame(_Sensitivity.GetParameter,
                                                          _TimeBase.GetParameter,
                                                          _TriggerSlope.GetParameter,
                                                          _TriggerMode.GetParameter,
                                                          _Couple.GetParameter,
                                                          _triggerLevel,
                                                          (byte)_triggerPos,
                                                          _RecordLength.GetParameter,
                                                          _verticalPosition);
            return curParam;
        }

        private bool SetConfig()
        {
            return true;
        }

        ///<summary>
        /// Set JyeScope object parameters to device. Return true in succes, throws ParametersNotSetException otherwise.
        ///</summary>
        ///
        public bool SetCurrentParameters()
        {

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutTime) 
            {
                CurrParamDataFrame curParam = null;
                try
                {
                    curParam = Get_CurParamDataFrame_From_Current_Object_State();
                    WriteFrame(curParam);
                } catch (Exception ex)
                {
                    _GetCurrentParameters();
                    throw new ParametersNotSetException("Cannot set parameters. Error while writing or creating DataFrame: " + ex.Message);
                }
                Thread.Sleep(_readDelay);
                if (!ChangeParamAcknowledged())
                {
                    //do it again 
                }
                else
                {
                    return true;
                }
            }
            _GetCurrentParameters();
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

            lock (SerialPort)
            {
                SerialPort.Read(buffer, 0, bufferSize);
            }

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
                    string name = "Record Length";
                    string unit = "Points";
                    string value = Convert.ToString((int)length);
                    Parameter<Config.RecordLength> RecordLength = new Parameter<Config.RecordLength>(name, value, unit, false, (DSO.Config.RecordLength)length);
                    _AvailableRecordLength.Add(RecordLength);
                }
            }

            foreach (var couple in Enum.GetValues(typeof(DSO.Config.Coupling)))
            {
                if (((int)couple >= (int)ScopeConfig.MinCoupleSetting && (int)couple <= (int)ScopeConfig.MaxCoupleSetting))
                {
                    string name = "Couple";
                    string unit = "Coupling";
                    string value = Convert.ToString(couple);
                    Parameter<Config.Coupling> Couple = new Parameter<Config.Coupling>(name, value, unit, !ScopeConfig.CoupleChangeableByHost, (DSO.Config.Coupling)couple);
                    _AvailableCoupleSettings.Add(Couple);
                }
            }

            foreach (var sensitivity in Enum.GetValues(typeof(DSO.Config.VerticalSensitivity)))
            {
                if ((int)sensitivity >= (int)ScopeConfig.MinVerticalSensitivity && (int)sensitivity <= (int)ScopeConfig.MaxVerticalSensitivity)
                {
                    string name = "Vertical Sensitivity";
                    string unit =  (Regex.Match(Convert.ToString(sensitivity), @"(?![div])(?!\d).*").Value) + "/DIV";
                    string value = Regex.Match(Convert.ToString(sensitivity), @"(\d{1,})").Value;
                    Parameter<Config.VerticalSensitivity> Sensitivity = new Parameter<Config.VerticalSensitivity>(name, value, unit, !ScopeConfig.SensitivityChangeableByHost, (DSO.Config.VerticalSensitivity)sensitivity);
                    _AvailableSensitivitySettings.Add(Sensitivity);
                }
            }

            foreach (var timebase in Enum.GetValues(typeof(DSO.Config.Timebase)))
            {
                if ((int)timebase >= (int)ScopeConfig.MinTimebaseSetting && (int)timebase <= (int)ScopeConfig.MaxTimebaseSetting)
                {
                    string name = "Timebase";
                    string unit = (Regex.Match(Convert.ToString(timebase), @"(?![div])(?!\d).*").Value) + "/DIV";
                    string value = Regex.Match(Convert.ToString(timebase), @"(\d{1,})").Value;
                    Parameter<Config.Timebase> Timebase = new Parameter<Config.Timebase>(name, value, unit, false, (DSO.Config.Timebase)timebase);
                    _AvailableTimebaseSettings.Add(Timebase);
                }
            }

            foreach (var mode in Enum.GetValues(typeof(DSO.Config.TriggerMode)))
            {
                if ((int)mode >= (int)ScopeConfig.MinTriggerModeSetting && (int)mode <= (int)ScopeConfig.MaxTriggerModeSetting)
                {
                    string name = "Trigger mode";
                    string unit = "Mode";
                    string value = Convert.ToString(mode);
                    Parameter<Config.TriggerMode> TriggerMode = new Parameter<Config.TriggerMode>(name, value, unit, false, (DSO.Config.TriggerMode)mode);
                    _AvailableTriggerModeSettings.Add(TriggerMode);
                }
            }

            foreach (var slope in Enum.GetValues(typeof(DSO.Config.Slope)))
            {
                if ((int)slope >= (int)ScopeConfig.MinSlopeModeSetting && (int)slope <= (int)ScopeConfig.MaxSlopeModeSetting)
                {
                    string name = "Trigger slope";
                    string unit = "Edge";
                    string value = Convert.ToString(slope);
                    Parameter<Config.Slope> TriggerSlope = new Parameter<Config.Slope>(name, value, unit, false, (DSO.Config.Slope)slope);
                    _AvailableTriggerSlopeSettings.Add(TriggerSlope);
                }
            }
        }
    }
}

