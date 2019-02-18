using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace test2.Class
{
    public class ReserveDetail
    {
        public string Id_user { get; set; }
        public string Name { get; set; }
        public int BookingID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateModified { get; set; }
        public string Location { get; set; }
        public string Size { get; set; }
        public string NumberVacancy { get; set; }
    }
}
