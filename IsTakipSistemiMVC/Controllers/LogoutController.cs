using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult Index()
        {
            Session.Clear(); // Oturum verilerini temizler
            Session.Abandon(); // Oturumu sonlandırır
            return RedirectToAction("Index", "Login"); // Login sayfasına yönlendirir
        }

    }
}