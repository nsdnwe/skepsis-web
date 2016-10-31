using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SkepsisWeb.Models {
    public class DB : DbContext {
        public DbSet<Article> Articles { get; set; }
        public DbSet<PublicEvent> PublicEvents { get; set; }
        public DbSet<Prize> Prizes { get; set; }
        public DbSet<Magazine> Magazines { get; set; }
        public DbSet<MagazineArticle> MagazineArticles { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

    }
}