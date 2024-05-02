using Diplom.Domain.Enum;

namespace Diplom.Domain.ViewModels;

public class AccessPermissionViewModel
{
    public string Initiator { get; set; }
    public string Addressee { get; set; }
    public AccessStatus Status { get; set; }
    public DateTime Date { get; set; }
}