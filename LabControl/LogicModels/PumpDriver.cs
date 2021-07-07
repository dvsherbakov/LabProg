using LabControl.ClassHelpers;
using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LabControl.LogicModels
{
    internal class PumpDriver
    {
        private PumpSerial _portInput;
        private PumpSerial _portOutput;
        public string PortStrInput { get; set; }
        public string PortStrOutput { get; set; }
        public bool DirectionInput { get; set; }
        public bool DirectionOutput { get; set; }
        public float PumpingSpeed { get; set; }

        private DistMeasureRes _measuredLvl;
        private double _requiredLvl;
        private bool _isTwoPump;
        private bool _isPumpActive;
        private SpeedGradeItem _prevSpeed;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;
        public event LogMessage SetInputSpeed;
        public event LogMessage SetOutputSpeed;

        private readonly List<SpeedGradeItem> _speedGradeItems = new List<SpeedGradeItem> {
            new SpeedGradeItem{Different=1.00, Speed="250 ", value=250},
            new SpeedGradeItem{Different=0.85, Speed="150 ", value = 150},
            new SpeedGradeItem{Different=0.70, Speed="100 ", value = 100},
            new SpeedGradeItem{Different=0.45, Speed="65.0", value = 65},
            new SpeedGradeItem{Different=0.30, Speed="45.0", value = 45},
            new SpeedGradeItem{Different=0.20, Speed="35.0", value = 35},
            new SpeedGradeItem{Different=0.10, Speed="25.0", value = 25},
            new SpeedGradeItem{Different=0.08, Speed="20.0", value = 20},
            new SpeedGradeItem{Different=0.04, Speed="11.0", value = 11},
            new SpeedGradeItem{Different=0.01, Speed="2.5 ", value = 2.5f},
            new SpeedGradeItem{Different=0.005, Speed="0.5 ", value=0.5f},
            new SpeedGradeItem{Different=0.00, Speed="0.0 ", value=0}
        };

        public PumpDriver()
        {
            _isPumpActive = false;
            PumpingSpeed = 0f;
        }

        public void ConnectToPorts()
        {
            _portInput = new PumpSerial(PortStrInput, DirectionInput);
            _portInput.SetLogMessage += TestLog;
            _portInput?.OpenPort();
            _portInput?.AddCounterClockwiseDirection();
            if (!_isTwoPump) return;
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
            _measuredLvl = lvl;
            if (_isTwoPump) OperateTwoPump(); else OperatePump();
        }

        public void SetRequiredLvl(double lvl)
        {
            _requiredLvl = lvl;
        }

        public void SetPumpActive(bool active)
        {
            _isPumpActive = active;
        }

        public void ToggleTwoPump(bool isTwo)
        {
            _isTwoPump = isTwo;
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

        private SpeedGradeItem GetNextSpeed(float speed) =>
            _speedGradeItems.Where(x => x.value >= speed).OrderBy(y => y.value).ToArray()?[1];

        private SpeedGradeItem GetPrevSpeed(float speed) =>
            _speedGradeItems.Where(x => x.value <= speed).OrderByDescending(y => y.value).ToArray()?[1];

        private SpeedGradeItem GetPumpSpeed(float add = 0)
        {
            var subLevel = Math.Abs(_requiredLvl - _measuredLvl.Dist) + add / 20;

            return _speedGradeItems.Where(x => x.Different < subLevel).OrderByDescending(x => x.Different).FirstOrDefault();
        }


        private void OperatePump()
        {
            var speed = GetPumpSpeed().value > _prevSpeed.value ? GetNextSpeed(_prevSpeed.value) : GetPrevSpeed(_prevSpeed.value);

            if (!_isPumpActive)
            {
                StopPump(true);
                return;
            }
            if (speed == _prevSpeed)
            {
                if (Math.Abs(double.Parse(speed.Speed.Trim(), CultureInfo.InvariantCulture)) < 0.001)
                {
                    StopPump(true);
                }
            }
            else
            {
                _portInput?.AddSpeed(speed.Speed);
                SetInputSpeed?.Invoke(speed.Speed);
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
            if (speed.value != 0)
            {
                StartPump(true);
            }
            _prevSpeed = speed;
        }

        private void OperateTwoPump()
        {
            var speed = GetPumpSpeed();
            var direction = GetDirection();

            if (PumpingSpeed != 0)
            {
                var sp = _speedGradeItems.Where(x => Math.Abs(PumpingSpeed - x.value) < 0.05).OrderByDescending(y => y.value).FirstOrDefault();
                _portInput.AddSpeed(direction == Direction.Clockwise ? GetPumpSpeed(PumpingSpeed).Speed : sp?.Speed);
                _portOutput.AddSpeed(direction == Direction.CounterClockwise ? GetPumpSpeed(PumpingSpeed).Speed : sp?.Speed);
                StartPump(true);
                StartPump(false);
                return;
            }

            if (!_isPumpActive || speed.value == 0 || (direction == Direction.Stop))
            {
                StopPump(true);
                StopPump(false);
                return;
            }
            switch (direction)
            {
                case Direction.Clockwise:
                    _portInput.AddSpeed(speed.Speed);
                    StartPump(true);
                    _portOutput.AddSpeed("0.5 ");
                    StopPump(false);
                    SetInputSpeed?.Invoke(speed.Speed);
                    SetOutputSpeed?.Invoke("0");
                    break;
                case Direction.CounterClockwise:
                    _portOutput.AddSpeed(speed.Speed);
                    StartPump(false);
                    _portInput.AddSpeed("0.5 ");
                    StopPump(true);
                    SetInputSpeed?.Invoke("0");
                    SetOutputSpeed?.Invoke(speed.Speed);
                    break;
                default:
                    StopPump(false);
                    StopPump(true);
                    SetInputSpeed?.Invoke("0");
                    SetOutputSpeed?.Invoke("0");
                    break;
            }
        }

        private Direction GetDirection()
        {
            var currentDifferent = _requiredLvl - _measuredLvl.Dist;
            if (Math.Abs(currentDifferent) < 0.001) return Direction.Stop;
            var tmpDirection = (currentDifferent > 0);
            if (_measuredLvl.IsSingle) tmpDirection = !tmpDirection;

            if (_measuredLvl.IsSingle)
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
