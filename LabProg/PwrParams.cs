using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabProg
{
    internal class PwrParams
    {
        //### System writable holding registers
        const int reg_write_scratch = 1;

        const int reg_addr = 2;
        const int reg_baud = 3;
        const int reg_stop = 4;
        const int reg_parity = 5;
        const int reg_default_app = 6;
        const int reg_exec_app = 7;
        const int reg_tfs_format = 8;
        const int reg_tfs_delete = 9;
        const int reg_tfs_create = 10;
        const int reg_tfs_close = 11;
        const int reg_tfs_write_block = 12;
        const int reg_tfs_find = 13;
        const int reg_tfs_read_block = 14;
        const int reg_tfs_free_space = 15;
        const int reg_write_date = 16;
        const int reg_write_time = 17;
        const int reg_play_file = 18;

        //### System readable holding registers
        const int reg_read_scratch = 1;

        const int reg_read_date = 16;
        const int reg_read_time = 17;
        const int reg_read_tempr = 19;

        //#SmartPWR Read/Write Holding Registeres
        public static int[] Modes = { REG_P0_ENABLE, REG_P1_ENABLE, REG_P2_ENABLE, REG_P3_ENABLE, REG_P4_ENABLE, REG_P5_ENABLE};
        public static int[] Amplitudes = { REG_P0_AMPLITUDE, REG_P1_AMPLITUDE, REG_P2_AMPLITUDE, REG_P3_AMPLITUDE, REG_P4_AMPLITUDE, REG_P5_AMPLITUDE };
        public static int[] Biases = { REG_P0_BIAS, REG_P1_BIAS, REG_P2_BIAS, REG_P3_BIAS, REG_P4_BIAS, REG_P5_BIAS };
        public static int[] Freqs = { REG_P0_FREQ, REG_P1_FREQ, REG_P2_FREQ, REG_P3_FREQ, REG_P4_FREQ, REG_P5_FREQ };
        public static int[] Dutys = { REG_P0_DUTY, REG_P1_DUTY, REG_P2_DUTY, REG_P3_DUTY, REG_P4_DUTY, REG_P5_DUTY };
        public static int[] Phases = { REG_P0_PHASE, REG_P1_PHASE, REG_P2_PHASE, REG_P3_PHASE, REG_P4_PHASE, REG_P5_PHASE };
        public static int[] MaxVolts = { REG_P0_MAX_VOLTS, REG_P1_MAX_VOLTS, REG_P2_MAX_VOLTS, REG_P3_MAX_VOLTS, REG_P4_MAX_VOLTS, REG_P5_MAX_VOLTS };
        public static int[] MaxAmps = { REG_P0_MAX_AMPS, REG_P1_MAX_AMPS, REG_P2_MAX_AMPS, REG_P3_MAX_AMPS, REG_P4_MAX_AMPS, REG_P5_MAX_AMPS }; 

        public const int REG_P0_ENABLE = 2000;
        const int REG_P0_MODE = 2001;
        const int REG_P0_AMPLITUDE = 2002;
        public const int REG_P0_BIAS = 2003;
        const int REG_P0_FREQ = 2004;
        const int REG_P0_DUTY = 2005;
        const int REG_P0_PHASE = 2006;
        public const int REG_P0_MAX_VOLTS = 2007;
        const int REG_P0_MAX_AMPS = 2008;

        public const int REG_P1_ENABLE = 2010;
        const int REG_P1_MODE = 2011;
        const int REG_P1_AMPLITUDE = 2012;
        public const int REG_P1_BIAS = 2013;
        const int REG_P1_FREQ = 2014;
        const int REG_P1_DUTY = 2015;
        const int REG_P1_PHASE = 2016;
        public const int REG_P1_MAX_VOLTS = 2017;
        const int REG_P1_MAX_AMPS = 2018;

        public const int REG_P2_ENABLE = 2020;
        const int REG_P2_MODE = 2021;
        const int REG_P2_AMPLITUDE = 2022;
        public const int REG_P2_BIAS = 2023;
        const int REG_P2_FREQ = 2024;
        const int REG_P2_DUTY = 2025;
        const int REG_P2_PHASE = 2026;
        public const int REG_P2_MAX_VOLTS = 2027;
        const int REG_P2_MAX_AMPS = 2028;

        public const int REG_P3_ENABLE = 2030;
        const int REG_P3_MODE = 2031;
        const int REG_P3_AMPLITUDE = 2032;
        public const int REG_P3_BIAS = 2033;
        const int REG_P3_FREQ = 2034;
        const int REG_P3_DUTY = 2035;
        const int REG_P3_PHASE = 2036;
        public const int REG_P3_MAX_VOLTS = 2037;
        const int REG_P3_MAX_AMPS = 2038;

        public const int REG_P4_ENABLE = 2040;
        const int REG_P4_MODE = 2041;
        const int REG_P4_AMPLITUDE = 2042;
        public const int REG_P4_BIAS = 2043;
        const int REG_P4_FREQ = 2044;
        const int REG_P4_DUTY = 2045;
        const int REG_P4_PHASE = 2046;
        public const int REG_P4_MAX_VOLTS = 2047;
        const int REG_P4_MAX_AMPS = 2048;

        public const int REG_P5_ENABLE = 2050;
        const int REG_P5_MODE = 2051;
        const int REG_P5_AMPLITUDE = 2052;
        public const int REG_P5_BIAS = 2053;
        const int REG_P5_FREQ = 2054;
        const int REG_P5_DUTY = 2055;
        const int REG_P5_PHASE = 2056;
        public const int REG_P5_MAX_VOLTS = 2057;
        const int REG_P5_MAX_AMPS = 2058;

        //## SmartPWR Read-only Holding Registers

        const int REG_VERSION_DEVICE = 1024; //# { Device, OS, Application }
        const int REG_VERSION_OS = 1025; //# { Device, OS, Application }
        const int REG_VERSION_APP = 1026; //# { Device, OS, Application }

        const int REG_P0_CUR_VOLTS = 3000;
        const int REG_P1_CUR_VOLTS = 3001;
        const int REG_P2_CUR_VOLTS = 3002;
        const int REG_P3_CUR_VOLTS = 3003;
        const int REG_P4_CUR_VOLTS = 3004;
        const int REG_P5_CUR_VOLTS = 3005;

        const int REG_P0_CUR_AMPS = 3010;
        const int REG_P1_CUR_AMPS = 3011;
        const int REG_P2_CUR_AMPS = 3012;
        const int REG_P3_CUR_AMPS = 3013;
        const int REG_P4_CUR_AMPS = 3014;
        const int REG_P5_CUR_AMPS = 3015;

        const int REG_P0_REGULATED_BIAS = 3020;
        const int REG_P1_REGULATED_BIAS = 3021;
        const int REG_P2_REGULATED_BIAS = 3022;
        const int REG_P3_REGULATED_BIAS = 3023;
        const int REG_P4_REGULATED_BIAS = 3024;
        const int REG_P5_REGULATED_BIAS = 3025;
    }
}
