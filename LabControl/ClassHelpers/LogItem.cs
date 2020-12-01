using System;

namespace LabControl.ClassHelpers
{
    class LogItem
    {
        public DateTime Current { get; private set; }
        public string Message { get; private set; }

        public LogItem(DateTime dt, string msg)
        {
            Current = dt;
            Message = msg;
        }

    }
}
