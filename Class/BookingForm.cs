using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace test2.Class
{
    public class BookingForm
    {
        public int BookingID { get; set; }
        public string UserID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string Size { get; set; }
        public string NumberVacancy { get; set; }
    }
}
