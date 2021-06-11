using System;
using System.Collections.Generic;

namespace LabControl.DataModels
{
    internal class PwrItem
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
                Bias = GetBias(raw);
                Frequency = GetFrequency(raw);
                Duty = GetDuty(raw);
                Phase = GetPhase(raw);
                MaxVolts = GetMaxVolts(raw);
                MaxAmps = GetMaxAmps(raw);
            }
        }

        private static int GetMode(IReadOnlyList<byte> ar)
        {
            int res = ar[6];
            return res;
        }

        private static int GetAmplitude(IReadOnlyList<byte> ar)
        {
            byte[] ampRes = { ar[8], ar[7] };
            int res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        private static int GetBias(IReadOnlyList<byte> ar)
        {
            byte[] ampRes = { ar[10], ar[9] };
            int res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        private static int GetFrequency(IReadOnlyList<byte> ar)
        {
            byte[] ampRes = { ar[12], ar[11] };
            int res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        private static int GetDuty(IReadOnlyList<byte> ar)
        {
            byte[] ampRes = { ar[14], ar[13] };
            int res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        private static int GetPhase(IReadOnlyList<byte> ar)
        {
            byte[] ampRes = { ar[16], ar[15] };
            int res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        private static int GetMaxVolts(IReadOnlyList<byte> ar)
        {
            byte[] ampRes = { ar[18], ar[17] };
            int res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }

        private static int GetMaxAmps(IReadOnlyList<byte> ar)
        {
            byte[] ampRes = { ar[20], ar[19] };
            int res = BitConverter.ToInt16(ampRes, 0);
            return res;
        }
    }
}
