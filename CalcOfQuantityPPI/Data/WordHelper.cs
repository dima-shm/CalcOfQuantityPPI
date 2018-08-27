using Word = Microsoft.Office.Interop.Word;
using System.Web;
using CalcOfQuantityPPI.ViewModels.Request;
using CalcOfQuantityPPI.Models;
using System;
using System.Collections.Generic;
using CalcOfQuantityPPI.ViewModels.Calc;

namespace CalcOfQuantityPPI.Data
{
    public class WordHelper
    {
        private DatabaseHelper db;
        private string templateFileName = HttpContext.Current.Server.MapPath("~/App_Data/Template.docx");

        public WordHelper()
        {
            db = new DatabaseHelper();
        }

        public void CreateFile(RequestViewModel model)
        {
            List<PPIViewModel> allPPIInDepartment = db.GetPPIViewModelByDepartment(db.GetDepartment(model.DepartmentId));
            Word.Application wordApp = new Word.Application();       
            Word.Document wordDocument = new Word.Document();
            try
            {
                wordApp.Visible = false;
                wordDocument = wordApp.Documents.Open(templateFileName);
                ReplaqceWords(wordDocument, model);
                Word.Table table = wordDocument.Tables[2];
                AddColumns(table, allPPIInDepartment);
                AddRows(table, model);
                FillTable(table, model, allPPIInDepartment);
                wordDocument.SaveAs(HttpContext.Current.Server.MapPath("~/App_Data/Result.docx"));
            }
            catch (Exception ex) { }
            finally
            {
                wordDocument.Close();
                wordApp.Quit();
            }
        }

        private void ReplaqceWords(Word.Document wordDocument, RequestViewModel requestViewModel)
        {
            ReplaqceWordStub("{department}", db.GetDepartment(requestViewModel.DepartmentId).Name, wordDocument);
            ReplaqceWordStub("{year}", DateTime.Now.Year.ToString(), wordDocument);
        }

        private void ReplaqceWordStub(string stubToReplace, string text, Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }

        private void AddColumns(Word.Table table, List<PPIViewModel> allPPIInDepartment)
        {
            for (int j = 0; j < allPPIInDepartment.Count; j++)
            {
                table.Columns.Add();
                table.Cell(1, j + 4).Range.Text = allPPIInDepartment[j].PPIName;
            }
            SplitColumns(table, allPPIInDepartment);
        }

        private void SplitColumns(Word.Table table, List<PPIViewModel> allPPIInDepartment)
        {
            for (int j = 0; j < allPPIInDepartment.Count * 2; j++)
            {
                table.Cell(2, j + 4).Range.Text = "Количество, всего";
                if ((j % 2) == 0)
                {
                    table.Cell(2, j + 4).Split(1, 2);
                    table.Cell(2, j + 4).Range.Text = "Количество, на одного работника";
                }
            }
        }

        private void AddRows(Word.Table table, RequestViewModel requestViewModel)
        {
            for (int i = 0; i < requestViewModel.ProfessionViewModelList.Count; i++)
            {
                table.Rows.Add();
                table.Cell(i + 3, 1).Range.Text = (i + 1).ToString();
                table.Cell(i + 3, 2).Range.Text = requestViewModel.ProfessionViewModelList[i].ProfessionName;
                table.Cell(i + 3, 3).Range.Text = requestViewModel.ProfessionViewModelList[i].EmployeesQuantity.ToString();
            }
        }

        private void FillTable(Word.Table table, RequestViewModel requestViewModel, List<PPIViewModel> allPPIInDepartment)
        {
            for (int i = 0; i < requestViewModel.ProfessionViewModelList.Count; i++)
            {
                for (int j = 0; j < allPPIInDepartment.Count; j++)
                {
                    for (int k = 0; k < requestViewModel.ProfessionViewModelList[i].QuantityOfPPI.Length; k++)
                    {
                        if (allPPIInDepartment[j].PPIName == requestViewModel.ProfessionViewModelList[i].QuantityOfPPI[k].PersonalProtectiveItemName)
                        {
                            table.Cell(i + 3, j + j + 4).Range.Text = 
                                requestViewModel.ProfessionViewModelList[i].QuantityOfPPI[k].QuantityForOneEmployee.ToString();
                            table.Cell(i + 3, j + j + 5).Range.Text = 
                                requestViewModel.ProfessionViewModelList[i].QuantityOfPPI[k].TotalQuantity.ToString();
                        }
                    }
                }
            }
        }
    }
}