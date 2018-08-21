using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalcOfQuantityPPI.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        private Department Department { get; set; }
    }
}