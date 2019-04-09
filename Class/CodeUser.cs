using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace test2.Class
{
    public class CodeUser
    {
        public int Id_reserve { get; set; }
        [StringLength(6)]
        public string Code { get; set; }
    }
}
