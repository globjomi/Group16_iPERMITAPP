using System.Web.Mvc;
using Group16_iPERMITAPP.Models;

namespace Group16_iPERMITAPP.Controllers
{
    public class AccountController : Controller
    {
        private EORepository _eoRepo = new EORepository();
        private RERepository _reRepo = new RERepository();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string id, string password)
        {
            // Check EO
            var eo = _eoRepo.GetById(id, password);
            if (eo != null)
            {
                Session["UserId"] = eo.EOId.ToString();
                Session["UserType"] = "EO";
                return RedirectToAction("Index", "EODashboard");
            }

            // Check RE
            var re = _reRepo.GetById(id, password);
            if (re != null)
            {
                Session["UserId"] = re.REId;
                Session["UserType"] = "RE";
                return RedirectToAction("Index", "REDashboard");
            }

            // Invalid credentials
            ViewBag.Error = "Invalid ID or Password!";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}