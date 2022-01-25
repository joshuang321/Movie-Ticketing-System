using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;


namespace Movie_Ticketing_System
{
    class OnlineDatabase
    {
        public struct MovieDatabaseError
        {
            public string status_message { get; set; }
            public int status_code { get; set; }
        }
        public struct MovieDatabaseRequestToken
        {
            public string expires_at { get; set; }
            public string request_token { get; set; }

            public MovieDatabaseRequestToken(string Expires_at, string Request_token)
            {
                expires_at = Expires_at; request_token = Request_token;
            }
        }
        public struct MovieDatabaseDeleteRequestBody
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
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(onlinemoviedb_baselink);
                    HttpRequestMessage httpDeleteRequest = new HttpRequestMessage(HttpMethod.Delete,
                        @"/authentication/session?api_key=" + api_key_session);
                    httpDeleteRequest.Content = new StringContent(JsonConvert.SerializeObject(new MovieDatabaseDeleteRequestBody(session_id)));
                    Task<HttpResponseMessage> deleteSessionTask = httpClient.SendAsync(httpDeleteRequest);
                    deleteSessionTask.Wait();
                    HttpResponseMessage httpResponse = deleteSessionTask.Result;
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error occured. Failed to exit session."); return;
                    }
                }
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
        public struct Genre
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public struct MovieDatabaseMovieDetails
        {
            public struct ProductionCompany
            {
                public string name { get; set; }
                public int id { get; set; }
                public string logo_path { get; set; }
                public string country { get; set; }
            }
            public struct ProductionCountry
            {
                public string iso_3166_1 { get; set; }
                public string name { get; set; }
            }
            public struct SpokenLanguage
            {
                public string iso_639_1 { get; set; }
                public string name { get; set; }
            }

            public bool adult { get; set; }
            public string backdrop_path { get; set; }
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
            public string tagline { get; set; }
            public string title { get; set; }
            public bool video { get; set; }
            public float vote_average { get; set; }
            public int vote_count { get; set; }
        }
        public struct MovieDatabaseAlternativeTitles
        {
            public struct Title
            {
                public string iso_3166_1 { get; set; }
                public string title { get; set; }
                public string type { get; set; }
            }

            public int id { get; set; }
            public Title[] titles { get; set; }
        }
        public struct MovieDatabaseImages
        {
            public struct Backdrop
            {
                public float aspect_ratio { get; set; }
                public string file_path { get; set; }
                public int height { get; set; }
                public string iso_639_1 { get; set; }
                public int vote_average { get; set; }
                public int vote_count { get; set; }
                public int width { get; set; }
            }
            public struct Poster
            {
                public float aspect_ratio { get; set; }
                public string file_path { get; set; }
                public int height { get; set; }
                public string iso_639_1 { get; set; }
                public int vote_average { get; set; }
                public int vote_count { get; set; }
                public int width { get; set; }
            }

            public int id { get; set; }
            public Backdrop[] backdrops { get; set; }
            public Poster[] posters { get; set; }
        }
        public struct Result
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
        public struct MovieDatabaseReccommendations
        {
            public int page { get; set; }
            public Result[] results { get; set; }
            public int total_pages { get; set; }
            public int total_results { get; set; }
        }
        public struct MovieDatabaseSimilarMovies
        {
            public int page { get; set; }
            public Result[] results { get; set; }
            public int total_pages { get; set; }
            public int total_results { get; set; }
        }
        public struct MovieDatabaseReleaseDates
        {
            public struct ReleaseDate
            {
                public string certification { get; set; }
                public string iso_639_1 { get; set; }
                public string release_date { get; set; }
                public int type { get; set; }
                public string note { get; set; }
            }
            public struct Result
            {
                public string iso_3166_1 { get; set; }
                public ReleaseDate[] release_dates { get; set; }
            }
            public int id { get; set; }
            public MovieDatabaseReleaseDates.Result[] results { get; set; }
        }
        public struct MovieDatabaseVideos
        {
            public struct Result
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
        public struct MovieDatabaseAccountDetails
        {
            public struct Gravatar
            {
                public string hash { get; set; }
            }
            public struct Avatar
            {
                public Gravatar gravatar { get; set; }
            }

            public Avatar avatar { get; set; }
            public int id { get; set; }
            public string iso_639_1 { get; set; }
            public string iso_3166_1 { get; set; }
            public string name { get; set; }
            public bool include_adult { get; set; }
            public string username { get; set; }
        }
        public struct MovieDatabaseGuestRatedMovies
        {
            public struct Result
            {
                public bool adult { get; set; }
                public string backdropPath { get; set; }
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
        public static JObject SendHttpClientMessage(HttpClient httpClient, string requestUrl, HttpMethod httpMethod,
            JObject jObject, [CallerLineNumber] int lineno = 0,[CallerMemberName] string membername = null,
            [CallerFilePath] string filename = null)
        {
            string message = null;
            if (null != jObject) 
            {
               message = jObject.ToString(Formatting.None); Console.WriteLine(message);
            }
            HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, requestUrl);
            if (null != jObject) httpRequest.Content = new StringContent(message);
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
            return JObject.Parse(response);
        }
        public static JObject SendHttpClientMessage(HttpClient httpClient, string requestUrl, HttpMethod httpMethod,
            string jObjectstring, [CallerLineNumber] int lineno = 0, [CallerMemberName] string membername = null,
            [CallerFilePath] string filename = null)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, requestUrl);
            if (null != jObjectstring) httpRequest.Content = new StringContent(jObjectstring);
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
            return JObject.Parse(response);
        }
        public static JObject GetFromMovieDatabase(string requestUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(OnlineDatabase.onlinemoviedb_baselink);
                JObject jObject = OnlineDatabase.SendHttpClientMessage(httpClient, requestUrl,
                    HttpMethod.Get, (JObject)null);
                if (jObject.ContainsKey("success"))
                {
                    OnlineDatabase.MovieDatabaseError movieDatabaseError = jObject.ToObject<OnlineDatabase.MovieDatabaseError>();
                    Console.WriteLine($"movieDatabase Error occured. Status code: ${movieDatabaseError.status_code}\r\n" +
                        $"{movieDatabaseError.status_message}");
                    if (7 == movieDatabaseError.status_code) Console.WriteLine($"Invalid api_key: {OnlineDatabase.api_key_session}");
                    return null;
                }
                return jObject;
            }
        }

        public const string onlinemoviedb_baselink = "https://api.themoviedb.org/3";
        public static string api_key_session = null;
    }
}
