using CalcOfQuantityPPI.Data;
using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    public class SearchController : Controller
    {
        private DatabaseHelper db;

        public SearchController()
        {
            db = new DatabaseHelper();
        }

        [HttpGet]
        public ActionResult SearchByDepartmentName()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchByPPIName()
        {
            return View();
        }
    }
}