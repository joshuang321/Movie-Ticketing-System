//============================================================
// Student Number : S10205140B
// Student Name : Joshua Ng
// Module Group : T04
//============================================================

namespace Movie_Ticketing_System
{
    class Adult : Ticket
    {
        public bool popcornOffer { get; set; }
        public Adult() : base()
        {
            popcornOffer = false;
        }
        public Adult(Screening Screening, bool PopcornOffer) : base(Screening)
        {
            popcornOffer = PopcornOffer;
        }
        public override double CalculatePrice()
        {
            if ("2D" == screening.screeningType)
            {
                if ((int)screening.screeningDateTime.DayOfWeek < 5)
                    return 11.0 + addPopcorn();
                else if ((int)screening.screeningDateTime.DayOfWeek < 7
                    && 0 == (int)screening.screeningDateTime.DayOfWeek)
                    return 14.0 + addPopcorn();
            }
            else if ("3D" == screening.screeningType)
            {
                if ((int)screening.screeningDateTime.DayOfWeek < 5)
                    return 8.5 + addPopcorn();
                else if ((int)screening.screeningDateTime.DayOfWeek < 7
                    && 0 == (int)screening.screeningDateTime.DayOfWeek)
                    return 12.5 + addPopcorn();
            }
            return 0.0;
        }
        double addPopcorn()
        {
            return popcornOffer ? 3 : 0;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
