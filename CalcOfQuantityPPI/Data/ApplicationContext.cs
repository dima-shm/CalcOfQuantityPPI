using CalcOfQuantityPPI.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace CalcOfQuantityPPI.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Department> Departments { get; set; }

        public DbSet<Profession> Professions { get; set; }

        public DbSet<ProfessionsInDepartment> ProfessionsInDepartment { get; set; }

        public DbSet<PersonalProtectiveItem> PersonalProtectiveItems { get; set; }

        public DbSet<PPIForProfession> PPIForProfession { get; set; }

        public DbSet<Request> Requests { get; set; }

        public DbSet<ProfessionsInRequest> ProfessionsInRequest { get; set; }

        public DbSet<PPIInRequest> PPIInRequest { get; set; }

        public ApplicationContext() : base("IdentityDb") { }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
    }
}