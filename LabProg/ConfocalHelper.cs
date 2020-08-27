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

        private bool IsTwoPump { get => Properties.Settings.Default.IsTwoPump; }

        private void OperatePump(DistMeasureRes MeasureRes)
        {
            Dispatcher.Invoke(() => ConfocalLb.Text = MeasureRes.Dist.ToString("N5"));

            var speed = GetPumpSpeed(MeasureRes);

            if ((speed == _prevSpeed) || !PumpActive) return;
            var direction = GetDirection(MeasureRes);
            switch (direction)
            {
                case Direction.Stop:
                    if (IsTwoPump)
                        _pumpSecondSerial.StopPump();
                    _pumpSerial.StopPump();
                    break;
                case Direction.Clockwise:
                    if (!IsTwoPump)
                        _pumpSerial.SetClockwiseDirection();
                    else if (!speed.Equals("0   "))
                        _pumpSerial.StartPump();
                    break;
                case Direction.CounterClockwise:
                    if (!IsTwoPump)
                        _pumpSerial.SetCounterClockwiseDirection();
                    else if (!speed.Equals("0   "))
                        _pumpSecondSerial.StartPump();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!speed.Equals("0   ")&&!IsTwoPump)
            {
                _pumpSerial.StartPump();
            }

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
            if (IsTwoPump)
            {
                _pumpSerial.SetClockwiseDirection();
                _pumpSecondSerial.SetClockwiseDirection();
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

        private void TbTwoPumpToggle()
        {
            if (Properties.Settings.Default.IsTwoPump)
            {
                TbTwoPump.Text = "Два насоса";
                if (TbFirstPump != null) TbFirstPump.Text = "Порт притока";
                if (TbSecondPump != null) TbSecondPump.Visibility = Visibility.Visible;
                if (CbPumpSecondPort != null) CbPumpSecondPort.Visibility = Visibility.Visible;
                if (_pumpSecondSerial == null && CbPumpSecondPort != null) _pumpSecondSerial = new PumpSerial(CbPumpSecondPort.Text, Properties.Settings.Default.PumpReverse);
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

