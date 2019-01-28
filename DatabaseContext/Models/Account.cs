using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace test2.DatabaseContext.Models
{
    public class Account
    {
        [Key]
        public string Id_account { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int Point { get; set; }
        //public int FineCount { get; set; }
        //public int PayFineCount { get; set; }
    }
}
