using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SkepsisWeb.Models;

namespace SkepsisWeb.ViewModels {
    public class IndexViewModel {
        public IEnumerable<Article> Articles; 
        public IEnumerable<PublicEvent> PublicEvents;
        public Prize HuuhaaPrize;
        public Prize SokratesPrize;
        public MagazineViewModel MagazineViewModel;
    }
}