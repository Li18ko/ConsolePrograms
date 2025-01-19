using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace WebInstallationOfFloorsApplication;

public class AppDbContext: DbContext {
    public DbSet<User> User { get; set; }
    public DbSet<Task> Task { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Task>()
            .Property(t => t.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Worker" },
            new Role { Id = 3, Name = "Manager" }
        );

        modelBuilder.Entity<User>().HasData(
            Enumerable.Range(1, 5).Select(i => {
                return new User {
                    Id = i,
                    Name = $"User {i}",
                    Login = $"Login{i}",
                    Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes($"Password{i}"))),
                    ChatId = -4681939671
                };
            }).ToArray()
        );
        
        modelBuilder.Entity<User>().HasData(
            Enumerable.Range(1, 5).Select(i => {
                return new User {
                    Id = i + 5,
                    Name = $"User {i + 5}",
                    Login = $"Login{i + 5}",
                    Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes($"Password{i + 5}"))),
                    ChatId = -4701642993
                };
            }).ToArray()
        );
        
        modelBuilder.Entity<User>().HasData(
            new User {
                Id = 11, 
                Name = "User 11",
                Login = "Login11",
                Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes("Password11"))),
                ChatId = -4681939671
            }
        );
        
        modelBuilder.Entity<User>().HasData(
            new User {
                Id = 12, 
                Name = "User 12",
                Login = "Login12",
                Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes("Password12"))),
                ChatId = -4701642993
            }
        );
        
        
        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
        
        modelBuilder.Entity<UserRole>().HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
        modelBuilder.Entity<UserRole>().HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<UserRole>().HasData(
            Enumerable.Range(1, 10).Select(i => {
                return new UserRole {
                    UserId = i,
                    RoleId = 2
                };
            }).ToArray() 
        );

        modelBuilder.Entity<UserRole>().HasData(
            Enumerable.Range(1, 2).Select(i => {
                return new UserRole {
                    UserId = i + 10,
                    RoleId = 3
                };
            }).ToArray()
        );
    }

}