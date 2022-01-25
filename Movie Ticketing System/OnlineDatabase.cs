using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // TO DO: Send DELETE to delete the current Session.
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

        public const string onlinemoviedb_baselink = "https://api.themoviedb.org/3";
        public static string api_key_session = null;
    }
}
