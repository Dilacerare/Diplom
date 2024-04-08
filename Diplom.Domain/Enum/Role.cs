using System.ComponentModel.DataAnnotations;

namespace Diplom.Domain.Enum;

public enum Role
{
    [Display(Name = "Админ")]
    Admin = 0,
    [Display(Name = "Врач")]
    Doctor = 1,
    [Display(Name = "Пациент")]
    Patient = 2
}