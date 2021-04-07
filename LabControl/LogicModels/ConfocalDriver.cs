using LabControl.ClassHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Timers;

namespace LabControl.LogicModels
{
    internal class ConfocalDriver
    {
        private readonly bool f_IsFakeData = false;
        private readonly Random f_FakeRnd;

        private readonly Timer f_ConfocalTimer;

        public delegate void ObtainedData(DistMeasureRes xData);
        public event ObtainedData ObtainedDataEvent;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        private bool f_MeasuredActive;

        private readonly ObservableCollection<ConfocalDataItem> f_MeasureLogCollection;
        

        public ConfocalDriver()
        {
            f_ConfocalTimer = new Timer
            {
                Interval = 5000
            };
            f_ConfocalTimer.Elapsed += InterceptInfo;
            f_FakeRnd = new Random();
            f_MeasureLogCollection = new ObservableCollection<ConfocalDataItem>();
        }

        private void InterceptInfo(object source, ElapsedEventArgs e)
        {
            var x = new DistMeasureRes { Dist = 0 };
            if (f_IsFakeData)
            {
                var dx = f_FakeRnd.NextDouble();
                x.Dist = 0.3 + dx*0.1;
                x.IsSingle = false;
                f_MeasureLogCollection.Add(new ConfocalDataItem(x.Dist));
                ObtainedDataEvent?.Invoke(x);
                return;
            }

            var timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
         
            timestamp++;
            while (Math.Abs(x.Dist) < 0.000001)
            {
                using var wc = new WebClient();
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

                ObtainedDataEvent?.Invoke(x);
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

        public double[] GetLastFragment()
        {
            return f_MeasureLogCollection.OrderByDescending(x => x.MomentDateTime).Take(300).Select(x => x.Value)
                .ToArray();
        }

    }
}
