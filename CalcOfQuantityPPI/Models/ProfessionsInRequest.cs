using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalcOfQuantityPPI.Models
{
    public class ProfessionsInRequest
    {
        [Key]
        public int Id { get; set; }

        public int? RequestId { get; set; }
        [ForeignKey("RequestId")]
        private Request Request { get; set; }

        public int? ProfessionId { get; set; }
        [ForeignKey("ProfessionId")]
        private Profession Profession { get; set; }

        public int EmployeesQuantity { get; set; }
    }
}