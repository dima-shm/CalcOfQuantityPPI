using CalcOfQuantityPPI.Data;
using System.Collections.Generic;

namespace CalcOfQuantityPPI.ViewModels.Request
{
    public class EditRequestViewModel
    {
        public int? RequestId { get; set; }

        public int? DepartmentId { get; set; }

        public List<ProfessionViewModel> ProfessionViewModelList { get; set; }

        public DatabaseHelper DatabaseHelper { get; set; }
    }
}