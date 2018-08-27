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
        private string TemplateFileName = HttpContext.Current.Server.MapPath("~/App_Data/Template.docx");

        public WordHelper()
        {
            db = new DatabaseHelper();
        }

        public void CreateFile(RequestViewModel model)
        {
            Word.Application wordApp = new Word.Application();
            Word.Document wordDocument = new Word.Document();

            try
            {
                wordApp.Visible = false;
                wordDocument = wordApp.Documents.Open(TemplateFileName);

                ReplaqceWordStub("{department}", db.GetDepartment(model.DepartmentId).Name, wordDocument);
                ReplaqceWordStub("{year}", DateTime.Now.Year.ToString(), wordDocument);

                Word.Table _table = wordDocument.Tables[2];

                List<PPIViewModel> allPPIInDepartment = db.GetPPIViewModelByDepartment(db.GetDepartment(model.DepartmentId));
                // Added Columns
                for (int j = 0; j < allPPIInDepartment.Count; j++)
                {
                    _table.Columns.Add();
                    _table.Cell(1, j + 4).Range.Text = allPPIInDepartment[j].PPIName;
                }
                for (int j = 0; j < allPPIInDepartment.Count * 2; j++)
                {
                    _table.Cell(2, j + 4).Range.Text = "Количество, всего";
                    if ((j % 2) == 0)
                    {
                        _table.Cell(2, j + 4).Split(1, 2);
                        _table.Cell(2, j + 4).Range.Text = "Количество, на одного работника";
                    }
                }

                // Added Rows
                for (int i = 0; i < model.ProfessionViewModelList.Count; i++)
                {
                    _table.Rows.Add();
                    _table.Cell(i + 3, 1).Range.Text = (i + 1).ToString();
                    _table.Cell(i + 3, 2).Range.Text = model.ProfessionViewModelList[i].ProfessionName;
                    _table.Cell(i + 3, 3).Range.Text = model.ProfessionViewModelList[i].EmployeesQuantity.ToString();
                }

                //Data
                for (int i = 0; i < model.ProfessionViewModelList.Count; i++)
                {
                    for (int j = 0; j < allPPIInDepartment.Count; j++)
                    {
                        for (int k = 0; k < model.ProfessionViewModelList[i].QuantityOfPPI.Length; k++)
                        {
                            if (allPPIInDepartment[j].PPIName == model.ProfessionViewModelList[i].QuantityOfPPI[k].PersonalProtectiveItemName)
                            {
                                _table.Cell(i + 3, j + j + 4).Range.Text = model.ProfessionViewModelList[i].QuantityOfPPI[k].QuantityForOneEmployee.ToString();
                                _table.Cell(i + 3, j + j + 5).Range.Text = model.ProfessionViewModelList[i].QuantityOfPPI[k].TotalQuantity.ToString();
                            }
                        }
                    }
                }

                wordDocument.SaveAs(HttpContext.Current.Server.MapPath("~/App_Data/Result.docx"));
            }
            catch (Exception ex) { }
            finally
            {
                wordDocument.Close();
                wordApp.Quit();
            }
        }

        private void ReplaqceWordStub(string stubToReplace, string text, Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }
    }
}