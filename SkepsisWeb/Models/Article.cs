using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkepsisWeb.Models {
    public class Article {
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime ArticleDatetime { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime PublishDatetime { get; set; }

        public string Ingress { get; set; }
        public string Body { get; set; }
        [Required]
        public bool Deleted { get; set; }
    }
}