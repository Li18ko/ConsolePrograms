using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using TelegramBot.Helper;
using TelegramBot.Model;
using Task = TelegramBot.Model.Task;

namespace TelegramBot;

public class AppDbContext: DbContext {
    public DbSet<User> Users { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Worker" },
            new Role { Id = 3, Name = "Manager" }
        );

        modelBuilder.Entity<User>().HasData(
            Enumerable.Range(1, 13).Select(i => {
                return new User {
                    Id = i,
                    Name = $"User {i}",
                    Login = $"Login{i}",
                    Password = PasswordHelper.HashPassword($"Password{i}", PasswordHelper.GenerateSalt())
                };
            }).ToArray()
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
            Enumerable.Range(1, 3).Select(i => {
                return new UserRole {
                    UserId = i + 10,
                    RoleId = 3
                };
            }).ToArray()
        );
    }

}