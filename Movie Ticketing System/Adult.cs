using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie_Ticketing_System
{
    class Adult : Ticket
    {
        public bool popcornOffer { get; set; }
        public Adult() : base()
        {
            popcornOffer = false;
        }
        public Adult(Screening Screening, bool PopcornOffer) : base(Screening)
        {
            popcornOffer = PopcornOffer;
        }
        public override double CalculatePrice()
        {
            return 0.0;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
