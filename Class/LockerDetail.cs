using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace test2.Class
{
    public class LockerDetail
    {
        public string LockerID { get; set; }
        public string Location { get; set; }
        public List<VacancyDetail> Vacancieslist = new List<VacancyDetail>();
    }
}
