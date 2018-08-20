using Microsoft.AspNet.Identity.EntityFramework;

namespace CalcOfQuantityPPI.Models
{
    public class Role : IdentityRole
    {
        public Role()
        {
        }

        public string Description { get; set; }
    }
}