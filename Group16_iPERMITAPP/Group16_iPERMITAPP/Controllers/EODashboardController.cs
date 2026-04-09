using System.Web.Mvc;

namespace Group16_iPERMITAPP.Controllers
{
    public class EODashboardController : Controller
    {
        public ActionResult Index()
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "EO")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}