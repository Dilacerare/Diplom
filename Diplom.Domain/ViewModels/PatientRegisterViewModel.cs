using System.ComponentModel.DataAnnotations;

namespace Diplom.Domain.ViewModels;

public class PatientRegisterViewModel
{
    [Required(ErrorMessage = "Укажите имя")]
    [MaxLength(20, ErrorMessage = "Имя должно иметь длину меньше 20 символов")]
    [MinLength(3, ErrorMessage = "Имя должно иметь длину больше 3 символов")]
    public string Login { get; set; }
        
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Укажите пароль")]
    [MinLength(6, ErrorMessage = "Пароль должен иметь длину больше 6 символов")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Подтвердите пароль")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string PasswordConfirm { get; set; }
    
    [Required(ErrorMessage = "Укажите ФИО")]
    public string PatientName { get; set; }
    
    [Required(ErrorMessage = "Укажите возраст")]
    [Range(0, 150, ErrorMessage = "Диапазон возраста должен быть от 0 до 150")]
    public int Age { get; set; }
    
    [Required(ErrorMessage = "Укажите группу крови")]
    public string BloodType { get; set; }
    
    [Required(ErrorMessage = "Укажите аллергию или пропишите 'Отсутствует'")]
    public string Allergies { get; set; }

    public string Initiator { get; set; }
}