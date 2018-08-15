using CalcOfQuantityPPI.Models;
using System.Data.Entity;

namespace CalcOfQuantityPPI.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<ProfessionsInDepartment> ProfessionsInDepartment { get; set; }
        public DbSet<PersonalProtectiveItem> PersonalProtectiveItems { get; set; }
        public DbSet<PPIForProfession> PPIForProfession { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
}