using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace WebInstallationOfFloorsApplication;

public class AppDbContext: DbContext {
    public DbSet<User?> User { get; set; }
    public DbSet<Task> Task { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    public DbSet<Function> Function { get; set; }
    public DbSet<RoleFunction> RoleFunction { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Task>()
            .Property(t => t.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "Имеет все права", Active = true},
            new Role { Id = 2, Name = "Worker", Description = "Обычный работник. Имеет права на добавление/редактирование" +
                                                              "/удаление/просмотр аккаунта и чтение выданных задач", Active = true },
            new Role { Id = 3, Name = "Manager", Description = "Менеджер имеет право добавлять/изменять/удалять/" +
                                                               "просматривать аккаунты/задачи и просматривать роли", Active = true }
        );

        modelBuilder.Entity<Function>().HasData(
            new Function { Id = 1, Code = "UserList", Name = "Чтение пользователей"},
            new Function { Id = 2, Code = "User", Name = "Чтение пользователя"},
            new Function { Id = 3, Code = "UserEdit", Name = "Редактирование пользователя"},
            new Function { Id = 4, Code = "UserDelete", Name = "Удаление пользователя"},
            new Function { Id = 5, Code = "UserAdd", Name = "Добавление пользователя"},
            
            new Function { Id = 6, Code = "TaskList", Name = "Чтение всех задач"},
            new Function { Id = 7, Code = "Task", Name = "Чтение задачи"},
            new Function { Id = 8, Code = "TaskEdit", Name = "Редактирование задачи"},
            new Function { Id = 9, Code = "TaskDelete", Name = "Удаление задачи"},
            new Function { Id = 10, Code = "TaskAdd", Name = "Добавление задачи"},
            
            new Function { Id = 11, Code = "RoleList", Name = "Чтение ролей"},
            new Function { Id = 12, Code = "Role", Name = "Чтение роли"},
            new Function { Id = 13, Code = "RoleEdit", Name = "Редактирование роли"},
            new Function { Id = 14, Code = "RoleDelete", Name = "Удаление роли"},
            new Function { Id = 15, Code = "RoleAdd", Name = "Добавление роли"}
        );

        modelBuilder.Entity<User>().HasData(
            Enumerable.Range(1, 5).Select(i => {
                return new User {
                    Id = i,
                    Name = $"User {i}",
                    Email = $"user{i}@example.com",
                    Login = $"Login{i}",
                    Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes($"Password{i}"))),
                    ChatId = -4681939671,
                    CreatedAt = DateTimeOffset.UtcNow,
                    LastRevision = DateTimeOffset.UtcNow
                };
            }).ToArray()
        );
        
        modelBuilder.Entity<User>().HasData(
            Enumerable.Range(1, 5).Select(i => {
                return new User {
                    Id = i + 5,
                    Name = $"User {i + 5}",
                    Email = $"user{i + 5}@example.com",
                    Login = $"Login{i + 5}",
                    Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes($"Password{i + 5}"))),
                    ChatId = -4701642993,
                    CreatedAt = DateTimeOffset.UtcNow,
                    LastRevision = DateTimeOffset.UtcNow
                };
            }).ToArray()
        );
        
        modelBuilder.Entity<User>().HasData(
            new User {
                Id = 11, 
                Name = "User 11",
                Email = $"user11@example.com",
                Login = "Login11",
                Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes("Password11"))),
                ChatId = -4681939671,
                CreatedAt = DateTimeOffset.UtcNow,
                LastRevision = DateTimeOffset.UtcNow
            }
        );
        
        modelBuilder.Entity<User>().HasData(
            new User {
                Id = 12, 
                Name = "User 12",
                Email = $"user12@example.com",
                Login = "Login12",
                Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes("Password12"))),
                ChatId = -4701642993,
                CreatedAt = DateTimeOffset.UtcNow,
                LastRevision = DateTimeOffset.UtcNow
            }
        );
        
        modelBuilder.Entity<User>().HasData(
            new User {
                Id = 13, 
                Name = "SUPER_ADMIN",
                Email = $"ADMIN@example.com",
                Login = "SUPER_ADMIN",
                Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes("SUPER_ADMIN"))),
                ChatId = -9999999999,
                CreatedAt = DateTimeOffset.UtcNow,
                LastRevision = DateTimeOffset.UtcNow
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
        
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole {
                UserId = 13,
                RoleId = 1
            }
        );
        
        modelBuilder.Entity<RoleFunction>().HasKey(rf => new { rf.RoleId, rf.FunctionId });
        modelBuilder.Entity<RoleFunction>().HasOne(rf => rf.Role).WithMany(r => r.RoleFunctions).HasForeignKey(rf => rf.RoleId);
        modelBuilder.Entity<RoleFunction>().HasOne(rf => rf.Function).WithMany(f => f.RoleFunctions).HasForeignKey(rf => rf.FunctionId);

        modelBuilder.Entity<RoleFunction>().HasData(
            Enumerable.Range(1, 15).Select(i => {
                return new RoleFunction {
                    RoleId = 1,
                    FunctionId = i
                };
            }).ToArray()
        );
        
        modelBuilder.Entity<RoleFunction>().HasData(
            Enumerable.Range(1, 12).Select(i => {
                return new RoleFunction {
                    RoleId = 3,
                    FunctionId = i
                };
            }).ToArray()
        );

        modelBuilder.Entity<RoleFunction>().HasData(
            Enumerable.Range(1, 6).Select(i => {
                return new RoleFunction {
                    RoleId = 2,
                    FunctionId = i + 1
                };
            }).ToArray()
        );
    }

}