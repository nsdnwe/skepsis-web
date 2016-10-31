using SkepsisWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkepsisWeb.ViewModels {
    public class MagazinesViewModel {
        public int Year;
        public IEnumerable<Magazine> Magazines;
        public IEnumerable<MagazineArticle> MagazineArticles;
    }
}