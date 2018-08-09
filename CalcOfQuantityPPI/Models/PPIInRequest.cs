using System.ComponentModel.DataAnnotations;

namespace CalcOfQuantityPPI.Models
{
    public class PPIInRequest
    {
        [Key]
        public int Id { get; set; }
        public Request Request { get; set; }
        public PPIForProfession PPIForProfession { get; set; }
        public int Quantity { get; set; }
    }
}