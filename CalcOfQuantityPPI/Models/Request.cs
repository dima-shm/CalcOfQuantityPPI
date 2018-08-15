using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalcOfQuantityPPI.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }
        public string Date { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        private Department Department { get; set; }
    }
}