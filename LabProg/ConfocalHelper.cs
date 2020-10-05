using System;
using System.Windows;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Timer = System.Timers.Timer;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace LabProg
{

    public partial class MainWindow : Window
    {
        private bool PumpActive { get; set; }

        private string f_PrevSpeed;
        //public delegate void NextPrimeDelegate();

        private readonly List<SpeedGradeItem> f_SpeedGrades = new List<SpeedGradeItem> {
            new SpeedGradeItem{Different=3.00, Speed="250 "},
            new SpeedGradeItem{Different=2.00, Speed="150 "},
                new SpeedGradeItem{Different=1.00, Speed="65.0"},
                new SpeedGradeItem{Different=0.50, Speed="45.0"},
                new SpeedGradeItem{Different=0.30, Speed="35.0"},
                new SpeedGradeItem{Different=0.10, Speed="25.0"},
                new SpeedGradeItem{Different=0.08, Speed="20.0"},
                new SpeedGradeItem{Different=0.04, Speed="11.0"},
                new SpeedGradeItem{Different=0.01, Speed="2.5 "},
                new SpeedGradeItem{Different=0.005, Speed="0.5 "},
                new SpeedGradeItem{Different=0.00, Speed="0.0 "}
        };

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
                        Dispatcher.Invoke(() =>
                            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message }));
                    }
                    if (!IsTwoPump) OperatePump(x); else OperateTwoPump(x);
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
                double.TryParse(txt, out var lvl);
                return lvl;
            }
        }

        private static bool IsTwoPump => Properties.Settings.Default.IsTwoPump;

        private void OperatePump(DistMeasureRes measureRes)
        {
            Dispatcher.Invoke(() => ConfocalLb.Text = measureRes.Dist.ToString("N5"));

            var speed = GetPumpSpeed(measureRes);

            if (!PumpActive)
            {
                _pumpSerial.AddStopPump();
                return;
            }
            if (speed == f_PrevSpeed)
            {
                if (Math.Abs(double.Parse(speed.Trim(), CultureInfo.InvariantCulture)) < 0.001)
                {
                    _pumpSerial.AddStopPump();
                }
            }
            else
            {
                _pumpSerial.AddSpeed(speed);

            }
            var direction = GetDirection(measureRes);
            switch (direction)
            {
                case Direction.Stop:
                    _pumpSerial.AddStopPump();
                    break;
                case Direction.Clockwise:
                    _pumpSerial.AddClockwiseDirection();
                    break;
                case Direction.CounterClockwise:

                    _pumpSerial.AddCounterClockwiseDirection();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!speed.Equals("0.0 "))
            {
                _pumpSerial.AddStartPump();
            }

            f_PrevSpeed = speed;
        }

        private void OperateTwoPump(DistMeasureRes measureRes)
        {
            var speed = GetPumpSpeed(measureRes);
            Dispatcher.Invoke(() => ConfocalLb.Text = measureRes.Dist.ToString("N5"));
            var direction = GetDirection(measureRes);

            if (!PumpActive || speed.Equals("0.0 ") || (direction == Direction.Stop))
            {
                _pumpSerial.AddStopPump();
                _pumpSecondSerial.AddStopPump();


                return;
            }
            switch (direction)
            {
                case Direction.Clockwise:
                    _pumpSerial.AddSpeed(speed);
                    _pumpSerial.AddStartPump();
                    _pumpSecondSerial.AddSpeed("0.5 ");
                    _pumpSecondSerial.AddStopPump();
                    break;
                case Direction.CounterClockwise:
                    _pumpSecondSerial.AddSpeed(speed);
                    _pumpSecondSerial.AddStartPump();
                    _pumpSerial.AddSpeed("0.5 ");
                    _pumpSerial.AddStopPump();
                    break;
                case Direction.Stop:
                    _pumpSecondSerial.AddStopPump();
                    _pumpSerial.AddStopPump();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Direction GetDirection(DistMeasureRes currentLevel)
        {
            var currentDifferent = SelectedLevel - currentLevel.Dist;
            if (Math.Abs(currentDifferent) < 0.001) return Direction.Stop;
            var tmpDirection = (currentDifferent > 0);
            if (currentLevel.IsSingle) tmpDirection = !tmpDirection;

            //Debug.WriteLine($"direction diff={currentDifferent}, issingle={currentLevel.IsSingle}");
            return tmpDirection ? Direction.Clockwise : Direction.CounterClockwise;
        }

        private string GetPumpSpeed(DistMeasureRes currentLevel)
        {
            var subLevel = Math.Abs(currentLevel.Dist - SelectedLevel);
            //Debug.WriteLine($"Speed setup diff = {subLevel}");
            //var ss = f_SpeedGrades.Where(x => x.Different < subLevel).OrderByDescending(x => x.Different).FirstOrDefault()?.Speed;
           // Debug.WriteLine($"Speed  = {ss}");
            return f_SpeedGrades.Where(x => x.Different < subLevel).OrderByDescending(x => x.Different).FirstOrDefault()?.Speed;
        }

        private void PumpPortOn(object sender, RoutedEventArgs e)
        {
            PumpActive = true;
            if (f_ConfocalTimer == null)
            {
                f_ConfocalTimer = new Timer
                {
                    Interval = 2000
                };
                //_confocalTimer.Elapsed += PeackInfo;
                //Временно закомментировано, во избежание многоразовой подписки на событие
            }
            f_ConfocalTimer.Start();
            if (!_pumpSerial.Active)
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

            if (!IsTwoPump) return;
            //Тут указываются начальные направления насосов
            _pumpSerial.AddCounterClockwiseDirection();
            _pumpSecondSerial?.AddClockwiseDirection();
        }
        private void PumpPortOff(object sender, RoutedEventArgs e)
        {
            PumpActive = false;
            _pumpSerial?.AddStopPump();
            _pumpSecondSerial?.AddStopPump();
            f_ConfocalTimer.Stop();
        }

        private void PumpStartButton(object sender, RoutedEventArgs e)
        {
            if (!_pumpSerial.Active)
            {
                try
                {
                    _pumpSerial.OpenPort();
                }
                catch (Exception ex)
                {
                    LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message });
                    CbPumpActive.IsChecked = false;
                    return;
                }
            }

            var senderName = ((Button) sender).Name;
            if (senderName == "FirstPumpStart") _pumpSerial.AddStartPump();
            else _pumpSecondSerial.AddStartPump();

        }

        private void PumpStopButton(object sender, RoutedEventArgs e)
        {
            var senderName = ((Button)sender).Name;
            if (senderName == "FirstPumpStop") _pumpSerial.AddStopPump();
            else _pumpSecondSerial.AddStopPump();
        }

        private void StartPerforation(object sender, RoutedEventArgs e)
        {
            if (_pumpSerial == null || _pumpSecondSerial == null) return;
            double.TryParse(Properties.Settings.Default.PerforatingPumpingSpeed, out double dSpeed);
            _pumpSerial.AddSpeed(FormatSpeed(dSpeed));
            _pumpSecondSerial.AddSpeed(FormatSpeed(dSpeed));
            _pumpSerial.AddStartPump();
            _pumpSecondSerial.AddStartPump();

        }

        private static string FormatSpeed(double speed)
        {
            if (Math.Abs(speed) < 0.01) return "0.5 ";
            if (speed < 10.0) return speed.ToString("N1")+" ";
            if (speed < 100) return speed.ToString("N1");
            return speed.ToString("N0")+" ";

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

        private void TbFirstReverseOn(object sender, RoutedEventArgs e)
        {
            if (_pumpSerial != null) _pumpSerial.PumpReverse = true;
        }
        private void TbFirstReverseOff(object sender, RoutedEventArgs e)
        {
            if (_pumpSerial != null) _pumpSerial.PumpReverse = false;
        }

        private void TbSecondReverseOn(object sender, RoutedEventArgs e)
        {
            if (_pumpSecondSerial != null) _pumpSecondSerial.PumpReverse = true;
        }
        private void TbSecondReverseOff(object sender, RoutedEventArgs e)
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
                if (ExPerforate!=null)  ExPerforate.Visibility = Visibility.Visible;
            }
            else
            {
                TbTwoPump.Text = "Один насос";
                if (TbFirstPump != null) TbFirstPump.Text = "Порт насоса";
                if (TbSecondPump != null) TbSecondPump.Visibility = Visibility.Collapsed;
                if (CbPumpSecondPort != null) CbPumpSecondPort.Visibility = Visibility.Collapsed;
                if (ExPerforate != null) ExPerforate.Visibility = Visibility.Collapsed;
            }
        }

    }

    internal class DistMeasureRes
    {
        public double Dist { get; set; }
        public bool IsSingle { get; set; }
    }

    internal enum Direction
    {
        Clockwise, CounterClockwise, Stop
    }

    internal class SpeedGradeItem
    {
        public double Different;
        public string Speed;
    }
}

