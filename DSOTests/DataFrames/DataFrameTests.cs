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
        string str = "254,226,16,0,79,4,68,83,79,48,54,56,0,0,0,0,255,254,192,4,0,52,254,192,8,1,50,126,127,126,128,127,127,127,128,126,128,127,127,127,128,126,128,127,127,127,128,126,127,127,127,127,128,126,128,127,127,127,128,126,128,127,127,127,127,126,128,127,127,127,127,127,127,127,127,126,128,127,128,127,127,127,128,126,128,127,127,127,128,127,128,127,127,127,128,127,128,127,127,127,128,126,128,127,127,127,128,126,128,127,128,127,128,127,128,127,127,127,128,126,128,127,127,127,128,126,128,127,127,127,128,126,128,127,127,127,128,126,128,127,127,127,127,126,128,127,127,127,128,126,128,127,127,127,128,126,128,126,127,127,127,126,128,126,128,127,128,127,128,126,128,127,127,127,128,127,128,127,127,127,128,126,128,127,128,127,128,126,128,127,127,127,128,126,128,127,127,127,128,126,128,127,127,127,128,126,128,126,127,127,128,126,128,127,127,127,127,126,128,127,128,127,128,126,128,127,127,127,127,127,128,127,128,127,127,127,128,126,127,127,127,127,128,127,128,127,127,127,128,126,128,127,127,127,128,127,128,127,127,127,128,126,128,127,127,127,128,126,128,127,127,127,128,126,128,127,127,127,128,126,128,127,127,40,0,0,0,254,226,16,0,79,4,68,83,79,48,54,56,0,0,0,0,255,254,226,16,0,79,4,68,83,79,48,54,56,0,0,0,0,255,254,192,56,0,48,1,0,0,0,13,5,2,0,127,0,129,255,13,5,2,0,127,0,129,255,31,3,128,132,30,0,2,0,1,0,255,0,0,0,100,0,0,0,0,0,0,0,0,4,0,0,0,1,0,0,7,0,254,192,56,0,48,1,0,0,0,13,5,2,0,127,0,129,255,13,5,2,0,127,0,129,255,31,3,128,132,30,0,2,0,1,0,255,0,0,0,100,0,0,0,0,0,0,0,0,4,0,0,0,1,0,0,7,0";

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