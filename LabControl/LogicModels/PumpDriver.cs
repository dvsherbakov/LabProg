using LabControl.ClassHelpers;
using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LabControl.LogicModels
{
    class PumpDriver
    {
        private PumpSerial _portInput;
        private PumpSerial _portOutput;
        public string PortStrInput { get; set; }
        public string PortStrOutput { get; set; }
        public bool DirectionInput { get; set; }
        public bool DirectionOutput { get; set; }

        private DistMeasureRes f_MeasuredLvl;
        private double f_RequiredLvl;
        private bool f_IsTwoPump;
        private bool f_IsPumpActive;
        private string f_PrevSpeed;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;
        public event LogMessage SetInputSpeed;
        public event LogMessage SetOutputSpeed;

        private readonly List<SpeedGradeItem> f_SpeedGrades = new List<SpeedGradeItem> {
            new SpeedGradeItem{Different=2.00, Speed="250 "},
            new SpeedGradeItem{Different=1.00, Speed="150 "},
            new SpeedGradeItem{Different=0.70, Speed="65.0"},
            new SpeedGradeItem{Different=0.30, Speed="45.0"},
            new SpeedGradeItem{Different=0.20, Speed="35.0"},
            new SpeedGradeItem{Different=0.10, Speed="25.0"},
            new SpeedGradeItem{Different=0.08, Speed="20.0"},
            new SpeedGradeItem{Different=0.04, Speed="11.0"},
            new SpeedGradeItem{Different=0.01, Speed="2.5 "},
            new SpeedGradeItem{Different=0.005, Speed="0.5 "},
            new SpeedGradeItem{Different=0.00, Speed="0.0 "}
        };

        public PumpDriver()
        {
            f_IsPumpActive = false;
        }

        public void ConnectToPorts()
        {
            _portInput = new PumpSerial(PortStrInput, DirectionInput);
            _portInput.SetLogMessage += TestLog;
            _portInput?.OpenPort();
            _portInput?.AddCounterClockwiseDirection();
            if (!f_IsTwoPump) return;
            _portOutput = new PumpSerial(PortStrOutput, DirectionOutput);
            _portOutput.SetLogMessage += TestLog;
            _portOutput?.OpenPort();
            _portOutput?.AddClockwiseDirection();
        }

        public void Disconnect()
        {
            _portInput?.ClosePort();
            _portOutput?.ClosePort();
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        public void SetMeasuredLevel(DistMeasureRes lvl)
        {
            f_MeasuredLvl = lvl;
            if (f_IsTwoPump) OperateTwoPump(); else OperatePump();
        }

        public void SetRequiredLvl(double lvl)
        {
            f_RequiredLvl = lvl;
        }

        public void SetPumpActive(bool active)
        {
            f_IsPumpActive = active;
        }

        public void TogleTwoPump(bool isTwo)
        {
            f_IsTwoPump = isTwo;
        }

        private void StopPump(bool isFirst)
        {
            if (isFirst) _portInput?.AddStopPump();
            else _portOutput?.AddStopPump();
        }

        private void StartPump(bool isFirst)
        {
            if (isFirst) _portInput?.AddStartPump();
            else _portOutput?.AddStartPump();
        }

        private string GetPumpSpeed()
        {
            var subLevel = Math.Abs(f_RequiredLvl - f_MeasuredLvl.Dist);

            return f_SpeedGrades.Where(x => x.Different < subLevel).OrderByDescending(x => x.Different).FirstOrDefault()?.Speed;
        }

        private void OperatePump()
        {
            var speed = GetPumpSpeed();
            if (!f_IsPumpActive)
            {
                StopPump(true);
                return;
            }
            if (speed == f_PrevSpeed)
            {
                if (Math.Abs(double.Parse(speed.Trim(), CultureInfo.InvariantCulture)) < 0.001)
                {
                    StopPump(true);
                }
            }
            else
            {
                _portInput?.AddSpeed(speed);
                SetInputSpeed?.Invoke(speed);
            }
            var direction = GetDirection();
            switch (direction)
            {
                case Direction.Stop:
                    //_pumpSerial.AddStopPump();
                    StopPump(true);
                    break;
                case Direction.Clockwise:
                    _portInput?.AddClockwiseDirection();
                    break;
                case Direction.CounterClockwise:

                    _portInput?.AddCounterClockwiseDirection();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (!speed.Equals("0.0 "))
            {
                StartPump(true);
            }
            f_PrevSpeed = speed;
        }

        private void OperateTwoPump()
        {
            var speed = GetPumpSpeed();
            var direction = GetDirection();

            if (!f_IsPumpActive || speed.Equals("0.0 ") || (direction == Direction.Stop))
            {
                StopPump(true);
                StopPump(false);
                return;
            }
            switch (direction)
            {
                case Direction.Clockwise:
                    _portInput.AddSpeed(speed);
                    StartPump(true);
                    _portOutput.AddSpeed("0.5 ");
                    StopPump(false);
                    SetInputSpeed?.Invoke(speed);
                    SetOutputSpeed?.Invoke("0");
                    break;
                case Direction.CounterClockwise:
                    _portOutput.AddSpeed(speed);
                    StartPump(false);
                    _portInput.AddSpeed("0.5 ");
                    StopPump(true);
                    SetInputSpeed?.Invoke("0");
                    SetOutputSpeed?.Invoke(speed);
                    break;
                case Direction.Stop:
                    StopPump(false);
                    StopPump(true);
                    SetInputSpeed?.Invoke("0");
                    SetOutputSpeed?.Invoke("0");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Direction GetDirection()
        {
            var currentDifferent = f_RequiredLvl - f_MeasuredLvl.Dist;
            if (Math.Abs(currentDifferent) < 0.001) return Direction.Stop;
            var tmpDirection = (currentDifferent > 0);
            if (f_MeasuredLvl.IsSingle) tmpDirection = !tmpDirection;

            if (f_MeasuredLvl.IsSingle)
                return !tmpDirection ? Direction.Clockwise : Direction.CounterClockwise;
            else 
                return tmpDirection ? Direction.Clockwise : Direction.CounterClockwise;
        }
    }

    internal enum Direction
    {
        Clockwise, CounterClockwise, Stop
    }
}
