using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkepsisWeb.Models {
    public class PublicEvent {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime EventDatetime { get; set; }

        public string Ingress { get; set; }
        public string Body { get; set; }
        [Required]
        public string City { get; set; }
        public bool Deleted { get; set; }
    }
}