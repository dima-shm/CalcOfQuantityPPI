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
            RequestViewModel model = new RequestViewModel
            {
                Departments = _context.Departments.Where(p => p.ParentDepartment == null)
            };
            return View(model);
        }

        [HttpPost]
        public string CreateRequest(string parentDepartment, bool isUseSubsidiaryDepartment, string subsidiaryDepartment,
            ICollection<int> ProfessionId, ICollection<string> Name, ICollection<string> QuantityOfPPI)
        {
            string s = "<b>Подразделение:</b> " + parentDepartment + "<br />";
            if (isUseSubsidiaryDepartment)
            {
                s += "<b>Дочернее подразделение:</b> " + subsidiaryDepartment + "<br />";
            }
            if (Name != null && QuantityOfPPI != null && ProfessionId != null)
            {
                for (int i = 0; i < Name.Count; i++)
                {
                    s += "  <b>" + ProfessionId.ElementAt(i) + "</b><br />";
                    s += Name.ElementAt(i) + " " + QuantityOfPPI.ElementAt(i) + " " + "<br />";
                }
            }
            return s;
        }

        #region Partial Views

        [HttpGet]
        public PartialViewResult SubsidiaryDepartmentList(int id)
        {
            return PartialView(_context.Departments.Where(c => c.ParentDepartment.Id == id).ToList());
        }

        [HttpGet]
        public PartialViewResult ProfessionsAndPPITable(int id)
        {
            List<Profession> professions = GetProfessionsByDepartmentId(id);
            List<PPIForProfession> ppiForProfession = GetPPIForProfession();
            List<PersonalProtectiveItem> ppi = GetPPIByDepartmentId(id);
            RequestViewModel model = new RequestViewModel
            {
                Departments = _context.Departments.Where(p => p.ParentDepartment == null),
                ProfessionsTableViewModel = new ProfessionsTableViewModel
                {
                    Professions = professions,
                    PPIForProfession = ppiForProfession,
                    PPI = _context.PersonalProtectiveItems
                }
            };
            return PartialView(model);
        }

        #endregion

        #region Helpers

        private List<Profession> GetProfessionsByDepartmentId(int id)
        {
            List<ProfessionsInDepartment> professionsInDepartment = _context.ProfessionsInDepartment.Where(c => c.DepartmentId == id).ToList();
            List<Profession> professions = new List<Profession>();
            List<int?> professionsId = new List<int?>();
            foreach (ProfessionsInDepartment profession in professionsInDepartment)
            {
                professionsId.Add(profession.ProfessionId);
                professions.Add(_context.Professions.Find(profession.ProfessionId));
            }
            return professions;
        }

        private List<PPIForProfession> GetPPIForProfession()
        {
            return _context.PPIForProfession.ToList();
        }

        private List<PersonalProtectiveItem> GetPPIByDepartmentId(int id)
        {
            List<PPIForProfession> ppiForProfession = _context.PPIForProfession.Where(c => c.ProfessionId == id).ToList();
            List<PersonalProtectiveItem> ppi = new List<PersonalProtectiveItem>();
            List<int?> professionsId = new List<int?>();
            foreach (PPIForProfession p in ppiForProfession)
            {
                professionsId.Add(p.PPIId);
                ppi.Add(_context.PersonalProtectiveItems.Find(p.ProfessionId));
            }
            return ppi;
        }

        #endregion
    }
}