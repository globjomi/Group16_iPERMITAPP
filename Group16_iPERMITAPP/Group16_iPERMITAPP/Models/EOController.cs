using System.Web.Mvc;
using Group16_iPERMITAPP.Models;

namespace Group16_iPERMITAPP.Controllers
{
    public class EOController : Controller
    {
        private EORepository _repo = new EORepository();

        public ActionResult Index()
        {
            var eoList = _repo.GetAll();
            return View(eoList);
        }
    }
}