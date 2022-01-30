//============================================================
// Student Number : S10205140B
// Student Name : Joshua Ng
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;

namespace Movie_Ticketing_System
{
    class Movie
    {
        public string title { get; set; }
        public int duration { get; set; }
        public string classification { get; set; }
        public DateTime? openingDate { get; set; }
        public List<string> genreList { get; set; }
        public List<Screening> screeningList { get; set; }
        public Movie()
        {
            duration = 0; classification = null; openingDate = null;
            genreList = null; screeningList = null;
        }
        public Movie(string Title, int Duration, string Classification, DateTime? OpeningDate,
            List<string> GenreList)
        {
            title = Title; duration = Duration; classification = Classification;
            openingDate = OpeningDate; genreList = GenreList;
        }
        public void AddGenre(string genre)
        {
            genreList.Add(genre);
        }
        public void AddScreening(Screening screening)
        {
            screeningList.Add(screening);
        }
        public override string ToString()
        {
            return string.Format("{0, -26}", title) + duration + "\t\t" + classification + "\t\t"
                + openingDate + "\t" + string.Join('/', genreList);
        }
    }
}
