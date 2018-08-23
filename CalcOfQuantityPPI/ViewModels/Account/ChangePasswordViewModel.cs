using System.ComponentModel.DataAnnotations;

namespace CalcOfQuantityPPI.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }

        public string Login { get; set; }

        [Required]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }
    }
}