using DSO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace DSO
{
    public abstract class JyeScope : IScope
    {
        // public event EventHandler DataReceived = new EventHandler((byte[] data) => { });
        // public delegate void EventHandler(byte[] data);
        public event System.EventHandler NewDataInBuffer = delegate { };
        public event System.EventHandler Info = delegate { };
        public delegate void NewDataInBufferEventHandler();
        public delegate void InfoEventHandler();


        private List<byte> LongBuffer = new List<byte>();
        private byte[] CurrentBuffer = null;

        public JyeScope(IStreamResource port)
        {
            SerialPort = port;
            port.DataReceivedEvent += Port_DataReceivedEvent;
        }

        private void Port_DataReceivedEvent(object sender, EventArgs e)
        {
            //populate buffer
            Info(sender, null);
            CurrentBuffer = ((byte[]) sender);
            LongBuffer.AddRange(CurrentBuffer);
  
            if (LongBuffer.Count() > 2048) //for now length of full buffer
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

        public bool Connect()
        {
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
            Thread myThread = new Thread(BackgroundCapture);
            myThread.IsBackground = true;
            myThread.Start();
            return true;

        }
        private void BackgroundCapture()
        {
            do
            {
                    int bufferSize = SerialPort.BytesToRead;
                    byte[] buffer = new byte[bufferSize];
                    SerialPort.Read(buffer, 0, bufferSize);
                    Port_DataReceivedEvent(buffer, null);
                    Thread.Sleep(50);
            } while (true);

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
    }
}

