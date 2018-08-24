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
        public event System.EventHandler NewDataInBuffer = delegate { };
        public event System.EventHandler Info = delegate { };
        public delegate void NewDataInBufferEventHandler();
        public delegate void InfoEventHandler();
        private System.Timers.Timer ReadTimer;
        private int _readDelay = 50;
        private int _recordLength = 1024;
        private bool _stopCapture = false;
        private Dictionary<int, string> _AvailableTriggerModeSettings = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableTriggerSlopeSettings = new Dictionary<int, string>();
        private Dictionary<int, string> _AvailableCoupleSettings = new Dictionary<int, string>();
 

        private List<byte> LongBuffer = new List<byte>();
        private byte[] CurrentBuffer = null;

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
            CurrentBuffer = ((byte[]) sender);
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

        public IScope Create()
        {
            return this;
        }

        public bool Destroy()
        {
            SerialPort.Dispose();
            return true;
        }
        
        public IStreamResource SerialPort
        {
            get;
            private set;
        }

        public abstract Dictionary<int, string> AvailableTimebaseSettings { get; }
    

        public Dictionary<int, string> AvailableCoupleSettings
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<int, string> AvailableTriggerSlopeSettings
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public abstract Dictionary<int, string> AvailableSenitivitySettings { get; }
     

        public Dictionary<int, string> AvailableTriggerModeSettings
        {
            get
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int TriggerPos
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int TriggerLevel
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Sensitivity
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int TriggerMode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Couple
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int TriggerSlope
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public abstract Dictionary<int, string> AvailableRecordLength { get; }

        public int RecordLength
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Connect()
        {
            //ReadTimer = new System.Timers.Timer();
            //ReadTimer.Elapsed += new System.Timers.ElapsedEventHandler(ReadBuffer);
            //ReadTimer.Interval = _readDelay;
            //ReadTimer.Start();
            Thread BackgroundReader = new Thread(ReadBuffer);
            BackgroundReader.IsBackground = true;
            BackgroundReader.Start();

            WriteFrame(new ScopeControlFrames.EnterUSBScopeMode());
            
            if (ScopeReady())
            {
                return true;
            }else
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

        protected bool WriteFrame(DataFrame frame)
        {
            SerialPort.Write(frame.Data, 0, frame.Data.Count());
            return true;
        }

        protected byte[] ReadData()
        {
             byte[] data = new byte[2048];
             SerialPort.Read(data, 0, data.Length);
            //var data = SerialPort.ReadExisting
            return data;
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
                   System.Threading.Thread.Sleep(10);
                }
            }
            return null;
        }
           

        public bool ScopeReady() //seems to be same in each jye scope
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
                System.Threading.Thread.Sleep(10);
            }
            return false;
        }


        public abstract CurrParamDataFrame GetCurrentParameters();
        public abstract CurrParamDataFrame SetCurrentParameters();
        public abstract DataFrame GetData();

        public bool StartCapture()
        {
            _stopCapture = false;
            return true;
        }
       
        private void ReadBuffer()
        {
            while(!_stopCapture)
                {
                int bufferSize = SerialPort.BytesToRead;
                byte[] buffer = new byte[bufferSize];
                SerialPort.Read(buffer, 0, bufferSize);
                Port_DataReceivedEvent(buffer, null);
                Thread.Sleep(_readDelay);
            }
               
        }
        public byte[] GetBuffer()
        {
            return CurrentBuffer;
        }
        public byte[] GetLongBuffer()
        {
            return LongBuffer.ToArray();
        }

        public bool StopCapture()
        {
            throw new NotImplementedException();
        }

        public long[] GetScaledData()
        {
            throw new NotImplementedException();
        }
    }
}

