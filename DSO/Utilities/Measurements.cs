using DSO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.Utilities
{
    ///<summary>
    ///Class that allows scaling data from scope raw data in buffer.
    ///</summary>
    ///
    public static class Measurements
    {
        ///<summary>
        ///Returns scaled measurements from buffer. If buffer is not contain measurements, null is returned. 
        ///<param name="data">Raw buffer data</param>
        ///<param name="voltsPerDiv">Voltage per division (in volts)</param>
        ///<param name="pointsPerDiv">Points per division (from scope Config class)</param>
        ///<param name="recordLength">Current scope record length</param>
        ///</summary>
        ///
        public static float[] GetFromBuffer (byte[] data, float voltsPerDiv, int pointsPerDiv, int recordLength, int vertPos)
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
                    if (rawData.Count() == recordLength- 5)
                    {                       
                        return getScaledMeasurements(rawData, voltsPerDiv, pointsPerDiv, vertPos) ?? null;
                    }
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

                        return getScaledMeasurements(rawData, voltsPerDiv, pointsPerDiv, vertPos) ?? null;
                    }
                }
                catch (InvalidDataFrameException ex2)
                {
                   
                }
            }
            return null;
        }

        private static float[] getScaledMeasurements(byte[] data, float voltsPerDiv, int pointsPerDiv, int vertPos)
        {
            float[] scaled = new float[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {
                scaled[i] = (GetScaledData(data[i], voltsPerDiv, pointsPerDiv, vertPos));
            }
            return scaled;
        }

        ///<summary>
        ///Returns scaled measurement from single raw data. If data is not valid, null is returned. 
        ///<param name="data">Raw data</param>
        ///<param name="voltsPerDiv">Voltage per division (in volts)</param>
        ///<param name="pointsPerDiv">Points per division (from scope Config class)</param>
        ///<param name="recordLength">Current scope record length</param>
        ///</summary>
        ///
        public static float GetScaledData(int data, float voltsPerDiv, int pointsPerDiv, int vertPos) ///to be changed
        {
            if (data < byte.MaxValue)
            {
                return ((data - (128)) * (voltsPerDiv / pointsPerDiv));
            }
            else
            {
                return 0;
            }
        }
        ///<summary>
        ///Return raw data from real one. If data is not valid, 0 is returned. 
        ///<param name="scaled">Scaled data</param>
        ///<param name="voltsPerDiv">Voltage per division (in volts)</param>
        ///<param name="pointsPerDiv">Points per division (from scope Config class)</param>
        ///</summary>
        ///
        public static byte GetRawData(float scaled, float voltsPerDiv, int pointsPerDiv)   ///to be changed
        {
            try
            {
                return Convert.ToByte((scaled / (voltsPerDiv / pointsPerDiv)) + 128);

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
