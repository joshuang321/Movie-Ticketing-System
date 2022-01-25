using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie_Ticketing_System
{
    class Order
    {
        public int orderNo { get; set; }
        public DateTime orderDateTime { get; set; }
        public double amount { get; set; }
        public string status { get; set; }
        public List<Ticket> ticketList { get; set; }
        public Order()
        {
            orderNo = 0; orderDateTime = DateTime.Now; amount = 0.0;
            status = null; ticketList = null;
        }
        public Order(int OrderNo, DateTime OrderDateTime)
        {
            orderNo = OrderNo; orderDateTime = OrderDateTime; amount = 0.0;
            status = null; ticketList = new();
        }
        public void AddTicket(Ticket ticket)
        {
            ticketList.Add(ticket);
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
