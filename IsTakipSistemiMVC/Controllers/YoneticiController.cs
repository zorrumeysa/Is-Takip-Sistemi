using IsTakipSistemiMVC.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class YoneticiController : Controller
    {


        IsTakipDBEntities entity = new IsTakipDBEntities();

        // Yöneticinin ana sayfası
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        // İş atama sayfası
        public ActionResult Ata()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var calisanlar = (from p in entity.Personeller
                                  where p.personelBirimId == birimId &&
                                p.personelYetkiTurId == 2
                                  select p).ToList();

                ViewBag.personeller = calisanlar;

                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Ata(FormCollection formCollection)
        {
            string isBaslik = formCollection["isBaslik"];
            string isAciklama = formCollection["isAciklama"];
            int secilenPersonelId = Convert.ToInt32(formCollection["selectPer"]);

            // Yeni bir iş nesnesi oluştur
            Isler yeniIs = new Isler
            {
                isBaslik = isBaslik,
                isAciklama = isAciklama,
                isPersonelId = secilenPersonelId,
                iletilenTarih = DateTime.Now,
                isDurumId = 1,
                isOkunma = false
            };

            // Veritabanına kaydet
            entity.Isler.Add(yeniIs);
            entity.SaveChanges();

            return RedirectToAction("Takip", "Yonetici");
        }

        // İş takip sayfası
        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var calisanlar = (from p in entity.Personeller
                                  where p.personelBirimId == birimId && p.personelYetkiTurId == 2
                                  select p).ToList();

                ViewBag.personeller = calisanlar;

                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Takip(int selectPer)
        {
            var secilenPersonel = (from p in entity.Personeller where p.personelId == selectPer select p).FirstOrDefault();

            TempData["secilen"] = secilenPersonel;

            return RedirectToAction("Listele", "Yonetici");
        }

        // Seçilen personelin işleri
        [HttpGet]
        public ActionResult Listele()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                Personeller secilenPersonel = (Personeller)TempData["secilen"];

                try
                {
                    var isler = (from i in entity.Isler
                                 where i.isPersonelId == secilenPersonel.personelId
                                 select i).ToList().OrderByDescending(i => i.iletilenTarih);
                    ViewBag.isler = isler;
                    ViewBag.personel = secilenPersonel;
                    ViewBag.isSayisi = isler.Count();

                    return View();
                }
                catch (Exception)
                {
                    return RedirectToAction("Takip", "Yonetici");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}
