using SkepsisWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

// NuGet: SendGrid
// Spam testing
// Parempi: http://www.mail-tester.com
// http://www.isnotspam.com 

namespace SkepsisWeb.Helpers {
    public static class EmailHelpers {
        public static void SendNewMemberEmail(Member m, string toEmail, HttpServerUtilityBase server) {

            string template = string.Format(@"
JÄSENHAKEMUS

Nimi: {0}
Osoite: {1}
Osoite: {2}
Email: {3}
Puhelin: {4}
Koulutus: {5}
Ammatti: {6}
Tyyppi: {7}
Lisätietoja: {8}
Ip-osoite: {9}
Luotu (UTC): {10}",
            m.Name, m.Address, m.ZipAndCity, m.Email, m.Phone, m.Education, m.Profession, m.Type, m.Info, m.IpAddress, m.Created.ToString("dd.MM.yy hh:mm:ss"));

            MailMessage msg = new MailMessage();

            msg.To.Add(toEmail);
            msg.Subject = "Skepsis jäsenhakemus - " + m.Name;
            msg.Body = template;

            sendEmail(msg, server);
        }

        public static void SendFeedbackEmail(Feedback f, string toEmail, HttpServerUtilityBase server) {

            string template = string.Format(@"
PALAUTE

Nimi: {0}
Email: {1}
Puhelin: {2}
Palaute: {3}
Ip-osoite: {4}
Luotu (UTC): {5}",
            f.Name, f.Email, f.Phone, f.Info, f.IpAddress, f.Created.ToString("dd.MM.yy hh:mm:ss"));

            MailMessage msg = new MailMessage();

            msg.To.Add(toEmail);
            msg.Subject = "Skepsis palaute - " + f.Name;
            msg.Body = template;

            sendEmail(msg, server);
        }


        //public static void SendNewPasswordEmail(string name, string email, string password) {
        //    MailMessage msg = new MailMessage();

        //    msg.To.Add(email);
        //    msg.Subject = "New Pin'n'Meet password";
        //    msg.Body = getNewPasswordEmailBody(name, email, password);

        //    sendEmail(msg);
        //}

        //public static void SendViolationReportEmail(string id, string name, string targetId, string targetName, string description) {
        //    MailMessage msg = new MailMessage();

        //    msg.To.Add("niko.wessman@nsd.fi");
        //    msg.Subject = "Pin'n'Meet violation report";
        //    msg.Body = getViolationEmailBody(id, name, targetId, targetName, description);

        //    sendEmail(msg);
        //}

        //public static void SendNewUserEmail(string name, string imageLink) {
        //    MailMessage msg = new MailMessage();

        //    msg.To.Add("niko.wessman@nsd.fi");
        //    msg.Subject = "New Pin'n'Meet user";
        //    msg.Body = getNewUserBody(name, imageLink);

        //    sendEmail(msg);
        //}

        //public static void SendNewMatchEmail(string name1, string imageLink1, string name2, string imageLink2, string venueName, string venueCity) {
        //    MailMessage msg = new MailMessage();

        //    msg.To.Add("niko.wessman@nsd.fi");
        //    msg.Subject = "New Pin'n'Meet - Match";
        //    msg.Body = getNewMatchBody(name1, imageLink1, name2, imageLink2, venueName, venueCity);

        //    sendEmail(msg);
        //}


        private static void sendEmail(MailMessage msg, HttpServerUtilityBase server) {
            msg.From = new MailAddress("skepsis-web@skepsis.fi");
            msg.IsBodyHtml = false;
            SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            var credentials = new NetworkCredential("azure_3b493c49ee514bb1d7e377ce388c2410@azure.com", getEmailPassword(server));
            smtpClient.Credentials = credentials;

            smtpClient.Send(msg);
        }

        public static string getEmailPassword(HttpServerUtilityBase server) {
            // Try is local
            try {
                return System.IO.File.ReadAllText(server.MapPath(@"~/email.key"));
            } catch  {
                // I guess we are in Azure
                return Environment.GetEnvironmentVariable("EMAIL_PASSWORD").ToString();
            }
        }



//        private static string getViolationEmailBody(string id, string name, string targetId, string targetName, string description) {
//            string template = @"
//Violating user: {targetName} {targetId}

//Reporting user: {name} {id} 

//{description}
//                ";

//            template = template.Replace("{name}", name);
//            template = template.Replace("{id}", id);
//            template = template.Replace("{targetName}", targetName);
//            template = template.Replace("{targetId}", targetId);
//            template = template.Replace("{description}", description);
//            return template;
//        }

//        private static string getNewUserBody(string name, string imageLink) {
//            string template = @"
//New user name: {targetName} 
//Image: {targetImage} 
//                ";

//            template = template.Replace("{targetName}", name);
//            template = template.Replace("{targetImage}", imageLink);
//            return template;
//        }

//        private static string getNewMatchBody(string name1, string imageLink1, string name2, string imageLink2, string venueName, string venueCity) {
//            string template = @"
//User1: {name1} {imageLink1} 
//User2: {name2} {imageLink2}
//Venue: {venueName} in {venueCity}
//                ";

//            template = template.Replace("{name1}", name1);
//            template = template.Replace("{name2}", name2);
//            template = template.Replace("{imageLink1}", imageLink1);
//            template = template.Replace("{imageLink2}", imageLink2);
//            template = template.Replace("{venueName}", venueName);
//            template = template.Replace("{venueCity}", venueCity);
//            return template;
//        }


//        private static string getNewPasswordEmailBody(string name, string email, string password) {
//            string template = @"
//Hi {name},

//Your new Pin'n'Meet password is {password} 
 
//We highly recommend you to change the password the next time you sign in Pin'n'Meet. 

//You’re receiving this email because you requested a new password for your Pin'n'Meet account. If this wasn’t you, please ignore this email.

//Regards, 
//Pin’n’Meet security team
//                ";

//            template = template.Replace("{name}", name);
//            template = template.Replace("{email}", email);
//            template = template.Replace("{password}", password);
//            return template;
//        }
    }
}