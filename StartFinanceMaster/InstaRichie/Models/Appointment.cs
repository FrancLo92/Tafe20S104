using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    public class Appointment
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string AptmtName { get; set; }

        public string AptmtDesc { get; set; }

        [NotNull]
        public string Location { get; set; }

        [NotNull]
        public string AptmtDate { get; set; }

        [NotNull]
        public string StartTime { get; set; }

        [NotNull]
        public string EndTime { get; set; }
        
    }
}
