using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.DataModels
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        public DateTime Dt { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
