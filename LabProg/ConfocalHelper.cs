﻿using System;
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
        private bool PumpActive { get; set; }
        private bool FDirection { get; set; }
        private int PrevRev { get; set; }
        private string _prevSpeed;
        public delegate void NextPrimeDelegate();

        private void PeackInfo(object source, System.Timers.ElapsedEventArgs e)
        {
            var timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var x = 0.0;
            timestamp++;
            while (Math.Abs(x) < 0.00000001)
            {
                using (var wc = new WebClient())
                {
                    var json = wc.DownloadString("http://169.254.168.150/datagen.php?type=Meas&callback=callback&_=" +
                                timestamp);
                    x = GetTemp(json);


                    ShowRes(x.ToString("N5"));
                    //Invoke(new AddMessageDelegate(ShowRes), x.ToString("N5"));
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

        private static double GetTemp(string reqwest)
        {
            const string pattern = @"\[([0-9,-]+)\]"; //@"callback\(\[((\[((-?\d+,?){8})\],?)+)\]\);";
            var regex = new Regex(pattern);
            var lst = new List<double>();
            foreach (Match match in regex.Matches(reqwest))
            {
                var tmp = match.Groups[1].Value.Split(',');
                if (tmp.Length <= 1) continue;
                if (double.TryParse(tmp[1], out var outD)) lst.Add(outD / 1000000);
            }
            return lst.Count > 0 ? lst.Average() : 0;
        }

        //private delegate void AddMessageDelegate(string message);

        private void ShowRes(string message)
        {
            Dispatcher.Invoke(()=>ConfocalLb.Text = message);
            string txt = null;
            Dispatcher.Invoke(()=>txt= CbConfocalLevel.Text);
            if (txt.Length == 0) txt = @"0.0";
            var res = double.Parse(message) - double.Parse(txt.Replace('.', ','));
            if ((Math.Abs(res) > 0.001) && AutoStop)
            {
                pumpSerial.StartPump();
                AutoStop = false;
                return;
            }

            int rvs;
            if (res < 0) rvs = 1; else rvs = -1;

            if (rvs != PrevRev)
            {
                if (rvs < 0)
                    pumpSerial.SetCounterClockwiseDirection();
                if (rvs > 0)
                    pumpSerial.SetClockwiseDirection();
                if (rvs == 0)
                    pumpSerial.StopPump();
                PrevRev = rvs;
            }
            else if (Math.Abs(res) > 0.001 && PumpActive)
            {
                var speed = "11.9";

                if (Math.Abs(res) > 0.001) speed = "0.5 "; //ToFourStr(0.5);
                if (Math.Abs(res) > 0.005) speed = "5   ";
                if (Math.Abs(res) > 0.01) speed = "15.0"; //ToFourStr(15);
                if (Math.Abs(res) > 0.05) speed = "35.5";
                if (Math.Abs(res) > 0.1) speed = "125 "; //ToFourStr(75);
                if (Math.Abs(res) > 1) speed = "280 "; //ToFourStr(333);
                if (_prevSpeed == speed) return;

                pumpSerial.SetSpeed(speed);


                _prevSpeed = speed;
            }
            else
            {
                pumpSerial.StopPump();
                AutoStop = true;
            }
        }

        private void PumpPortOn(object sender, RoutedEventArgs e)
        {
            PumpActive = true;
           if (ConfocalTimer==null){
                ConfocalTimer = new Timer
                {
                    Interval = 1000
                };
                ConfocalTimer.Elapsed += PeackInfo;
            }
            ConfocalTimer.Start();
            if (!pumpSerial.Active()) pumpSerial.OpenPort();
        }
        private void PumpPortOff(object sender, RoutedEventArgs e)
        {
            PumpActive = false;
            ConfocalTimer.Stop();
        }
    }
}