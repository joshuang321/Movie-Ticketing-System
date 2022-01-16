using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Movie_Ticketing_System
{
    class Program
    {
        // If C# forces me to use unmanaged code, it won't be a problem!
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll")]
        public static extern int GetConsoleMode(IntPtr hConsoleMode, IntPtr dwMode);
        [DllImport("kernel32.dll")]
        public static extern int SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

        const int STD_INPUT_HANDLE = -10;
        const int STD_OUTPUT_HANDLE = -11;
        static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        static int gOrderNo = 1;
        static int gScreeningNo = 1001;

        static public List<Movie> MovieData;
        static public List<Cinema> CinemaData;
        static public List<Screening> ScreeningData;
        static public List<Order> OrderData;

        static void Main(string[] args)
        {
            InitializeApp();
            DeleteMovieScreeningSession();
            ScreeningData.ForEach(Print);
        }
        static void Print(Object obj) { Console.WriteLine(obj); }
        static void LoadMovieData()
        {
            string[] strarray; string strline;
            DateTime? dateTime; DateTime dateTimeNotNull;

            using (StreamReader sr = new StreamReader("Movie.csv"))
            {
                sr.ReadLine();
                while ((strline = sr.ReadLine()) != null)
                {
                    strarray = strline.Split(',');
                    if (DateTime.TryParse(strarray[4], out dateTimeNotNull))
                        dateTime = dateTimeNotNull;
                    else dateTime = null;

                    MovieData.Add(new Movie(strarray[0], int.Parse(strarray[1]),
                        strarray[3], dateTime, GenreListFromString(strarray[2])));
                }
            }

        }
        static List<string> GenreListFromString(string strgenre)
        {
            List<string> genreList = new();
            genreList.AddRange(strgenre.Split('/'));
            return genreList;
        }
        static void LoadCinemaData()
        {
            string[] strarray; string strline;
            using (StreamReader sr = new StreamReader("Cinema.csv"))
            {
                sr.ReadLine();
                while ((strline = sr.ReadLine()) != null)
                {
                    strarray = strline.Split(',');
                    CinemaData.Add(new Cinema(strarray[0], int.Parse(strarray[1]),
                        int.Parse(strarray[2])));
                }
            }
        }
        static void LoadScreeningData()
        {
            string[] strarray; string strline;
            Screening screening;
            using (StreamReader sr = new StreamReader("Screening.csv"))
            {
                sr.ReadLine();
                while ((strline = sr.ReadLine()) != null)
                {
                    strarray = strline.Split(',');
                    screening = new Screening(gScreeningNo++, DateTime.Parse(strarray[0]),
                        strarray[1], CinemaData.Find(x => strarray[2] == x.name &&
                        int.Parse(strarray[3]) == x.hallNo),
                        MovieData.Find(x => strarray[4] == x.title));
                    screening.seatsRemaining = screening.cinema.capacity;
                    ScreeningData.Add(screening);
                }
            }
        }
        static void InitializeApp()
        {
            MovieData = new(); CinemaData = new(); ScreeningData = new(); OrderData = new();
            LoadMovieData(); LoadCinemaData(); LoadScreeningData();
            MovieData.ForEach(delegate (Movie movie)
            {
                movie.screeningList = ScreeningData.FindAll(x => movie == x.movie);
            });
        }
        static void ListMovieScreenings()
        {
            Console.WriteLine("{0,-26}Duration\tClassification\tOpeningDate\t\tGenres", "Title");
            MovieData.ForEach(Print);
            Console.Write("\r\nSelect a movie to watch: ");
            string title = Console.ReadLine();
            Movie moviesearch = MovieData.Find(x => title == x.title);
            if (null == moviesearch)
            {
                Console.WriteLine("Failed to find Movie with title: " + title);
                return;
            }
            Console.WriteLine("\r\nScreening Number\tScreening Date Time\tScreeningType\t" +
                "Seats Remaining\tCinema\t\tHall Number\tCapacity\tTitle");
            List<Screening> allmovieScreenings = ScreeningData.FindAll(
                x => moviesearch == x.movie);
            allmovieScreenings.ForEach(Print);
        }
        static void AddMovieScreeningSession()
        {
            MovieData.ForEach(Print);
            Console.Write("\r\nSelect a movie: ");
            string title = Console.ReadLine();
            Movie movie = MovieData.Find(x=> title == x.title);
            if (null == movie)
            {
                Console.WriteLine("Failed to find to find Movie with title: " + title);
                return;
            }
            Console.Write("Select a screening type: ");
            string screentype = Console.ReadLine();
            if ("2D" != screentype && "3D" != screentype)
            {
                Console.WriteLine("Invalid screening type!");
            }
            Console.Write("Enter screening date time: ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime screeningdateTime))
            {
                Console.WriteLine("Failed to provide a valid DateTime Format!");
                return;
            }
            Console.Write("\r\n");
            CinemaData.ForEach(Print);
            Console.Write("\r\nSelect Cinema Hall: ");
            int index = int.Parse(Console.ReadLine());
            if (index > (CinemaData.Count -1) || index < 0)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            Cinema chosencinema = CinemaData[index];
            bool isAvailable = true;
            ScreeningData.ForEach(delegate (Screening screening) {
                if (chosencinema == screening.cinema && 
                (screeningdateTime < screening.screeningDateTime.AddMinutes(screening.movie.duration + 15)
                && screeningdateTime > screening.screeningDateTime) ||
                (screeningdateTime.AddMinutes(movie.duration + 15) < screening.screeningDateTime.AddMinutes(screening.movie.duration + 15)
                && screeningdateTime.AddMinutes(movie.duration + 15) > screening.screeningDateTime))
                {
                    isAvailable = false; return;
                }
            });
            if (!isAvailable)
            {
                Console.WriteLine("Cinema hall is currently screening at that time!");
                return;
            }
            ScreeningData.Add(new Screening(gScreeningNo++, screeningdateTime, screentype,
                chosencinema, movie));
            Console.WriteLine("Movie Screening creation successful!");
        }
        static void DeleteMovieScreeningSession()
        {
            ScreeningData.ForEach(Print);
            Console.Write("\r\nSelection Session: ");
            if (int.TryParse(Console.ReadLine(), out int index) || index < 1 
                || index > ScreeningData.Count)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            ScreeningData.RemoveAt(index -1);
            Console.WriteLine("Movie Screening removal successful!");
        }
        static void OrderMovieTicket()
        {
            int index;
            MovieData.ForEach(Print);
            Console.Write("\r\nSelect a Movie: ");
            if (!int.TryParse(Console.ReadLine(), out index)
                || index < 1 || index > MovieData.Count)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            List<Screening> filteredscreening = ScreeningData.FindAll(x => MovieData[index] == x.movie);
            filteredscreening.ForEach(Print);
            Console.Write("\r\nSelect Movie Screening: ");
            if (!int.TryParse(Console.ReadLine(), out index)
                || index < 1 || index > MovieData.Count)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            Screening chosenscreening = filteredscreening[index];
            Console.Write("Number of Tickets to order: ");
            if (!int.TryParse(Console.ReadLine(), out index) || index < 0)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            if (index < chosenscreening.seatsRemaining)
            {
                Console.WriteLine("Not enough seats of the chosen session!");
                return;
            }
            Order newOrder = new Order(gOrderNo++, DateTime.Now);
            newOrder.status = "Unpaid";
            string ticketType;
            Ticket genericTicket = null;
            TimeSpan timeSpan = newOrder.orderDateTime - chosenscreening.screeningDateTime;
            for (int i = 0; i < index; i++)
            {
                Console.WriteLine("Ticket No." + (i + 1));
                if (timeSpan.Days > 7)
                {
                    Console.Write("Please state ticket type: ");
                    ticketType = Console.ReadLine();
                }
                else ticketType = "Adult";
                switch (ticketType)
                {
                    case "Student":
                        Console.Write("Please state your level of study: ");
                        string levelOfStudy = Console.ReadLine();
                        if ("Primary" != levelOfStudy && "Secondary" != levelOfStudy && "Tertiary" != levelOfStudy)
                        {
                            Console.WriteLine("Failed to state a valid level of study!");
                            return;
                        }
                        genericTicket = new Student(chosenscreening, levelOfStudy);
                        if ("2D" == chosenscreening.screeningType)
                        {
                            if ((int)chosenscreening.screeningDateTime.DayOfWeek < 5)
                                newOrder.amount += 8.0;
                            else if ((int)chosenscreening.screeningDateTime.DayOfWeek < 7
                                && 0 == (int)chosenscreening.screeningDateTime.DayOfWeek)
                                newOrder.amount += 14.0;
                        }
                        else if ("3D" == chosenscreening.screeningType)
                        {
                            if ((int)chosenscreening.screeningDateTime.DayOfWeek < 5)
                                newOrder.amount += 7.0;
                            else if ((int)chosenscreening.screeningDateTime.DayOfWeek < 7
                                && 0 == (int)chosenscreening.screeningDateTime.DayOfWeek)
                                newOrder.amount += 12.5;
                        }
                        break;

                    case "Senior Citizen":
                        Console.Write("Please state your year of birth: ");
                        if (!int.TryParse(Console.ReadLine(), out int yob))
                        {
                            Console.WriteLine("Failed to provide a valid year of birth!");
                            return;
                        }
                        genericTicket = new SeniorCitizen(chosenscreening, yob);
                        if ("2D" == chosenscreening.screeningType)
                        {
                            if ((int)chosenscreening.screeningDateTime.DayOfWeek < 5)
                                newOrder.amount += 6.0;
                            else if ((int)chosenscreening.screeningDateTime.DayOfWeek < 7
                                && 0 == (int)chosenscreening.screeningDateTime.DayOfWeek)
                                newOrder.amount += 14.0;
                        }
                        else if ("3D" == chosenscreening.screeningType)
                        {
                            if ((int)chosenscreening.screeningDateTime.DayOfWeek < 5)
                                newOrder.amount += 5.0;
                            else if ((int)chosenscreening.screeningDateTime.DayOfWeek < 7
                                && 0 == (int)chosenscreening.screeningDateTime.DayOfWeek)
                                newOrder.amount += 12.5;
                        }
                        break;

                    case "Adult":
                        Console.Write("Would you like popcorn for an extra $3? [Y/N]: ");
                        string yn = Console.ReadLine();
                        if (1 != yn.Length || 'y' != yn[0] || 'Y' != yn[0]
                            || 'n' != yn[0] || 'N' != yn[0])
                        {
                            Console.WriteLine("Invalid Input!"); break;
                        }
                        genericTicket = new Adult(chosenscreening, ('y'==yn[0] || 'Y'==yn[0]) ? true : false);
                        if ("2D" == chosenscreening.screeningType)
                        {
                            if ((int)chosenscreening.screeningDateTime.DayOfWeek < 5)
                                newOrder.amount += 11.0;
                            else if ((int)chosenscreening.screeningDateTime.DayOfWeek < 7
                                && 0 == (int)chosenscreening.screeningDateTime.DayOfWeek)
                                newOrder.amount += 14.0;
                        }
                        else if ("3D" == chosenscreening.screeningType)
                        {
                            if ((int)chosenscreening.screeningDateTime.DayOfWeek < 5)
                                newOrder.amount += 8.5;
                            else if ((int)chosenscreening.screeningDateTime.DayOfWeek < 7
                                && 0 == (int)chosenscreening.screeningDateTime.DayOfWeek)
                                newOrder.amount += 12.5;
                        }
                        break;
                }
                newOrder.ticketList.Add(genericTicket);
            }
            Console.Write("Amount payable: {0:C2}\r\nPRESS ANY KEY TO MAKE PAYMENT",
                newOrder.amount);
            Console.ReadKey();
            chosenscreening.seatsRemaining -= index;
            newOrder.status = "Paid";
        }
        static void CancelTicketOrder()
        {
            Console.Write("Please state order number to be cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int ordernumber))
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            Order ordersearch = OrderData.Find(x => ordernumber == x.orderNo);
            if (null == ordersearch)
            {
                Console.WriteLine("Failed to find Order of Order No." + ordernumber +
                    "\r\nOrder Cancellation Unsuccessful!");
                return;
            }
            ordersearch.ticketList[0].screening.seatsRemaining += ordersearch.ticketList.Count;
            ordersearch.status = "Cancelled";
            Console.WriteLine("Amount refunded: {0:C2}\r\nCancellation Successful!", ordersearch.amount);
        }
    }
}
