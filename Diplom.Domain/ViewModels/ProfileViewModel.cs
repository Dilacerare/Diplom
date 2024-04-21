using System.ComponentModel.DataAnnotations;
using Diplom.Domain.Entity;

namespace Diplom.Domain.ViewModels;

public class ProfileViewModel
{
    public string Login { get; set; }
    
    [Required(ErrorMessage = "Укажите ФИО")]
    public string PatientName { get; set; }
    
    [Required(ErrorMessage = "Укажите возраст")]
    [Range(0, 150, ErrorMessage = "Диапазон возраста должен быть от 0 до 150")]
    public int Age { get; set; }
    
    [Required(ErrorMessage = "Группу крови")]
    public string BloodType { get; set; }
    
    [Required(ErrorMessage = "Укажите аллергию или пропишите 'Отсутствует'")]
    public string Allergies { get; set; }
    public List<MedicalReport> MedicalReports { get; set; }
}