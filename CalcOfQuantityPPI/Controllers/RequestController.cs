using CalcOfQuantityPPI.Data;
using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    public class RequestController : Controller
    {
        private ApplicationContext _context = new ApplicationContext();

        [HttpGet]
        public ActionResult CreateRequest()
        {
            SelectList parentDepartments = 
                new SelectList(_context.Departments.Where(p => p.ParentDepartment == null), "Id", "Name");
            ViewBag.ParentDepartment = parentDepartments;
            SelectList subsidiaryDepartments = 
                new SelectList(_context.Departments.Where(c => c.ParentDepartment.Id == _context.Departments.FirstOrDefault(p => p.ParentDepartment == null).Id), "Id", "Name");
            ViewBag.SubsidiaryDepartment = subsidiaryDepartments;
            return View(new RequestViewModel());
        }

        [HttpPost]
        public string CreateRequest(RequestViewModel model)
        {
            string s = "<b>Подразделение:</b> " + _context.Departments.Find(model.ParentDepartment).Name + "<br />" +
                "<b>Дочернее подразделение:</b> " + _context.Departments.Find(model.SubsidiaryDepartment).Name + "<br /><br />";
            for (int k = 0; k < model.ProfessionsTableViewModel.ProfessionId.Count(); k++)
            {
                //int? professionId = model.ProfessionsTableViewModel.Professions.Select(p => p.Id).ElementAt(k);
                int? professionId = model.ProfessionsTableViewModel.ProfessionId.ElementAt(k);
                s += "<b>Профессия: " + _context.Professions.Find(professionId).Name + "</b> " +
                    "<i>(Численность: " + model.ProfessionsTableViewModel.QuantityOfEmployees.ElementAt(k) + ")</i><br />";
                try
                {
                    List<int?> ppiIdList = _context.PPIForProfession.Where(p => p.ProfessionId == professionId).Select(p => p.PPIId).ToList();
                    for (int i = 0; i < ppiIdList.Count(); i++)
                    {
                        s += "<i>" + _context.PersonalProtectiveItems.Find(ppiIdList.ElementAt(i)).Name + "</i>: ";
                        s += "<b>" + model.ProfessionsTableViewModel.QuantityOfPPIForOneEmployee.ElementAt(k + i) + "</b>";
                        s += " Всего: <i>" + model.ProfessionsTableViewModel.TotalQuantityOfPPI.ElementAt(k + i) + "</i><br />";
                    };
                }
                catch (Exception ex) { s += "Exception" + ex.Message + "<br />"; }
                s += "<br />";
            }
            return s;
        }

        #region Partial Views

        [HttpGet]
        public PartialViewResult SubsidiaryDepartmentList(int departmentId)
        {
            return PartialView(_context.Departments.Where(c => c.ParentDepartment.Id == departmentId).ToList());
        }

        [HttpGet]
        public PartialViewResult ProfessionsAndPPITable(int departmentId)
        {
            RequestViewModel model = new RequestViewModel
            {
                ProfessionsTableViewModel = new ProfessionsTableViewModel
                {
                    Professions = GetProfessionsByDepartmentId(departmentId),
                    PPIForProfession = GetPPIForProfession(),
                    PPI = GetPersonalProtectiveItems()
                }
            };
            return PartialView(model);
        }

        #endregion

        #region Helpers

        private List<Profession> GetProfessionsByDepartmentId(int id)
        {
            List<Profession> professions = new List<Profession>();
            foreach (ProfessionsInDepartment profession in _context.ProfessionsInDepartment.Where(c => c.DepartmentId == id).ToList())
            {
                professions.Add(_context.Professions.Find(profession.ProfessionId));
            }
            return professions;
        }

        private List<PPIForProfession> GetPPIForProfession()
        {
            return _context.PPIForProfession.ToList();
        }

        private List<PersonalProtectiveItem> GetPersonalProtectiveItems()
        {
            return _context.PersonalProtectiveItems.ToList();
        }

        #endregion
    }
}