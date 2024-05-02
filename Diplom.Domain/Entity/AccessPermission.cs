using Diplom.Domain.Enum;

namespace Diplom.Domain.Entity;

public class AccessPermission
{
    public long Id { get; set; }
    public string Initiator { get; set; }
    public string Addressee { get; set; }
    public AccessStatus Status { get; set; }
    public DateTime Date { get; set; }
}