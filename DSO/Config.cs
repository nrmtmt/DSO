using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSO
{
    public class Config
    {
        public enum ScopeType
        {
            DSO082 = 2,
            DSO094 = 3,
            DSO068 = 4,
            DSO112A = 5,
        }

        public enum Coupling
        {
            DC = 0,
            AC = 1,
            GND = 2,
        }

        public enum RecordLength
        {
            _256 = 256,
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048,
            _4096 = 4096,
            _8192 = 8192,
            _16384 = 16384
        }

        public enum Slope
        {
            Falling = 0,
            Rising = 1,
        }

        public enum TriggerMode
        {
            Auto = 0,
            Normal = 1,
            Single = 2,
        }

        public enum TriggerSource
        {
            Internal = 0,
            External = 1,
            CH_A = 2,
            CH_B = 3,
        }

        public enum Timebase
        {
            divCustom = 0,
            div50min = 1,
            div20min = 2,
            div10min = 3,
            div5min = 4,
            div2min = 5,
            div1min = 6,
            div50s = 7,
            div20s = 8,
            div10s = 9,
            div5s = 10,
            div2s = 11,
            div1s = 12,
            div500ms = 13,
            div200ms = 14,
            div100ms = 15,
            div50ms = 16,
            div20ms = 17,
            div10ms = 18,
            div5ms = 19,
            div2ms = 20,
            div1ms = 21,
            div500us = 22,
            div200us = 23,
            div100us = 24,
            div50us = 25,
            div20us = 26,
            div10us = 27,
            div5us = 28,
            div2us = 29,
            div1us = 30,
            div500ns = 31,
            div200ns = 32,
            div100ns = 33,
            div50ns = 34,
            div20ns = 35,
            div10ns = 36,
            div5ns = 37
        }

        public enum VerticalSensitivity
        {
            divCustom = 0,
            div100V = 1,
            div50V = 2,
            div20V = 3,
            div10V = 4,
            div5V = 5,
            div2V = 6,
            div1V = 7,
            div500mV = 8,
            div200mV = 9,
            div100mV = 10,
            div50mV = 11,
            div20mV = 12,
            div10mV = 13,
            div5mV = 14,
            div2mV = 15,
        }

        public enum ScopeState
        {
            AutoState = 0,
            ManualState = 1,
        }

        public enum DataLoggerRefVoltage
        {
            AREF = 0,
            AVCC = 1,
            INTERNAL = 2
        }

        public enum DataLoggerDataAdjustment
        {
            RightAdjustment = 0,
            LeftAdjustment = 1
        }
    }
}
