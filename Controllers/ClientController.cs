using IEP_Projekat.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEP_Projekat.Controllers
{
    public class ClientController : Controller
    {
        Forum _context = new Forum();
        private PayPal.Api.Payment payment;
        private string package = "option1";

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult TokensPage()
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
            return View("Tokens",m);
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TokenOrdersPage()
        {
            int y = int.Parse(Session["KorisnikId"].ToString());
            List<Porudzbine> list = (from x in _context._Porudzbine where x.KorisnikId==y select x).ToList();
            return View("TokenOrders", list);
        }
        [HttpPost]
        public void DeleteQuestion(int pitanjeId)
        {
            Pitanje pit = _context._Pitanje.ToList().SingleOrDefault(u => u.PitanjeId == pitanjeId);
            List<Odgovor> list1 = (from x in _context._Odgovor where x.PitanjeId==pitanjeId select x).ToList();
            foreach(Odgovor o in list1)
            {
                List<LikeDislike> ld= (from x in _context._LikeDislike where x.OdgovorId == o.OdgovorId select x).ToList();
                if (ld.Count()>0)
                {
                    foreach (LikeDislike del in ld)
                        _context._LikeDislike.Remove(del);
                }
                _context._Odgovor.Remove(o);
            }
            _context._Pitanje.Remove(pit);
            _context.SaveChanges();
            Response.Redirect("~/Home/Index");
        }
        [HttpPost]
        public void AskQuestion(string kategorijaPitanja,string naslovPitanja,string opisPitanja)
        {
            _context._Pitanje.Add(new Pitanje
            {
                AutorId = int.Parse(Session["KorisnikId"].ToString()),
                KategorijaId = kategorijaPitanja,
                Naslov = naslovPitanja,
                Tekst = opisPitanja,
                VremePravljenja = DateTime.Now,
                VremeZakljucavanja = DateTime.Parse("1900-01-01 12:00:00")
            });
            _context.SaveChanges();
            Response.Redirect("~/Home/Index");
        }
        public void Lock(int pitanjeId)
        {
            Pitanje pit = _context._Pitanje.ToList().SingleOrDefault(u => u.PitanjeId == pitanjeId);
            pit.VremeZakljucavanja = DateTime.Now;
            _context.SaveChanges();
            Response.Redirect("~/Home/Question/" + pitanjeId);
        }
        public void Unlock(int pitanjeId)
        {
            Pitanje pit = _context._Pitanje.ToList().SingleOrDefault(u => u.PitanjeId == pitanjeId);
            pit.VremeZakljucavanja = DateTime.Parse("1900-01-01 00:00:00");
            _context.SaveChanges();
            Response.Redirect("~/Home/Question/" + pitanjeId);
        }
        [HttpPost]
        public ActionResult PostPackage(string exampleRadios){
            package = exampleRadios;
            Session["Package"] = package;
            return PaymentWithPaypal();
        }

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal
                //Payer Id will be returned when payment proceeds or click to pay
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class

                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                                "/Client/PaymentWithPayPal?";

                    //here we are generating guid for storing the paymentID received in session
                    //which will be used in the payment execution
                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    //get links returned from paypal in response to Create function call
                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {

                    // This function exectues after receving all parameters for the payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    //If executed payment failed then we will show payment failure message to user
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("Test", false);
                    }
                }
            }
            catch (Exception ex)
            {
                return View("Test", false);
            }

            //on successful payment, show success page to user.
            Paketi p = _context._Paketi.ToList().SingleOrDefault(u => u.PaketId == 2);
            if (Session["Package"].ToString() == "option2")
            {
                p = _context._Paketi.ToList().SingleOrDefault(u => u.PaketId == 3);
            }
            if (Session["Package"].ToString() == "option3")
            {
                p = _context._Paketi.ToList().SingleOrDefault(u => u.PaketId == 4);
            }
            _context._Porudzbine.Add(new Porudzbine
            {
                KorisnikId = int.Parse(Session["KorisnikId"].ToString()),
                Vreme=DateTime.Now,
                Tokeni=p.Kolicina,
                Cena = p.Cena.GetValueOrDefault()
            });
            int xy = int.Parse(Session["KorisnikId"].ToString());
            Korisnik k = _context._Korisnik.ToList().SingleOrDefault(u => u.KorisnikId == xy);
            k.Tokeni += p.Kolicina;
            _context.SaveChanges();
            return View("Test", true);
        }


        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }


        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };
            string itemName = "Silver token package",itemChar="S";
            Paketi p = _context._Paketi.ToList().SingleOrDefault(u => u.PaketId == 2);
            if (package == "option2")
            {
                p = _context._Paketi.ToList().SingleOrDefault(u => u.PaketId == 3);
                itemName = "Gold token package"; itemChar = "G";
            }
            if (package == "option3")
            {
                p = _context._Paketi.ToList().SingleOrDefault(u => u.PaketId == 4);
                itemName = "Platinum token package"; itemChar = "P";
            }
            string pricee = p.Cena.ToString();
            string quantityy = p.Kolicina.ToString();
            //Adding Item Details like name, currency, price etc
            itemList.items.Add(new Item()
            {
                name = itemName,
                currency = "USD",
                price = pricee,
                quantity = "1",
                sku = itemChar
            });

            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            // Adding Tax, shipping and Subtotal details
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = pricee
            };

            //Final amount with details
            var amount = new Amount()
            {
                currency = "USD",
                total = pricee, // Total must be equal to sum of tax, shipping and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();
            // Adding description about the transaction
            transactionList.Add(new Transaction()
            {
                description = "This package has "+quantityy+" tokens",
                invoice_number = Convert.ToString((new Random()).Next(100000)), //Generate an Invoice No
                amount = amount,
                item_list = itemList
            });


            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);
        }
        public ActionResult NoTokens()
        {
            return View();
        }
        public ActionResult ChannelsPage()
        {
            Paketi p = _context._Paketi.ToList().SingleOrDefault(u => u.PaketId == 1);
            int y = int.Parse(Session["KorisnikId"].ToString());

            dynamic mymodel = new ExpandoObject();
            mymodel.Cena = p.Kolicina;

            List<Ucesnici> ucesca = (from x in _context._Ucesnici where x.KlijentId == y select x).ToList();
            List<Kanal> kanali = new List<Kanal>();
            foreach(Ucesnici u in ucesca)
            {
                Kanal k = _context._Kanal.ToList().SingleOrDefault(x => x.KanalId == u.KanalId);
                if (k.Status)
                {
                    kanali.Add(k);
                }
            }
            mymodel.Kanali = kanali;

            return View(mymodel);
        }
        public void CloseChannel(int id)
        {
            Kanal k = _context._Kanal.ToList().SingleOrDefault(x => x.KanalId == id);
            k.Status = false;
            _context.SaveChanges();
            Response.Redirect("~/Client/ChannelsPage");
        }
        [HttpPost]
        public void CreateChannel(string naslovKanala)
        {
            int y = int.Parse(Session["KorisnikId"].ToString());
            Paketi cena = _context._Paketi.ToList().SingleOrDefault(u => u.PaketId == 1);
            Korisnik kor = _context._Korisnik.ToList().SingleOrDefault(u => u.KorisnikId == y);
            if (kor.Tokeni < cena.Kolicina)
            {
                Response.Redirect("~/Client/NoTokens");
                return;
            }
            else
            {
                kor.Tokeni -= cena.Kolicina;
            }

            Kanal k=new Kanal
            {
                Naziv= naslovKanala,
                OtvaracId=y,
                Status=true,
                VremeOtvaranja=DateTime.Now
            };
            _context._Kanal.Add(k);
            _context._Ucesnici.Add(new Ucesnici
            {
                KanalId=k.KanalId,
                KlijentId=y
            });

            _context.SaveChanges();
            Response.Redirect("~/Home/Channel/"+k.KanalId);
        }
        
    }
}