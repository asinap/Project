using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace test2.Class
{
    public class PhoneUser
    {
        public string Id_account { get; set; }
        [StringLength(10)]
        public string Phone { get; set; }
    }
}
