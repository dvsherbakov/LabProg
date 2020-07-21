using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabProg.Cams
{
    class CamType
    {
        public int Id { get; private set; }
        public string CamName { get; private set; }

        public CamType(int id, string name)
        {
            Id = id;
            CamName = name;
        }
    }
}
