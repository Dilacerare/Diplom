using System.Security.Cryptography;
using Diplom.Domain.Entity;
using Diplom.Domain.Enum;
using Diplom.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Diplom.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<User> Users { get; set; }

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
                    HashCode = "test",
                    PublickKey = publicKey,
                    PrivateKey = privateKey,
                    Role = Role.Admin
                }
            });

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Login).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Password).IsRequired();
                
        });
    }
}