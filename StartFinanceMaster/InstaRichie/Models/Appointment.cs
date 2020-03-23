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

        [Unique]
        public string AptmnName { get; set; }

       public string AptmnDesc { get; set; }

        [NotNull]
        public DateTime AptmnDate { get; set; }
    }
}
