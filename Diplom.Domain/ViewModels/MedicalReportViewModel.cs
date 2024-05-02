using System.ComponentModel.DataAnnotations;

namespace Diplom.Domain.ViewModels;

public class MedicalReportViewModel
{
    public string Initiator { get; set; }
    
    public string Login { get; set; }
    
    [Required(ErrorMessage = "Укажите диагноз")]
    public string Conclusion { get; set; }
    
    [Required(ErrorMessage = "Укажите описание")]
    public string Description { get; set; }
    
    [Required(ErrorMessage = "Укажите рекоммендации")]
    public string Recommendation { get; set; }
}