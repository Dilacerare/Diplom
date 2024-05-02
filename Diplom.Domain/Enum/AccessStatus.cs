using System.ComponentModel.DataAnnotations;

namespace Diplom.Domain.Enum;

public enum AccessStatus
{
    [Display(Name = "Запрашивается")]
    Requested = 0,
    [Display(Name = "Отказано")]
    Refused = 1,
    [Display(Name = "Разрешено")]
    Allowed = 2,
    [Display(Name = "Закончился")]
    Ended = 3,
}