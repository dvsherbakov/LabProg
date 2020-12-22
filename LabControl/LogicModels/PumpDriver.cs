using LabControl.ClassHelpers;
using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private bool f_IsTwoPump;
        private bool f_IsPumpActive;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

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

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        public void SetMeasuredLevel(DistMeasureRes lvl)
        {
            f_MeasuredLvl = lvl;
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

        private string GetPumpSpeed(DistMeasureRes currentLevel)
        {
            var subLevel = Math.Abs(currentLevel.Dist - f_MeasuredLvl.Dist);
            
            return f_SpeedGrades.Where(x => x.Different < subLevel).OrderByDescending(x => x.Different).FirstOrDefault()?.Speed;
        }
    }

    internal enum Direction
    {
        Clockwise, CounterClockwise, Stop
    }
}
