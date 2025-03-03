using System.Security.Cryptography;
using System.Text;
using Mapster;

namespace WebInstallationOfFloorsApplication;

public class MappingConfig: IRegister {
    
    public void Register(TypeAdapterConfig config) {
        config.NewConfig<TaskInsertDto, Task>()
            .Map(dest => dest.CreatedAt, src => DateTimeOffset.UtcNow)
            .Map(dest => dest.Status, src => TaskStatus.Open);
        
        config.NewConfig<TaskUpdateDto, Task>();
        config.NewConfig<Task, TaskUpdateDto>();
        
        config.NewConfig<User, UserWithRolesGetDto>()
            .Map(dest => dest.Roles, src => src.UserRoles.Select(ur => ur.RoleId).ToList());
        
        config.NewConfig<UserWithRolesInsertDto, User>()
            .Map(dest => dest.CreatedAt, src => DateTimeOffset.UtcNow)
            .Map(dest => dest.LastRevision, src => DateTimeOffset.UtcNow)
            .Map(dest => dest.Password, src => 
                Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(src.Password))))
            .Map(dest => dest.UserRoles, src => src.RoleIds.Select(roleId => new UserRole {
                RoleId = roleId,
                User = null
            }).ToList());
        
        config.NewConfig<User, UserWithRolesUpdateDto>()
            .Map(dest => dest.RoleIds, src => src.UserRoles.Select(ur => ur.RoleId).ToList())
            .Map(dest => dest.Password, src => "******");

        config.NewConfig<UserWithRolesUpdateDto, User>()
            .Map(dest => dest.LastRevision, src => DateTimeOffset.UtcNow)
            .Map(dest => dest.Password, src => 
                Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(src.Password))))
            .Map(dest => dest.UserRoles, src => src.RoleIds.Select(roleId => new UserRole {
                RoleId = roleId, 
                User = null
            }).ToList());
    }
}