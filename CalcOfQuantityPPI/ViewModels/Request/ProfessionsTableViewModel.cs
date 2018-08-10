using CalcOfQuantityPPI.Models;
using System.Collections.Generic;

namespace CalcOfQuantityPPI.ViewModels.Request
{
    public class ProfessionsTableViewModel
    {
        public IEnumerable<Profession> Professions { get; set; }
        public IEnumerable<PPIForProfession> PPIForProfession { get; set; }
        public IEnumerable<PersonalProtectiveItem> PPI { get; set; }
    }
}