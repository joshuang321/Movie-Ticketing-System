using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie_Ticketing_System
{
    class RequestToken
    {
        public bool success { get; set; }
        public string expires_at { get; set; }
        public string request_token { get; set; }
    }
}
