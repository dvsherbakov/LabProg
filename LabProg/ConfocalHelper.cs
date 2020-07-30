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
        //public delegate void NextPrimeDelegate();

        private static List<SpeedGradeItem> SpeedGrades = new List<SpeedGradeItem> {
                new SpeedGradeItem{different=1.00, speed="280 "},
                new SpeedGradeItem{different=0.10, speed="125 "},
                new SpeedGradeItem{different=0.50, speed="35.5"},
                new SpeedGradeItem{different=0.10, speed="15.0"},
                new SpeedGradeItem{different=0.05, speed="5   "},
                new SpeedGradeItem{different=0.01, speed="0.5 "},
                new SpeedGradeItem{different=0.00, speed="0   "}
        };

        private void PeackInfo(object source, System.Timers.ElapsedEventArgs e)
        {
            var timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            DistMeasureRes x = new DistMeasureRes { Dist = 0 };
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
                            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message }));
                    }
                    OperatePump(x);
                }
            }
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
            DistMeasureRes res = new DistMeasureRes { Dist = 0, IsSingle = true };

            if (res2.Average() > res1.Average())
            {
                res.Dist = res2.Average() - res1.Average();
                res.IsSingle = false;
            } else
                res.Dist = res1.Average();

            return res;
        }

        private double SetupLevel
        {
            get
            {
                string txt = null;
                Dispatcher.Invoke(() => txt = CbConfocalLevel.Text);
                double.TryParse(txt, out double res);
                return res;
            }
        }

        private  bool IsReverce { get => Properties.Settings.Default.PumpReverse; }

        private void OperatePump(DistMeasureRes MeasureRes)
        {
            Dispatcher.Invoke(() => ConfocalLb.Text = MeasureRes.Dist.ToString("N5"));

            var direction = GetDirection(MeasureRes);
            var speed = GetPumpSpeed(MeasureRes);

            if ((speed == _prevSpeed) || !PumpActive) return;

            switch (direction) {
                case Direction.Stop:
                    _pumpSerial.StopPump();
                    break;
                case Direction.Clockwise:
                    _pumpSerial.SetClockwiseDirection();
                    break;
                case Direction.CounterClockwise:
                    _pumpSerial.SetCounterClockwiseDirection();
                    break;
            }

            if (!speed.Equals("0   ")) _pumpSerial.StartPumpAsync();
            _prevSpeed = speed;

        }

        private Direction GetDirection(DistMeasureRes currentLevel)
        {
            var currentDifferent = SetupLevel - currentLevel.Dist;
            if (Math.Abs(currentDifferent) < 0.001) return Direction.Stop;
            var tmpDirection = ((currentDifferent > 0) == IsReverce) != currentLevel.IsSingle;

            return tmpDirection ? Direction.Clockwise : Direction.CounterClockwise;
        }

        private string GetPumpSpeed(DistMeasureRes currentLevel)
        {
            var subLevel = Math.Abs(currentLevel.Dist - SetupLevel);
            return SpeedGrades.Where(x => x.different < subLevel).OrderByDescending(x => x.different).FirstOrDefault().speed;
        }

        private void PumpPortOn(object sender, RoutedEventArgs e)
        {
            PumpActive = true;
           if (_confocalTimer==null){
                _confocalTimer = new Timer
                {
                    Interval = 1000
                };
                //_confocalTimer.Elapsed += PeackInfo;
                //Временно закомментировано, во избежание многоразовой подписки на событие
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
            _pumpSerial.StopPump();
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
            _pumpSerial.StartPumpAsync();
        }
    }

    class DistMeasureRes
    {
        public double Dist { get; set; }
        public bool IsSingle { get; set; }
    }

    internal enum Direction
    {
        Clockwise, CounterClockwise, Stop
    }

    class SpeedGradeItem
    {
        public double different;
        public string speed;
    }
}

