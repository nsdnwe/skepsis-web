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
Lisätietoja: {7}
Ip-osoite: {8}
Luotu (UTC): {9}",
            m.Name, m.Address, m.ZipAndCity, m.Email, m.Phone, m.Education, m.Profession, m.Info, m.IpAddress, m.Created.ToString("dd.MM.yyyy HH:mm:ss"));

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
            f.Name, f.Email, f.Phone, f.Info, f.IpAddress, f.Created.ToString("dd.MM.yyyy HH:mm:ss"));

            MailMessage msg = new MailMessage();

            msg.To.Add(toEmail);
            msg.Subject = "Skepsis palaute - " + f.Name;
            msg.Body = template;

            sendEmail(msg, server);
        }

        private static void sendEmail(MailMessage msg, HttpServerUtilityBase server) {
            msg.From = new MailAddress("web-sivut@skepsis.fi");
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
    }
}
