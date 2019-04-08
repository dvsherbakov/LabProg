using System.Collections.Generic;
using System.Linq;

namespace LabProg
{
    public class PwrMode
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Mode { get; set; }
    }

    public class PwrModes
    {
        private List<PwrMode> Lst { get; }

        public PwrModes()
        {
            Lst = new List<PwrMode>
            {
                new PwrMode{Id=1, Type = 0, Mode = "Регулирование по напряжению"},
                new PwrMode{Id=2, Type = 1, Mode = "Регулирование по току"},
                new PwrMode{Id=3, Type = 2, Mode = "Cинусоидальный сигнал"},
                new PwrMode{Id=4, Type = 3, Mode = "ШИМ сигнал"}
            };
        }

        public Dictionary<int, string> GetValues()
        {
            return Lst.ToDictionary(x => x.Type, x => x.Mode);
        }
    }
}
