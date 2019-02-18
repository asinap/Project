using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace test2.Class
{
    public class UserOverview
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public int Using { get; set; }
        public int TimeUp { get; set; }
        public int Point { get; set; }
        public List<WebForm> BookingList = new List<WebForm>();
    }
}
