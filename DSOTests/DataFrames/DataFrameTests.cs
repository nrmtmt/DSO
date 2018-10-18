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
using DSO.Exceptions;

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
            bool result = false;
            try
            {
                var data = Data();
                DSO.DataFrames.DSO112.CurrConfigDataFrame config = new DataFrames.DSO112.CurrConfigDataFrame(Data());
                result = true;
            }
            catch (InvalidDataFrameException)
            {
            }
            Assert.AreEqual(true, result);
        }

        [TestMethod()]
        public void CurrConfigDataFrameTest068()
        {
            bool result = false;
            try
            {
                var data = Data();
                DSO.DataFrames.DSO068.CurrConfigDataFrame config = new DataFrames.DSO068.CurrConfigDataFrame(Data());
                result = true;
            }
            catch (InvalidDataFrameException)
            {
            }
            Assert.AreEqual(true, result);
        }

        [TestMethod()]
        public void CommandAcknowledgedDataFrameTest()
        {
            bool result = false;
            try
            {
                var data = Data();
                CommandAcknowledgedDataFrame ack = new CommandAcknowledgedDataFrame(data);
                result = true;
            }
            catch (InvalidDataFrameException)
            { 
            }
            Assert.AreEqual(true, result);
        }

        [TestMethod()]
        public void CurrParamDataFrameTest()
        {
            bool result = false;
            try
            {
                var data = Data();
                CurrParamDataFrame param = new CurrParamDataFrame(Data());
                result = true;
            }
            catch (InvalidDataFrameException)
            {
            }
            Assert.AreEqual(true, result);   
        }

        [TestMethod()]
        public void DataBlockDataFrameTest()
        {
            bool result = false;
            try
            {
                var data = Data();
                DataBlockDataFrame block = new DataBlockDataFrame(Data());
                result = true;
            }
            catch (InvalidDataFrameException)
            {
            }
            Assert.AreEqual(true, result);
        }

        [TestMethod()]
        public void DataSampleDataFrameTest()
        {
            bool result = false;
            try
            {
                var data = Data();
                DataSampleDataFrame sample = new DataSampleDataFrame(Data());
                result = true;
            }
            catch (InvalidDataFrameException)
            {
            }
            Assert.AreEqual(true, result);
        }

        [TestMethod()]
        public void ScopeReadyDataFrameTest()
        {
            bool result = false;
            try
            {
                var data = Data();
                DSO.ScopeControlFrames.ScopeReady ready = new ScopeControlFrames.ScopeReady(Data());
                result = true;
            }
            catch (InvalidDataFrameException)
            {
            }
            Assert.AreEqual(true, result);
        }
    }
}