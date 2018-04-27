using SkepsisWeb.Helpers;
using SkepsisWeb.Models;
using SkepsisWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkepsisWeb.Controllers {
    public class SivutController : Controller {
        private const string NEW_MEMBERSHIP_REQUEST_EMAIL_ADDRESSES = "niko.wessman@nsd.fi"; // "secretary@skepsis.fi,niko.wessman@nsd.fi";
        private const string FEEDBACK_EMAIL_ADDRESSES = "secretary@skepsis.fi,niko.wessman@nsd.fi"; // 

        private DB db = new DB();
        public ActionResult Etusivu() {
            ViewBag.OgPageUrl = "Etusivu";

            var vm = new IndexViewModel();
            var evd = DateTime.UtcNow.AddDays(-1);
            vm.Articles = db.Articles.Where(z => !z.Deleted && z.PublishDatetime <= DateTime.UtcNow).OrderByDescending(z => z.PublishDatetime).Take(3).ToList();
            vm.PublicEvents = db.PublicEvents.Where(z => !z.Deleted && z.EventDatetime >= evd).OrderBy(z => z.EventDatetime).Take(3).ToList();
            vm.HuuhaaPrize = db.Prizes.Where(z => z.HuuhaaPrizeEnabled).OrderByDescending(z => z.PrizeID).First();
            vm.SokratesPrize = db.Prizes.Where(z => z.SokratesPrizeEnabled).OrderByDescending(z => z.PrizeID).First();

            vm.MagazineViewModel = new MagazineViewModel();
            vm.MagazineViewModel.Magazine = db.Magazines.Where(z => z.Enabled).OrderByDescending(z => z.MagazineID).First();
            string magazineID = vm.MagazineViewModel.Magazine.MagazineID;
            vm.MagazineViewModel.MagazineArticles = db.MagazineArticles.Where(z => z.MagazineID == magazineID).ToList();

            return View(vm);
        }

        // --------------------------------------------------------------------------------------
        // Yhdistys
        // --------------------------------------------------------------------------------------
        public ActionResult MikaSkepsisOn() {
            ViewBag.OgPageUrl = "MikaSkepsisOn";
            return View();
        }
        public ActionResult SkepsisPaSvenska() {
            ViewBag.OgPageUrl = "SkepsisPaSvenska";
            return View();
        }
        public ActionResult SkepsisInEnglish() {
            ViewBag.OgPageUrl = "SkepsisInEnglish";
            return View();
        }
        public ActionResult Henkilohistoria() {
            ViewBag.OgPageUrl = "Henkilohistoria";
            return View();
        }
        public ActionResult YhdistyksenSaannot() {
            ViewBag.OgPageUrl = "YhdistyksenSaannot";
            return View();
        }
        public ActionResult Yhteystiedot() {
            ViewBag.OgPageUrl = "Yhteystiedot";
            return View("Yhteystiedot2");
        }
        public ActionResult Yhteystiedot2() {
            ViewBag.OgPageUrl = "Yhteystiedot";
            return View();
        }
        public ActionResult AnnaPalautetta() {
            ViewBag.OgPageUrl = "AnnaPalautetta";
            return View(new Feedback());
        }
        [HttpPost]
        public ActionResult AnnaPalautetta(Feedback feedback) {
            ViewBag.OgPageUrl = "AnnaPalautetta";
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "Palauteteksti on pakollinen tieto.");
                return View(feedback);
            }

            feedback.IpAddress = Request.ServerVariables["REMOTE_HOST"].ToString();
            feedback.Created = DateTime.UtcNow;

            db.Feedbacks.Add(feedback);
            db.SaveChanges();

            EmailHelpers.SendFeedbackEmail(feedback, FEEDBACK_EMAIL_ADDRESSES, Server);
            var blank = new Feedback() {
                CompletedMessage = "Kiitos. Palautteesi on vastaanotettu."
            };
            return View(blank);
        }
        public ActionResult LiitySkepsiksenJaseneksi() {
            ViewBag.OgPageUrl = "LiitySkepsiksenJaseneksi";
            return View(new Member());
        }
        [HttpPost]
        public ActionResult LiitySkepsiksenJaseneksi(Member member) {
            ViewBag.OgPageUrl = "LiitySkepsiksenJaseneksi";
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "Nimi, katuosoite, postinumero ja postitoimipaikka ovat pakollisia tietoja.");
                return View(member);
            }

            member.IpAddress = Request.ServerVariables["REMOTE_HOST"].ToString();
            member.Created = DateTime.UtcNow;
            member.Type = "FULL";

            db.Members.Add(member);
            db.SaveChanges();

            //if (member.Type == "FULL") member.Type = "Jäsenyys"; else member.Type = "Vain lehti !";
            EmailHelpers.SendNewMemberEmail(member, NEW_MEMBERSHIP_REQUEST_EMAIL_ADDRESSES, Server);
            var blank = new Member() {
                CompletedMessage = "Kiitos. Jäsenhakemuksesi on vastaanotettu."
            };
            return View(blank);
        }
        
        // --------------------------------------------------------------------------------------
        // Toiminta
        // --------------------------------------------------------------------------------------
        public ActionResult Uutiset() {
            ViewBag.OgPageUrl = "Uutiset";
            var articles = db.Articles.Where(z => !z.Deleted && z.PublishDatetime <= DateTime.UtcNow).OrderByDescending(z => z.PublishDatetime).Take(30).ToList();
            return View(articles);
        }
        public ActionResult Tapahtumat() {
            ViewBag.OgPageUrl = "Tapahtumat";
            var limit = DateTime.UtcNow.AddYears(-1);
            var publicEvents = db.PublicEvents.Where(z => !z.Deleted && z.EventDatetime >= limit).OrderBy(z => z.EventDatetime).Take(30).ToList();
            return View(publicEvents);
        }
        public ActionResult HuuhaaJaSokratesPalkinnot() {
            ViewBag.OgPageUrl = "HuuhaaJaSokratesPalkinnot";
            // Only the last ones made public
            var res = new LastPrizesViewModel();
            res.HuuhaaPrize = db.Prizes.Where(z => z.HuuhaaPrizeEnabled).OrderByDescending(z => z.PrizeID).First();
            res.SokratesPrize = db.Prizes.Where(z => z.SokratesPrizeEnabled).OrderByDescending(z => z.PrizeID).First();
            return View(res);
        }
        // id = vuosi
        public ActionResult HuuhaaPalkinto(string id = "") {
            ViewBag.OgPageUrl = "HuuhaaPalkinto/" + id;
            var res = db.Prizes.Single(z => z.PrizeID == id && z.HuuhaaPrizeEnabled);
            return View(res);
        }
        // id = vuosi
        public ActionResult SokratesPalkinto(string id = "") {
            ViewBag.OgPageUrl = "SokratesPalkinto/" + id;
            var res = db.Prizes.Single(z => z.PrizeID == id && z.SokratesPrizeEnabled);
            return View(res);
        }
        public ActionResult HuuhaaPalkinnot() {
            ViewBag.OgPageUrl = "HuuhaaPalkinnot";
            var res = db.Prizes.Where(z => z.HuuhaaPrizeEnabled && z.HuuhaaPrizeText != null).OrderByDescending(z => z.PrizeID).ToList();
            return View(res);
        }
        public ActionResult SokratesPalkinnot() {
            ViewBag.OgPageUrl = "SokratesPalkinnot";
            var res = db.Prizes.Where(z => z.SokratesPrizeEnabled && z.SokratesPrizeText != null).OrderByDescending(z => z.PrizeID).ToList();
            return View(res);
        }

        public ActionResult SkepsiksenHaasteJaStipendi() {
            ViewBag.OgPageUrl = "SkepsiksenHaasteJaStipendi";
            return View();
        }
        public ActionResult NilsMustelinRahasto() {
            ViewBag.OgPageUrl = "NilsMustelinRahasto";
            return View();
        }

        // --------------------------------------------------------------------------------------
        // Julkaisuja
        // --------------------------------------------------------------------------------------
        // 

        // Lista vuosista ja yleistä tietoa Skeptikko-lehdestä
        public ActionResult SkeptikkoLehdet() {
            ViewBag.OgPageUrl = "SkeptikkoLehdet";
            return View();
        }

        // Yhden vuoden lehdet
        // id = vuosi
        public ActionResult SkeptikkoLehti(int id = 0) {
            ViewBag.OgPageUrl = "SkeptikkoLehti/" + id.ToString();
            var res = new MagazinesViewModel();
            res.Year = id;
            res.Magazines = db.Magazines.Where(z => z.MagazineID.StartsWith(id.ToString()) && z.Enabled).ToList();
            res.MagazineArticles = db.MagazineArticles.Where(z => z.MagazineID.StartsWith(id.ToString())).ToList();
            return View(res);
        }
        public ActionResult Artikkelit() {
            ViewBag.OgPageUrl = "Artikkelit";
            return View();
        }
        public ActionResult Linkkeja() {
            ViewBag.OgPageUrl = "Linkkeja";
            return View();
        }
        public ActionResult AudioJaVideoTallenteet() {
            ViewBag.OgPageUrl = "AudioJaVideoTallenteet";
            return View();
        }
        // --------------------------------------------------------------------------------------
        // Muut
        // --------------------------------------------------------------------------------------
        public ActionResult Haku() {
            ViewBag.OgPageUrl = "Haku";
            return View();
        }
        public ActionResult KeskustelusivustonSaannot() {
            ViewBag.OgPageUrl = "KeskustelusivustonSaannot";
            return View();
        }
        // --------------------------------------------------------------------------------------
        // Legacy
        // --------------------------------------------------------------------------------------
        public ActionResult Yhteys() {
            return RedirectToAction("Yhteystiedot");
        }
        public ActionResult Saannot() {
            return RedirectToAction("YhdistyksenSaannot");
        }

        public ActionResult Testing() {
            //EmailHelpers.SendNewMemberEmail("Niko", "Niko.wessman@nsd.fi", Server);
            return Content("Done");
        }
    }
}









