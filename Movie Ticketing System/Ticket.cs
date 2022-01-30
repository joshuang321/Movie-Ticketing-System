//============================================================
// Student Number : S10205140B
// Student Name : Joshua Ng
// Module Group : T04
//============================================================

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
