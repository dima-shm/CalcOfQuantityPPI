using CalcOfQuantityPPI.Models;
using System.Collections.Generic;

namespace CalcOfQuantityPPI.ViewModels.Request
{
    public class RequestViewModel
    {
        public int DepartmentId { get; set; }
        public ProfessionsTableViewModel ProfessionsTableViewModel { get; set; }
    }
}