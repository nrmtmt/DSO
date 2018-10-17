using Microsoft.VisualStudio.TestTools.UnitTesting;
using DSO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSO.DataFrames.DSO068;
using DSO.DataFrames;
using DSO.DataFrames.DSO112;

namespace DSO.Tests
{
    [TestClass()]
    public class DataFrameTests
    {
        //254, 230, 4, 0, 0
        string str = "254, 230, 4, 0, 0";

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
        public void CurrConfigDataFrameTest112A()
        { 
         DSO.DataFrames.DSO112.CurrConfigDataFrame config = new DataFrames.DSO112.CurrConfigDataFrame(Data());
        }
        public void CurrConfigDataFrameTest068()
        {
            DSO.DataFrames.DSO068.CurrConfigDataFrame config = new DataFrames.DSO068.CurrConfigDataFrame(Data());
        }

        [TestMethod()]
        public void CommandAcknowledgedDataFrameTest()
        {
            var data = Data();
           CommandAcknowledgedDataFrame config = new CommandAcknowledgedDataFrame(data);
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