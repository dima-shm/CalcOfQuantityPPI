using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels.Request;
using System.Collections.Generic;
using System.Linq;

namespace CalcOfQuantityPPI.Data
{
    public class DatabaseHelper
    {
        private ApplicationContext _context = new ApplicationContext();

        public List<Department> GetDepartments(int? parentDepartmentId = null)
        {
            return _context.Departments.Where(p => p.ParentDepartmentId == parentDepartmentId).ToList();
        }

        public Department GetDepartment(int? id)
        {
            return _context.Departments.Find(id);
        }

        public Department GetDepartmentByParentId(int? parentId = null)
        {
            return _context.Departments.FirstOrDefault(d => d.ParentDepartmentId == parentId);
        }

        public List<ProfessionViewModel> GetProfessionViewModelListByDepartmentId(int? departmentId)
        {
            List<ProfessionViewModel> professionViewModelList = new List<ProfessionViewModel>();
            List<PersonalProtectiveItem> ppi = new List<PersonalProtectiveItem>();
            foreach (Profession p in GetProfessionsByDepartmentId(departmentId))
            {
                ppi = GetPPIByProfessionId(p.Id);
                professionViewModelList.Add(new ProfessionViewModel
                {
                    ProfessionName = _context.Professions.Find(p.Id).Name,
                    EmployeesQuantity = 0,
                    QuantityOfPPI = GetQuantityOfPPIViewModel(ppi)
                });
            }
            return professionViewModelList;
        }

        private List<Profession> GetProfessionsByDepartmentId(int? departmentId)
        {
            List<Profession> professions = new List<Profession>();
            if (isParentDepartment(GetDepartment(departmentId)))
            {
                professions = GetProfessionsById(GetDepartmentByParentId(departmentId).Id);
            }
            else
            {
                professions = GetProfessionsById(departmentId);
            }
            return professions;
        }

        private bool isParentDepartment(Department department)
        {
            return department.ParentDepartmentId == null;
        }

        private List<Profession> GetProfessionsById(int? departmentId)
        {
            List<Profession> professions = new List<Profession>();
            foreach (ProfessionsInDepartment profession in GetProfessionsInDepartment(departmentId))
            {
                professions.Add(_context.Professions.Find(profession.ProfessionId));
            }
            return professions;
        }

        private List<ProfessionsInDepartment> GetProfessionsInDepartment(int? departmentId)
        {
            return _context.ProfessionsInDepartment.Where(c => c.DepartmentId == departmentId).ToList();
        }

        private List<PersonalProtectiveItem> GetPPIByProfessionId(int? professionId)
        {
            List<PersonalProtectiveItem> ppiList = new List<PersonalProtectiveItem>();
            foreach (PPIForProfession ppi in GetPPIForProfession(professionId))
            {
                ppiList.Add(_context.PersonalProtectiveItems.Find(ppi.PPIId));
            }
            return ppiList;
        }

        private List<PPIForProfession> GetPPIForProfession(int? professionId)
        {
            return _context.PPIForProfession.Where(p => p.ProfessionId == professionId).ToList();
        }

        private QuantityOfPPIViewModel[] GetQuantityOfPPIViewModel(List<PersonalProtectiveItem> ppiList)
        {
            List<QuantityOfPPIViewModel> QuantityOfPPI = new List<QuantityOfPPIViewModel>();
            foreach (PersonalProtectiveItem ppi in ppiList)
            {
                QuantityOfPPI.Add(new QuantityOfPPIViewModel{ PersonalProtectiveItemName = ppi.Name});
            }
            return QuantityOfPPI.ToArray();
        }

    }
}