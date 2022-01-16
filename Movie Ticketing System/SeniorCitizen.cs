using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie_Ticketing_System
{
    class SeniorCitizen : Ticket
    {
        public int yearOfBirth { get; set; }
        public SeniorCitizen() : base()
        {
            yearOfBirth = 0;
        }
        public SeniorCitizen(Screening Screening, int YearOfBirth) : base(Screening)
        {
            yearOfBirth = YearOfBirth;
        }
        public override double CalculatePrice()
        {
            return 0.0;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
