using System.ComponentModel.DataAnnotations;

namespace CalcOfQuantityPPI.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Фамилия и инициалы пользователя")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Роль пользователя")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
    }
}