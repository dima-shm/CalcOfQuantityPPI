using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels.Calc;
using CalcOfQuantityPPI.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalcOfQuantityPPI.Data
{
    public class DatabaseHelper
    {
        private ApplicationContext _context = new ApplicationContext();

        public List<Department> GetDepartments(int? parentDepartmentId = null)
        {
            return _context.Departments.Where(d => d.ParentDepartmentId == parentDepartmentId).ToList();
        }

        public Department GetDepartment(int? id)
        {
            return _context.Departments.Find(id);
        }

        public Department GetDepartmentByParentId(int? parentDepartmentId = null)
        {
            return _context.Departments.FirstOrDefault(d => d.ParentDepartmentId == parentDepartmentId);
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

        public Profession GetProfessionByName(string professionName)
        {
            return _context.Professions.First(p => p.Name == professionName);
        }

        public PersonalProtectiveItem GetPPIByName(string ppiName)
        {
            return _context.PersonalProtectiveItems.First(p => p.Name == ppiName);
        }

        public void AddRequest(Request request)
        {
            _context.Requests.Add(request);
            _context.SaveChanges();
        }

        public void AddProfessionsInRequest(ProfessionsInRequest professionsInRequest)
        {
            _context.ProfessionsInRequest.Add(professionsInRequest);
            _context.SaveChanges();
        }

        public void AddPPIInRequest(PPIInRequest ppiInRequest)
        {
            _context.PPIInRequest.Add(ppiInRequest);
            _context.SaveChanges();
        }

        public Department GetDepartment(string name)
        {
            return _context.Departments.First(d => d.Name == name);
        }

        public List<PPIViewModel> GetPPIViewModelByDepartment(Department department)
        {
            List<Profession> professionsInDepartment = GetProfessionsById(department.Id);
            List<List<int?>> ppiIdForProfessions = new List<List<int?>>();
            foreach (Profession profession in professionsInDepartment)
            {
                ppiIdForProfessions.Add(_context.PPIForProfession
                    .Where(p => p.ProfessionId == profession.Id).Select(p => p.PPIId).ToList());
            }
            List<PersonalProtectiveItem> personalProtectiveItems = new List<PersonalProtectiveItem>();
            foreach (List<int?> professions in ppiIdForProfessions)
            {
                foreach (int ppiId in professions)
                {
                    personalProtectiveItems.Add(_context.PersonalProtectiveItems.Find(ppiId));
                }
            }
            personalProtectiveItems = personalProtectiveItems.Distinct().ToList();
            List<PPIViewModel> ppiViewModel = new List<PPIViewModel>();
            foreach (PersonalProtectiveItem ppi in personalProtectiveItems)
            {
                ppiViewModel.Add(new PPIViewModel
                {
                    PPIName = ppi.Name,
                    QuantityOfPPI = GetTotalQuantityOfPPIForAYear(department.Id, ppi.Id)
                });
            }
            return ppiViewModel;
        }

        #region PrivateMethods

        private int GetTotalQuantityOfPPIForAYear(int departmentId, int ppiId)
        {
            int quantity = 0;
            List<Request> requests = _context.Requests
                .Where(r => r.DepartmentId == departmentId && r.Date.Year == DateTime.Now.Year).ToList();
            List<List<ProfessionsInRequest>> professionsInRequest = new List<List<ProfessionsInRequest>>();
            foreach (Request r in requests)
            {
                professionsInRequest.Add(_context.ProfessionsInRequest
                    .Where(p => p.RequestId == r.Id).ToList());
            }
            foreach (List<ProfessionsInRequest> profInReq in professionsInRequest)
            {
                foreach (ProfessionsInRequest pInReq in profInReq)
                {
                    foreach (PPIInRequest ppi in _context.PPIInRequest
                        .Where(p => p.ProfessionsInRequestId == pInReq.Id))
                    {
                        if (ppi.PPIId == ppiId)
                        {
                            quantity += ppi.TotalQuantity;
                        }
                    }
                }
            }
            return quantity;
        }

        private List<Profession> GetProfessionsByDepartmentId(int? departmentId)
        {
            List<Profession> professions = new List<Profession>();
            if (isStructuralDepartment(GetDepartment(departmentId)))
            {
                professions = GetProfessionsById(GetDepartmentByParentId(departmentId).Id);
            }
            else
            {
                professions = GetProfessionsById(departmentId);
            }
            return professions;
        }

        private bool isStructuralDepartment(Department department)
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
                QuantityOfPPI.Add(new QuantityOfPPIViewModel { PersonalProtectiveItemName = ppi.Name });
            }
            return QuantityOfPPI.ToArray();
        }

        #endregion
    }
}