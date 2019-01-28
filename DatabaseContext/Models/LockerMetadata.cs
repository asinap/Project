using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace test2.DatabaseContext.Models
{
    public class LockerMetadata
    {
        [Key]
        public string Mac_address { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
    }
}
