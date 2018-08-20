using CalcOfQuantityPPI.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalcOfQuantityPPI.ViewModels.Account
{
    public class EditViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Фамилия и инициалы пользователя")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Роли пользователя")]
        public List<Role> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        public EditViewModel()
        {
            AllRoles = new List<Role>();
            UserRoles = new List<string>();
        }
    }
}