using System;
using System.Windows;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Timer = System.Timers.Timer;
using System.Globalization;
using System.Threading.Tasks;
using System.Diagnostics;

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

        private List<SpeedGradeItem> SpeedGrades = new List<SpeedGradeItem> {
            new SpeedGradeItem{different=3.00, speed="250 "},
            new SpeedGradeItem{different=2.00, speed="150"},
                new SpeedGradeItem{different=1.00, speed="75.0"},
                new SpeedGradeItem{different=0.50, speed="65.0"},
                new SpeedGradeItem{different=0.30, speed="55.0"},
                new SpeedGradeItem{different=0.10, speed="45.0"},
                new SpeedGradeItem{different=0.08, speed="20.0"},
                new SpeedGradeItem{different=0.04, speed="11.0"},
                new SpeedGradeItem{different=0.02, speed="2.5 "},
                new SpeedGradeItem{different=0.01, speed="0.5 "},
                new SpeedGradeItem{different=0.00, speed="0.0 "}
        };

        private void PeackInfo(object source, System.Timers.ElapsedEventArgs e)
        {
            var timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            DistMeasureRes x = new DistMeasureRes { Dist = 0 };
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
                        Dispatcher.Invoke(() =>
                            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message }));
                    }
                    if (!IsTwoPump) OperatePump(x); else OperateTwoPump(x);
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
            if (res1.Count == 0 || res2.Count == 0) return res;

            if (res2.Average() > res1.Average())
            {
                res.Dist = res2.Average() - res1.Average();
                res.IsSingle = false;
            }
            else
                res.Dist = res1.Average();

            return res;
        }

        private double SelectedLevel
        {
            get
            {
                string txt = null;
                Dispatcher.Invoke(() => txt = CbConfocalLevel.Text.Trim().Replace('.', ','));
                double.TryParse(txt, out double lvl);
                return lvl;
            }
        }

        // private  bool IsReverce { get => Properties.Settings.Default.PumpReverse; }

        private bool IsTwoPump { get => Properties.Settings.Default.IsTwoPump; }

        private void OperatePump(DistMeasureRes MeasureRes)
        {
            Dispatcher.Invoke(() => ConfocalLb.Text = MeasureRes.Dist.ToString("N5"));

            var speed = GetPumpSpeed(MeasureRes);

            if (!PumpActive)
            {
                _pumpSerial.StopPump();
                return;
            }
            if (speed == _prevSpeed)
            {
                if (double.Parse(speed.Trim(), CultureInfo.InvariantCulture) == 0)
                {
                    _pumpSerial.StopPump();
                }
            }
            else
            {
                _pumpSerial.SetSpeed(speed);

            }
            var direction = GetDirection(MeasureRes);
            switch (direction)
            {
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

            if (!speed.Equals("0.0 "))
            {
                _pumpSerial.StartPump();
            }

            _prevSpeed = speed;
        }

        private void OperateTwoPump(DistMeasureRes measureRes)
        {
            var speed = GetPumpSpeed(measureRes);
            Dispatcher.Invoke(() => ConfocalLb.Text = measureRes.Dist.ToString("N5"));
            var direction = GetDirection(measureRes);

            if (!PumpActive || speed.Equals("0.0 ") || (direction == Direction.Stop))
            {
                _pumpSerial.StopPump();
                _pumpSecondSerial.StopPump();


                return;
            }
            

            if (direction == Direction.Clockwise)
            {
                _pumpSerial.SetSpeed(speed);
                _pumpSerial.StartPump();
                _pumpSecondSerial.SetSpeed("0.5 ");
                _pumpSecondSerial.StopPump();

            }
            if (direction == Direction.CounterClockwise)
            {
                _pumpSecondSerial.SetSpeed(speed);
                _pumpSecondSerial.StartPump();
                _pumpSerial.SetSpeed("0.5 ");
                _pumpSerial.StopPump();

            }

        }

        private Direction GetDirection(DistMeasureRes currentLevel)
        {
            var currentDifferent = SelectedLevel - currentLevel.Dist;
            if (Math.Abs(currentDifferent) < 0.001) return Direction.Stop;
            var tmpDirection = (currentDifferent > 0);
            if (currentLevel.IsSingle) tmpDirection = !tmpDirection;

            Debug.WriteLine($"direction diff={currentDifferent}, issingle={currentLevel.IsSingle}");
            return tmpDirection ? Direction.Clockwise : Direction.CounterClockwise;
        }

        private string GetPumpSpeed(DistMeasureRes currentLevel)
        {
            var subLevel = Math.Abs(currentLevel.Dist - SelectedLevel);
            Debug.WriteLine($"Speed setup diff = {subLevel}");
            var ss = SpeedGrades.Where(x => x.different < subLevel).OrderByDescending(x => x.different).FirstOrDefault().speed;
            Debug.WriteLine($"Speed  = {ss}");
            return SpeedGrades.Where(x => x.different < subLevel).OrderByDescending(x => x.different).FirstOrDefault().speed;
        }

        private void PumpPortOn(object sender, RoutedEventArgs e)
        {
            PumpActive = true;
            if (_confocalTimer == null)
            {
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
                try
                {
                    if (IsTwoPump)
                    {
                        if (_pumpSecondSerial == null) _pumpSecondSerial = new PumpSerial(CbPumpSecondPort.Text, Properties.Settings.Default.PumpSecondReverse, AddLogBoxMessage);
                        _pumpSecondSerial.OpenPort();
                    }
                }
                catch (Exception ex)
                {
                    LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message });
                    CbPumpActive.IsChecked = false;
                }
            }
            if (IsTwoPump)
            {
                //Тут указываются начальные направления насосов
                _pumpSerial.SetCounterClockwiseDirection();
                _pumpSecondSerial.SetClockwiseDirection();
            }
        }
        private void PumpPortOff(object sender, RoutedEventArgs e)
        {
            PumpActive = false;
            if (_pumpSerial != null) _pumpSerial.StopPump();
            if (_pumpSecondSerial != null) _pumpSecondSerial.StopPump();
            _confocalTimer.Stop();
        }

        private void PumpStartButton(object sender, RoutedEventArgs e)
        {
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
            _pumpSerial.StartPump();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CbConfocalLevel.Text = ConfocalLb.Text;
        }

        private void TbTwoPump_Checked(object sender, RoutedEventArgs e)
        {
            TbTwoPumpToggle();
        }

        private void TbTwoPump_UnChecked(object sender, RoutedEventArgs e)
        {
            TbTwoPumpToggle();
        }

        private void TbFirstReverceOn(object sender, RoutedEventArgs e)
        {
            if (_pumpSerial != null) _pumpSerial.PumpReverse = true;
        }
        private void TbFirstReverceOff(object sender, RoutedEventArgs e)
        {
            if (_pumpSerial != null) _pumpSerial.PumpReverse = false;
        }

        private void TbSecondReverceOn(object sender, RoutedEventArgs e)
        {
            if (_pumpSecondSerial != null) _pumpSecondSerial.PumpReverse = true;
        }
        private void TbSecondReverceOff(object sender, RoutedEventArgs e)
        {
            if (_pumpSecondSerial != null) _pumpSecondSerial.PumpReverse = false;
        }

        private void TbTwoPumpToggle()
        {
            if (Properties.Settings.Default.IsTwoPump)
            {
                TbTwoPump.Text = "Два насоса";
                if (TbFirstPump != null) TbFirstPump.Text = "Порт притока";
                if (TbSecondPump != null) TbSecondPump.Visibility = Visibility.Visible;
                if (CbPumpSecondPort != null) CbPumpSecondPort.Visibility = Visibility.Visible;
                if (_pumpSecondSerial == null && CbPumpSecondPort != null) _pumpSecondSerial = new PumpSerial(CbPumpSecondPort.Text, Properties.Settings.Default.PumpSecondReverse, AddLogBoxMessage);
            }
            else
            {
                TbTwoPump.Text = "Один насос";
                if (TbFirstPump != null) TbFirstPump.Text = "Порт насоса";
                if (TbSecondPump != null) TbSecondPump.Visibility = Visibility.Collapsed;
                if (CbPumpSecondPort != null) CbPumpSecondPort.Visibility = Visibility.Collapsed;
            }
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

