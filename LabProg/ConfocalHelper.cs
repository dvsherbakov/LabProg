using System;
using System.Windows;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace LabProg
{

    public partial class MainWindow : Window
    {
        private bool AutoStop { get; set; }
        private DateTime AutoStopTime { get; set; }
        private bool PumpActive { get; set; }
        private bool FDirection { get; set; }
        private int PrevRev { get; set; }
        private string _prevSpeed;
        public delegate void NextPrimeDelegate();

        private void PeackInfo(object source, System.Timers.ElapsedEventArgs e)
        {
            var timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            DistMeasureRes x = new DistMeasureRes {Dist=0};
            timestamp++;
            while (Math.Abs(x.Dist) < 0.00000001)
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
                        Dispatcher.Invoke(() =>
                            LogBox.Items.Insert(0, new LogBoxItem {Dt = DateTime.Now, LogText = ex.Message}));
                    }

                    ShowRes(x);
                    
                }
            }
        }

        private static string ToFourStr(double src)
        {
            var res = src.ToString(src < 10.0 ? "#.##" : "##.#").Replace(',', '.');
            if (res.Length == 3)
                res = "0" + res;
            return res;
        }

        private static DistMeasureRes GetTemp(string reqwest)
        {
            const string pattern = @"\[([0-9,-]+)\]"; //@"callback\(\[((\[((-?\d+,?){8})\],?)+)\]\);";
            var regex = new Regex(pattern);
            var res1 = new List<double>();
            var res2 = new List<double>();
            foreach (Match match in regex.Matches(reqwest))
            {
                var tmp = match.Groups[1].Value.Split(',');
                if (tmp.Length <= 1) continue;
                if (double.TryParse(tmp[1], out var outD)) res1.Add(outD / 1000000);
                if (double.TryParse(tmp[2], out var outE)) res2.Add(outE / 1000000);
            }
            DistMeasureRes res = new DistMeasureRes { Dist=0, IsSingle=true};

            if (res2.Average() > res1.Average())
            {
                res.Dist = res2.Average() - res1.Average();
                res.IsSingle = false;
            }

            else
                res.Dist = res1.Average();

            return res;
        }

        //private delegate void AddMessageDelegate(string message);

        private void ShowRes(DistMeasureRes MeasureRes)
        {
            var reverce = Properties.Settings.Default.PumpReverse;
            Dispatcher.Invoke(() => ConfocalLb.Text = MeasureRes.Dist.ToString("N5"));
            string txt = null;
            Dispatcher.Invoke(() => txt = CbConfocalLevel.Text);
            if (txt.Length == 0) txt = @"0.0";

            var subLevel =
                MeasureRes.Dist - double.Parse(txt.Replace('.', ','));
            
            if ((Math.Abs(subLevel) > 0.001) && AutoStop)
            {
                _pumpSerial.StartPump();
                AutoStop = false;
                return;
            }

            int rvs;
            if (subLevel < 0) rvs = 1; else rvs = -1;
            if (reverce) rvs = rvs * (-1);
            if (!MeasureRes.IsSingle) rvs = rvs * (-1);

            if (rvs != PrevRev)
            {
                if (rvs < 0)
                    _pumpSerial.SetCounterClockwiseDirection();
                if (rvs > 0)
                    _pumpSerial.SetClockwiseDirection();
                if (rvs == 0)
                    _pumpSerial.StopPump();
                PrevRev = rvs;
            }
            else if (Math.Abs(subLevel) > 0.001 && PumpActive)
            {
                var speed = "11.9";

                if (Math.Abs(subLevel) > 0.001) speed = "0.5 "; //ToFourStr(0.5);
                if (Math.Abs(subLevel) > 0.005) speed = "5   ";
                if (Math.Abs(subLevel) > 0.01) speed = "15.0"; //ToFourStr(15);
                if (Math.Abs(subLevel) > 0.05) speed = "35.5";
                if (Math.Abs(subLevel) > 0.1) speed = "125 "; //ToFourStr(75);
                if (Math.Abs(subLevel) > 1) speed = "280 "; //ToFourStr(333);
                if (_prevSpeed == speed) return;
                
                _pumpSerial.SetSpeed(speed);

                _prevSpeed = speed;
            }
            else
            {
                _pumpSerial.StopPump();
                AutoStop = true;
            }
        }

        private void PumpPortOn(object sender, RoutedEventArgs e)
        {
            PumpActive = true;
           if (_confocalTimer==null){
                _confocalTimer = new Timer
                {
                    Interval = 1000
                };
                _confocalTimer.Elapsed += PeackInfo;
            }
            _confocalTimer.Start();
            if (!_pumpSerial.Active())
            {
                try
                {
                    _pumpSerial.OpenPort();
                }
                catch (Exception ex)
                {
                    LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message });
                    CbPumpActive.IsChecked = false;
                }

            }
        }
        private void PumpPortOff(object sender, RoutedEventArgs e)
        {
            PumpActive = false;
            _confocalTimer.Stop();
        }

        private void PumpStartButton(object sender, RoutedEventArgs e)
        {
            if (!_pumpSerial.Active()) {
                try
                {
                    _pumpSerial.OpenPort();
                }
                catch (Exception ex)
                {
                    LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message });
                    CbPumpActive.IsChecked = false;
                }
            }
            _pumpSerial.StartPump();
        }
    }

    class DistMeasureRes
    {
        public double Dist { get; set; }
        public bool IsSingle { get; set; }
    }
}

