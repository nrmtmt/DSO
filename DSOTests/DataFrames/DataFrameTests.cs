using Microsoft.VisualStudio.TestTools.UnitTesting;
using DSO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSO.DataFrames.DSO068;
using DSO.DataFrames;

namespace DSO.Tests
{
    [TestClass()]
    public class DataFrameTests
    {
        string str = "254,192,52,0,34,11,1,16,0,0,0,0,0,21,0,0,0,0,1,127,0,2,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";

        public byte[] Data()
        {
            List<byte> _data = new List<byte>();
            foreach(var dt in str.Split(','))
            {
                _data.Add(Convert.ToByte(dt));
            }
            return _data.ToArray();
        }

        [TestMethod()]
        public void CurrConfigDataFrameTest()
        { 
           // CurrConfigDataFrame config = new CurrConfigDataFrame(Data());

          var param =  (new CurrParamDataFrame((DSO.Config.VerticalSensitivity)8,
                                                             (DSO.Config.Timebase)27,
                                                             (DSO.Config.Slope)0,
                                                             (DSO.Config.TriggerMode)0,
                                                             (DSO.Config.Coupling)1,
                                                             (byte)127,
                                                             (byte)50,
                                                             DSO.Config.RecLength[Array.IndexOf(DSO.Config.RecLength, 0)],
                                                             127));
        }

        [TestMethod()]
        public void CurrParamDataFrameTest()
        {
            CurrParamDataFrame config = new CurrParamDataFrame(Data());
        }

        [TestMethod()]
        public void DataBlockDataFrameTest()
        {
            DataBlockDataFrame config = new DataBlockDataFrame(Data());
        }
        [TestMethod()]
        public void DataSampleDataFrameTest()
        {
            DataSampleDataFrame config = new DataSampleDataFrame(Data());
        }
        [TestMethod()]
        public void ScopeReadyDataFrameTest()
        {
            DSO.ScopeControlFrames.ScopeReady config = new ScopeControlFrames.ScopeReady(Data());
        }


    }
}