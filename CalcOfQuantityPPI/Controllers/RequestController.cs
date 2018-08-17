using CalcOfQuantityPPI.Data;
using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels.Request;
using System;
using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    public class RequestController : Controller
    {
        private DatabaseHelper db;

        public RequestController()
        {
            db = new DatabaseHelper();
        }

        [HttpGet]
        public ActionResult CreateRequest()
        {
            ViewBag.ParentDepartments = new SelectList(db.GetDepartments(), "Id", "Name"); ;
            ViewBag.SubsidiaryDepartments = new SelectList(db.GetDepartments(db.GetDepartmentByParentId(null).Id), "Id", "Name"); ;
            return View(new RequestViewModel());
        }

        [HttpPost]
        public ActionResult CreateRequest(RequestViewModel model)
        {
            if (!isEmptyPPIInModel(model))
            {
                AddRequest(model);
                return View("RequestSuccess");
            }
            else
            {
                return View("RequestFailed");
            }
        }

        #region Partial Views

        [HttpGet]
        public PartialViewResult SubsidiaryDepartmentList(int id)
        {
            return PartialView(db.GetDepartments(id));
        }

        [HttpGet]
        public PartialViewResult ProfessionsAndPPITable(int id)
        {
            RequestViewModel model = new RequestViewModel
            {
                ProfessionViewModelList = db.GetProfessionViewModelListByDepartmentId(id)
            };
            return PartialView(model);
        }

        #endregion

        #region Helpers

        private bool isEmptyPPIInModel(RequestViewModel model)
        {
            return CountOfPPIInModel(model) == 0;
        }

        private int CountOfPPIInModel(RequestViewModel model)
        {
            int count = 0;
            foreach (ProfessionViewModel profession in model.ProfessionViewModelList)
                if (profession.EmployeesQuantity != 0)
                    foreach (QuantityOfPPIViewModel quantityOfPPI in profession.QuantityOfPPI)
                        if (quantityOfPPI.QuantityForOneEmployee != 0)
                            count++;
            return count;
        }

        private void AddRequest(RequestViewModel model)
        {
            Department department = db.GetDepartment(model.DepartmentId);
            Request request = ConstructRequest(department.Id);
            db.AddRequest(request);
            foreach (ProfessionViewModel profession in model.ProfessionViewModelList)
            {
                if (profession.EmployeesQuantity != 0)
                {
                    ProfessionsInRequest professionsInRequest = ConstructProfessionsInRequest(request.Id, profession);
                    db.AddProfessionsInRequest(professionsInRequest);
                    foreach (QuantityOfPPIViewModel quantityOfPPI in profession.QuantityOfPPI)
                    {
                        if (quantityOfPPI.QuantityForOneEmployee != 0)
                        {
                            PPIInRequest ppi = ConstructPPIInRequest(professionsInRequest.Id, quantityOfPPI);
                            db.AddPPIInRequest(ppi);
                        }
                    }
                }
            }
        }

        private Request ConstructRequest(int departmentId)
        {
            Request request = new Request
            {
                Date = DateTime.Now,
                DepartmentId = departmentId
            };
            return request;
        }

        private ProfessionsInRequest ConstructProfessionsInRequest(int requestId, ProfessionViewModel profession)
        {
            ProfessionsInRequest professionsInRequest = new ProfessionsInRequest
            {
                RequestId = requestId,
                ProfessionId = db.GetProfessionByName(profession.ProfessionName).Id,
                EmployeesQuantity = profession.EmployeesQuantity
            };
            return professionsInRequest;
        }

        private PPIInRequest ConstructPPIInRequest(int professionsInRequestId, QuantityOfPPIViewModel quantityOfPPI)
        {
            PPIInRequest ppi = new PPIInRequest
            {
                ProfessionsInRequestId = professionsInRequestId,
                PPIId = db.GetPPIByName(quantityOfPPI.PersonalProtectiveItemName).Id,
                QuantityOfPPI = quantityOfPPI.QuantityForOneEmployee,
                TotalQuantity = quantityOfPPI.TotalQuantity
            };
            return ppi;
        }

        #endregion
    }
}