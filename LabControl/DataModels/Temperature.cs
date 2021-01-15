using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.DataModels
{
    public class Temperature
    {
        [Key]
        public int Id { get; set; }
        public DateTime Dt { get; set; }
        public float Tmp { get; set; }
    }
}
