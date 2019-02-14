using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace test2.DatabaseContext.Models
{
    public class Reservation
    {
        [Key]
        public int Id_reserve { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        // reserve by day by day 
        // maximum for reserve day is 3 days
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime DateModified { get; set; }
        //  public int CountOpen { get; set; }
        public bool Optional { get; set; }
        public string Size { get; set; }
        public string Location { get; set; }
        [ForeignKey("Account")]
        public string Id_account { get; set; }
        [ForeignKey("Vacancy")]
        public int Id_vacancy { get; set; }
    }
}
