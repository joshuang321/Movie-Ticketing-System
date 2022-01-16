using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie_Ticketing_System
{
    abstract class Ticket
    {
        public Screening screening { get; set; }
        public Ticket()
        {
            screening = null;
        }
        public Ticket(Screening Screening)
        {
            screening = Screening;
        }
        public abstract double CalculatePrice();
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
