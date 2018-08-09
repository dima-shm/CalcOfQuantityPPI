using System.ComponentModel.DataAnnotations;

namespace CalcOfQuantityPPI.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Head { get; set; }
        public Department ParentDepartment { get; set; }
    }
}