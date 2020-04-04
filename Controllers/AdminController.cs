using IEP_Projekat.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEP_Projekat.Controllers
{
    public class AdminController : Controller
    {
        Forum _context = new Forum();

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AdminSettings()
        {
            List<Paketi> pak = _context._Paketi.ToList();
            dynamic m = new ExpandoObject();
            foreach (Paketi p in pak)
            {
                if (p.PaketId == 1)
                {
                    m.CH = p.Kolicina;
                }
                if (p.PaketId == 2)
                {
                    m.SA = p.Kolicina;
                    m.SP = p.Cena;
                }
                if (p.PaketId == 3)
                {
                    m.GA = p.Kolicina;
                    m.GP = p.Cena;
                }
                if (p.PaketId == 4)
                {
                    m.PA = p.Kolicina;
                    m.PP = p.Cena;
                }
            }
            return View(m);
        }
        public ActionResult AllUsers()
        {
            List<Korisnik> kor = _context._Korisnik.ToList();
            return View(kor);
        }

        [HttpPost]
        public ActionResult AddCategory(string category)
        {
            Kategorije kat = _context._Kategorije.ToList().SingleOrDefault(u => u.Naziv == category);
            if (kat != null)
            {
                return View("Test", false);
            }
            else
            {
                _context._Kategorije.Add(new Kategorije
                {
                    Naziv = category
                });
                _context.SaveChanges();
                return View("Test", true);
            }
            //return View("AdminSettings");
        }

        public ActionResult EditUserPage(int id)
        {
            Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.KorisnikId == id);
            return View("EditUser", kor);
        }
        public ActionResult CreateUser()
        {
            return View("EditUser", null);
        }
        public void DeleteCommentReq(int odgovorId)
        {
            Odgovor odg = _context._Odgovor.ToList().SingleOrDefault(u => u.OdgovorId == odgovorId);
            List<LikeDislike> ld = (from x in _context._LikeDislike where x.OdgovorId == odg.OdgovorId select x).ToList();
            if (ld.Count() > 0)
            {
                foreach (LikeDislike del in ld)
                    _context._LikeDislike.Remove(del);
            }
            Odgovor odg2 = _context._Odgovor.ToList().SingleOrDefault(u => u.ReplyNaId == odgovorId);
            if (odg2 != null)
            {
                DeleteCommentReq(odg2.OdgovorId);
            }
            _context._Odgovor.Remove(odg);
            _context.SaveChanges();
        }
        public void DeleteComment(int odgovorId,int pitanjeId)
        {
            DeleteCommentReq(odgovorId);
            Response.Redirect("~/Home/Question/"+pitanjeId);
        }
        [HttpPost]
        public ActionResult EditUser(string uName,string uSurname, string uMail, string uPassword,string uRole, string uStatus, string uPrivacy, int uId = 0)
        {
            Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.KorisnikId == uId);
            if (kor != null)
            {
                kor.Ime = uName;
                kor.Prezime = uSurname;
                kor.Mail = uMail;
                kor.Lozinka = uPassword;
                if (uRole == "Client")
                {
                    kor.Uloga = 0;
                }
                else if (uRole == null)
                {
                    kor.Uloga = 2;
                }
                else
                {
                    kor.Uloga = 1;
                }
                if (uStatus == "Active" || uStatus==null)
                {
                    kor.Status = 1;
                }
                else
                {
                    kor.Status = 0;
                }
                if (uPrivacy == "Public")
                {
                    kor.Private = false;
                }
                else
                {
                    kor.Private = true;
                }
                _context.SaveChanges();
                List<Korisnik> lista = _context._Korisnik.ToList();
                return View("AllUsers", lista);
            }
            else
            {
                _context._Korisnik.Add(new Korisnik
                {
                    Ime = uName,
                    Prezime = uSurname,
                    Mail = uMail,
                    Lozinka = uPassword,
                    Tokeni = 0,
                    Uloga = (uRole=="Client"?0:1),
                    Status = (uStatus == "Active" ? 1 : 0),
                    Private = (uPrivacy == "Private" ? true : false)
                });
                _context.SaveChanges();
                List<Korisnik> lista = _context._Korisnik.ToList();
                return View("AllUsers", lista);
            }
        }

        [HttpPost]
        public void UpdateChannelPrice(int ch)
        {
            Paketi p = _context._Paketi.ToList().SingleOrDefault(u => u.PaketId==1);
            p.Kolicina = ch;
            _context.SaveChanges();
            Response.Redirect("~/Admin/AdminSettings");

        }
        [HttpPost]
        public void UpdatePackages(int sa, int sp, int ga, int gp, int pa, int pp)
        {
            List<Paketi> pak = _context._Paketi.ToList();
            foreach (Paketi p in pak)
            {
                if (p.PaketId == 2)
                {
                    p.Kolicina = sa;
                    p.Cena = sp;
                }
                if (p.PaketId == 3)
                {
                    p.Kolicina = ga;
                    p.Cena = gp;
                }
                if (p.PaketId == 4)
                {
                    p.Kolicina = pa;
                    p.Cena = pp;
                }
            }
            _context.SaveChanges();
            Response.Redirect("~/Admin/AdminSettings");
        }
    }
}