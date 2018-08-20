using Microsoft.AspNet.Identity.EntityFramework;

namespace CalcOfQuantityPPI.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public User()
        {
        }
    }
}