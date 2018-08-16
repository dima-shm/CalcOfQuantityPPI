using System.Collections.Generic;

namespace CalcOfQuantityPPI.ViewModels.Request
{
    public class RequestViewModel
    {
        public int DepartmentId { get; set; }
        public List<ProfessionViewModel> ProfessionViewModelList { get; set; }
    }
}