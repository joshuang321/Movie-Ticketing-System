//============================================================
// Student Number : S10205140B
// Student Name : Joshua Ng
// Module Group : T04
//============================================================

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
            if ("2D" == screening.screeningType)
            {
                if ((int)screening.screeningDateTime.DayOfWeek < 5)
                    return 8.0;
                else if ((int)screening.screeningDateTime.DayOfWeek < 7
                    && 0 == (int)screening.screeningDateTime.DayOfWeek)
                    return 14.0;
            }
            else if ("3D" == screening.screeningType)
            {
                if ((int)screening.screeningDateTime.DayOfWeek < 5)
                    return 7.0;
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
