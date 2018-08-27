using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Timers;

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

        //back fields
        private Dictionary<int, string> _AvailableTriggerModeSettings = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableTriggerSlopeSettings = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableCoupleSettings = new Dictionary<int, string>();
        private int _readDelay = 30;
        private int _recordLength = 1024;
        private int _timeBase = 2;
        private int _triggerPos;
        private int _triggerLevel;
        private int _senstivity;
        private int _triggerMode;
        private int _couple;
        private int _triggerSlope;

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

            if (WriteFrame(new ScopeControlFrames.GetConfig()))
            {
                try
                {
                    CurrConfigDataFrame CurrConfig = new CurrConfigDataFrame(CurrentBuffer);
                    return CurrConfig;
                }
                catch (InvalidDataFrameException ex)
                {
                    GetCurrentConfig();
                }
            }
            return null;
        }
        private bool SetConfig()
        {
            return true;
        }
        public bool ScopeReady()
        {
            try
            {
                if (new ScopeControlFrames.ScopeReady(CurrentBuffer) != null)
                {
                    return true;
                }
            }
            catch (InvalidDataFrameException ex)
            {
            }
            return false;
        }

        public CurrParamDataFrame GetCurrentParameters()
        {
            try
            {
                if (WriteFrame(new ScopeControlFrames.GetParam()))
                {
                    CurrParamDataFrame CurrParam = new CurrParamDataFrame(CurrentBuffer);
                    return CurrParam;
                }
            }
            catch (InvalidDataFrameException ex)
            {
                GetCurrentParameters(); //need to add some logic or timeout
            }
            return null;
        }

        public bool SetCurrentParameters()
        {
            var curParam = new DSO.CurrParamDataFrame((DSO.Config.Slope)_triggerSlope, 
                                                     (DSO.Config.Timebase)_timeBase, 
                                                     (DSO.Config.TriggerMode)_triggerMode, 
                                                     (byte)_triggerLevel,
                                                     (byte)_triggerPos, 
                                                     DSO.Config.RecLength[Array.IndexOf(DSO.Config.RecLength, _recordLength)]);
            WriteFrame(curParam);
            var curParam2 = GetCurrentParameters();
            if(curParam == curParam2)
            {
                return true;
            }else
            {
                SetCurrentParameters(); //need to add some logic or timeout
            }
            return false;
        }

        public bool StartCapture()
        {
            _stopCapture = false;
            return true;
        }

        private void ReadBuffer()
        {
            while (!_stopCapture)
            {
                int bufferSize = SerialPort.BytesToRead;
                byte[] buffer = new byte[bufferSize];
                SerialPort.Read(buffer, 0, bufferSize);
                Port_DataReceivedEvent(buffer, null);
                Thread.Sleep(_readDelay);
            }
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
            Thread BackgroundReader = new Thread(ReadBuffer);
            BackgroundReader.IsBackground = true;
            BackgroundReader.Start();

            WriteFrame(new ScopeControlFrames.EnterUSBScopeMode());

            if (ScopeReady())
            {
                return true;
            }
            else
            {
                return false;
            }
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

        public bool StopCapture()
        {
            throw new NotImplementedException();
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
                return _senstivity;
            }

            set
            {
                _senstivity = value;
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

