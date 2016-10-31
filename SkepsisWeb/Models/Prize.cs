using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkepsisWeb.Models {
    public class Prize {
        [Key]
        public string PrizeID { get; set; }
        public string HuuhaaPrizeLead { get; set; }
        public string HuuhaaPrizeTitle { get; set; }
        public string HuuhaaPrizeSubTitle { get; set; }
        public string HuuhaaPrizeText { get; set; }
        public bool HuuhaaPrizeEnabled { get; set; }
        public string SokratesPrizeLead { get; set; }
        public string SokratesPrizeTitle { get; set; }
        public string SokratesPrizeSubTitle { get; set; }
        public string SokratesPrizeText { get; set; }
        public bool SokratesPrizeEnabled { get; set; }
    }
}