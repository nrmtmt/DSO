using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;

namespace DSO
{
    /// <summary>
    ///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public class SerialPortAdapter : IStreamResource
    {
        private const string NewLine = "\r\n";
        private SerialPort _serialPort;

        private int bufferSize = 2048;
        public event System.EventHandler DataReceivedEvent;

        public SerialPortAdapter(SerialPort serialPort)
        {
            Debug.Assert(serialPort != null, "Argument serialPort cannot be null.");
            _serialPort = serialPort;
           // _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
            _serialPort.NewLine = NewLine;
        }

        public int InfiniteTimeout
        {
            get { return SerialPort.InfiniteTimeout; }
        }

        public int ReadTimeout
        {
            get { return _serialPort.ReadTimeout; }
            set { _serialPort.ReadTimeout = value; }
        }

        public int WriteTimeout
        {
            get { return _serialPort.WriteTimeout; }
            set { _serialPort.WriteTimeout = value; }
        }

        public int BufferSize
        {
            get { return bufferSize; }  
        }

        public int BytesToRead
        {
            get
            {
               return _serialPort.BytesToRead; 
            }
        }

        public void DiscardInBuffer()
        {
            _serialPort.DiscardInBuffer();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _serialPort.Read(buffer, offset, count);
        }
      
        public void Write(byte[] buffer, int offset, int count)
        {
            _serialPort.Write(buffer, offset, count);
        }

        public void Write(DataFrame frame)
        {
            _serialPort.Write(frame.Data, 0, frame.Data.Count());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serialPort?.Dispose();
                _serialPort = null;
            }
        }

        private void ReadBuffer(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[bufferSize];
            _serialPort.Read(data, 0, bufferSize);
            DataReceivedEvent(data, null);
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = _serialPort.BytesToRead;
            byte[] data = new byte[bytesToRead];
            _serialPort.Read(data, 0, bytesToRead);
            DataReceivedEvent(data, null);
        }

    }
}
