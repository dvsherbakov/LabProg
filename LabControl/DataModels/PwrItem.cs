using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.DataModels
{
    class PwrItem
    {
        public int Mode { get; set; }
        public int Amplitude { get; set; }
        public int Bias { get; set; }
        public int Frequency { get; set; }
        public int Duty { get; set; }
        public int Phase { get; set; }
        public int MaxVolts { get; set; }
        public int MaxAmps { get; set; }

        public bool IsCorrect { get; set; }

        public PwrItem()
        {
            Mode = 0;
            Amplitude = 0;
            Bias = 0;
            Frequency = 0;
            Duty = 0;
            Phase = 0;
            MaxVolts = 0;
            MaxAmps = 0;
            IsCorrect = true;
        }
        public PwrItem(byte[] raw)
        {
            IsCorrect = true;
            if (raw.Length < 23)
            {
                IsCorrect = false;
            }
            else if (raw[21] == 0 && raw[22] == 0)
            {
                IsCorrect = false;
            }
            else
            {
                Mode = GetMode(raw);
                Amplitude = GetAmplitude(raw);
                Bias = GetBIAS(raw);
                Frequency = GetFrequency(raw);
                Duty = GetDuty(raw);
                Phase = GetPhase(raw);
                MaxVolts = GetMaxVolts(raw);
                MaxAmps = GetMaxAmps(raw);
            }
        }

        static int GetMode(byte[] ar)
        {
            var res = 0;
            res = ar[6];
            return res;
        }

        static int GetAmplitude(byte[] ar)
        {
            int res = 0;
            byte[] ampRes = { ar[8], ar[7] };
            res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        static int GetBIAS(byte[] ar)
        {
            int res = 0;
            byte[] ampRes = { ar[10], ar[9] };
            res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        static int GetFrequency(byte[] ar)
        {
            int res = 0;
            byte[] ampRes = { ar[12], ar[11] };
            res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        static int GetDuty(byte[] ar)
        {
            int res = 0;
            byte[] ampRes = { ar[14], ar[13] };
            res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        static int GetPhase(byte[] ar)
        {
            int res = 0;
            byte[] ampRes = { ar[16], ar[15] };
            res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        static int GetMaxVolts(byte[] ar)
        {
            int res = 0;
            byte[] ampRes = { ar[18], ar[17] };
            res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        static int GetMaxAmps(byte[] ar)
        {
            int res = 0;
            byte[] ampRes = { ar[20], ar[19] };
            res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }
    }
}
