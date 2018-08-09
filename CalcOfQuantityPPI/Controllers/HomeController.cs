using CalcOfQuantityPPI.Data;
using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext _context = new ApplicationContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View(_context.Departments.Where(p => p.ParentDepartment == null));
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public string CreateRequest()
        {
            return "CreateRequest";
        }

        #region Partial Views

        [HttpGet]
        public ActionResult SubsidiaryDepartmentList(int id)
        {
            return PartialView(_context.Departments.Where(c => c.ParentDepartment.Id == id).ToList());
        }

        [HttpGet]
        public ActionResult ProfessionsAndPPITable(int id)
        {
            List<Profession> professions = GetProfessionsByDepartmentId(id);
            List<PPIForProfession> ppiForProfession = GetPPIForProfession();
            List<PersonalProtectiveItem> ppi = GetPPIByDepartmentId(id);

            ProfessionsTableModel professionsTableModel = new ProfessionsTableModel
            {
                Professions = professions,
                PPIForProfession = ppiForProfession,
                PPI = _context.PersonalProtectiveItems
            };

            return PartialView(professionsTableModel);
        }

        #endregion

        #region Helpers

        private List<Profession> GetProfessionsByDepartmentId(int id)
        {
            List<ProfessionsInDepartment> professionsInDepartment = _context.ProfessionsInDepartment.Where(c => c.DepartmentId == id).ToList();
            List<Profession> professions = new List<Profession>();
            List<int?> professionsId = new List<int?>();
            for (int i = 0; i < professionsInDepartment.Count; i++)
            {
                professionsId.Add(professionsInDepartment[i].ProfessionId);
                professions.Add(_context.Professions.Find(professionsInDepartment[i].ProfessionId));
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
            List<int?> professionsId2 = new List<int?>();
            for (int i = 0; i < ppiForProfession.Count; i++)
            {
                professionsId2.Add(ppiForProfession[i].PPIId);
                ppi.Add(_context.PersonalProtectiveItems.Find(ppiForProfession[i].ProfessionId));
            }
            return ppi;
        }

        #endregion
    }
}