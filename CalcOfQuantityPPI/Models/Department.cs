using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalcOfQuantityPPI.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Head { get; set; }

        public int? ParentDepartmentId { get; set; }
        [ForeignKey("ParentDepartmentId")]
        private Department ParentDepartment { get; set; }
    }
}