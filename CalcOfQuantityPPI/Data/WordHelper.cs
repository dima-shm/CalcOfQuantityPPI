using System.Web;
using CalcOfQuantityPPI.ViewModels.Request;
using System;
using System.Collections.Generic;
using CalcOfQuantityPPI.ViewModels.Calc;
using Xceed.Words.NET;

namespace CalcOfQuantityPPI.Data
{
    public class WordHelper
    {
        private DatabaseHelper db;

        public const string Path = "~/App_Data/";

        private string templateFileName = HttpContext.Current.Server.MapPath(Path + "Template.docx");

        public WordHelper()
        {
            db = new DatabaseHelper();
        }

        public void CreateFile(RequestViewModel model)
        {
            List<PPIViewModel> allPPIInDepartment = db.GetPPIViewModelByDepartment(db.GetDepartment(model.DepartmentId));

            DocX document = DocX.Load(templateFileName);
            ReplaqceWords(document, model);
            Table table = document.Tables[0];
            AddColumns(table, allPPIInDepartment);
            AddRows(table, model);
            FillTable(table, model, allPPIInDepartment);
            document.SaveAs(HttpContext.Current.Server.MapPath(Path + "Result.docx"));
        }

        private void ReplaqceWords(DocX document, RequestViewModel requestViewModel)
        {
            document.ReplaceText("{department}",
                db.GetDepartment(requestViewModel.DepartmentId).Name);
            document.ReplaceText("{year}",
                DateTime.Now.Year.ToString());
        }

        private void AddColumns(Table table, List<PPIViewModel> allPPIInDepartment)
        {
            for (int i = 0; i < allPPIInDepartment.Count * 2; i++)
            {
                table.InsertColumn();
            }
            MergeHeaderCells(table, allPPIInDepartment);
        }

        private void MergeHeaderCells(Table table, List<PPIViewModel> allPPIInDepartment)
        {
            for (int i = 0; i < allPPIInDepartment.Count; i++)
            {
                table.Rows[0].MergeCells(i + 3, i + 4);
                table.Rows[0].Cells[i + 3].InsertParagraph(allPPIInDepartment[i].PPIName);
            }
            FillHeader(table, allPPIInDepartment);
        }

        private void FillHeader(Table table, List<PPIViewModel> allPPIInDepartment)
        {
            for (int i = 0; i < allPPIInDepartment.Count * 2; i++)
            {
                if ((i % 2) == 0)
                {
                    table.Rows[1].Cells[i + 3].InsertParagraph("Количество, на одного работника");
                    table.Rows[1].Cells[i + 4].InsertParagraph("Количество, всего");
                }
            }
        }

        private void AddRows(Table table, RequestViewModel requestViewModel)
        {
            for (int i = 0; i < requestViewModel.ProfessionViewModelList.Count; i++)
            {
                table.InsertRow();
                table.Rows[i + 2].Cells[0].InsertParagraph((i + 1).ToString());
                table.Rows[i + 2].Cells[1].InsertParagraph(requestViewModel.ProfessionViewModelList[i].ProfessionName);
                table.Rows[i + 2].Cells[2].InsertParagraph(requestViewModel.ProfessionViewModelList[i].EmployeesQuantity.ToString());
            }
        }

        private void FillTable(Table table, RequestViewModel requestViewModel, List<PPIViewModel> allPPIInDepartment)
        {
            for (int i = 0; i < requestViewModel.ProfessionViewModelList.Count; i++)
            {
                for (int j = 0; j < allPPIInDepartment.Count; j++)
                {
                    for (int k = 0; k < requestViewModel.ProfessionViewModelList[i].QuantityOfPPI.Length; k++)
                    {
                        if (allPPIInDepartment[j].PPIName == requestViewModel.ProfessionViewModelList[i].QuantityOfPPI[k].PersonalProtectiveItemName)
                        {
                            table.Rows[i + 2].Cells[j + j + 3]
                                .InsertParagraph(requestViewModel.ProfessionViewModelList[i].QuantityOfPPI[k].QuantityForOneEmployee.ToString());
                            table.Rows[i + 2].Cells[j + j + 4]
                               .InsertParagraph(requestViewModel.ProfessionViewModelList[i].QuantityOfPPI[k].TotalQuantity.ToString());
                        }
                    }
                }
            }
            CalcResult(table, requestViewModel, allPPIInDepartment);
        }

        private void CalcResult(Table table, RequestViewModel requestViewModel, List<PPIViewModel> allPPIInDepartment)
        {
            AddResultRow(table, requestViewModel);
            for (int j = 0; j < allPPIInDepartment.Count; j++)
            {
                int result = 0;
                for (int i = 0; i < requestViewModel.ProfessionViewModelList.Count; i++)
                {
                    for (int k = 0; k < requestViewModel.ProfessionViewModelList[i].QuantityOfPPI.Length; k++)
                    {
                        if (allPPIInDepartment[j].PPIName == requestViewModel.ProfessionViewModelList[i].QuantityOfPPI[k].PersonalProtectiveItemName)
                        {
                            result +=
                                requestViewModel.ProfessionViewModelList[i].QuantityOfPPI[k].TotalQuantity;
                        }
                    }
                }
                table.Rows[requestViewModel.ProfessionViewModelList.Count + 2].Cells[j + j + 4]
                    .InsertParagraph(result.ToString());
            }
            MergeResultCells(table, requestViewModel);
        }

        private void AddResultRow(Table table, RequestViewModel requestViewModel)
        {
            table.InsertRow();
            table.Rows[requestViewModel.ProfessionViewModelList.Count + 2].Cells[0]
                .InsertParagraph("Итого на год:");
        }

        private void MergeResultCells(Table table, RequestViewModel requestViewModel)
        {
            table.Rows[requestViewModel.ProfessionViewModelList.Count + 2]
                .MergeCells(0, 2);
        }
    }
}