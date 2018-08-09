using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalcOfQuantityPPI.Models
{
    public class ProfessionsInDepartment
    {
        [Key]
        public int Id { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        private Department Department { get; set; }

        public int? ProfessionId { get; set; }
        [ForeignKey("ProfessionId")]
        private Profession Profession { get; set; }
    }
}