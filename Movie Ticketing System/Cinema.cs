using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie_Ticketing_System
{
    class Cinema
    {
        public string name { get; set; }
        public int hallNo { get; set; }
        public int capacity { get; set; }
        public Cinema()
        {
            name = null; hallNo = 0; capacity = 0;
        }
        public Cinema(string Name, int HallNo, int Capacity)
        {
            name = Name; hallNo = HallNo; capacity = Capacity;
        }
        public override string ToString()
        {
            return name + "\t" + hallNo + "\t\t" + capacity;
        }
    }
}
