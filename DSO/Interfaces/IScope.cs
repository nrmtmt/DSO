using DSO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DSO
{
    public interface IScope
    {
        event EventHandler NewDataInBuffer;
        IScope Create();
        bool Destroy();
        bool Connect();
        bool Disconnect();
        bool StartCapture();
        bool StopCapture();
        byte[] GetBuffer();
    }
}
