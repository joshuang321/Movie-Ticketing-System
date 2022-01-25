using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace Movie_Ticketing_System
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeApp();
            /*
            while (true) 
            {
                Console.WriteLine("Movie Database System\r\n" +
                                  "---------------------\r\n" +
                                  "1. Offline Movie Database\r\n" +
                                  "2. Online Movie Database via The Movie Database (TMDB)\r\n");
                if (!int.TryParse(Console.ReadLine(), out int options) || options < 1 || options > 2)
                {
                    Console.WriteLine("Invalid Option!"); continue;
                }
                if (1 == options) OfflineDatabaseInterface();
                else OnlineDatabaseInterface();
            }
            */
        }

        static void Print(Object obj) { Console.WriteLine(obj); }
        static void OfflineDatabaseInterface()
        {
            Console.Write("Offline Database System\r\n" +
                          "-----------------------\r\n" +
                          "1. List all movie Screenings\r\n" +
                          "2. Add a movie Screening\r\n" +
                          "3. Remove a movie Screening\r\n" +
                          "4. Order a movie Ticket\r\n" +
                          "5. Cancel a movie Ticket\r\n");
            if (!int.TryParse(Console.ReadLine(), out int options) || options < 1 || options > 5)
            {
                Console.WriteLine("Invalid Option!"); return;
            }
            switch (options)
            {
                case 1: ListMovieScreenings(); break;
                case 2: AddMovieScreeningSession(); break;
                case 3: DeleteMovieScreeningSession(); break;
                case 4: OrderMovieTicket(); break;
                case 5: CancelTicketOrder(); break;
            }
        }
        static void OnlineDatabaseInterface()
        {
            Console.WriteLine("Online Movie Database through TMDB\r\n" +
                              "----------------------------------\r\n" +
                              "1. Create a new Login Session\r\n" +
                              "2. Create a new Guest Session\r\n");
            if (!int.TryParse(Console.ReadLine(), out int options) || options < 1 || options > 2)
            {
                    Console.WriteLine("Invalid option!"); return;
            }
            OnlineDatabase.MovieDatabaseSession currentSession;
            if (1 == options) currentSession = CreateLoginSession();
            else currentSession = CreateGuestSession();

            if (currentSession is OnlineDatabase.MovieDatabaseSession)
            {
                Console.WriteLine("What do you want to do?\r\n" +
                              "1.  Get Movie Details\r\n" +
                              "2.  Get Alternative Movie Titles\r\n" +
                              "3.  Get Movie Images\r\n" +
                              "4.  Get Movie Recommendations\r\n" +
                              "5.  Get Similar Movies\r\n" +
                              "6.  Get Movie Release Dates\r\n" +
                              "7.  Get Movie Videos\r\n" +
                              "8. Get Account Details\r\n");

                if (!int.TryParse(Console.ReadLine(), out options) || options < 1 || options > 8)
                {
                    Console.WriteLine("Invalid Input!"); currentSession.ExitSession();
                    return;
                }
                switch (options)
                {
                    case 1: GetMovieDetails(); break;
                    case 2: GetAlternativeMovieTitles(); break;
                    case 3: GetMovieImages(); break;
                    case 4: GetMovieRecommendations(); break;
                    case 5: GetSimilarMovies(); break;
                    case 6: GetMovieReleaseDates(); break;
                    case 7: GetMovieVideos(); break;
                    case 8: GetAccountDetails(currentSession.session_id); break;
                }
            }
            else
            {
                Console.WriteLine("What do you want to do?\r\n" +
                                  "-----------------------\r\n" +
                                  "1. Get Rated Movies\r\n");
                if (!int.TryParse(Console.ReadLine(), out options) || options != 1)
                {
                    Console.WriteLine("Invalid Input!"); currentSession.ExitSession();
                    return;
                }
                GuestGetRatedMovies(currentSession.session_id);
            }
            currentSession.ExitSession();
        }
        static string GetFromMovieDatabase(string requestUri)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(OnlineDatabase.onlinemoviedb_baselink);
                Task<HttpResponseMessage> getresponseTask = httpClient.GetAsync(requestUri);
                getresponseTask.Wait();
                HttpResponseMessage httpResponse = getresponseTask.Result;
                if (!httpResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error occured, please try again.");
                    return null;
                }
                Task<string> readStringTask = httpResponse.Content.ReadAsStringAsync();
                readStringTask.Wait();
                JObject jObject = JObject.Parse(readStringTask.Result);
                if (jObject.ContainsKey("success"))
                {
                    OnlineDatabase.MovieDatabaseError movieDatabaseError = JsonConvert.DeserializeObject
                        <OnlineDatabase.MovieDatabaseError>(readStringTask.Result);
                    Console.WriteLine($"movieDatabase Error occured. Status code: ${movieDatabaseError.status_code}\r\n" +
                        $"{movieDatabaseError.status_message}");
                    if (7 == movieDatabaseError.status_code) Console.WriteLine($"Invalid api_key: {OnlineDatabase.api_key_session}");
                    return null;
                }
                return readStringTask.Result;
            }
        }
        static OnlineDatabase.MovieDatabaseSession CreateLoginSession()
        {
            OnlineDatabase.MovieDatabaseRequestToken request_token = 
                new OnlineDatabase.MovieDatabaseRequestToken();
            Console.WriteLine("Enter username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(OnlineDatabase.onlinemoviedb_baselink);
                Task<HttpResponseMessage> getresponseTask = httpClient.GetAsync(@"/authentication/token/new?api_key=" +
                        OnlineDatabase.api_key_session);
                getresponseTask.Wait();
                HttpResponseMessage responseMessage = getresponseTask.Result;
                if (!responseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error occured, please try again,"); return null;
                }
                Task<string> readStringTask = responseMessage.Content.ReadAsStringAsync();
                readStringTask.Wait();
                JObject jObject = JObject.Parse(readStringTask.Result);
                if (!(bool)jObject.SelectToken("success"))
                {
                    OnlineDatabase.MovieDatabaseError movieDatabaseError = JsonConvert.DeserializeObject
                        <OnlineDatabase.MovieDatabaseError>(readStringTask.Result);
                    Console.WriteLine($"movieDatabase Error occured. Status code: ${movieDatabaseError.status_code}\r\n" +
                        $"{movieDatabaseError.status_message}");
                    if (7 == movieDatabaseError.status_code) Console.WriteLine($"Invalid api_key: {OnlineDatabase.api_key_session}");
                    return null;
                }
                request_token.expires_at = (string)jObject.SelectToken("expires_at");
                request_token.request_token = (string)jObject.SelectToken("request_token");

                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("request_token", request_token.request_token)
                });
                getresponseTask = httpClient.PostAsync(@"/authentication/token/validate_with_login?api_key=" + OnlineDatabase.api_key_session,
                    content);
                getresponseTask.Wait();
                responseMessage = getresponseTask.Result;
                if (!responseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error occured, please try again."); return null;
                }
                readStringTask = responseMessage.Content.ReadAsStringAsync();
                readStringTask.Wait();
                jObject = JObject.Parse(readStringTask.Result);
                if(!(bool)jObject.SelectToken("success"))
                {
                    OnlineDatabase.MovieDatabaseError movieDatabaseError = JsonConvert.DeserializeObject
                        <OnlineDatabase.MovieDatabaseError>(readStringTask.Result);
                    Console.WriteLine($"movieDatabase Error occured. Status code: ${movieDatabaseError.status_code}\r\n" +
                        $"{movieDatabaseError.status_message}");
                    if (7 == movieDatabaseError.status_code) Console.WriteLine($"Invalid api_key: {OnlineDatabase.api_key_session}");
                    return null;
                }
                return new OnlineDatabase.MovieDatabaseSession((string)jObject.SelectToken("session_id"));
            }
        }
        static OnlineDatabase.MovieDatabaseSession CreateGuestSession()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(OnlineDatabase.api_key_session);
                Task<HttpResponseMessage> getguestsessionTask = httpClient.GetAsync($"/authentication/guest_session/new?api_key=" +
                    OnlineDatabase.api_key_session);
                getguestsessionTask.Wait();
                HttpResponseMessage httpResponse = getguestsessionTask.Result;
                if (!httpResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error occured. Please try again.");
                }
                Task<string> readStringTask = httpResponse.Content.ReadAsStringAsync();
                readStringTask.Wait();
                JObject jObject = JObject.Parse(readStringTask.Result);
                if (!(bool)jObject.SelectToken("success"))
                {
                    OnlineDatabase.MovieDatabaseError movieDatabaseError = JsonConvert.DeserializeObject
                        <OnlineDatabase.MovieDatabaseError>(readStringTask.Result);
                    Console.WriteLine($"movieDatabase Error occured. Status code: ${movieDatabaseError.status_code}\r\n" +
                        $"{movieDatabaseError.status_message}");
                    if (7 == movieDatabaseError.status_code) Console.WriteLine($"Invalid api_key: {OnlineDatabase.api_key_session}");
                    return null;
                }
                return new OnlineDatabase.MovieDatabaseGuestSession((string)jObject.SelectToken("session_id"), (string)jObject.SelectToken("expires_at"));
            }
        }
        static int PromptForMovieTitle()
        {
            Console.Write("Enter movie to query: ");
            string movietitle = Console.ReadLine();
            int movieid = GetMovieIdsFromMovieDatabaseAPI(movietitle);
            if (-1 == movieid)
            {
                Console.WriteLine($"Failed to find movie: {movietitle}");
            }
            return movieid;
        }

        static void GetMovieDetails()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            string result = GetFromMovieDatabase(@"/movie/" + movieid + @"?api_key=" + OnlineDatabase.api_key_session + @"&language=en-US");
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseMovieDetails movieDetails = JsonConvert.DeserializeObject<
                    OnlineDatabase.MovieDatabaseMovieDetails>(result);

                // do whatever here
            }
        }
        static void GetAlternativeMovieTitles()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            string result = GetFromMovieDatabase(@"/movie/" + movieid + @"/alternative_titles" + @"?api_key=" + OnlineDatabase.api_key_session);
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseAlternativeTitles movietitles = JsonConvert.DeserializeObject<
                    OnlineDatabase.MovieDatabaseAlternativeTitles>(result);

                // do whatever here
            }
        }
        static void GetMovieImages()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            string result = GetFromMovieDatabase(@"/movie/" + movieid + @"/images" + @"?api_key=" + OnlineDatabase.api_key_session +
                @"&language=en-US&include_image_language=null");
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseImages movieImages = JsonConvert.DeserializeObject<
                    OnlineDatabase.MovieDatabaseImages>(result);

                // do whatever here
            }
        }
        static void GetMovieRecommendations()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            Console.WriteLine("Enter page number to look at (1-1000): ");
            if (!int.TryParse(Console.ReadLine(), out int pagenumber) || pagenumber < 1 || pagenumber > 1000)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            string result = GetFromMovieDatabase(@"/movie/" + movieid + @"/recommendations" + @"?api_key=" + OnlineDatabase.api_key_session +
                @"&language=en-US&page=" + pagenumber);
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseReccommendations movieReccomendations = JsonConvert.DeserializeObject<
                    OnlineDatabase.MovieDatabaseReccommendations>(result);
                
                // do whatever here
            }
        }
        static void GetSimilarMovies()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            Console.WriteLine("Enter page number to look at (1-1000): ");
            if (!int.TryParse(Console.ReadLine(), out int pagenumber) || pagenumber < 1 || pagenumber > 1000)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            string result = GetFromMovieDatabase(@"/movie/" + movieid + @"/similar" + @"?api_key=" + OnlineDatabase.api_key_session +
                @"&language=en-US&page=" + pagenumber);
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseSimilarMovies similarMovies = JsonConvert.DeserializeObject<
                    OnlineDatabase.MovieDatabaseSimilarMovies>(result);
                
                // do whatever here
            }
        }
        static void GetMovieReleaseDates()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            string result = GetFromMovieDatabase(@"/movie/" + movieid + @"release_dates?api_key=" + OnlineDatabase.api_key_session);
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseReleaseDates movieReleaseDates = JsonConvert.DeserializeObject<
                    OnlineDatabase.MovieDatabaseReleaseDates>(result);

                // do whatever here
            }
        }
        static void GetMovieVideos()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            string result = GetFromMovieDatabase(@"/movie/" + movieid + @"/videos?api_key=" +
                OnlineDatabase.api_key_session + "&language=en-US");
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseVideos movieVideos = JsonConvert.DeserializeObject<
                    OnlineDatabase.MovieDatabaseVideos>(result);

                // do whatever here
            }
        }
        static void GetAccountDetails(string session_id)
        {
            string result = GetFromMovieDatabase(@"/account?api_key=" + OnlineDatabase.api_key_session +
                "&session_id=" + session_id);
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseAccountDetails accountDetails = JsonConvert.DeserializeObject<
                    OnlineDatabase.MovieDatabaseAccountDetails>(result);

                // do whatever here
            }
        }
        static void GuestGetRatedMovies(string session_id)
        {
            string inputResponse;
            Console.WriteLine("Do you want it to be sorted by ASC? [Y/N]: ");
            if (1 != (inputResponse = Console.ReadLine()).Length  && "y" != inputResponse && "Y" != inputResponse
                && "n" != inputResponse && "N" != inputResponse)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            string result = GetFromMovieDatabase(@"/guest_session/" + session_id + "/rated/movies?api_key=" +
                OnlineDatabase.api_key_session + @"&language=en-US&sort_by=created_at." +
                (("Y" == inputResponse || "y" == inputResponse) ? "asc" : "desc"));
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseGuestRatedMovies guestRatedMovies = JsonConvert.DeserializeObject<
                    OnlineDatabase.MovieDatabaseGuestRatedMovies>(result);

                // do whatever here
            }
        }
        static void LoadMovieData()
        {
            string[] strarray; string strline;
            DateTime? dateTime; DateTime dateTimeNotNull;

            using (StreamReader sr = new StreamReader("Movie.csv"))
            {
                sr.ReadLine();
                while (null != (strline = sr.ReadLine()))
                {
                    strarray = strline.Split(',');
                    if (DateTime.TryParse(strarray[4], out dateTimeNotNull))
                        dateTime = dateTimeNotNull;
                    else dateTime = null;

                    OfflineDatabase.MovieData.Add(new Movie(strarray[0], int.Parse(strarray[1]),
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
                    OfflineDatabase.CinemaData.Add(new Cinema(strarray[0], int.Parse(strarray[1]),
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
                while (null != (strline = sr.ReadLine()))
                {
                    strarray = strline.Split(',');
                    screening = new Screening(OfflineDatabase.gScreeningNo++, DateTime.Parse(strarray[0]),
                        strarray[1], OfflineDatabase.CinemaData.Find(x => strarray[2] == x.name &&
                        int.Parse(strarray[3]) == x.hallNo),
                        OfflineDatabase.MovieData.Find(x => strarray[4] == x.title));
                    screening.seatsRemaining = screening.cinema.capacity;
                    OfflineDatabase.ScreeningData.Add(screening);
                }
            }
        }
        static void InitializeApp()
        {
            OfflineDatabase.MovieData = new(); OfflineDatabase.CinemaData = new(); OfflineDatabase.ScreeningData = new(); 
            OfflineDatabase.OrderData = new();
            LoadMovieData(); LoadCinemaData(); LoadScreeningData();
            OfflineDatabase.MovieData.ForEach(delegate (Movie movie)
            {
                movie.screeningList = OfflineDatabase.ScreeningData.FindAll(x => movie == x.movie);
            });
        }
        static void ListMovieScreenings()
        {
            Console.WriteLine("\x1b[1m\x1b[104mTitle\t\t\tDuration\tClassification\tOpeningDate\t\tGenres\x1b[0m");
            OfflineDatabase.MovieData.ForEach(Print);
            Console.Write("\r\nSelect a movie to watch: ");
            string title = Console.ReadLine();
            Movie moviesearch = OfflineDatabase.MovieData.Find(x => title == x.title);
            if (null == moviesearch)
            {
                Console.WriteLine("\x1b[1m\x1b[31mFailed to find a movie with title: " + title + "\x1b[0m");
                return;
            }
            Console.WriteLine("\r\n\x1b[1m\x1b[104mScreening Number\tScreening Date Time\tScreeningType\t" +
                "Seats Remaining\tCinema\t\tHall Number\tCapacity\tTitle\x1b[0m");
            List<Screening> allmovieScreenings = OfflineDatabase.ScreeningData.FindAll(
                x => moviesearch == x.movie);
            allmovieScreenings.ForEach(Print);
        }
        static void AddMovieScreeningSession()
        {
            Console.WriteLine("\x1b[1m\x1b[104mTitle\t\t\tDuration\tClassification\tOpeningDate\t\tGenres\x1b[0m");
            OfflineDatabase.MovieData.ForEach(Print);
            Console.Write("\r\nSelect a movie: ");
            string title = Console.ReadLine();
            Movie movie = OfflineDatabase.MovieData.Find(x => title == x.title);
            if (null == movie)
            {
                Console.WriteLine("\x1b[1m\x1b[31mFailed to find to find Movie with title: " + title + "\x1b[0m");
                return;
            }
            Console.Write("Select a screening type: ");
            string screentype = Console.ReadLine();
            if ("2D" != screentype && "3D" != screentype)
            {
                Console.WriteLine("\x1b[1m\x1b[31mInvalid screening type!\x1b[0m"); return;
            }
            Console.Write("Enter screening date time: ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime screeningdateTime))
            {
                Console.WriteLine("\x1b[1m\x1b[31mFailed to provide a valid DateTime Format!\x1b[0m");
                return;
            }
            Console.Write("\r\n");
            Console.WriteLine("\x1b[1m\x1b[104mNo.\tName\t\tHall No.\tCapacity\x1b[0m");
            int i = 0;
            OfflineDatabase.CinemaData.ForEach(delegate (Cinema cinema)
            {
                Console.Write(++i + ".\t");
                Print(cinema);
            });

            Console.Write("\r\nSelect Cinema Hall No.: ");
            if (!int.TryParse(Console.ReadLine(), out int index) ||
                index > OfflineDatabase.CinemaData.Count || index < 1)
            {
                Console.WriteLine("\x1b[0m\x1b[31mInvalid Input!\x1b[0m"); return;
            }
            Cinema chosencinema = OfflineDatabase.CinemaData[index-1];
            bool isAvailable = true;
            OfflineDatabase.ScreeningData.ForEach(delegate (Screening screening)
            {
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
                Console.WriteLine("\x1b[1m\x1b[31mCinema hall is currently screening at that time!\x1b[0m");
                return;
            }
            OfflineDatabase.ScreeningData.Add(new Screening(OfflineDatabase.gScreeningNo++, screeningdateTime, screentype,
                chosencinema, movie));
            Console.WriteLine("\x1b[1m\x1b[32mMovie Screening creation successful!\x1b[0m");
        }
        static void DeleteMovieScreeningSession()
        {
            OfflineDatabase.ScreeningData.ForEach(Print);
            Console.Write("\r\nSelect Session to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1001
                || index > OfflineDatabase.gScreeningNo)
            {
                Console.WriteLine("\x1b[1m\x1b[31mInvalid Input!\x1b[0m"); return;
            }
            int screeningindex = OfflineDatabase.ScreeningData.FindIndex(x => index == x.screeningNo);
            if (-1 == screeningindex)
            {
                Console.WriteLine("\x1b[1m\x1b[31mScreening doesn't exist!\x1b[0m"); return;
            }
            OfflineDatabase.ScreeningData.RemoveAt(screeningindex);
            Console.WriteLine("Movie Screening removal successful!");
        }
        static void OrderMovieTicket()
        {
            int index;
            OfflineDatabase.MovieData.ForEach(Print);
            Console.Write("\r\nSelect a Movie: ");
            if (!int.TryParse(Console.ReadLine(), out index)
                || index < 1 || index > OfflineDatabase.MovieData.Count)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            List<Screening> filteredscreening = OfflineDatabase.ScreeningData.FindAll(x => 
                OfflineDatabase.MovieData[index -1] == x.movie);
            filteredscreening.ForEach(Print);
            Console.Write("\r\nSelect Movie Screening: ");
            if (!int.TryParse(Console.ReadLine(), out index)
                || index < 1 || index > OfflineDatabase.MovieData.Count)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            Screening chosenscreening = filteredscreening[index-1];
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
            Order newOrder = new Order(OfflineDatabase.gOrderNo++, DateTime.Now);
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
                        genericTicket = new Adult(chosenscreening, ('y' == yn[0] || 'Y' == yn[0]) ? true : false);
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
                newOrder.AddTicket(genericTicket);
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
            Order ordersearch = OfflineDatabase.OrderData.Find(x => ordernumber == x.orderNo);
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

        static int GetMovieIdsFromMovieDatabaseAPI(string movietitle)
        {
            string datafilename = "movie_ids_" + DateTime.Now.AddDays(-1).ToString("MM_dd_yyyy") + ".json";
            string downloadlink = "http://files.tmdb.org/p/exports/" + datafilename + ".gz";

            if (!File.Exists(datafilename))
            {
                using (WebClient wc = new WebClient())
                {
                    datafilename += ".gz";
                    wc.DownloadFile(downloadlink, datafilename);
                    using (FileStream ofs = new FileStream(datafilename, FileMode.Open))
                    {
                        datafilename = datafilename.Remove(datafilename.Length - 3);
                        using (FileStream decompfs = File.Create(datafilename))
                        {
                            using (GZipStream decompStream = new GZipStream(ofs, CompressionMode.Decompress))
                            {
                                decompStream.CopyTo(decompfs);
                            }
                        }
                    }
                    File.Delete(datafilename + ".gz");
                }
            }
            MovieDatabaseObject MovieDatabaseObj = null;
            using (StreamReader sr = new StreamReader(datafilename))
            {
                string stringl;
                while (null != (stringl = sr.ReadLine()))
                {
                    MovieDatabaseObj = JsonConvert.DeserializeObject<MovieDatabaseObject>(stringl);
                    if (movietitle == MovieDatabaseObj.original_title) break;
                    MovieDatabaseObj = null;
                }
            }
            if (null != MovieDatabaseObj) return MovieDatabaseObj.id;
            return -1;
        }

    }
}
