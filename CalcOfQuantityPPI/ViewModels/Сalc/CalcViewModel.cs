﻿using CalcOfQuantityPPI.Data;
using System.Collections.Generic;

namespace CalcOfQuantityPPI.ViewModels.Calc
{
    public class CalcViewModel
    {
        public int StructuralDepartmentId { get; set; }

        public int DepartmentId { get; set; }

        public List<PPIViewModel> PPIViewModel { get; set; }

        public DatabaseHelper DatabaseHelper { get; set; }
    }
}