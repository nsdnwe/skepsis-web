using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkepsisWeb.Models {
    public class Member {
        public int ID { get; set; }
        public string Type { get; set; } // FULL or MAGAZINE
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string ZipAndCity { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Profession { get; set; }
        public string Education { get; set; }
        public string Info { get; set; }
        public string IpAddress { get; set; }
        public DateTime Created { get; set; }

        [NonSerialized]
        public string CompletedMessage;
    }
}