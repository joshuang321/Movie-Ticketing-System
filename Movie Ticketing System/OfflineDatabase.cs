//============================================================
// Student Number : S10205140B
// Student Name : Joshua Ng
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;

namespace Movie_Ticketing_System
{
    class OfflineDatabase
    {
        public static int gOrderNo = 1;
        public static int gScreeningNo = 1001;

        public static List<Movie> MovieData;
        public static List<Cinema> CinemaData;
        public static List<Screening> ScreeningData;
        public static List<Order> OrderData;

        public class NumMovies : IComparable<NumMovies>
        {
            public Movie movie { get; set; }
            public int num { get; set; }
            public NumMovies(Movie Movie, int Num)
            {
                movie = Movie; num = Num;
            }
            public int CompareTo(NumMovies numMovies)
            {
                return num.CompareTo(numMovies.num);
            }
        }
    }
}
