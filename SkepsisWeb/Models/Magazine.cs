using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkepsisWeb.Models {
    public class Magazine {
        [Key]
        public string MagazineID { get; set; } // yyyy-MM
        public string MagazineCoverImageUrl { get; set; }
        public string MagazinePdfFileUrl { get; set; }
        public bool Enabled { get; set; }
        public bool PublicPdf { get; set; }
    }
}