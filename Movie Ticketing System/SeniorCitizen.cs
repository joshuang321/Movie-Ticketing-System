//============================================================
// Student Number : S10205140B
// Student Name : Joshua Ng
// Module Group : T04
//============================================================

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
            if ("2D" == screening.screeningType)
            {
                if ((int)screening.screeningDateTime.DayOfWeek < 5)
                    return 6.0;
                else if ((int)screening.screeningDateTime.DayOfWeek < 7
                    && 0 == (int)screening.screeningDateTime.DayOfWeek)
                    return 14.0;
            }
            else if ("3D" == screening.screeningType)
            {
                if ((int)screening.screeningDateTime.DayOfWeek < 5)
                    return 5.0;
                else if ((int)screening.screeningDateTime.DayOfWeek < 7
                    && 0 == (int)screening.screeningDateTime.DayOfWeek)
                    return 12.5;
            }
            return 0.0;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
