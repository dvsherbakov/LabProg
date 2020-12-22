using LabControl.ClassHelpers;
using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace LabControl.LogicModels
{
    class ConfocalDriver
    {

        private readonly Timer f_ConfocalTimer;

        public delegate void ResievedData(DistMeasureRes xData);
        public event ResievedData ResievedDataEvent;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        private bool f_MeasuredActive;

        public ConfocalDriver()
        {
            f_ConfocalTimer = new Timer
            {
                Interval = 5000
            };
            f_ConfocalTimer.Elapsed += PeackInfo;
        }

        private void PeackInfo(object source, System.Timers.ElapsedEventArgs e)
        {
            var timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var x = new DistMeasureRes { Dist = 0 };
            timestamp++;
            while (Math.Abs(x.Dist) < 0.000001)
            {
                using (var wc = new WebClient())
                {
                    try
                    {
                        var json = wc.DownloadString(
                            "http://169.254.168.150/datagen.php?type=Meas&callback=callback&_=" +
                            timestamp);
                        x = GetTemp(json);
                    }
                    catch (Exception ex)
                    {
                        SetLogMessage?.Invoke(ex.Message);
                    }
                    //if (!IsTwoPump) OperatePump(x); else OperateTwoPump(x);
                    ResievedDataEvent?.Invoke(x);
                }
            }
        }

        private static DistMeasureRes GetTemp(string request)
        {
            const string pattern = @"\[([0-9,-]+)\]"; //@"callback\(\[((\[((-?\d+,?){8})\],?)+)\]\);";
            var regex = new Regex(pattern);
            var res1 = new List<double>();
            var res2 = new List<double>();
            foreach (Match match in regex.Matches(request))
            {
                var tmp = match.Groups[1].Value.Split(',');
                if (tmp.Length <= 1) continue;
                if (double.TryParse(tmp[1], out var outD)) res1.Add(outD / 1000000);
                if (double.TryParse(tmp[2], out var outE)) res2.Add(outE / 1000000);
            }

            var res = new DistMeasureRes { Dist = 0, IsSingle = true };
            if (res1.Count == 0 || res2.Count == 0) return res;

            if (res2.Average() > 1000) return new DistMeasureRes { Dist = res1.Average(), IsSingle = true };

            if (res2.Average() > res1.Average())
            {
                res.Dist = res2.Average() - res1.Average();
                res.IsSingle = false;
            }
            else
                res.Dist = res1.Average();

            return res;
        }

        public void SetMeasuredActive(bool active)
        {
            f_MeasuredActive = active;
            if (f_MeasuredActive) f_ConfocalTimer.Start();
            else f_ConfocalTimer.Stop();
        }

    }
}
