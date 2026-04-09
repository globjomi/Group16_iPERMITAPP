using System.Web.Mvc;

namespace Group16_iPERMITAPP.Controllers
{
    public class REDashboardController : Controller
    {
        public ActionResult Index()
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "RE")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}