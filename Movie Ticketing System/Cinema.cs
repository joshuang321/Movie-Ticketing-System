//============================================================
// Student Number : S10205140B
// Student Name : Joshua Ng
// Module Group : T04
//============================================================

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
