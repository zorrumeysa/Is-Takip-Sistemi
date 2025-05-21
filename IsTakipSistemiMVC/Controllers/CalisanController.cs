using IsTakipSistemiMVC.Models;  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class CalisanController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();

        // Çalışan ana sayfası
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)  // Personel ise (Yetki türü 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);

                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var birim = entity.Birimler.FirstOrDefault(b => b.birimId == birimId);

                if (birim != null)
                {
                    ViewBag.birimAd = birim.birimAd;
                }

                // Personelin işlerini al
                var isler = (from i in entity.Isler
                             where i.isPersonelId == personelId && i.isOkunma == false
                             orderby i.iletilenTarih descending select i).ToList();

                // İşleri View'a göndermek
                ViewBag.isler = isler;

                return View();
            }

            // Yetki türü 2 olmayan kullanıcıları Login sayfasına yönlendir
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public ActionResult Index(int isId)
        {
            var tekIs =(from i in entity.Isler where i.isId==isId select i).FirstOrDefault();

            tekIs.isOkunma = true;
            entity.SaveChanges();
            return RedirectToAction("Yap", "Calisan");
        }



        // Çalışanın işlerini tamamlama sayfası
        public ActionResult Yap()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);

                var isler = entity.Isler
                    .Where(i => i.isPersonelId == personelId && i.isDurumId == 1)
                    .OrderByDescending(i => i.iletilenTarih)
                    .ToList();

                ViewBag.isler = isler;
                return View();
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public ActionResult Yap(int isId, string isYorum)
        {
            try
            {
                var tekIs = entity.Isler.FirstOrDefault(i => i.isId == isId);

                if (tekIs == null)
                {
                    TempData["HataMesaji"] = "İş bulunamadı!";
                    return RedirectToAction("Yap", "Calisan");
                }

                if (string.IsNullOrWhiteSpace(isYorum))
                {
                    isYorum = "Kullanıcı yorum yapmadı.";
                }

                tekIs.yapılanTarih = DateTime.Now;
                tekIs.isDurumId = 2;
                tekIs.isYorum = isYorum;

                entity.SaveChanges();

                TempData["BasariMesaji"] = "İş başarıyla tamamlandı.";
            }
            catch (Exception ex)
            {
                TempData["HataMesaji"] = "Hata oluştu: " + ex.Message;
            }

            return RedirectToAction("Yap", "Calisan");
        }



        // Çalışanın iş takip ekranı
        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);

                var isler = (from i in entity.Isler
                             join d in entity.Durumlarr on i.isDurumId equals d.durumId
                             where i.isPersonelId == personelId
                             orderby i.iletilenTarih descending
                             select new IsDurum
                             {
                                 isBaslik = i.isBaslik,
                                 isAciklama = i.isAciklama,
                                 iletilenTarih = i.iletilenTarih,
                                 yapılanTarih = i.yapılanTarih,
                                 durumAd = d.durumAd,
                                 isYorum = i.isYorum
                             }).ToList();

                IsDurumModel model = new IsDurumModel { isDurumlarr = isler };

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


    }
}
