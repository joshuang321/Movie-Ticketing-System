//============================================================
// Student Number : S10205140B
// Student Name : Joshua Ng
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net.Http;

/*          ------------------------------------------------------------\
            *                                                           \
            *                                                           \
            * List of Functions to implement further and test later on  \
            * --------------------------------------------------------- \
            * OfflineDatabaseInterface()
            * OnlineDatabaseInterface()
            * GetMovieFromDatabase(testrequestUrl)
            * CreateLoginSession()
            * CreateGuestSession()
            * PromptForMovieTitle()
            * GetMovieDetails()
            * GetAlternativeMovieTitle
            * GetMovieImages()
            * GteMovieReccomendations()
            * GetSimilarMovies()
            * GetMovieReleaseDates()
            * GetMovieVideos()
            * GetAccountDetails(test_session_id)
            * GuestGetRatedMovies(test_guest_session_id)
*/

namespace Movie_Ticketing_System
{
    class Program
    {
        public static string[] protected_filenames { get; set; }
        public static bool init_onlinedatabase { get; set; }
        public static bool init_offlinedatabase { get; set; }

        static void Main(string[] args)
        {
            protected_filenames = Directory.GetFiles(Directory.GetCurrentDirectory());
            
            while (true) 
            {
                Console.WriteLine("Movie Database System\r\n---------------------\r\n1. Offline Movie Database\r\n" +
                                  "2. Online Movie Database via The Movie Database (TMDB)\r\n3. Exit");
                if (!ValidateOptionInput(3, out int options)) continue;
                if (1 == options) OfflineDatabaseInterface();
                else if (2 == options) OnlineDatabaseInterface();
                else break;
            }
            

            CleanUpProgram();
        }
        static bool ValidateOptionInput(int maxOption, out int option, int minOption = 1)
        {
            Console.Write("Your option: ");
            if(!int.TryParse(Console.ReadLine(), out option) || option < minOption || option > maxOption)
            {
                Console.WriteLine("\x1b[0m\x1b[31mInvalid Option!\x1b[0m"); return false;
            }
            Console.Clear();
            return true;
        }
        static void CleanUpProgram()
        {
            int j = 0; string[] currentDir = Directory.GetFiles(Directory.GetCurrentDirectory());
            for (int i = 0; i < currentDir.Length; i++)
            {
                for (; j < protected_filenames.Length; j++)
                {
                    if (protected_filenames[j] == currentDir[i])
                        break;
                }
                if (protected_filenames.Length == j)
                    File.Delete(currentDir[i]);
                j = 0;
            }
        }
        static void InitializeOffDatabase()
        {
            OfflineDatabase.MovieData = new List<Movie>();
            OfflineDatabase.CinemaData = new List<Cinema>();
            OfflineDatabase.ScreeningData = new List<Screening>();
            OfflineDatabase.OrderData = new List<Order>();

            LoadMovieData();
            LoadCinemaData();
            LoadScreeningData();
            OfflineDatabase.MovieData.ForEach(delegate (Movie movie)
            {
                movie.screeningList = OfflineDatabase.ScreeningData.FindAll(x => movie == x.movie);
            });
            init_offlinedatabase = true;
        }
        static void InitializeOnlineDatabase()
        {
            OnlineDatabase.httpClientSession = new HttpClient();
            OnlineDatabase.httpClientSession.BaseAddress = new Uri(OnlineDatabase.onlinemoviedb_baselink);
            OnlineDatabase.fileIE = new List<OnlineDatabase.FileInternetExplorer>();
            Console.Write("Input API key to use: ");
            OnlineDatabase.api_key_session = Console.ReadLine();
            OnlineDatabase.GetMovieListDetailsFromMDAPI();
            OnlineDatabase.CacheGenres();
            init_onlinedatabase = true;
            Console.Clear();
        }
        static void OfflineDatabaseInterface()
        {
            Console.Write("Offline Database System\r\n-----------------------\r\n1. List all movie Screenings\r\n2. Add a movie Screening\r\n" +
                          "3. Remove a movie Screening\r\n4. Order a movie Ticket\r\n5. Cancel a movie Ticket\r\n");
            if (!ValidateOptionInput(5, out int options)) return;
            if (!init_offlinedatabase) InitializeOffDatabase();
            switch (options)
            {
                case 1: ListMovieScreenings(); break;
                case 2: AddMovieScreeningSession(); break;
                case 3: DeleteMovieScreeningSession(); break;
                case 4: OrderMovieTicket(); break;
                case 5: CancelTicketOrder(); break;
                case 6: ReccommendMovies(); break;
                case 7: DisplayScreeningSeatsDesc(); break;
            }
        }
        static void OnlineDatabaseInterface()
        {
            Console.WriteLine("Online Movie Database through TMDB\r\n----------------------------------\r\n" +
                "1. Create a new Login Session\r\n2. Create a new Guest Session\r\n");
            if (!ValidateOptionInput(2, out int options)) return;
            if (!init_onlinedatabase) InitializeOnlineDatabase();
            OnlineDatabase.MovieDatabaseSession currentSession;
            if (1 == options) currentSession = CreateLoginSession();
            else currentSession = CreateGuestSession();

            if (null == currentSession) return;

            if (currentSession is OnlineDatabase.MovieDatabaseSession)
            {
                Console.WriteLine("What do you want to do?\r\n-----------------------\r\n1.  Get Movie Details\r\n" +
                    "2.  Get Alternative Movie Titles\r\n3.  Get Movie Images\r\n4.  Get Movie " +
                    "Recommendations\r\n5.  Get Movie Release Dates\r\n6.  Get Movie Videos" +
                    "\r\n7. Get Account Details\r\n");

                if (!ValidateOptionInput(7, out options)) return;
                switch (options)
                {
                    case 1: GetMovieDetails(); break;
                    case 2: GetAlternativeMovieTitles(); break;
                    case 3: GetMovieImages(); break;
                    case 4: GetMovieRecommendations(); break;
                    case 5: GetMovieReleaseDates(); break;
                    case 6: GetMovieVideos(); break;
                    case 7: GetAccountDetails(currentSession.session_id); break;
                }
            }
            else
            {
                Console.WriteLine("What do you want to do?\r\n-----------------------\r\n1. Get Rated Movies\r\n");
                if (!int.TryParse(Console.ReadLine(), out options) || 1 != options)
                {
                    Console.WriteLine("Invalid Input!"); currentSession.ExitSession();
                    return;
                }
                GuestGetRatedMovies(currentSession.session_id);
            }
            currentSession.ExitSession();
        }
        static OnlineDatabase.MovieDatabaseSession CreateLoginSession()
        {
            Console.Write("Enter username: "); string username = Console.ReadLine();
            Console.Write("Enter password: "); string password = Console.ReadLine();

            JObject result = OnlineDatabase.GetFromMovieDatabase(@"3/authentication/token/new?api_key=" +
                OnlineDatabase.api_key_session);
            OnlineDatabase.MovieDatabaseRequestToken request_token = new OnlineDatabase.MovieDatabaseRequestToken(
                (string)result.SelectToken("expires_at"), (string)result.SelectToken("request_token"));

            OnlineDatabase.MovieDatabaseAccountLogin accountLogin = new OnlineDatabase.MovieDatabaseAccountLogin(
                    username, password, request_token.request_token);

            result = OnlineDatabase.SendHttpClientMessage(OnlineDatabase.httpClientSession, @"3/authentication/token/validate_with_login?api_key=" +
                OnlineDatabase.api_key_session, HttpMethod.Post, JObject.FromObject(accountLogin));
            if (null != result) return new OnlineDatabase.MovieDatabaseSession(OnlineDatabase.GetValidatedSession((string)result.SelectToken("request_token")));
            return null;
        }
        static OnlineDatabase.MovieDatabaseSession CreateGuestSession()
        {
            JObject result = OnlineDatabase.GetFromMovieDatabase($"3/authentication/guest_session/new?api_key=" +
            OnlineDatabase.api_key_session);
            if (null != result) return new OnlineDatabase.MovieDatabaseGuestSession((string)result.SelectToken("guest_session_id"),
                (string)result.SelectToken("expires_at"));
            return null;
        }
        static int PromptForMovieTitle()
        {
            Console.Write("Enter movie to query: ");
            string movietitle = Console.ReadLine();
            int movieid = OnlineDatabase.GetMovieIdFromMovieTitle(movietitle);
            if (-1 == movieid) Console.WriteLine($"Failed to find movie: {movietitle}");

            return movieid;
        }

        static void GetMovieDetails()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            JObject result = OnlineDatabase.GetFromMovieDatabase(@"3/movie/" + movieid + @"?api_key=" +
                OnlineDatabase.api_key_session + @"&language=en-US");
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseMovieDetails movieDetails = result.ToObject<
                    OnlineDatabase.MovieDatabaseMovieDetails>();
                if (null != movieDetails)
                {
                    if (null != movieDetails.belongs_to_collection) GetCollectionDetails(movieDetails.belongs_to_collection.id);
                    using (StreamWriter sw = new StreamWriter("movie_details_" + movieid + ".html", false))
                    {
                        sw.Write(OnlineDatabase.htmlbody_start);
                        sw.Write(@"<h1 class=""h1 text-center"">" + movieDetails.title + @"</h1><div class=""text-center""><img src=""");
                        sw.Write(OnlineDatabase.tmdb_imagebase_link + movieDetails.backdrop_path);
                        sw.Write(@"""></div><dl class=""row h3"">");

                        PrintDescription(sw, "Overview", movieDetails.overview);
                        PrintDescription(sw, "Genres", string.Join(", ", (object[])movieDetails.genres));
                        PrintDescription(sw, "Runtime", movieDetails.runtime.ToString() + " Minutes");
                        PrintDescription(sw, "Status", movieDetails.status);
                        PrintDescription(sw, "Age Restriction", movieDetails.adult ? "Adult" : "For All Ages");
                        PrintDescription(sw, "Popularity", movieDetails.popularity.ToString());
                        PrintDescription(sw, "Vote Average", movieDetails.vote_average.ToString());
                        PrintDescription(sw, "Vote Count", movieDetails.vote_count.ToString());

                        sw.Write(@"<a class=""h2 link-primary"" href=""" + ((null != movieDetails.belongs_to_collection) ? 
                            (movieDetails.belongs_to_collection.id + ".html") : "#") + @""">See collections here</h2>");

                        sw.Write(@"</dt>");
                        sw.Write(OnlineDatabase.htmlbody_end);
                        sw.Flush();
                    }
                    if (null != movieDetails.belongs_to_collection) OnlineDatabase.OpenHTMLonInternetExplorer("movie_details_" + movieid + ".html",
                        movieDetails.belongs_to_collection.id + ".html");
                    else OnlineDatabase.OpenHTMLonInternetExplorer("movie_details_" + movieid + ".html");
                }
            }
        }
        static void GetCollectionDetails(int collection_id)
        {
            JObject result = OnlineDatabase.GetFromMovieDatabase(@"3/collection/" + collection_id +
                "?api_key=" + OnlineDatabase.api_key_session + @"&language=en-US");

            if (null != result)
            {
                OnlineDatabase.MovieDatabaseCollections movieCollections = result.ToObject<
                        OnlineDatabase.MovieDatabaseCollections>();
                using (StreamWriter sw = new StreamWriter(collection_id.ToString() + ".html", false))
                {
                    sw.Write(OnlineDatabase.htmlbody_start);
                    sw.Write(OnlineDatabase.carousel_start);

                    sw.Write(OnlineDatabase.carouselfirstslide_start);
                    sw.Write(@"""" + OnlineDatabase.tmdb_imagebase_link + movieCollections.backdrop_path + @"""");
                    sw.Write(OnlineDatabase.carouselfirstslide_end);

                    foreach (OnlineDatabase.MovieDatabaseCollections.Part part in movieCollections.parts)
                    {
                        if (null != part.backdrop_path)
                        {
                            sw.Write(OnlineDatabase.carouselslide_start);
                            sw.Write(@"""" + OnlineDatabase.tmdb_imagebase_link + part.backdrop_path + @"""");
                            sw.Write(OnlineDatabase.carouselslide_end);
                        }
                    }
                    sw.Write(OnlineDatabase.carousel_end);
                    sw.Write(OnlineDatabase.htmlbody_end);
                }
            }
        }
        static void PrintDescription(StreamWriter sw, string dt, string dd)
        {
            sw.Write(@"<dt class=""col-sm-3"">");
            sw.Write(dt);
            sw.Write(@"</dt><dd class=""col-sm-9"">");
            sw.Write(dd);
            sw.Write("</dd>");
        }
        static void GetAlternativeMovieTitles()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            JObject result = OnlineDatabase.GetFromMovieDatabase(@"3/movie/" + movieid + @"/alternative_titles" + @"?api_key=" +
                OnlineDatabase.api_key_session);
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseAlternativeTitles movietitles = result.ToObject<
                    OnlineDatabase.MovieDatabaseAlternativeTitles>();
                if (0 == movietitles.titles.Length)
                {
                    Console.WriteLine("The queried movie has no alternative titles!");
                    return;
                }
                Console.WriteLine("Alternative Titles: ");
                Array.ForEach(movietitles.titles, Console.WriteLine);
            }
        }
        static void GetMovieImages()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            JObject result = OnlineDatabase.GetFromMovieDatabase(@"3/movie/" + movieid + @"/images" + @"?api_key=" + OnlineDatabase.api_key_session +
                @"&language=en-US&include_image_language=null");
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseImages movieImages = result.ToObject<
                    OnlineDatabase.MovieDatabaseImages>();

                CreateHTMLFileFromStringPaths("backdrop_" + movieid + ".html", movieImages.backdrops);
                CreateHTMLFileFromStringPaths("posters_" + movieid + ".html", movieImages.posters);
                using (StreamWriter sw = new StreamWriter("movieimages_" + movieid + ".html", false))
                {
                    sw.Write(OnlineDatabase.htmlbody_start);
                    sw.Write(@"< dl class=""row h3"">");

                    PrintDescription(sw, "Backdrops", "backdrop_" + movieid + ".html");
                    PrintDescription(sw, "Posters", "posters_" + movieid + ".html");

                    sw.Write(@"</dt>" + OnlineDatabase.htmlbody_end);
                }
                OnlineDatabase.OpenHTMLonInternetExplorer("movieimages_" + movieid + ".html");
            }
        }
        static void CreateHTMLFileFromStringPaths(string filename, object[] image_paths,
            string first_imagepath = null)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                sw.Write(OnlineDatabase.htmlbody_start);
                sw.Write(OnlineDatabase.carousel_start);
                sw.Write(OnlineDatabase.carouselslide_start);
                sw.Write(@"""" + OnlineDatabase.tmdb_imagebase_link + ((null != first_imagepath) ? first_imagepath :
                    image_paths[0]) + @"""");
                sw.Write(OnlineDatabase.carouselfirstslide_end);
                for (int i = (null != first_imagepath) ? 1 : 0; i < image_paths.Length; i++)
                {
                    sw.Write(OnlineDatabase.carouselslide_start);
                    sw.Write(@"""" + OnlineDatabase.tmdb_imagebase_link + image_paths[i] + @"""");
                    sw.Write(OnlineDatabase.carouselslide_end);
                }
                sw.Write(OnlineDatabase.carousel_end);
                sw.Write(OnlineDatabase.htmlbody_end);
            }
        }
        static void GetMovieRecommendations()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            Console.Write("Enter page number to look at (1-1000): ");
            if (!int.TryParse(Console.ReadLine(), out int pagenumber) || pagenumber < 1 || pagenumber > 1000)
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            JObject result = OnlineDatabase.GetFromMovieDatabase(@"3/movie/" + movieid + @"/recommendations" + @"?api_key=" +
                OnlineDatabase.api_key_session + @"&language=en-US&page=" + pagenumber);
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseReccommendations movieReccomendations = result.ToObject<
                    OnlineDatabase.MovieDatabaseReccommendations>();
                if (0 == movieReccomendations.total_results)
                {
                    Console.WriteLine("No results found for this movie! Please try another.");
                    return;
                }
                else if (0 == movieReccomendations.results.Length)
                {
                    Console.WriteLine("No results for this page. Try page 1 - " + movieReccomendations.total_pages + ".");
                    return;
                }
                using (StreamWriter sw = new StreamWriter("movie_recommendations_" + movieid + ".html", false))
                {
                    sw.Write(OnlineDatabase.htmlbody_start);
                    foreach (OnlineDatabase.Result movieresult in movieReccomendations.results)
                    {
                        sw.Write(@"<h1 class=""h1 text-center"">" + movieresult.title + @"</h1><div class=""text-center""><img src=""");
                        sw.Write(OnlineDatabase.tmdb_imagebase_link + movieresult.backdrop_path);
                        sw.Write(@"""></div><dl class=""row h3"">");

                        PrintDescription(sw, "Genres", string.Join(", ", (object[])OnlineDatabase.FindGenresFromId(movieresult.genre_ids)));
                        PrintDescription(sw, "Age Restriction", movieresult.adult ? "Adult" : "For All Ages");
                        PrintDescription(sw, "Popularity", movieresult.popularity.ToString());
                        PrintDescription(sw, "Vote Average", movieresult.vote_average.ToString());
                        PrintDescription(sw, "Vote Count", movieresult.vote_count.ToString());

                        sw.Write(@"</dt>");
                    }
                    sw.Write(@"</dt>" + OnlineDatabase.htmlbody_end);
                    sw.Flush();
                }
                OnlineDatabase.OpenHTMLonInternetExplorer("movie_recommendations_" + movieid + ".html");
            }
        }
        static void GetMovieReleaseDates()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            JObject result = OnlineDatabase.GetFromMovieDatabase(@"3/movie/" + movieid + @"/release_dates?api_key=" +
                OnlineDatabase.api_key_session);
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseReleaseDates movieReleaseDates = result.ToObject<
                    OnlineDatabase.MovieDatabaseReleaseDates>();

                Console.WriteLine("Releases:\r\n---------");
                foreach (OnlineDatabase.MovieDatabaseReleaseDates.Result releaseResult in movieReleaseDates.results)
                {
                    Console.WriteLine($"Country: {releaseResult.iso_3166_1}\r\n-----------");
                    foreach (OnlineDatabase.MovieDatabaseReleaseDates.ReleaseDate releaseDate in releaseResult.release_dates)
                    {
                        Console.WriteLine($"Release Date: {releaseDate.release_date}\r\nType: {releaseDate.type}\r\n" +
                            $"Note: {releaseDate.note}");
                    }
                }    
            }
        }
        static void GetMovieVideos()
        {
            int movieid = PromptForMovieTitle();
            if (-1 == movieid) return;
            JObject result = OnlineDatabase.GetFromMovieDatabase(@"3/movie/" + movieid + @"/videos?api_key=" +
                OnlineDatabase.api_key_session + "&language=en-US");
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseVideos movieVideos = result.ToObject<
                    OnlineDatabase.MovieDatabaseVideos>();

                using (StreamWriter sw = new StreamWriter("movie_videos_" + movieid + ".html", false))
                {
                    sw.Write(OnlineDatabase.htmlbody_start);
                    foreach (OnlineDatabase.MovieDatabaseVideos.Result movieResult in movieVideos.results)
                    {
                        sw.Write(@"<h1 class=""h1 text-center"">" + movieResult.name + @"</h1>");
                        sw.Write(OnlineDatabase.youtube_embed_start);
                        sw.Write(movieResult.key);
                        sw.Write(OnlineDatabase.youtube_embed_end);
                    }
                    sw.Write(OnlineDatabase.htmlbody_end);
                }
                OnlineDatabase.OpenHTMLonInternetExplorer("movie_videos_" + movieid + ".html");
            }
        }
        static void GetAccountDetails(string session_id)
        {
            JObject result = OnlineDatabase.GetFromMovieDatabase(@"3/account?api_key=" + OnlineDatabase.api_key_session +
                "&session_id=" + session_id);
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseAccountDetails accountDetails = result.ToObject<
                    OnlineDatabase.MovieDatabaseAccountDetails>();

                using (StreamWriter sw = new StreamWriter("account_details_" + session_id + ".html", false))
                {
                    sw.Write(OnlineDatabase.htmlbody_start);
                    sw.Write(@"<h1 class=""h1 text-center"">" + accountDetails.name + @"</h1><div class=""text-center""><img src=""");
                    sw.Write(GetImageLinkFromAvatar(accountDetails.avatar));
                    sw.Write(@"""></div><dl class=""row h3"">");
                }
                OnlineDatabase.OpenHTMLonInternetExplorer("account_details_" + session_id + ".html");
            }
        }
        static string GetImageLinkFromAvatar(OnlineDatabase.MovieDatabaseAccountDetails.Avatar avatar)
        {
            return (OnlineDatabase.default_gravatar_hash == avatar.gravatar.hash) ? OnlineDatabase.tmdb_imagebase_link + avatar.tmdb.avatar_path :
                OnlineDatabase.gravatar_imagebase_link + avatar.gravatar.hash;
        }
        static void GuestGetRatedMovies(string session_id)
        {
            string inputResponse;
            Console.WriteLine("Do you want it to be sorted by ASC? [Y/N]: ");
            if (1 != (inputResponse = Console.ReadLine()).Length || ("y" != inputResponse && "Y" != inputResponse
                && "n" != inputResponse && "N" != inputResponse))
            {
                Console.WriteLine("Invalid Input!"); return;
            }
            JObject result = OnlineDatabase.GetFromMovieDatabase(@"3/guest_session/" + session_id + "/rated/movies?api_key=" +
                OnlineDatabase.api_key_session + @"&language=en-US&sort_by=created_at." +
                (("Y" == inputResponse || "y" == inputResponse) ? "asc" : "desc"));
            if (null != result)
            {
                OnlineDatabase.MovieDatabaseGuestRatedMovies guestRatedMovies = result.ToObject<
                    OnlineDatabase.MovieDatabaseGuestRatedMovies>();

                if (0 == guestRatedMovies.total_results)
                {
                    Console.WriteLine("No results found for this movie! Please try another.");
                    return;
                }
                else if (0 == guestRatedMovies.results.Length)
                {
                    Console.WriteLine("No results for this page. Try page 1 - " + guestRatedMovies.total_pages + ".");
                    return;
                }
                using (StreamWriter sw = new StreamWriter("movie_recommendations_" + session_id + ".html", false))
                {
                    sw.Write(OnlineDatabase.htmlbody_start);
                    foreach (OnlineDatabase.MovieDatabaseGuestRatedMovies.Result movieresult in guestRatedMovies.results)
                    {
                        sw.Write(@"<h1 class=""h1 text-center"">" + movieresult.title + @"</h1><div class=""text-center""><img src=""");
                        sw.Write(OnlineDatabase.tmdb_imagebase_link + movieresult.backdrop_path);
                        sw.Write(@"""></div><dl class=""row h3"">");

                        PrintDescription(sw, "Genres", string.Join(", ", (object[])OnlineDatabase.FindGenresFromId(movieresult.genre_ids)));
                        PrintDescription(sw, "Age Restriction", movieresult.adult ? "Adult" : "For All Ages");
                        PrintDescription(sw, "Popularity", movieresult.popularity.ToString());
                        PrintDescription(sw, "Vote Average", movieresult.vote_average.ToString());
                        PrintDescription(sw, "Vote Count", movieresult.vote_count.ToString());

                        sw.Write(@"</dt>");
                    }
                    sw.Write(@"</dt>" + OnlineDatabase.htmlbody_end);
                }
                OnlineDatabase.OpenHTMLonInternetExplorer("movie_recommendations_" + session_id + ".html");
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
        static void ListMovieScreenings()
        {
            Console.WriteLine("\x1b[1m\x1b[104mTitle\t\t\tDuration\tClassification\tOpeningDate\t\tGenres\x1b[0m");
            OfflineDatabase.MovieData.ForEach(Console.WriteLine);
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
            
            moviesearch.screeningList.ForEach(Console.WriteLine);
        }
        static void AddMovieScreeningSession()
        {
            Console.WriteLine("\x1b[1m\x1b[104mTitle\t\t\tDuration\tClassification\tOpeningDate\t\tGenres\x1b[0m");
            OfflineDatabase.MovieData.ForEach(Console.WriteLine);
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
                Console.WriteLine(cinema);
            });

            Console.Write("\r\nSelect Cinema Hall No.: ");
            if (!ValidateOptionInput(OfflineDatabase.CinemaData.Count, out int index)) return;
            Cinema chosencinema = OfflineDatabase.CinemaData[index - 1];
            bool isAvailable = null == OfflineDatabase.ScreeningData.FindAll(x => chosencinema == x.cinema).Find(x => (screeningdateTime <= x.screeningDateTime.AddMinutes(movie.duration + 15)
                 && screeningdateTime >= x.screeningDateTime) || (screeningdateTime.AddMinutes(movie.duration + 15) <= x.screeningDateTime.AddMinutes(movie.duration + 15)
                 && screeningdateTime.AddMinutes(movie.duration + 15) >= x.screeningDateTime));
            if (!isAvailable)
            {
                Console.WriteLine("\x1b[1m\x1b[31mCinema hall is currently screening at that time!\x1b[0m");
                return;
            }
            Screening newScreening = new Screening(OfflineDatabase.gScreeningNo++, screeningdateTime, screentype,
                chosencinema, movie);
            OfflineDatabase.ScreeningData.Add(newScreening);
            movie.AddScreening(newScreening);
            Console.WriteLine("\x1b[1m\x1b[32mMovie Screening creation successful!\x1b[0m");
        }
        static void DeleteMovieScreeningSession()
        {
            Console.WriteLine("Screenings\r\n----------");
            OfflineDatabase.ScreeningData.ForEach(Console.WriteLine);
            Console.Write("\r\nSelect Session to cancel (session ID): ");
            if (!ValidateOptionInput(OfflineDatabase.gScreeningNo, out int index, 1001)) return;
            int screeningindex = OfflineDatabase.ScreeningData.FindIndex(x => index == x.screeningNo);
            if (-1 == screeningindex)
            {
                Console.WriteLine("\x1b[1m\x1b[31mScreening doesn't exist!\x1b[0m"); return;
            }
            OfflineDatabase.ScreeningData[screeningindex].movie.screeningList.Remove(OfflineDatabase.ScreeningData[screeningindex]);
            OfflineDatabase.ScreeningData.RemoveAt(screeningindex);
            Console.WriteLine("\x1b[1m\x1b[32mMovie Screening removal successful!\x1b[0m");
        }
        static void OrderMovieTicket()
        {
            Console.WriteLine("\x1b[1m\x1b[104mTitle\t\t\tDuration\tClassification\tOpeningDate\t\tGenres\x1b[0m");
            OfflineDatabase.MovieData.ForEach(Console.WriteLine);
            Console.Write("\r\nSelect a Movie: ");
            string movietitle = Console.ReadLine();
            Movie movieSearch = OfflineDatabase.MovieData.Find(x => movietitle == x.title);
            List<Screening> filteredscreening = OfflineDatabase.ScreeningData.FindAll(x => movieSearch == x.movie);
            filteredscreening.ForEach(Console.WriteLine);
            Console.Write("\r\nSelect Session (session ID): ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index > OfflineDatabase.gScreeningNo)
            {
                Console.WriteLine("\x1b[0m\x1b[31mInvalid Option!\x1b[0m"); return;
            }
            Screening chosenscreening = filteredscreening.Find(x=> index == x.screeningNo);
            if (null == chosenscreening)
            {
                Console.WriteLine("\x1b[1m\x1b[31mScreening doesn't exist!\x1b[0m"); return;
            }
            Console.Write("Number of Tickets to order: ");
            if (!int.TryParse(Console.ReadLine(), out index) || index < 0)
            {
                Console.WriteLine("\x1b[0m\x1b[31mInvalid Option!\x1b[0m"); return;
            }
            if (index > chosenscreening.seatsRemaining)
            {
                Console.WriteLine("\x1b[0m\x1b[31mNot enough seats of the chosen session!\x1b[0m");
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
                        break;

                    case "Senior Citizen":
                        Console.Write("Please state your year of birth: ");
                        if (!int.TryParse(Console.ReadLine(), out int yob))
                        {
                            Console.WriteLine("Failed to provide a valid year of birth!");
                            return;
                        }
                        genericTicket = new SeniorCitizen(chosenscreening, yob);
                        break;

                    case "Adult":
                        Console.Write("Would you like popcorn for an extra $3? [Y/N]: ");
                        string response = Console.ReadLine();
                        if (1 != response.Length || ("y" != response && "Y" != response
                            && "n" != response && "N" != response))
                        {
                            Console.WriteLine("Invalid Input!"); break;
                        }
                        genericTicket = new Adult(chosenscreening, ("y" == response || "Y" == response)
                            ? true : false);
                        break;
                }
                newOrder.AddTicket(genericTicket);
            }
            Console.WriteLine("Amount payable: {0:C2}\r\nPRESS ANY KEY TO MAKE PAYMENT",
                newOrder.amount);
            Console.ReadKey();
            chosenscreening.seatsRemaining -= index;
            newOrder.status = "Paid";
            OfflineDatabase.OrderData.Add(newOrder);
        }
        static void CancelTicketOrder()
        {
            Console.WriteLine("OrderNo\tOrder Date\t\tOrder Price\tOrder Status");
            OfflineDatabase.OrderData.ForEach(Console.WriteLine);
            Console.Write("\r\nPlease state order number to be cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int ordernumber))
            {
                Console.WriteLine("\x1b[0m\x1b[31mInvalid Option!\x1b[0m"); return;
            }
            Order ordersearch = OfflineDatabase.OrderData.Find(x => ordernumber == x.orderNo);
            if (null == ordersearch)
            {
                Console.WriteLine("Failed to find Order of Order No." + ordernumber + "\r\n\x1b[0m\x1b[31mOrder Cancellation Unsuccessful!\x1b[0m");
                return;
            }
            if ("Paid" != ordersearch.status)
            {
                Console.WriteLine("\x1b[0m\x1b[31mCannot cancel a unpaid or cancelled Order!\x1b[0m"); return;
            }
            ordersearch.ticketList[0].screening.seatsRemaining += ordersearch.ticketList.Count;
            ordersearch.status = "Cancelled";
            Console.WriteLine("\x1b[1m\x1b[32mAmount refunded: {0:C2}\r\nCancellation Successful!\x1b[0m", ordersearch.amount);
        }
        static void ReccommendMovies()
        {
            List<OfflineDatabase.NumMovies> ReccomendedMovies = new List<OfflineDatabase.NumMovies>();
            int movieIndex = 0;
            foreach (Order order in OfflineDatabase.OrderData)
            {
                foreach (Ticket ticket in order.ticketList)
                {
                    if (-1 == (movieIndex = ReccomendedMovies.FindIndex(x => ticket.screening.movie == x.movie)))
                    {
                        ReccomendedMovies.Add(new OfflineDatabase.NumMovies(ticket.screening.movie, 1));
                        continue;
                    }
                    ReccomendedMovies[movieIndex].num++;
                }
            }
            if (0 == ReccomendedMovies.Count)
            {
                Console.WriteLine("No movies to be reccomended! Come back another time!");
                return;
            }
            ReccomendedMovies.Sort();
            Console.WriteLine("Recommended Movies to watch based on Popularity\r\n-----------------------------------------------");
            for (int i=0; i<ReccomendedMovies.Count; i++)
            {
                Console.WriteLine((i+1) + ". " + ReccomendedMovies[i].movie.title);
            }
        }
        static void DisplayScreeningSeatsDesc()
        {
            Screening[] screeningCp = new Screening[OfflineDatabase.ScreeningData.Count];
            Array.Copy(OfflineDatabase.ScreeningData.ToArray(), screeningCp, OfflineDatabase.ScreeningData.Count);
            Array.Sort(screeningCp);
            Console.WriteLine("Available Seats for each screening\r\n----------------------------------");
            for (int i =0; i<screeningCp.Length; i++)
            {
                Console.WriteLine((i + 1) + "." + screeningCp[i].screeningNo + " - " +
                    screeningCp[i].seatsRemaining);
            }
        }
    }
}