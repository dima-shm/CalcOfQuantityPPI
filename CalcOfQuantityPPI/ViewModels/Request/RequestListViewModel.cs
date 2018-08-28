using CalcOfQuantityPPI.Data;
using System.Collections.Generic;

namespace CalcOfQuantityPPI.ViewModels.Request
{
    public class RequestListViewModel
    {
        public List<Models.Request> Requests { get; set; }

        public DatabaseHelper DatabaseHelper { get; set; }
    }
}