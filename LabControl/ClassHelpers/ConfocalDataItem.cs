using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.ClassHelpers
{
    internal class ConfocalDataItem
    {
        public DateTime MomentDateTime { get; }
        public double Value { get; private set; }

        public ConfocalDataItem(double value)
        {
            MomentDateTime = DateTime.Now;
            Value = value;
        }
    }
}
