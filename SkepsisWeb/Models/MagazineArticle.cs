using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkepsisWeb.Models {
    public class MagazineArticle {
        public int ID { get; set; } // yyyy-MM
        public string MagazineID { get; set; } // yyyy-MM
        public string Author { get; set; }
        public string Title { get; set; }
        public string PdfFile { get; set; }
        public int Rank { get; set; }
    }
}