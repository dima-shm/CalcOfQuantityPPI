using CalcOfQuantityPPI.Data;
using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels.Calc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    public class СalcController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private DatabaseHelper db;

        public СalcController()
        {
            db = new DatabaseHelper();
        }

        [HttpGet]
        public ActionResult СalcByDepartmentName()
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Departments = new SelectList(db.GetDepartments(user.DepartmentId), "Name", "Name");
            return View(new CalcViewModel
            {
                StructuralDepartmentName = db.GetDepartment(user.DepartmentId).Name,
                PPIViewModel = new List<PPIViewModel>()
            });
        }

        [HttpPost]
        public ActionResult СalcByDepartmentName(CalcViewModel model)
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Departments = new SelectList(db.GetDepartments(user.DepartmentId), "Name", "Name");
            CalcViewModel resultModel = new CalcViewModel
            {
                StructuralDepartmentName = db.GetDepartment(user.DepartmentId).Name,
                DepartmentName = model.DepartmentName,
                PPIViewModel = db.PPIViewModelByDepartment(db.GetDepartment(model.DepartmentName))
            };
            return View(resultModel);
        }

        [HttpGet]
        public ActionResult СalcByPPIName()
        {
            return View();
        }
    }
}