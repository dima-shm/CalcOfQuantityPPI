using System.Collections.Generic;

namespace CalcOfQuantityPPI.ViewModels.Calc
{
    public class CalcViewModel
    {
        public string StructuralDepartmentName { get; set; }
        public string DepartmentName { get; set; }
        public List<PPIViewModel> PPIViewModel { get; set; }
    }
}