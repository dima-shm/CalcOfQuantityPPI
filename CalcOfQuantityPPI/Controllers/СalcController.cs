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
            ViewBag.Departments = new SelectList(db.GetDepartments(user.DepartmentId), "Id", "Name");
            return View(new CalcViewModel
            {
                StructuralDepartmentId = db.GetDepartment(user.DepartmentId).Id,
                PPIViewModel = new List<PPIViewModel>(),
                DatabaseHelper = db
            });
        }

        [HttpPost]
        public ActionResult СalcByDepartmentName(CalcViewModel model)
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Departments = new SelectList(db.GetDepartments(user.DepartmentId), "Id", "Name");
            CalcViewModel resultModel = new CalcViewModel
            {
                StructuralDepartmentId = db.GetDepartment(user.DepartmentId).Id,
                PPIViewModel = db.GetPPIViewModelByDepartment(db.GetDepartment(model.DepartmentId)),
                DatabaseHelper = db
            };
            return View(resultModel);
        }

        [HttpGet]
        public ActionResult СalcByStructuralDepartmentName()
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            InitDepartmentsOnViewBag();
            return View(new CalcViewModel
            {
                DepartmentId = db.GetDepartmentByParentId().Id,
                PPIViewModel = new List<PPIViewModel>()
            });
        }

        [HttpPost]
        public ActionResult СalcByStructuralDepartmentName(CalcViewModel model)
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            InitDepartmentsOnViewBag();
            CalcViewModel resultModel = new CalcViewModel
            {
                DepartmentId = model.DepartmentId,
                PPIViewModel = db.GetPPIViewModelByDepartment(db.GetDepartment(model.DepartmentId))
            };
            return View(resultModel);
        }

        [HttpGet]
        public ActionResult СalcByPPIName()
        {
            return View();
        }

        #region PartialViews

        [HttpGet]
        public PartialViewResult DepartmentList(int id)
        {
            return PartialView(db.GetDepartments(id));
        }

        #endregion

        #region Helpers

        private void InitDepartmentsOnViewBag()
        {
            ViewBag.StructuralDepartments = new SelectList(db.GetDepartments(), "Id", "Name");
            ViewBag.Departments = new SelectList(db.GetDepartments(db.GetDepartmentByParentId().Id), "Id", "Name");
        }

        #endregion
    }
}