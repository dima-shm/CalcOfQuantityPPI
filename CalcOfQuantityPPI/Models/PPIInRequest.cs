using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalcOfQuantityPPI.Models
{
    public class PPIInRequest
    {
        [Key]
        public int Id { get; set; }

        public int? ProfessionsInRequestId { get; set; }
        [ForeignKey("ProfessionsInRequestId")]
        private ProfessionsInRequest ProfessionsInRequest { get; set; }

        public int? PPIId { get; set; }
        [ForeignKey("PPIId")]
        private PersonalProtectiveItem PersonalProtectiveItem { get; set; }

        public int QuantityOfPPI { get; set; }

        public int TotalQuantity { get; set; }
    }
}