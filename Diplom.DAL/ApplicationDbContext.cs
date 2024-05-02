using System.Security.Cryptography;
using Diplom.Domain.Entity;
using Diplom.Domain.Enum;
using Diplom.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Buffer = Diplom.Domain.Entity.Buffer;

namespace Diplom.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Buffer> Buffers { get; set; }
    
    public DbSet<AccessPermission> AccessPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("Users").HasKey(x => x.Id);
            
            string publicKey;
            string privateKey;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                // Получение публичного и приватного ключей в виде строк
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
            }
                
            builder.HasData(new User[]
            {
                new User()
                {
                    Id = 1,
                    Login = "Admin",
                    Password = HashPasswordHelper.HashPassword("123456"),
                    HashCode = "0015r7L0AvmEpgJd7jeMEVr4TecnXdZ700Ru+mxwA4A=",
                    // HashCode = "test",
                    PublickKey = "<RSAKeyValue><Modulus>vA96FyTLc5xhPkfz6i3Ghe3O3y/qCefPGjpj6rIh5QhryDTsxvjzZoopGxMbCex0KN5hbgSJ9sIzvRD+pRio/lDBqxclXoPJ+mRl3cldhdZFjBl0mrDdcAI6Zktwbd3JfN2uNFW5jd3jql6YsYUKbYYpYq57giWsJ/hL68Fo+N6Chox7+3uH8Lthaiji32mwpTeMNvoo7QKz+vKh1mUhWHGVjRKaNzq4RyqT62H+XFw4lYZf/nPmioG0LuGw/3P2F9IkWEB4RAnwcocq9Xkl+EI/42NMCR9s5SwOD1KW1T9+NhULCrTzZgLCoqHy5xCFABxGNHFcSfk3qrjnZJ09cQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                    // PublickKey = publicKey,
                    PrivateKey = "<RSAKeyValue><Modulus>vA96FyTLc5xhPkfz6i3Ghe3O3y/qCefPGjpj6rIh5QhryDTsxvjzZoopGxMbCex0KN5hbgSJ9sIzvRD+pRio/lDBqxclXoPJ+mRl3cldhdZFjBl0mrDdcAI6Zktwbd3JfN2uNFW5jd3jql6YsYUKbYYpYq57giWsJ/hL68Fo+N6Chox7+3uH8Lthaiji32mwpTeMNvoo7QKz+vKh1mUhWHGVjRKaNzq4RyqT62H+XFw4lYZf/nPmioG0LuGw/3P2F9IkWEB4RAnwcocq9Xkl+EI/42NMCR9s5SwOD1KW1T9+NhULCrTzZgLCoqHy5xCFABxGNHFcSfk3qrjnZJ09cQ==</Modulus><Exponent>AQAB</Exponent><P>119T+JfBcxAf0WqIWsBebaNneMZITqNsBw4clL+4rtemUMGp8jHWWctOHHNH7GoO0avahXwgMJxnysTcLzmM7Z7vQz+uUtrfSzAee1Vs4u76Zr0fDPh61U79RHivF68kAP1pTGXSM3m4GBq4SNct6iecpjlPqv4EhzKBDPT9XD8=</P><Q>34k2l5x0F5dcReHIX1VXkhqne33/CDhQ9x0Mi/9tEFi8LI3mPoqvzRwkspBkPl7NhAyo1yDtCVOD9PbehmoP0dFOdx7Zla/iq9U3k8VaJDvOMDVoRZOYpkZxYuWceU/jm6UHdSfaTxueBEdPKfCAmomz70JyLDY1cjiHCxCYuk8=</Q><DP>JUQtd3pq0sobd1UDuxBGRppbsR4+LL1CWAYtE+AIyNgvwxF/opTVDjyLi4i3DUVcwxMFgMt1lnO50fA2WUWQCR3TMMO4GkYdFRmCbLzfVnUbhuN6l/f26Sn90PdA9MwtYq52pe2IbbfGDwWwlYoGO9oW1PxduKyzg+FNSzypCmk=</DP><DQ>vD9aiS1JmwBtxbARxS8iszjdKKN/3dVHYgPFqDRwDZ8cwUyyxKKY0FvOD86HjPrbikP7AEiLNhpt+yLXXUz+i4z/zlNdm7BmbJz/0+MUOYVf67teV5GnsQeLv2RsdMExhcbh0+i+8XXpieLfqQsP0pT6whgr/E2ejtVJ7KiKZgM=</DQ><InverseQ>Lhf2gXeLkMo3g9hOEhrPb/3S+o21h9gCWuuKX8LdSWy0ZBPWyZ/eLG6vbJjin+zAcUQRdZSS1ucbV5Y1X0XgUaBNELD7zqFx2Y7cnNZEEM8ZO8WyxXn7xh/G+3DldNi+CtCYCr3U9DmjewSDRHNGAZjmpw6eKJSFkfjO+4WI1i8=</InverseQ><D>I5xiqCNFi1zfZSXG4F9Oqmm/tK+kB8AnjXXlGbolhPM1RbIP7BWUMaST6BaUFir6TArgNC8T2PApT/H55lVnGtE7+yPk5aLbClkcmQTaes96V+8yD2DSbbVeTaSXY5aN4uEvbaWV/3E2/TnwfB0PPnIbQB5+MMTldqQj7D6xm/5jaEZA2Xk9XzNm24PlJVyrcPpU25ZVBQRIS1KC6V4NPjytgGhDhQLQTGnGFYJu0A11bhh1+Wmx4YvnwC5vhpKYPN9zud/jB1jDuJOGDMGR56LUBNgCxFUQmuLooTkNN+VZB9YiPFTu1/T5fl5DSK4MFN+pVS+XaejO5syaWApIqQ==</D></RSAKeyValue>",
                    // PrivateKey = privateKey,
                    Role = Role.Admin
                }
            });

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Login).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Password).IsRequired();
                
        });
        
        modelBuilder.Entity<Buffer>(builder =>
        {
            builder.ToTable("Buffer").HasKey(x => x.Id);
            
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Login).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.Addressee).IsRequired();
            builder.Property(x => x.OperationName).IsRequired();
                
        });
        
        modelBuilder.Entity<AccessPermission>(builder =>
        {
            builder.ToTable("AccessPermissions").HasKey(x => x.Id);
            
            builder.HasData(new AccessPermission[]
            {
                new AccessPermission()
                {
                    Id = 1,
                    Initiator = "Admin",
                    Addressee = "Dilacerare",
                    Status = AccessStatus.Ended,
                    Date = DateTime.Now
                }
            });
            
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.Initiator).IsRequired();
            builder.Property(x => x.Addressee).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.Date).IsRequired();
                
        });
    }
}