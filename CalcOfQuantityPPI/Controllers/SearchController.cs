using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult SearchByDepartmentName()
        {
            return View();
        }

        public ActionResult SearchByPPIName()
        {
            return View();
        }
    }
}