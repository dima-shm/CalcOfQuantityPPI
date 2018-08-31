using CalcOfQuantityPPI.Data;
using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseHelper db;

        public HomeController()
        {
            db = new DatabaseHelper();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(db);
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
    }
}