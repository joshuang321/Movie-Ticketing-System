using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public override string ToString()
        {
            return String.Format("{0, -26}", title) + duration + "\t\t" + classification + "\t\t"
                + openingDate + "\t" + string.Join('/', genreList);
        }
    }
}
