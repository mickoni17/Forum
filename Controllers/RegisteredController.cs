using IEP_Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEP_Projekat.Controllers
{
    public class RegisteredController : Controller
    {
        Forum _context = new Forum();

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
        
        public void Like(int pitanjeId, int odgovorId, int like)
        {
            LikeDislike l = _context._LikeDislike.ToList().SingleOrDefault(u => u.KorisnikId.ToString() == Session["KorisnikId"].ToString() && u.OdgovorId==odgovorId);
            if (l == null)
            {
                _context._LikeDislike.Add(new LikeDislike
                {
                    OdgovorId = odgovorId,
                    KorisnikId = int.Parse(Session["KorisnikId"].ToString()),
                    Ocena=like
                });
                _context.SaveChanges();
            }
            else
            {
                l.Ocena = like;
                _context.SaveChanges();
            }
            Response.Redirect("~/Home/Question/"+pitanjeId);
        }
        
        [HttpPost]
        public void Answer(string odgovorNaPitanje, int pitanjeId,int ?replyNaId=null)
        {
            Pitanje pit = _context._Pitanje.ToList().SingleOrDefault(u => u.PitanjeId == pitanjeId);
            if (Session["KorisnikId"] != null && pit.VremeZakljucavanja.Year == 1900)
            {
                _context._Odgovor.Add(new Odgovor
                {
                    KorsinikId = int.Parse(Session["KorisnikId"].ToString()),
                    PitanjeId = pitanjeId,
                    Vreme = DateTime.Now,
                    Tekst = odgovorNaPitanje,
                    ReplyNaId = replyNaId
                });
                _context.SaveChanges();
            }
            Response.Redirect("~/Home/Question/" + pitanjeId);
        }
        
    }
}