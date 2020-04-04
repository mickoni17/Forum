using IEP_Projekat.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEP_Projekat.Controllers
{
    public class AgentController : Controller
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

        public ActionResult AllChannels()
        {
            dynamic mymodel = new ExpandoObject();
            int y = int.Parse(Session["KorisnikId"].ToString());
            List<Ucesnici> lista=(from x in _context._Ucesnici where x.KlijentId == y select x).ToList();
            List<Kanal> kanali = (from x in _context._Kanal where x.Status == true select x).ToList();
            List<Kanal> da = new List<Kanal>();
            List<Kanal> ne = new List<Kanal>();
            foreach(Kanal k in kanali)
            {
                Ucesnici u= lista.SingleOrDefault(x => x.KanalId == k.KanalId && x.KlijentId==y);
                if (u != null)
                {
                    da.Add(k);
                }
                else
                {
                    ne.Add(k);
                }
            }
            List<Korisnik> korisnici = new List<Korisnik>();
            List<int> brojevi = new List<int>(); 
            foreach(Kanal k in da)
            {
                Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.KorisnikId == k.OtvaracId);
                if (kor!=null){
                    korisnici.Add(kor);
                }
                brojevi.Add((from x in _context._Ucesnici where x.KanalId == k.KanalId select x).ToList().Count());
            }
            foreach (Kanal k in ne)
            {
                Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.KorisnikId == k.OtvaracId);
                if (kor != null)
                {
                    korisnici.Add(kor);
                }
                brojevi.Add((from x in _context._Ucesnici where x.KanalId == k.KanalId select x).ToList().Count());
            }
            mymodel.Ucestvujem = da;
            mymodel.Ostali = ne;
            mymodel.Korisnici = korisnici;
            mymodel.Brojevi = brojevi;
            return View(mymodel);
        }
        public void LeaveChannel(int id)
        {
            int y = int.Parse(Session["KorisnikId"].ToString());
            Ucesnici u = _context._Ucesnici.SingleOrDefault(x => x.KanalId == id && x.KlijentId == y);
            if (u != null)
            {
                _context._Ucesnici.Remove(u);
                _context.SaveChanges();
            }
            Response.Redirect("~/Agent/AllChannels");
        }
        public void JoinChannel(int id)
        {
            int y = int.Parse(Session["KorisnikId"].ToString());
            Kanal k = _context._Kanal.SingleOrDefault(x => x.KanalId == id);
            Ucesnici u = _context._Ucesnici.SingleOrDefault(x => x.KanalId == id && x.KlijentId == y);
            if (k.Status)
            {
                if (u == null)
                {
                    _context._Ucesnici.Add(new Ucesnici
                    {
                        KanalId = id,
                        KlijentId = y
                    });
                    _context.SaveChanges();
                }
                Response.Redirect("~/Home/Channel/"+id);
                return;
            }
            Response.Redirect("~/Agent/AllChannels");
        }
    }
}