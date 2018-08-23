using System.ComponentModel.DataAnnotations;

namespace CalcOfQuantityPPI.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Фамилия и инициалы")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтвердите пароль")]
        public string PasswordConfirm { get; set; }

        public string Role { get; set; }

        public int StructuralDepartmentId { get; set; }

        public int DepartmentId { get; set; }
    }
}