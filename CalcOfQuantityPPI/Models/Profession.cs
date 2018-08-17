using System.ComponentModel.DataAnnotations;

namespace CalcOfQuantityPPI.Models
{
    public class Profession
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}