//============================================================
// Student Number : S10205140B
// Student Name : Joshua Ng
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Movie_Ticketing_System
{
    class OnlineDatabase
    {
        public class MovieDatabaseObject
        {
            public bool adult { get; set; }
            public int id { get; set; }
            public string original_title { get; set; }
            public float popularity { get; set; }
            public bool video { get; set; }
        }
        public class MovieDatabaseError
        {
            public string status_message { get; set; }
            public int status_code { get; set; }

            public MovieDatabaseError(string Status_message, int Status_code)
            {
                status_message = Status_message; status_code = Status_code;
            }
        }
        public class MovieDatabaseRequestToken
        {
            public string expires_at { get; set; }
            public string request_token { get; set; }

            public MovieDatabaseRequestToken(string Expires_at, string Request_token)
            {
                expires_at = Expires_at; request_token = Request_token;
            }
        }
        public class MovieDatabaseDeleteRequestBody
        {
            public string session_id { get; set; }
            public MovieDatabaseDeleteRequestBody(string Session_id)
            {
                session_id = Session_id;
            }
        }
        public class MovieDatabaseSession
        {
            public string session_id { get; set; }
            public MovieDatabaseSession(string Session_id)
            {
                session_id = Session_id;
            }
            public void ExitSession()
            {
                SendHttpClientMessage(httpClientSession, @"3/authentication/session?api_key=" + api_key_session,
                    HttpMethod.Delete, JObject.FromObject(new MovieDatabaseDeleteRequestBody(session_id)));
            }
        }
        public class MovieDatabaseGuestSession : MovieDatabaseSession
        {
            public string expires_at { get; set; }
            public MovieDatabaseGuestSession(string Session_id, string Expires_at) :
                base(Session_id)
            {
                expires_at = Expires_at;
            }
        }
        public class Genre
        {
            public int id { get; set; }
            public string name { get; set; }

            public override string ToString()
            {
                return name;
            }
        }
        public class MovieDatabaseGenre
        {
            public Genre[] genres { get; set; }
        }
        public class MovieDatabaseCollections
        {
            public class Part
            {
                public bool adult { get; set; }
                public string backdrop_path { get; set; }
                public int[] genre_ids { get; set; }
                public string original_language { get; set; }
                public string original_title { get; set; }
                public string overview { get; set; }
                public string release_date { get; set; }
                public string poster_path { get; set; }
                public float popularity { get; set; }
                public string title { get; set; }
                public bool video { get; set; }
                public float vote_average { get; set; }
                public int vote_count { get; set; }
            }
            public int id { get; set; }
            public string name { get; set; }
            public string overview { get; set; }
            public string poster_path { get; set; }
            public string backdrop_path { get; set; }
            public Part[] parts { get; set; }
        }
        public class MovieDatabaseMovieDetails
        {
            public class ProductionCompany
            {
                public string name { get; set; }
                public int id { get; set; }
                public string logo_path { get; set; }
                public string country { get; set; }
            }
            public class ProductionCountry
            {
                public string iso_3166_1 { get; set; }
                public string name { get; set; }
            }
            public class SpokenLanguage
            {
                public string iso_639_1 { get; set; }
                public string name { get; set; }
            }

            public bool adult { get; set; }
            public string backdrop_path { get; set; }
            public MovieDatabaseCollections belongs_to_collection { get; set; }
            public int budget { get; set; }
            public Genre[] genres { get; set; }
            public string homepage { get; set; }
            public int id { get; set; }
            public string imdb_id { get; set; }
            public string original_language { get; set; }
            public string original_title { get; set; }
            public string overview { get; set; }
            public int popularity { get; set; }
            public string poster_path { get; set; }
            public ProductionCompany[] production_companies { get; set; }
            public ProductionCountry[] production_countries { get; set; }
            public string release_date { get; set; }
            public int revenue { get; set; }
            public int runtime { get; set; }
            public SpokenLanguage[] spoken_languages { get; set; }
            public string status { get; set; }
            public string tagline { get; set; }
            public string title { get; set; }
            public bool video { get; set; }
            public float vote_average { get; set; }
            public int vote_count { get; set; }
        }
        public class MovieDatabaseAlternativeTitles
        {
            public class Title
            {
                public string iso_3166_1 { get; set; }
                public string title { get; set; }
                public string type { get; set; }

                public override string ToString()
                {
                    return title;
                }
            }

            public int id { get; set; }
            public Title[] titles { get; set; }
        }
        public class MovieDatabaseImages
        {
            public class Backdrop
            {
                public float aspect_ratio { get; set; }
                public string file_path { get; set; }
                public int height { get; set; }
                public string iso_639_1 { get; set; }
                public int vote_average { get; set; }
                public int vote_count { get; set; }
                public int width { get; set; }

                public override string ToString()
                {
                    return file_path;
                }
            }
            public class Poster
            {
                public float aspect_ratio { get; set; }
                public string file_path { get; set; }
                public int height { get; set; }
                public string iso_639_1 { get; set; }
                public int vote_average { get; set; }
                public int vote_count { get; set; }
                public int width { get; set; }

                public override string ToString()
                {
                    return file_path;
                }
            }

            public int id { get; set; }
            public Backdrop[] backdrops { get; set; }
            public Poster[] posters { get; set; }
        }
        public class Result
        {
            public string poster_path { get; set; }
            public bool adult { get; set; }
            public string overview { get; set; }
            public string release_date { get; set; }
            public int[] genre_ids { get; set; }
            public int id { get; set; }
            public string original_title { get; set; }
            public string original_language { get; set; }
            public string title { get; set; }
            public string backdrop_path { get; set; }
            public float popularity { get; set; }
            public int vote_count { get; set; }
            public bool video { get; set; }
            public float vote_average { get; set; }
        }
        public class MovieDatabaseReccommendations
        {
            public int page { get; set; }
            public Result[] results { get; set; }
            public int total_pages { get; set; }
            public int total_results { get; set; }
        }
        public class MovieDatabaseReleaseDates
        {
            public class ReleaseDate
            {
                public string certification { get; set; }
                public string iso_639_1 { get; set; }
                public string release_date { get; set; }
                public int type { get; set; }
                public string note { get; set; }
            }
            public class Result
            {
                public string iso_3166_1 { get; set; }
                public ReleaseDate[] release_dates { get; set; }
            }
            public int id { get; set; }
            public MovieDatabaseReleaseDates.Result[] results { get; set; }
        }
        public class MovieDatabaseVideos
        {
            public class Result
            {
                public string iso_639_1 { get; set; }
                public string iso_3166_1 { get; set; }
                public string name { get; set; }
                public string key { get; set; }
                public string site { get; set; }
                public int size { get; set; }
                public string type { get; set; }
                public bool official { get; set; }
                public string publish_at { get; set; }
                public string id { get; set; }
            }

            public int id { get; set; }
            public MovieDatabaseVideos.Result[] results { get; set; }
        }
        public class MovieDatabaseAccountDetails
        {
            public class Gravatar
            {
                public string hash { get; set; }
            }
            public class TMDB
            {
                public string avatar_path { get; set; }
            }
            public class Avatar
            {
                public Gravatar gravatar { get; set; }
                public TMDB tmdb { get; set; }
            }

            public Avatar avatar { get; set; }
            public int id { get; set; }
            public string iso_639_1 { get; set; }
            public string iso_3166_1 { get; set; }
            public string name { get; set; }
            public bool include_adult { get; set; }
            public string username { get; set; }
        }
        public class MovieDatabaseGuestRatedMovies
        {
            public class Result
            {
                public bool adult { get; set; }
                public string backdrop_path { get; set; }
                public int[] genre_ids { get; set; }
                public string original_language { get; set; }
                public string original_title { get; set; }
                public string overview { get; set; }
                public string release_date { get; set; }
                public string poster_path { get; set; }
                public float popularity { get; set; }
                public string title { get; set; }
                public bool video { get; set; }
                public float vote_average { get; set; }
                public int vote_count { get; set; }
                public int rating { get; set; }
            }

            public int page { get; set; }
            public MovieDatabaseGuestRatedMovies.Result[] results { get; set; }
            public int total_pages { get; set; }
            public int total_results { get; set; }
        }
        public class MovieDatabaseAccountLogin
        {
            public string username { get; set; }
            public string password { get; set; }
            public string request_token { get; set; }

            public MovieDatabaseAccountLogin(string Username, string Password, string Request_token)
            {
                username = Username; password = Password; request_token = Request_token;
            }
        }
        public class FileInternetExplorer
        {
            public string[] filenames { get; set; }
            public int processid { get; set; }

            public FileInternetExplorer(string[] Filenames, int Processid)
            {
                filenames = Filenames; processid = Processid;
            }
            public void DestroyProcessAssociatedFiles()
            {
                foreach (string filename in filenames)
                {
#if DEBUG
                    Console.WriteLine(filename);
#endif
                    if (File.Exists(filename)) File.Delete(filename);
                }
            }
        }
        public static JObject SendHttpClientMessage(HttpClient httpClient, string requestUrl, HttpMethod httpMethod,
            JObject jObject, [CallerLineNumber] int lineno = 0, [CallerMemberName] string membername = null,
            [CallerFilePath] string filename = null)
        {
            string message = null;
            if (null != jObject)
            {
                message = jObject.ToString(Formatting.None);
#if DEBUG
                Console.WriteLine(message);
#endif
            }
            HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, requestUrl);
            if (null != jObject) httpRequest.Content = new StringContent(message, Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> httpResponseTask = httpClient.SendAsync(httpRequest);
            httpResponseTask.Wait();
            HttpResponseMessage httpResponse = httpResponseTask.Result;
            Task<string> readResponseTask = httpResponse.Content.ReadAsStringAsync();
            string response = readResponseTask.Result;
            if (!httpResponse.IsSuccessStatusCode)
            {
#if DEBUG
                Console.WriteLine($"HTTP Response Unsuccessful in filename {filename}\r\nat member name" +
                    $" {membername} on line No. {lineno} with httpMethod " + httpMethod.ToString() + ":\r\n" + response);
#endif
                return null;
            }
#if DEBUG
            Console.WriteLine("Response: \r\n" + response);
#endif
            return JObject.Parse(response);
        }
        public static JObject SendHttpClientMessage(HttpClient httpClient, string requestUrl, HttpMethod httpMethod,
            string jObjectstring, [CallerLineNumber] int lineno = 0, [CallerMemberName] string membername = null,
            [CallerFilePath] string filename = null)
        {
#if DEBUG
            Console.WriteLine("jObjectstring: " + jObjectstring);
#endif
            HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, requestUrl);
            if (null != jObjectstring) httpRequest.Content = new StringContent(jObjectstring, Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> httpResponseTask = httpClient.SendAsync(httpRequest);
            httpResponseTask.Wait();
            HttpResponseMessage httpResponse = httpResponseTask.Result;
            Task<string> readResponseTask = httpResponse.Content.ReadAsStringAsync();
            string response = readResponseTask.Result;
            if (!httpResponse.IsSuccessStatusCode)
            {
#if DEBUG
                Console.WriteLine($"HTTP Response Unsuccessful in filename {filename}\r\nAt member name" +
                    $" {membername} on line No. {lineno} with httpMethod " + httpMethod.ToString() + ":\r\n" + response);
#endif
                return null;
            }
#if DEBUG
            Console.WriteLine("Response: \r\n" + response);
#endif
            return JObject.Parse(response);
        }
        public static JObject GetFromMovieDatabase(string requestUrl)
        {
            JObject jObject = SendHttpClientMessage(httpClientSession, requestUrl, HttpMethod.Get, (JObject)null);
            if (null != jObject && jObject.ContainsKey("success") && true != (bool)jObject.SelectToken("success"))
            {
                MovieDatabaseError movieDatabaseError = jObject.ToObject<MovieDatabaseError>();
                Console.WriteLine($"movieDatabase Error occured. Status code: {movieDatabaseError.status_code}\r\n" +
                $"{movieDatabaseError.status_message}");
            }
            return jObject;
        }
        public static void CacheGenres()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(onlinemoviedb_baselink);
                JObject result = GetFromMovieDatabase(@"3/genre/list?api_key=" + api_key_session);
                if (null == result)
                {
                    throw new ApplicationException("TMDB: Failed to get genre list!");
                }
                genres = result.ToObject<MovieDatabaseGenre>().genres;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FindGenreFromId(int genreid)
        {
            return Array.Find<Genre>(genres, x => genreid == x.id).name;
        }
        public static string[] FindGenresFromId(int[] genreids)
        {
            string[] genres = new string[genreids.Length];
            for (int i = 0; i < genreids.Length; i++)
            {
                genres[i] = FindGenreFromId(genreids[i]);
            }
            return genres;
        }
        public static void OpenHTMLonInternetExplorer(string main_filename, params string[] other_filenames)
        {
            string response = null;
            Process internetexplorer = new Process();
            Console.Write(@"Opening the Movie Details on Internet Explorer... Do you still want to continue will the the details are shown? [Y\N]: ");
            if (1 != (response = Console.ReadLine()).Length || ("Y" != response && "y" != response
                && "N" != response && "n" != response))
            {
                Console.WriteLine("Invalid Input!"); File.Delete(main_filename);
                Array.ForEach<string>(other_filenames, x => File.Delete(x));
                return;
            }
            internetexplorer.StartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", @"""" +
                Path.GetFullPath(main_filename) + @"""");
            internetexplorer.Start();
            internetexplorer.EnableRaisingEvents = true;
            internetexplorer.Exited += new EventHandler(OnInternetExplorerExit);

            string[] all_filenames = new string[other_filenames.Length + 1];
            Array.Copy(other_filenames, all_filenames, other_filenames.Length);
            all_filenames[all_filenames.Length - 1] = main_filename;

            fileIE.Add(new FileInternetExplorer(all_filenames, internetexplorer.Id));

            if ("N" == response || "n" == response) internetexplorer.WaitForExit();
        }
        public static void OnInternetExplorerExit(object sender, EventArgs e)
        {
            Process process = (Process)sender;
            int i = fileIE.FindIndex(x => process.Id == x.processid);
            fileIE[i].DestroyProcessAssociatedFiles();
            fileIE.RemoveAt(i);
        }
        public static void GetMovieListDetailsFromMDAPI()
        {
            movielistdetails_filename = "movie_ids_" + DateTime.Now.AddDays(-1).ToString("MM_dd_yyyy") + ".json";
            string temp_filename = movielistdetails_filename;
            string downloadlink = "http://files.tmdb.org/p/exports/" + temp_filename + ".gz";

            if (!File.Exists(temp_filename))
            {
                using (WebClient wc = new WebClient())
                {
                    temp_filename += ".gz";
                    wc.DownloadFile(downloadlink, temp_filename);
                    using (FileStream ofs = new FileStream(temp_filename, FileMode.Open))
                    {
                        temp_filename = temp_filename.Remove(temp_filename.Length - 3);
                        using (FileStream decompfs = File.Create(temp_filename))
                        {
                            using (GZipStream decompStream = new GZipStream(ofs, CompressionMode.Decompress))
                            {
                                decompStream.CopyTo(decompfs);
                            }
                        }
                    }
                    File.Delete(temp_filename + ".gz");
                }
            }
        }
        public static int GetMovieIdFromMovieTitle(string movietitle)
        {
            MovieDatabaseObject MovieDatabaseObj = null;
            using (StreamReader sr = new StreamReader(movielistdetails_filename))
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
        public static string GetValidatedSession(string request_token)
        {
            JObject result = SendHttpClientMessage(httpClientSession, @"3/authentication/session/new?api_key=" + api_key_session,
                HttpMethod.Post, @"{""request_token"":""" + request_token + @"""}");
            if (!(bool)result.SelectToken("success"))
            {
                MovieDatabaseError movieDatabaseError = result.ToObject<MovieDatabaseError>();
                Console.WriteLine($"movieDatabase Error occured. Status code: {movieDatabaseError.status_code}\r\n" +
                $"{movieDatabaseError.status_message}");
            }
            return (string)result.SelectToken("session_id");
        }
#if DEBUG
        public static MovieDatabaseSession CreateTestSession()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(onlinemoviedb_baselink);
                JObject result = GetFromMovieDatabase(@"3/authentication/token/new?api_key=" + api_key_session);
                if (null == result) return null;
                MovieDatabaseRequestToken request_token = new MovieDatabaseRequestToken(
                    (string)result.SelectToken("expires_at"), (string)result.SelectToken("request_token"));
                MovieDatabaseAccountLogin accountLogin = new MovieDatabaseAccountLogin(
                    test_username, test_password, request_token.request_token);

                result = SendHttpClientMessage(httpClient, @"3/authentication/token/validate_with_login?api_key=" +
                    api_key_session, HttpMethod.Post, JObject.FromObject(accountLogin));
                if (null == result) return null;
                return new MovieDatabaseSession(GetValidatedSession((string)result.SelectToken("request_token")));
            }
        }
        public static string test_username { get; set; } public static string test_password { get; set; }
#endif

        public const string onlinemoviedb_baselink = "https://api.themoviedb.org/";
        public static string api_key_session { get; set; }
        public const string default_gravatar_hash = "ea5c5a5805e6ea740f35969dee4fe442";

        public const string htmlbody_start = @"<!DOCTYPE html><html><head><meta charset=""utf-8""><meta name=""viewport"" content = ""width=device-width, initial-scale=1.0""><link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css"" integrity=""sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm"" crossorigin=""anonymous""></head><body>";
        public const string htmlbody_end = @"<script src=""https://code.jquery.com/jquery-3.6.0.js"" integrity=""sha256-H+K7U5CnXl1h5ywQfKtSj8PCmoN9aaq30gDh27Xc0jk="" crossorigin=""anonymous""></script><script src=""https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js""" +
            @" integrity=""sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q"" crossorigin=""anonymous""></script><script src=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js"" integrity=""sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl"" crossorigin=""anonymous""></script></body></html>";
        public const string carousel_start = @"<div id=""carouselExampleIndicators"" class=""carousel slide"" data-ride=""carousel""><ol class=""carousel-indicators""><li data-target=""#carouselExampleIndicators"" data-slide-to=""0"" class=""active""></li><li data-target=""#carouselExampleIndicators"" data-slide-to=""1""></li><li data-target=""#carouselExampleIndicators"" data-slide-to=""2""></li></ol><div class=""carousel-inner"">";
        public const string carouselfirstslide_start = @"<div class=""carousel-item active""><img class=""d-block w-100"" src=";
        public const string carouselfirstslide_end = @"></div>";
        public const string carouselslide_start = @"<div class=""carousel-item""><img class=""d-block w-100"" src=";
        public const string carouselslide_end = @"></div>";
        public const string carousel_end = @"</div><a class=""carousel-control-prev"" href=""#carouselExampleIndicators"" role=""button"" data-slide=""prev""><span class=""carousel-control-prev-icon"" aria-hidden=""true""></span><span class=""sr-only"">" +
            @"Previous</span></a><a class=""carousel-control-next"" href=""#carouselExampleIndicators"" role=""button"" data-slide=""next""><span class=""carousel-control-next-icon"" aria-hidden=""true""></span><span class=""sr-only"">Next</span></a></div>";
        public const string tmdb_imagebase_link = @"https://image.tmdb.org/t/p/w500";
        public const string gravatar_imagebase_link = @"https://www.gravatar.com/avatar/";
        public const string youtube_embed_start = @"<div class=""embed-responsive embed-responsive-16by9""><iframe class=""embed-responsive-item"" src=""https://www.youtube.com/embed/";
        public const string youtube_embed_end = @"?rel=0"" allowfullscreen></iframe></div>";

        public static Genre[] genres { get; set; }
        public static List<FileInternetExplorer> fileIE { get; set; }
        public static string movielistdetails_filename { get; set; }
        public static HttpClient httpClientSession { get; set; }
    }
}
