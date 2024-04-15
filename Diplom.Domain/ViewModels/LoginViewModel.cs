using System.ComponentModel.DataAnnotations;

namespace Diplom.Domain.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите логин")]
    [MaxLength(20, ErrorMessage = "Логин должн иметь длину меньше 20 символов")]
    [MinLength(3, ErrorMessage = "Логин должн иметь длину больше 3 символов")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }
}