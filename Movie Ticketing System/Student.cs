using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie_Ticketing_System
{
    class Student : Ticket
    {
        public string levelOfStudy { get; set; }
        public Student() : base()
        {
            levelOfStudy = null;
        }
        public Student(Screening Screening, string LevelOfStudy) : base(Screening)
        {
            levelOfStudy = LevelOfStudy;
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
