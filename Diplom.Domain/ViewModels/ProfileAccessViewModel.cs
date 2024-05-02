using Diplom.Domain.Entity;

namespace Diplom.Domain.ViewModels;

public class ProfileAccessViewModel
{
    public bool Access { get; set; }
    
    public string Login { get; set; }
    
    public string PatientName { get; set; }
    
    public int Age { get; set; }
    
    public string BloodType { get; set; }
    
    public string Allergies { get; set; }
    
    public List<MedicalReport> MedicalReports { get; set; }
}