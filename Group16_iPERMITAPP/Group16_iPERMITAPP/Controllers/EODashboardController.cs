using Group16_iPERMITAPP.Models;
using System.Collections.Generic;
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

            // Try to load permit requests if repository exists (may be empty)
            List<PermitRequestViewModel> model = null;
            try
            {
                var prRepo = new PermitRequestRepository();
                model = prRepo.GetAllWithRE();
            }
            catch
            {
                model = new List<PermitRequestViewModel>();
            }

            // Always load RE list (so EO can view RE accounts even if no permit requests exist)
            var reRepo = new RERepository();
            var reList = reRepo.GetAll();
            ViewBag.REs = reList;

            return View(model);
        }
    }
}