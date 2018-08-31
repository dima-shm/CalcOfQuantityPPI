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
    public class CalcController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private DatabaseHelper db;

        public CalcController()
        {
            db = new DatabaseHelper();
        }

        [HttpGet]
        public ActionResult CalcByDepartmentName()
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.StructuralDepartments = new SelectList(db.GetDepartments().FindAll(d => d.Id == user.DepartmentId), "Id", "Name");
            ViewBag.Departments = new SelectList(db.GetDepartments(user.DepartmentId), "Id", "Name");
            return View(new CalcViewModel
            {
                StructuralDepartmentId = db.GetDepartment(user.DepartmentId).Id,
                PPIViewModel = new List<PPIViewModel>(),
                DatabaseHelper = db
            });
        }

        [HttpPost]
        public ActionResult CalcByDepartmentName(CalcViewModel model, bool isUseDepartment, int department)
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.StructuralDepartments = new SelectList(db.GetDepartments().FindAll(d => d.Id == user.DepartmentId), "Id", "Name");
            ViewBag.Departments = new SelectList(db.GetDepartments(user.DepartmentId), "Id", "Name");
            CalcViewModel resultModel;
            if (!isUseDepartment)
            {
                resultModel = new CalcViewModel
                {
                    StructuralDepartmentId = db.GetDepartment(user.DepartmentId).Id,
                    PPIViewModel = db.GetPPIViewModelByStructuralDepartment(db.GetDepartment(model.DepartmentId)),
                    DatabaseHelper = db
                };
            }
            else
            {
                resultModel = new CalcViewModel
                {
                    StructuralDepartmentId = db.GetDepartment(user.DepartmentId).Id,
                    DepartmentId = department,
                    PPIViewModel = db.GetPPIViewModelByDepartment(db.GetDepartment(department)),
                    DatabaseHelper = db
                };
            }     
            return View(resultModel);
        }

        [HttpGet]
        public ActionResult CalcByStructuralDepartmentName()
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
        public ActionResult CalcByStructuralDepartmentName(CalcViewModel model)
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
        public ActionResult CalcByPPIName()
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.PersonalProtectiveItems = new SelectList(db.GetPPIByDepartment(db.GetDepartment(user.DepartmentId)), "Id", "Name");
            return View(new CalcViewModel
            {
                DeparmentsViewModel = new List<DeparmentsViewModel>(),
                DatabaseHelper = db
            });
        }

        [HttpPost]
        public ActionResult CalcByPPIName(CalcViewModel model)
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.PersonalProtectiveItems = new SelectList(db.GetPPIByDepartment(db.GetDepartment(user.DepartmentId)), "Id", "Name");
            CalcViewModel resultModel = new CalcViewModel
            {
                DeparmentsViewModel = db.GetDeparmentsViewModelByDepartmentAndPPIId(db.GetDepartment(user.DepartmentId), model.PPIId),
                DatabaseHelper = db
            };
            return View(resultModel);
        }

        [HttpGet]
        public ActionResult CalcByPPINameAndStructuralDepartmentName()
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.StructuralDepartments = new SelectList(db.GetDepartments(), "Id", "Name");
            ViewBag.PersonalProtectiveItems = new SelectList(db.GetPPIByDepartment(db.GetDepartmentByParentId(user.DepartmentId)), "Id", "Name");
            return View(new CalcViewModel
            {
                StructuralDepartmentId = db.GetDepartmentByParentId(user.DepartmentId).Id,
                DeparmentsViewModel = new List<DeparmentsViewModel>(),
                DatabaseHelper = db
            });
        }

        [HttpPost]
        public ActionResult CalcByPPINameAndStructuralDepartmentName(CalcViewModel model)
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.StructuralDepartments = new SelectList(db.GetDepartments(), "Id", "Name");
            ViewBag.PersonalProtectiveItems = new SelectList(db.GetPPIByDepartment(db.GetDepartment(model.StructuralDepartmentId)), "Id", "Name");
            CalcViewModel resultModel = new CalcViewModel
            {
                StructuralDepartmentId = db.GetDepartmentByParentId(model.StructuralDepartmentId).Id,
                PPIId = model.PPIId,
                DeparmentsViewModel = db.GetDeparmentsViewModelByDepartmentAndPPIId(db.GetDepartment(model.StructuralDepartmentId), model.PPIId),
                DatabaseHelper = db
            };
            return View(resultModel);
        }

        #region PartialViews

        [HttpGet]
        public PartialViewResult DepartmentList(int id)
        {
            return PartialView(db.GetDepartments(id));
        }

        [HttpGet]
        public PartialViewResult PPIList(int id)
        {
            return PartialView(db.GetPPIByDepartment(db.GetDepartment(id)));
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