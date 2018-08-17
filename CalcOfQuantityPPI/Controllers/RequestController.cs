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
            string result = "";
            if (CountOfPPIInModel(model) != 0)
            {
                Department department = db.GetDepartment(model.DepartmentId);
                Request request = new Request
                {
                    Date = DateTime.Now,
                    DepartmentId = department.Id
                };
                db.AddRequest(request);

                for (int i = 0; i < model.ProfessionViewModelList.Count; i++)
                {
                    if (model.ProfessionViewModelList[i].EmployeesQuantity != 0)
                    {
                        ProfessionsInRequest profession = new ProfessionsInRequest
                        {
                            RequestId = request.Id,
                            ProfessionId = db.GetProfessionByName(model.ProfessionViewModelList[i].ProfessionName).Id,
                            EmployeesQuantity = model.ProfessionViewModelList[i].EmployeesQuantity
                        };
                        db.AddProfessionsInRequest(profession);

                        for (int j = 0; j < model.ProfessionViewModelList[i].QuantityOfPPI.Length; j++)
                        {
                            if (model.ProfessionViewModelList[i].QuantityOfPPI[j].QuantityForOneEmployee != 0)
                            {
                                PPIInRequest ppi = new PPIInRequest
                                {
                                    ProfessionsInRequestId = profession.Id,
                                    PPIId = db.GetPPIByName(model.ProfessionViewModelList[i].QuantityOfPPI[j].PersonalProtectiveItemName).Id,
                                    QuantityOfPPI = model.ProfessionViewModelList[i].QuantityOfPPI[j].QuantityForOneEmployee,
                                    TotalQuantity = model.ProfessionViewModelList[i].QuantityOfPPI[j].TotalQuantity
                                };
                                db.AddPPIInRequest(ppi);
                            }
                        }
                    }
                }
                result = "Операция прошла успешно.";
            }
            else
            {
                result = "Вы не указали количество СИЗ ни для одной професси.";
            }
            return View("RequestResult", model: result);
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

        private int CountOfPPIInModel(RequestViewModel model)
        {
            int count = 0;
            for (int i = 0; i < model.ProfessionViewModelList.Count; i++)
                if (model.ProfessionViewModelList[i].EmployeesQuantity != 0)
                    for (int j = 0; j < model.ProfessionViewModelList[i].QuantityOfPPI.Length; j++)
                        if (model.ProfessionViewModelList[i].QuantityOfPPI[j].QuantityForOneEmployee != 0)
                            count++;
            return count;
        }
    }
}