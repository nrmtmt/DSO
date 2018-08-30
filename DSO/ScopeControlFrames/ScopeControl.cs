using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSO.ScopeControlFrames
{
    /**what jyelab does:
     * write 
     *      00 FE E9 04 00 FF  //enter scope mode
     * read  
     *      FE E2 10 00 4F 04 44 53 4F 30 36 38 00 00 00 00
            FF FE C0 04 00 34  //scope model?
     * write                             
     *       00 FE C0 04 00 20  //get config
     * read
     *      FE C0 38 00 30 01 00 00 00 0D 05 02 00 7F 00 81
            FF 0D 05 02 00 7F 00 81 FF 1F 03 80 84 1E 00 02
            00 01 00 FF 00 00 00 64 00 00 00 00 00 00 00 00
            04 00 00 00 01 0A 07 0D 00 //current config
     * write
            00 FE C0 1E 00 22 0B 01 22 00 08 00 DC FF 10 00
            00 00 00 00 88 00 14 00 00 00 00 02 00 00 00 00

        and data is pushing out serial scope port
     * read
             FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE C0 0C
             00 33 85 00 00 00 20 00 00 00 FE C0 0C 00 33 84
             00 00 00 20 00 00 00 FE C0 0C 00 33 85 00 00 00
             20 00 00 00 FE C0 0C 00 33 84 00 00 00 20 00 00
             00 FE C0 0C 00 33 86 00 00 00 20 00 00 00 FE C0
             0C 00 33 84 00 00 00 20 00 00 00 FE C0 0C 00 33
             85 00 00 00 20 00 00 00 FE C0 0C 00 33 85 00 00
             00 20 00 00 00 FE C0 0C 00 33 85 00 00 00 20 00
             00 00 FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE
             C0 0C 00 33 86 00 00 00 20 00 00 00 FE C0 0C 00
             33 84 00 00 00 20 00 00 00 FE C0 0C 00 33 85 00
             00 00 20 00 00 00 FE C0 0C 00 33 85 00 00 00 20
             00 00 00 FE C0 0C 00 33 85 00 00 00 20 00 00 00
             FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE C0 0C
             00 33 85 00 00 00 20 00 00 00 FE C0 0C 00 33 83
             00 00 00 20 00 00 00 FE C0 0C 00 33 85 00 00 00
             20 00 00 00 FE C0 0C 00 33 85 00 00 00 20 00 00
             00 FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE C0
             0C 00 33 84 00 00 00 20 00 00 00 FE C0 0C 00 33
             86 00 00 00 20 00 00 00 FE C0 0C 00 33 84 00 00
             00 20 00 00 00 FE C0 0C 00 33 85 00 00 00 20 00
             00 00 FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE
             C0 0C 00 33 85 00 00 00 20 00 00 00 FE C0 0C 00
             33 83 00 00 00 20 00 00 00 FE C0 0C 00 33 85 00
             00 00 20 00 00 00 FE C0 0C 00 33 83 00 00 00 20
             00 00 00 FE C0 0C 00 33 85 00 00 00 20 00 00 00
             FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE C0 0C
             00 33 85 00 00 00 20 00 00 00 FE C0 0C 00 33 83
             00 00 00 20 00 00 00 FE C0 0C 00 33 85 00 00 00
             20 00 00 00 FE C0 0C 00 33 84 00 00 00 20 00 00
             00 FE C0 0C 00 33 85 00 00 00 20 00 00 00 FE C0
             0C 00 33 84 00 00 00 20 00 00 00 FE C0 0C 00 33
             85 00 00 00 20 00 00 00 FE C0 0C 00 33 84 00 00
             00 20 00 00 00 FE C0 0C 00 33 85 00 00 00 20 00
             00 00 FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE
             C0 0C 00 33 86 00 00 00 20 00 00 00 FE C0 0C 00
             33 84 00 00 00 20 00 00 00 FE C0 0C 00 33 85 00
             00 00 20 00 00 00 FE C0 0C 00 33 84 00 00 00 20
             00 00 00 FE C0 0C 00 33 84 00 00 00 20 00 00 00
             FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE C0 0C
             00 33 86 00 00 00 20 00 00 00 FE C0 0C 00 33 83
             00 00 00 20 00 00 00 FE C0 0C 00 33 85 00 00 00
             20 00 00 00 FE C0 0C 00 33 85 00 00 00 20 00 00
             00 FE C0 0C 00 33 85 00 00 00 20 00 00 00 FE C0
             0C 00 33 84 00 00 00 20 00 00 00 FE C0 0C 00 33
             85 00 00 00 20 00 00 00 FE C0 0C 00 33 84 00 00
             00 20 00 00 00 FE C0 0C 00 33 85 00 00 00 20 00
             00 00 FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE
             C0 0C 00 33 85 00 00 00 20 00 00 00 FE C0 0C 00
             33 84 00 00 00 20 00 00 00 FE C0 0C 00 33 85 00
             00 00 20 00 00 00 FE C0 0C 00 33 84 00 00 00 20
             00 00 00 FE C0 0C 00 33 86 00 00 00 20 00 00 00
             FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE C0 0C
             00 33 85 00 00 00 20 00 00 00
         until disconnect request
    * write
       *    00 FE E9 04 00 FF  
     * read
       *    FE C0 0C 00 33 84 00 00 00 20 00 00 00 FE E9 04   
            00 00     //part of data frame and acknowledged that scope exited usb mode                                        
                              
     **/




    public class GetParameters : DataFrame
    {
        public GetParameters() : base()
        {
            Byte[] Data = new Byte[] { 0xFE, 0xC0, 0x04, 0x00, 0x21 };
            base.Generate(Data);
        }
    }

    public class ScopeReady : DataFrame  //seems to not work
    {
        public ScopeReady() : base()
        {
            Byte[] Data = new Byte[] { 0xFE, 0xC0, 0x04, 0x00, 0x34 };
            base.Generate(Data);
        }
        public ScopeReady(byte[] data) : base(data, 226, 79)
        {
            if (FrameID != 226 || FrameSubID != 79) //"O letter in ascii                                                O
            {
                throw new InvalidDataFrameException("Wrong ScopeReady Data Frame");
            }
        }
        public DSO.Config.ScopeType ScopeType
        {
            get { return (DSO.Config.ScopeType)Data[5]; }
        }
    }

    public class GetData : DataFrame
    {
        public GetData() : base()
        {
            Byte[] Data = new Byte[] { 0xFE, 0xC0, 0x04, 0x00, 0x23 };
            base.Generate(Data);
        }

    }
    public class GetConfig : DataFrame
    {
        public GetConfig() : base()
        {
            Byte[] Data = new Byte[] { 0xFE, 0xC0, 0x04, 0x00, 0x20 };
            base.Generate(Data);
        }
    }
    public class SetState : DataFrame
    {
        public SetState(Config.ScopeState state) : base()
        {
            BitArray array = new BitArray(8, false);
            if ((int)state == 0)
            {
                array[7] = false;
            }
            else if ((int)state == 1)
            {
                array[7] = true;
            }
            else
            {
                throw new InvalidDataFrameException("Wrong SetState DataFrame - invalid state");
            }
            Byte[] Data = new Byte[] { 0xFE, 0xC0, 0x04, 0x00, 0x24, array.ToByte() };
            base.Generate(Data);
        }
    }
    public class EnterUSBScopeMode : DataFrame //dodaj zera na początku ramek
    {
        public EnterUSBScopeMode() : base()
        {
            Byte[] Data = new Byte[] { 0xFE, 0xE1, 0x04, 0x00, 0xC0 };
            base.Generate(Data);
        }
    }

    public class GetScopeType : DataFrame //dodaj zera na początku ramek
    {
        public GetScopeType() : base()
        {
            Byte[] Data = new Byte[] { 0xFE, 0xE0, 0x04, 0x00, 0x00 };
            base.Generate(Data);
        }
    }

    public class ExitUSBScopeMode : DataFrame
    {
        public ExitUSBScopeMode() : base()
        {
            Byte[] Data = new Byte[] { 0xFE, 0xE9, 0x04, 0x00, 0xFF };
            base.Generate(Data);
        }
    }

    public class USBScopeModeExited : DataFrame
    {
        public USBScopeModeExited() : base()
        {
            Byte[] Data = new Byte[] { 0xFE, 0xE9, 0x04, 0x00, 0x00 };
            base.Generate(Data);
        }
        public USBScopeModeExited(byte[] data) : base(data)
        {
            if (FrameID != 233 || FrameSubID != 00)
            {
                throw new InvalidDataFrameException("Wrong USBScopeModeExited Data Frame");
            }
        }
    }
    public class EnterDataLogMode : DataFrame
    {
        public EnterDataLogMode(Config.DataLoggerRefVoltage refV, Config.DataLoggerDataAdjustment dA) : base()
        {
            BitArray array = new BitArray(8, false);
            if ((int)dA == 1)
            {
                array[5] = true;
            }
            if ((int)refV == 2)
            {
                array[6] = true;
                array[7] = true;
            }
            else if ((int)refV == 1)
            {
                array[6] = true;
                array[7] = false;
            }
            else if ((int)refV == 0)
            {
                array[6] = false;
                array[7] = false;
            }
            Byte[] Data = new Byte[] { 0xFE, 0xC0, 0x04, 0x00, 0x24, array.ToByte() };
            base.Generate(Data);
        }
    }
}

