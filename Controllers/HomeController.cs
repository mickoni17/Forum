using IEP_Projekat.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using MailKit.Net.Smtp;
using MimeKit;

namespace IEP_Projekat.Controllers
{
    public class HomeController : Controller
    {
        Forum _context = new Forum();

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult Index(string filterKategorije = "Sve kategorije", string pretraga = "")
        {
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("Cookie"))
            {
                Session["KorisnikId"] =  this.ControllerContext.HttpContext.Request.Cookies["Cookie"].Value;
                Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.KorisnikId.ToString() == Session["KorisnikId"].ToString());
                Session["KorisnikUloga"] = kor.Uloga;
            }
            dynamic mymodel = new ExpandoObject();
            mymodel.Pitanja = _context._Pitanje.ToList();
            mymodel.Odgovori = _context._Odgovor.ToList();
            mymodel.Kategorije = _context._Kategorije.ToList();
            mymodel.FilterKategorija = filterKategorije;
            mymodel.Pretraga = pretraga;
            ViewBag.FilterKategorija = filterKategorije;
            return View(mymodel);
        }
        public ActionResult RegisterPage()
        {
            return View("Register");
        }
        public ActionResult LoginPage()
        {
            return View("Login");
        }
        public ActionResult ProfilePage(String id)
        {
            Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.KorisnikId.ToString() == id);
            List<Pitanje> pit = (from x in _context._Pitanje where x.AutorId==kor.KorisnikId select x).ToList();
            List<Odgovor> odg = (from x in _context._Odgovor where x.KorsinikId == kor.KorisnikId select x).ToList();
            dynamic mymodel = new ExpandoObject();
            mymodel.Korisnik = kor;
            mymodel.Pitanje = pit;
            mymodel.Odgovor = odg;
            return View("Profile", mymodel);
        }
        public ActionResult AdminSettings()
        {
            return View();
        }
        public ActionResult Question(int id,string sortiranje="Time")
        {
            Pitanje pit = _context._Pitanje.ToList().SingleOrDefault(u => u.PitanjeId == id);
            Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.KorisnikId == pit.AutorId);
            dynamic mymodel = new ExpandoObject();
            ViewBag.SortiranjeOdgovora = sortiranje;
            mymodel.Pitanje = pit;
            mymodel.ImeAutora = kor.Ime + " " + kor.Prezime;
            mymodel.Reply = (from x in _context._Odgovor where x.PitanjeId == pit.PitanjeId && x.ReplyNaId != null select x).ToList();
            mymodel.Lajkovi = (from x in _context._LikeDislike where x.Ocena == 1 select x).ToList();
            mymodel.Dislajkovi = (from x in _context._LikeDislike where x.Ocena == -1 select x).ToList();
            List<Odgovor> odgovori = (from x in _context._Odgovor where x.PitanjeId == pit.PitanjeId && x.ReplyNaId==null select x).ToList();
            if (sortiranje == "Time")
            {
                mymodel.Odgovori = odgovori.OrderByDescending(o => o.Vreme);
            }
            else 
            {
                int num = 0;
                if (sortiranje == "Likes")
                {
                    num = 1;
                }
                else
                {
                    num = -1;
                }
                List<Odgovor> odg = (from x in _context._Odgovor where x.PitanjeId == pit.PitanjeId && x.ReplyNaId == null select x).ToList();
                List<LikeDislike> laj = _context._LikeDislike.ToList();
                List<KeyValuePair<int,int>> res = new List<KeyValuePair<int, int>>();
                foreach(Odgovor o in odg)
                {
                    res.Add(new KeyValuePair<int, int>(o.OdgovorId,laj.Where(u=>u.OdgovorId==o.OdgovorId).Count(c=>c.Ocena==num)));
                }
                List<KeyValuePair<int, int>> res2 = res.OrderByDescending(v=>v.Value).ToList();
                List<Odgovor> sortirano = new List<Odgovor>();
                foreach(KeyValuePair<int, int> o in res2)
                {
                    sortirano.Add(odg.SingleOrDefault(x => x.OdgovorId == o.Key));
                }
                mymodel.Odgovori = sortirano;
            }
            return View(mymodel);
        }

        [HttpPost]
        public ActionResult Login(Korisnik k, string b = null)
        {
            Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.Mail == k.Mail && u.Lozinka == k.Lozinka);
            if (kor == null)
            {
                ViewBag.ErrorMessage = "Invalid username or password!";
                return View("Login");
            }
            else if (kor.Status == 0)
            {
                return View("Neaktivan");
            }
            else if (kor.Status == 2)
            {
                dynamic mymode = new ExpandoObject();
                mymode.Mail = kor.Mail;
                mymode.Password = kor.Lozinka;
                mymode.Validation = kor.Validation;
                ViewBag.ValidationMessage = "";
                return View("MailValidation", mymode);
            }
            else
            {
                Session["KorisnikId"] = kor.KorisnikId.ToString();
                Session["KorisnikUloga"] = kor.Uloga.ToString();

                HttpCookie cookie = new HttpCookie("Cookie");
                cookie.Value = kor.KorisnikId.ToString();
                if (b == null)
                    cookie.Expires = DateTime.Now.AddMinutes(10);
                else
                    cookie.Expires = DateTime.Now.AddYears(1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);

                ViewBag.ErrorMessage = "";
                dynamic mymodel = new ExpandoObject();
                mymodel.Pitanja = _context._Pitanje.ToList();
                mymodel.Odgovori = _context._Odgovor.ToList();
                mymodel.Kategorije = _context._Kategorije.ToList();
                mymodel.FilterKategorija = "Sve kategorije";
                mymodel.Pretraga = "";
                Response.Redirect("~/Home/Index");
                return View("Index",mymodel);
            }
        }
        [HttpPost]
        public ActionResult Register(Korisnik k)
        {
            if (ModelState.IsValid)
            {
                Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.Mail == k.Mail);
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var stringChars = new char[6];
                var random = new Random();
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                string val = new String(stringChars);

                if (kor != null)
                {
                    ViewBag.ErrorMessage = "Mail is already taken!";
                    return View("Register");
                }
                else
                {
                    ViewBag.ErrorMessage = "";
                    _context._Korisnik.Add(new Korisnik
                    {
                        Ime = k.Ime,
                        Prezime = k.Prezime,
                        Mail = k.Mail,
                        Lozinka = k.Lozinka,
                        Tokeni = 0,
                        Uloga = 0,
                        Status = 2,
                        Private = false,
                        Validation=val,
                        NewPassword=false
                    });


                    MimeMessage message = new MimeMessage();

                    MailboxAddress from = new MailboxAddress("FoRoom","iep.foroom@gmail.com");
                    message.From.Add(from);

                    MailboxAddress to = new MailboxAddress("User",k.Mail);
                    message.To.Add(to);

                    message.Subject = "Welcome to FoRoom!";
                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = "<h2>Hello "+k.Ime+"!</h2><h4>Your validation code is "+val+"</h4><h5>We are looking forward to you joining us!</h5>";
                    message.Body = bodyBuilder.ToMessageBody();

                    MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
                    client.Connect("smtp.gmail.com", 465, true);
                    client.Authenticate("iep.foroom@gmail.com", "adminadminic123");

                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();

                    _context.SaveChanges();

                    dynamic mymodel = new ExpandoObject();
                    mymodel.Mail=k.Mail;
                    mymodel.Password = k.Lozinka;
                    mymodel.Validation = val;
                    ViewBag.ValidationMessage = "";
                    return View("MailValidation",mymodel);
                }
            }
            else
            {
                if (k.Ime == null || k.Prezime == null || k.Mail == null || k.Lozinka == null)
                    ViewBag.ErrorMessage = "Please fill all the fields.";
                else
                if (k.Lozinka.Length < 6)
                    ViewBag.ErrorMessage = "Password must have at least 6 characters!";
                return View("Register");
            }
        }
        [HttpPost]
        public ActionResult Validation(string inputkod,string mail,string password,string validacija)
        {
            if (inputkod == validacija)
            {
                Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.Mail == mail && u.Lozinka == password);
                ViewBag.ValidationMessage = "";
                kor.Status = 1;
                _context.SaveChanges();
                return Login(kor);
            }
            else
            {
                ViewBag.ValidationMessage = "Incorrect validation code. Please try again.";
                dynamic mymodel = new ExpandoObject();
                mymodel.Mail = mail;
                mymodel.Password = password;
                mymodel.Validation = validacija;
                return View("MailValidation", mymodel);
            }
        }
        public ActionResult ForgotPassword1()
        {
            ViewBag.NoSuchUser = "";
            return View();
        }
        public ActionResult ForgotPassword2()
        {
            ViewBag.InvalidMail = "";
            ViewBag.PasswordDontMatch = "";
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string mail, string pass1,string pass2)
        {
            if (pass1 != pass2)
            {
                ViewBag.PasswordDontMatch = "Passwords don't match!";
            }
            else
            {
                ViewBag.PasswordDontMatch = "";
            }
            Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.Mail == mail);
            if (kor == null || (kor!=null && kor.NewPassword==false))
            {
                ViewBag.InvalidMail = "Invalid email address.";
            }
            else
            {
                ViewBag.InvalidMail = "";
            }
            if(ViewBag.InvalidMail=="" && ViewBag.PasswordDontMatch=="")
            {
                kor.NewPassword = false;
                kor.Lozinka = pass1;
                _context.SaveChanges();
                return View("Login");
            }
            return View("ForgotPassword2");
        }
        [HttpPost]
        public ActionResult SendNewPassword(string mail)
        {
            Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.Mail == mail);
            if (kor != null)
            {
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("FoRoom", "iep.foroom@gmail.com");
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress("User", mail);
                message.To.Add(to);

                message.Subject = "New password authorization";
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = "<a href=\"http://localhost:58497/Home/ForgotPassword2\">Click here to change your password</a>";
                message.Body = bodyBuilder.ToMessageBody();

                MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("iep.foroom@gmail.com", "adminadminic123");

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();

                kor.NewPassword = true;
                ViewBag.NoSuchUser = "";
                _context.SaveChanges();

                return View();
            }
            else
            {
                ViewBag.NoSuchUser = "There is no such user.";
                return View("ForgotPassword1");
            }
        }
        public ActionResult Logout()
        {

            Session.Remove("KorisnikId");
            Session.Remove("KorisnikUloga");
            Session.Abandon();

            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("Cookie"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["Cookie"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            /*dynamic mymodel = new ExpandoObject();
            mymodel.Pitanja = _context._Pitanje.ToList();
            mymodel.Odgovori = _context._Odgovor.ToList();
            mymodel.Kategorije = _context._Kategorije.ToList();
            mymodel.FilterKategorija = "Sve kategorije";
            mymodel.Pretraga ="";
            return View("Index", mymodel);*/
            return View("Login");
        }

        public ActionResult Channel(int id)
        {
            dynamic mymodel = new ExpandoObject();
            Kanal k = _context._Kanal.ToList().SingleOrDefault(u => u.KanalId == id);
            mymodel.Kanal = k;
            int y = int.Parse(Session["KorisnikId"].ToString());
            Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.KorisnikId == y);
            List<Poruka> list= (from x in _context._Poruka where x.KanalId == id select x).ToList();
            mymodel.Poruke = list;
            mymodel.Korisnik = kor;
            return View(mymodel);
        }


    }
}