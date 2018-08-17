using CalcOfQuantityPPI.Data;
using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels.Request;
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
        public string CreateRequest(RequestViewModel model)
        {
            Department department = db.GetDepartment(model.DepartmentId);
            string s = "<b>Подразделение:</b> " + db.GetDepartment(department.ParentDepartmentId).Name + "<br />"
                + "<b>Дочернее подразделение:</b> " + department.Name + "<br /><br />";
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
    }
}