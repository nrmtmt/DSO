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
        byte[] Data = new byte[] {};
        [TestMethod()]
        public void DataFrameTest()
        { 
            CurrConfigDataFrame config = new CurrConfigDataFrame(Data);
        }

        [TestMethod()]
        public void DataFrameTest1()
        {
            CurrParamDataFrame config = new CurrParamDataFrame(Data);
        }

        [TestMethod()]
        public void DataFrameTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.Fail();
        }
    }
}