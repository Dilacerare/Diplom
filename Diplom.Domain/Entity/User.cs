using Diplom.Domain.Enum;

namespace Diplom.Domain.Entity;

public class User
{
    public long Id { get; set; }

    public string Login { get; set; }
    
    public string Password { get; set; }
    
    public string HashCode { get; set; }
    
    public string PublickKey { get; set; }
    
    public string PrivateKey { get; set; }
    
    public Role Role { get; set; }
}