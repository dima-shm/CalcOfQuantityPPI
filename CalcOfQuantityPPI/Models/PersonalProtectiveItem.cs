using System.ComponentModel.DataAnnotations;

namespace CalcOfQuantityPPI.Models
{
    public class PersonalProtectiveItem
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string ProtectionClass { get; set; }

        public string WearPeriod { get; set; }
    }
}