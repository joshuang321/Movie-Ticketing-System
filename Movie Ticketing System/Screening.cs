//============================================================
// Student Number : S10205140B
// Student Name : Joshua Ng
// Module Group : T04
//============================================================

using System;

namespace Movie_Ticketing_System
{
    class Screening : IComparable<Screening>
    {
        public int screeningNo { get; set; }
        public DateTime screeningDateTime { get; set; }
        public string screeningType { get; set; }
        public int seatsRemaining { get; set; }
        public Cinema cinema { get; set; }
        public Movie movie { get; set; }
        public Screening()
        {
            screeningNo = 0; screeningDateTime = DateTime.Now; screeningType = null; seatsRemaining = 0;
            cinema = null; movie = null;
        }
        public Screening(int ScreeningNo, DateTime ScreeningDateTime, string ScreeningType,
            Cinema Cinema, Movie Movie)
        {
            screeningNo = ScreeningNo; screeningDateTime = ScreeningDateTime; screeningType = ScreeningType;
            cinema = Cinema; movie = Movie;
        }
        public int CompareTo(Screening screening)
        {
            return seatsRemaining.CompareTo(screening.seatsRemaining);
        }
        public override string ToString()
        {
            return screeningNo + "\t\t\t" + screeningDateTime + "\t" + screeningType
                + "\t\t" + seatsRemaining + "\t\t" + cinema + "\t\t" + movie.title;
        }
    }
}
