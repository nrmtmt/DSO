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
        string str = "254,192,8,1,50,129,178,177,177,178,176,128,128,128,127,129,177,177,177,178,176,129,128,128,127,129,177,177,177,178,176,129,127,128,127,129,177,177,178,178,176,129,127,128,128,128,176,177,178,178,176,128,127,128,128,129,176,177,177,177,177,128,127,128,128,128,176,178,177,177,177,128,127,128,128,128,176,178,177,177,177,128,127,128,128,128,177,178,177,177,177,128,127,129,128,128,177,179,177,177,177,128,127,129,128,128,176,178,177,177,177,128,127,129,127,128,177,178,176,177,177,128,127,129,127,128,178,178,176,178,177,128,127,129,127,128,178,178,176,178,177,127,127,129,127,128,178,178,176,178,177,127,128,129,127,128,178,178,176,178,177,128,128,128,127,128,177,178,176,178,176,128,128,128,127,129,178,177,176,178,176,128,128,128,127,129,177,177,177,178,176,128,128,128,127,129,177,177,177,178,176,128,128,128,127,129,177,177,178,178,176,129,127,128,127,129,177,177,178,178,176,129,127,128,128,129,177,177,178,178,176,129,127,128,128,129,177,177,178,178,176,128,127,128,128,129,177,177,178,177,176,128,127,128,128,129,176,178,177,177,177,32,0,0,0,254,226,16,0,79,4,68,83,79,48,54,56,0,0,0,0,255";

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
            CurrConfigDataFrame config = new CurrConfigDataFrame(Data());
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