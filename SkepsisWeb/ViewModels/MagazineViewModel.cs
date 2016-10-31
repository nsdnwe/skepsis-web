using SkepsisWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkepsisWeb.ViewModels {
    public class MagazineViewModel {
        public Magazine Magazine;
        public IEnumerable<MagazineArticle> MagazineArticles;
    }
}