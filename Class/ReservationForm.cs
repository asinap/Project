using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace test2.Class
{
    public class ReservationForm
    {
        public int Id_reserve { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public DateTime DateModified { get; set; }
        public string Size { get; set; }
        public string Location { get; set; }
        public string Id_account { get; set; }
        public int Id_vacancy { get; set; }
    }
}
