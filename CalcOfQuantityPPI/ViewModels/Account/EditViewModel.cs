using CalcOfQuantityPPI.Data;
using CalcOfQuantityPPI.Models;
using System.ComponentModel.DataAnnotations;

namespace CalcOfQuantityPPI.ViewModels.Account
{
    public class EditViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Фамилия и инициалы")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        public Role Role { get; set; }

        public Department Department { get; set; }

        public DatabaseHelper DatabaseHelper { get; set; }
    }
}