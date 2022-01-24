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
        public struct MovieDatabaseAccount
        {
            public string username { get; set; }
            public string password { get; set; }
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

            }
        }
        public class MovieDatabaseGuestSession : MovieDatabaseSession
        {
            public string expires_at { get; set; }
            public MovieDatabaseGuestSession(string Session_id, string Expires_at) : base(Session_id)
            {
                expires_at = Expires_at;
            }
        }
        public const string onlinemoviedb_baselink = "https://api.themoviedb.org/3";
        public static string api_key_session = null;
        public MovieDatabaseAccount session_account;
    }
}
