using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
    }
}