using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace test2.DatabaseContext.Models
{
    public class Notification
    {
        [Key]
        public int Id_notification { get; set; }
        public DateTime CreateTime { get; set; }
        //public DateTime Time { get; set; }
        public bool IsShow { get; set; }  //delete
        public int Id_reserve { get; set; }
        public bool Read { get; set; } //status for reading


        [ForeignKey("Content")]
        public int Id_content { get; set; }
        [ForeignKey("Account")]
        public string Id_account { get; set; }
        
        

    }
}
