﻿using SkepsisWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Mailjet.Client;
using Mailjet.Client.Resources;
using System.Threading.Tasks;
using System.Diagnostics;


// NuGet: SendGrid
// Spam testing
// Parempi: http://www.mail-tester.com
// http://www.isnotspam.com 
// https://sendgrid.com/docs/for-developers/sending-email/v3-csharp-code-example/

namespace SkepsisWeb.Helpers {
    public static class EmailHelpers {
        public static void SendNewMemberEmail(Member m, string toEmail, HttpServerUtilityBase server)
        {

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

        public static void SendFeedbackEmail(Feedback f, string toEmail, HttpServerUtilityBase server)
        {

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
        //private static void processAndSendEmail(string emailTo, string subject, string html, HttpServerUtilityBase server) {
        //    string apiKey = getEmailPassword(server);
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress("no-reply@nsd.fi", "Skepsis - Web-sivusto");
        //    List<EmailAddress> to = new List<EmailAddress>();
        //    string[] emailTos = emailTo.Trim().Replace("; ", ";").Replace(';', ',').Split(',');
        //    foreach (var item in emailTos) {
        //        to.Add(new EmailAddress(item));
        //    }
        //    var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, to, subject, "", html);
        //    client.SendEmailAsync(msg);
        //}
        private static void processAndSendEmail(string emailTo, string subject, string html, HttpServerUtilityBase server)
        {

            var email = new Email()
            {
                EmailFrom = "no-reply@nsd.net",
                EmailFromName = "Skepsis - Web-sivusto",
                EmailTo = emailTo,
                Html = html,
                Subject = subject,
            };
            Task.Run(() => sendMailAsync(email, server));

            return;

            // NsdMailjet is not in used any more

            //var json = new JObject();
            //json.Add("system", "SKEPSIS");
            //json.Add("emailTo", emailTo);
            //json.Add("subject", subject);
            //json.Add("html", html);
            //json.Add("emailFrom", "no-reply@nsd.net");
            //json.Add("emailFromName", "Skepsis - Web-sivusto");

            //var restClient = new RestClient("https://nsdmailjet.azurewebsites.net/api");
            //var restRequest = new RestRequest("/email", Method.POST);
            //restRequest.AddParameter("text/json", json, ParameterType.RequestBody);

            //var restResponse = restClient.Execute(restRequest);
            //if (restResponse.StatusCode.ToString() != "OK") throw new Exception("Error sending email");
        }

        private static async Task sendMailAsync(Email email, HttpServerUtilityBase server) {
            MailjetClient client = new MailjetClient(getMailjetApiKey(server), getMailjetApiSecret(server)) { Version = ApiVersion.V3_1 };

            string[] emailTos = email.EmailTo.Trim().Replace(", ", ",").Replace("; ", ";").Replace(';', ',').Split(',');
            MailjetResponse response;
            MailjetRequest request;

            // Safety feature. Remove when sure all works
            if (emailTos.Length >= 10) throw new Exception("Receiver count 10 or more");

            var eTos = new JArray();
            foreach (var item in emailTos) {
                var jObj = new JObject();
                jObj.Add(new JProperty("Email", item));
                jObj.Add(new JProperty("Name", item));
                eTos.Add(jObj);
            }

            request = new MailjetRequest { Resource = Send.Resource }
               .Property(Send.Messages, new JArray {
                        new JObject {
                            {"From", new JObject {
                            {"Email", email.EmailFrom},
                            {"Name", email.EmailFromName}
                        }},
                        {"To", eTos },
                        {"Subject", email.Subject},
                        {"HTMLPart", email.Html}
                   }
            });

            var startTime = DateTime.UtcNow;
            response = await client.PostAsync(request);
            var duration = DateTime.UtcNow - startTime;

            //System.Threading.Thread.Sleep(1000);

            if (response.IsSuccessStatusCode) {
                Debug.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Debug.WriteLine(response.GetData());
                email.Status = "OK";
                email.ErrorDescription = duration.TotalSeconds.ToString();
            } else {
                Debug.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                Debug.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Debug.WriteLine(response.GetData());
                Debug.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                email.Status = "ERROR";
                email.ErrorDescription = response.GetErrorMessage() + " " + response.GetErrorInfo().ToString() + " Status code: " + response.StatusCode.ToString();
                throw new Exception("ERROR");
            }
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

        public static string getMailjetApiKey(HttpServerUtilityBase server)
        {
            // Try is local
            try {
                return System.IO.File.ReadAllText(server.MapPath(@"~/MailjetApiKey.key"));
            }
            catch {
                // I guess we are in Azure
                return Environment.GetEnvironmentVariable("MAILJET_API_KEY").ToString();
            }
        }
        public static string getMailjetApiSecret(HttpServerUtilityBase server)
        {
            // Try is local
            try {
                return System.IO.File.ReadAllText(server.MapPath(@"~/MailjetApiSecret.key"));
            }
            catch {
                // I guess we are in Azure
                return Environment.GetEnvironmentVariable("MAILJET_API_SECRET").ToString();
            }
        }

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
