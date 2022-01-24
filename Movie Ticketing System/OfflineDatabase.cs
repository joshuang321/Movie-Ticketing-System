using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
