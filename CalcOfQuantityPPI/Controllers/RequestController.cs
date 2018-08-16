using CalcOfQuantityPPI.Data;
using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels.Request;
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
                new SelectList(_context.Departments.Where(p => p.ParentDepartmentId == null), "Id", "Name");
            ViewBag.ParentDepartmentList = parentDepartments;
            SelectList subsidiaryDepartments =
                new SelectList(_context.Departments.Where(c => c.ParentDepartmentId == _context.Departments.FirstOrDefault(p => p.ParentDepartmentId == null).Id), "Id", "Name");
            ViewBag.SubsidiaryDepartmentList = subsidiaryDepartments;
            return View(new RequestViewModel());
        }

        [HttpPost]
        public string CreateRequest(RequestViewModel model)
        {
            Department department = _context.Departments.Find(model.DepartmentId);
            string s = "<b>Подразделение:</b> " + _context.Departments.FirstOrDefault(d => d.Id == department.ParentDepartmentId).Name + "<br />"
                + "<b>Дочернее подразделение:</b> " + _context.Departments.Find(model.DepartmentId).Name + "<br /><br />";
            for (int i = 0; i < model.ProfessionViewModelList.Count; i++)
            {
                if (model.ProfessionViewModelList[i].EmployeesQuantity != 0)
                {
                    s += "<b>" + model.ProfessionViewModelList[i].ProfessionName + "</b>"
                    + "<i>(Численность: " + model.ProfessionViewModelList[i].EmployeesQuantity + ")</i><br />";
                    for (int j = 0; j < model.ProfessionViewModelList[i].QuantityOfPPI.Length; j++)
                    {
                        if (model.ProfessionViewModelList[i].QuantityOfPPI[j].QuantityForOneEmployee != 0)
                        {
                            s += "<i>" + model.ProfessionViewModelList[i].QuantityOfPPI[j].PersonalProtectiveItemName + "</i> - "
                                + "<b>" + model.ProfessionViewModelList[i].QuantityOfPPI[j].QuantityForOneEmployee + "</b> - "
                                + "Всего:" + (model.ProfessionViewModelList[i].QuantityOfPPI[j].TotalQuantity) + "<br />";
                        }
                    }
                }
            }
            return s;
        }

        #region Partial Views

        [HttpGet]
        public PartialViewResult SubsidiaryDepartmentList(int id)
        {
            return PartialView(_context.Departments.Where(c => c.ParentDepartmentId == id).ToList());
        }

        [HttpGet]
        public PartialViewResult ProfessionsAndPPITable(int id)
        {
            RequestViewModel model = new RequestViewModel
            {
                ProfessionViewModelList = GetProfessionViewModelListByDepartmentId(id)
            };
            return PartialView(model);
        }

        #endregion

        #region Helpers

        private List<Profession> GetProfessionsByDepartmentId(int id)
        {
            List<Profession> professions = new List<Profession>();
            Department department = _context.Departments.Find(id);
            if (isParentDepartment(department))
            {
                int? subsidiaryDepartmentId = GetFirstSubsidiaryDepartamentIdByParentDepartmentId(id);
                professions = GetProfessionsById(subsidiaryDepartmentId);
            }
            else
            {
                professions = GetProfessionsById(id);
            }
            return professions;
        }

        private bool isParentDepartment(Department department)
        {
            return department.ParentDepartmentId == null;
        }

        private List<Profession> GetProfessionsById(int? id)
        {
            List<Profession> professionList = new List<Profession>();
            foreach (ProfessionsInDepartment profession in _context.ProfessionsInDepartment.Where(c => c.DepartmentId == id).ToList())
            {
                professionList.Add(_context.Professions.Find(profession.ProfessionId));
            }
            return professionList;
        }

        private int? GetFirstSubsidiaryDepartamentIdByParentDepartmentId(int id)
        {
            return _context.Departments.FirstOrDefault(d => d.ParentDepartmentId == id).Id;
        }

        private List<ProfessionViewModel> GetProfessionViewModelListByDepartmentId(int id)
        {
            List<ProfessionViewModel> professionViewModelList = new List<ProfessionViewModel>();
            List<Profession> professions = GetProfessionsByDepartmentId(id);
            List<PersonalProtectiveItem> ppiList;
            foreach (Profession p in professions)
            {
                ppiList = GetPPIByProfessionId(p.Id);
                professionViewModelList.Add(new ProfessionViewModel
                {
                    ProfessionName = _context.Professions.Find(p.Id).Name,
                    EmployeesQuantity = 0,
                    QuantityOfPPI = GetQuantityOfPPIViewModel(ppiList)
                });
            }
            return professionViewModelList;
        }

        private List<PersonalProtectiveItem> GetPPIByProfessionId(int id)
        {
            List<PersonalProtectiveItem> ppiList = new List<PersonalProtectiveItem>();
            foreach (PPIForProfession ppi in _context.PPIForProfession.Where(p => p.ProfessionId == id).ToList())
            {
                ppiList.Add(_context.PersonalProtectiveItems.Find(ppi.PPIId));
            }
            return ppiList;
        }

        private QuantityOfPPIViewModel[] GetQuantityOfPPIViewModel(List<PersonalProtectiveItem> ppiList)
        {
            List<QuantityOfPPIViewModel> QuantityOfPPI = new List<QuantityOfPPIViewModel>();
            foreach (PersonalProtectiveItem ppi in ppiList)
            {
                QuantityOfPPI.Add(new QuantityOfPPIViewModel
                {
                    PersonalProtectiveItemName = ppi.Name
                });
            }
            return QuantityOfPPI.ToArray();
        }

        #endregion
    }
}