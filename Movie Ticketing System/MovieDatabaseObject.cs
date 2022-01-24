using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie_Ticketing_System
{
    class MovieDatabaseObject
    {
        public bool adult { get; set; }
        public int id { get; set; }
        public string original_title { get; set; }
        public float popularity { get; set; }
        public bool video { get; set; }
    }
}
