using SkepsisWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;


// NuGet: SendGrid
// Spam testing
// Parempi: http://www.mail-tester.com
// http://www.isnotspam.com 
// https://sendgrid.com/docs/for-developers/sending-email/v3-csharp-code-example/

namespace SkepsisWeb.Helpers {
    public static class EmailHelpers {
        public static void SendNewMemberEmail(Member m, string toEmail, HttpServerUtilityBase server) {

            string template = string.Format(@"
JÄSENHAKEMUS<br/>
<br/>
Nimi: {0}<br/>
Osoite: {1}<br/>
Osoite: {2}<br/>
Email: {3}<br/>
Puhelin: {4}<br/>
Koulutus: {5}<br/>
Ammatti: {6}<br/>
Lisätietoja: {7}<br/>
Ip-osoite: {8}<br/>
Luotu (UTC): {9} ",
            m.Name, m.Address, m.ZipAndCity, m.Email, m.Phone, m.Education, m.Profession, m.Info, m.IpAddress, m.Created.ToString("dd.MM.yyyy HH:mm:ss"));

            string subject = "Skepsis jäsenhakemus - " + m.Name;

            processAndSendEmail(toEmail, subject, template, server);
        }

        public static void SendFeedbackEmail(Feedback f, string toEmail, HttpServerUtilityBase server) {

            string template = string.Format(@"
PALAUTE<br/>
<br/>
Nimi: {0}<br/>
Email: {1}<br/>
Puhelin: {2}<br/>
Palaute: {3}<br/>
Ip-osoite: {4}<br/>
Luotu (UTC): {5}",
            f.Name, f.Email, f.Phone, f.Info, f.IpAddress, f.Created.ToString("dd.MM.yyyy HH:mm:ss"));

            string subject = "Skepsis palaute - " + f.Name;

            processAndSendEmail(toEmail, subject, template, server);
        }

        // If emailTo = "", receivers have been defined already
        //private static void processAndSendEmail(SendGridMessage message, HttpServerUtilityBase server, string emailTo) {
        //    //addEmailFooter(message);
        //    sendEmail(message, server, emailTo);
        //} 

        // emailTo in format "niko.wessman@nsd.fi,secretary@skepsis.fi"
        private static void processAndSendEmail(string emailTo, string subject, string html, HttpServerUtilityBase server) {
            string apiKey = getEmailPassword(server);
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("skepsis.noreply@skepsis.fi", "Skepsis - Web-sivusto");
            List<EmailAddress> to = new List<EmailAddress>();
            string[] emailTos = emailTo.Trim().Replace("; ", ";").Replace(';', ',').Split(',');
            foreach (var item in emailTos) {
                to.Add(new EmailAddress(item));
            }
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, to, subject, "", html);
            client.SendEmailAsync(msg);
        }


        //private static void sendEmail(MailMessage msg, HttpServerUtilityBase server) {
        //    msg.From = new MailAddress("web-sivut@skepsis.fi");
        //    msg.IsBodyHtml = false;
        //    SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
        //    var credentials = new NetworkCredential("azure_3b493c49ee514bb1d7e377ce388c2410@azure.com", getEmailPassword(server));
        //    smtpClient.Credentials = credentials;

        //    smtpClient.Send(msg);
        //}
        //private static void sendEmail(SendGridMessage message, HttpServerUtilityBase server,  string emailTo) {
        //    message.From = new MailAddress("web-sivut@skepsis.fi", "web-sivut@skepsis.fi");
        //    string[] emailTos = emailTo.Split(',');
        //    foreach (var item in emailTos) {
        //        message.AddTo(item);
        //    }

        //    var credentials = new NetworkCredential("azure_3b493c49ee514bb1d7e377ce388c2410@azure.com", getEmailPassword(server)); 
        //    var transportWeb = new Web(credentials);
        //    transportWeb.DeliverAsync(message);
        //}

        public static string getEmailPassword(HttpServerUtilityBase server) {
            // Try is local
            try {
                return System.IO.File.ReadAllText(server.MapPath(@"~/email2.key"));
            } catch  {
                // I guess we are in Azure
                return Environment.GetEnvironmentVariable("EMAIL_PASSWORD").ToString();
            }
        }
        // Not the best place. Refactor someday
        public static string getRecaptchaPrivateKey(HttpServerUtilityBase server) {
            // Try is local
            try {
                return System.IO.File.ReadAllText(server.MapPath(@"~/RecaptchaPrivate.key"));
            } catch {
                // I guess we are in Azure
                return Environment.GetEnvironmentVariable("recaptchaPrivateKey").ToString();
            }
        }
        public static CaptchaResponse ValidateCaptcha(string response, HttpServerUtilityBase server) {
            string secret = getRecaptchaPrivateKey(server);
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }
    }
    public class CaptchaResponse {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorMessage { get; set; }
    }

}
