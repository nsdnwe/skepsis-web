using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkepsisWeb.Models {
    public class Email {
        public int ID { get; set; }
        public string System { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Html { get; set; }
        public string EmailFrom { get; set; }
        public string EmailFromName { get; set; }
        public bool SendSeparately { get; set; }
        public DateTime SentDatetime { get; set; }
        public string Status { get; set; }
        public string ErrorDescription { get; set; }
    }
}