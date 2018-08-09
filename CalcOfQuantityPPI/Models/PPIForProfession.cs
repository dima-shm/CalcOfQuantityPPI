using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalcOfQuantityPPI.Models
{
    public class PPIForProfession
    {
        [Key]
        public int Id { get; set; }

        public int? ProfessionId { get; set; }
        [ForeignKey("ProfessionId")]
        private Profession Profession { get; set; }

        public int? PPIId { get; set; }
        [ForeignKey("PPIId")]
        private PersonalProtectiveItem PPI { get; set; }
    }
}