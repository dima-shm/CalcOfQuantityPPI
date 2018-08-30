using CalcOfQuantityPPI.Data;
using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels.Request;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    public class RequestController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private DatabaseHelper db;

        public RequestController()
        {
            db = new DatabaseHelper();
        }

        [HttpGet]
        public ActionResult CreateRequest()
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            RequestViewModel model = new RequestViewModel
            {
                DepartmentId = user.DepartmentId,
                ProfessionViewModelList = db.GetProfessionViewModelListByDepartmentId(user.DepartmentId),
                DatabaseHelper = db
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateRequest(RequestViewModel model)
        {
            if (!isEmptyPPIInModel(model))
            {
                AddRequest(model);
                new WordHelper().CreateFile(model);
                return View("RequestSuccess");
            }
            else
            {
                return View("RequestFailed");
            }
        }

        [HttpGet]
        public ActionResult RequestList()
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            RequestListViewModel model = new RequestListViewModel
            {
                Requests = db.GetRequests(user.DepartmentId != null ? user.DepartmentId : null),
                DatabaseHelper = db
            };
            model.Requests.Sort((x, y) => y.Date.CompareTo(x.Date));
            return View(model);
        }

        [HttpGet]
        public ActionResult EditRequest(int id)
        {
            EditRequestViewModel model = db.GetEditRequestViewModel(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditRequest(EditRequestViewModel model)
        {
            db.UpdateRequest(model);
            model.DatabaseHelper = db;
            return View(model);
        }

        [HttpGet]
        public ActionResult RemoveRequest(int id)
        {
            db.RemoveRequest(id);
            return View("RequestRemoveSuccess");
        }

        public FileResult GetFile()
        {
            string filePath = Server.MapPath(WordHelper.Path + "Result.docx");
            string fileType = "application/docx";
            string fileName = "Заявка.docx";
            return File(filePath, fileType, fileName);
        }

        #region PartialViews

        [HttpGet]
        public PartialViewResult ProfessionsAndPPITable(int id)
        {
            RequestViewModel model = new RequestViewModel
            {
                ProfessionViewModelList = db.GetProfessionViewModelListByDepartmentId(id)
            };
            return PartialView(model);
        }


        [HttpGet]
        public PartialViewResult EditProfessionsAndPPITable(int id)
        {
            EditRequestViewModel model = db.GetEditRequestViewModel(id);
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
                        if (quantityOfPPI.QuantityForOneEmployee != 0 && quantityOfPPI.TotalQuantity != 0)
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