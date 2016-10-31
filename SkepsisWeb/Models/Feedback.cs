using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkepsisWeb.Models {
    public class Feedback {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Info { get; set; }
        public string IpAddress { get; set; }
        public DateTime Created { get; set; }

        [NonSerialized]
        public string CompletedMessage;
    }
}