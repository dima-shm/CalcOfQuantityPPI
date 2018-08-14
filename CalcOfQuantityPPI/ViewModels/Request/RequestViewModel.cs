using CalcOfQuantityPPI.Models;
using System.Collections.Generic;

namespace CalcOfQuantityPPI.ViewModels.Request
{
    public class RequestViewModel
    {
        public int ParentDepartment { get; set; }
        public int SubsidiaryDepartment { get; set; }
        public ProfessionsTableViewModel ProfessionsTableViewModel { get; set; }
    }
}