using System;
using System.ComponentModel.DataAnnotations;

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
