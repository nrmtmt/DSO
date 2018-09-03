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
        string str = "254,192,4,0,52,254,192,8,1,50,129,128,129,128,129,127,129,128,128,126,129,128,128,127,129,127,127,127,129,128,128,128,129,127,127,128,129,128,128,128,128,127,128,128,129,127,129,128,128,127,129,128,129,127,129,128,127,127,129,128,128,127,129,127,127,128,129,128,128,128,129,127,128,128,129,128,128,128,128,127,129,128,129,127,129,128,128,127,129,128,129,127,129,127,127,127,129,128,128,127,129,127,127,127,129,128,128,128,129,127,128,128,129,128,128,128,128,127,128,128,129,127,129,128,129,127,128,128,128,127,128,128,129,127,129,128,128,127,129,128,129,127,129,128,128,127,129,128,128,127,129,127,127,127,129,128,128,128,129,127,128,128,129,127,128,127,128,127,128,128,129,127,129,128,128,126,129,128,129,127,129,128,128,127,129,128,129,127,129,127,128,127,129,128,128,127,129,127,127,128,129,128,128,127,128,127,128,128,129,128,128,128,128,127,128,128,129,127,129,128,128,127,129,128,129,127,129,128,128,127,129,128,128,127,129,127,127,128,129,128,128,128,129,127,128,128,129,128,128,128,128,126,128,128,129,127,128,128,128,127,129,128,129,127,129,128,8,0,0,0";

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