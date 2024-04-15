namespace Diplom.Domain.Entity;

public class MedicalRecord
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public int Age { get; set; }
    public string BloodType { get; set; }
    public string Allergies { get; set; }
    public string[] Medications { get; set; }
    public string[] MedicalHistory { get; set; }
    
    public MedicalRecord(int id, string patientName, int age, string bloodType, string allergies, string[] medications, string[] medicalHistory)
    {
        Id = id;
        PatientName = patientName;
        Age = age;
        BloodType = bloodType;
        Allergies = allergies;
        Medications = medications;
        MedicalHistory = medicalHistory;
    }
    
    public void DisplayMedicalRecord()
    {
        Console.WriteLine("Patient Name: " + PatientName);
        Console.WriteLine("Age: " + Age);
        Console.WriteLine("Blood Type: " + BloodType);
        Console.WriteLine("Allergies: " + Allergies);
        
        Console.WriteLine("Medications:");
        foreach (string medication in Medications)
        {
            Console.WriteLine("- " + medication);
        }
        
        Console.WriteLine("Medical History:");
        foreach (string entry in MedicalHistory)
        {
            Console.WriteLine("- " + entry);
        }
    }
}