using System.ComponentModel.DataAnnotations;

namespace CalcOfQuantityPPI.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }
        public string Date { get; set; }
        public Department Department { get; set; }
    }
}