using System.ComponentModel.DataAnnotations;
using Diplom.Domain.Entity;

namespace Diplom.Domain.ViewModels;

public class UserViewModel
{
    [Display(Name = "Id")]
    public long Id { get; set; }
        
    [Required(ErrorMessage = "Укажите логин")]
    [Display(Name = "Логин")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Укажите пароль")]
    [Display(Name = "Пароль")]
    public string Password { get; set; }
    
    [Display(Name = "Публичный ключ")]
    public string PublickKey { get; set; }
    
    [Display(Name = "Приватный ключ")]
    public string PrivateKey { get; set; }
    
    [Required(ErrorMessage = "Укажите роль")]
    [Display(Name = "Роль")]
    public string Role { get; set; }
    
    [Display(Name = "Мед Карта")]
    public MedicalRecord MedicalCard { get; set; }
}