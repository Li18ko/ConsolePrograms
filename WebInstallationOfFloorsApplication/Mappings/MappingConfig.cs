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
        
        config.NewConfig<User, UserGetDto>()
            .Map(dest => dest.Roles, src => src.UserRoles.Select(ur => ur.Role.Name).ToList());
        
        config.NewConfig<UserInsertDto, User>()
            .Map(dest => dest.CreatedAt, src => DateTimeOffset.UtcNow)
            .Map(dest => dest.LastRevision, src => DateTimeOffset.UtcNow)
            .Map(dest => dest.Password, src => 
                Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(src.Password))))
            .Map(dest => dest.UserRoles, src => src.RoleIds.Select(roleId => new UserRole {
                RoleId = roleId,
                User = null
            }).ToList());
        
        config.NewConfig<User, UserUpdateDto>()
            .Map(dest => dest.RoleIds, src => src.UserRoles.Select(ur => ur.RoleId).ToList())
            .Map(dest => dest.Password, src => "******");

        config.NewConfig<UserUpdateDto, User>()
            .Map(dest => dest.LastRevision, src => DateTimeOffset.UtcNow)
            .Map(dest => dest.Password, src => 
                Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(src.Password))))
            .Map(dest => dest.UserRoles, src => src.RoleIds.Select(roleId => new UserRole {
                RoleId = roleId, 
                User = null
            }).ToList());
        
        config.NewConfig<Role, RoleGetDto>()
            .Map(dest => dest.Functions, src => src.RoleFunctions.Select(rf => new FunctionDto {
                Id = rf.Function.Id,
                Code = rf.Function.Code,
                Name = rf.Function.Name
            }).ToList());
        
        config.NewConfig<Function, FunctionDto>();
        
        config.NewConfig<RoleInsertDto, Role>()
            .Map(dest => dest.RoleFunctions, src => src.FunctionIds.Select(fid => new RoleFunction {
                FunctionId = fid
            }).ToList());
        
        config.NewConfig<RoleUpdateDto, Role>()
            .Map(dest => dest.RoleFunctions, src => src.FunctionIds.Select(fid => new RoleFunction {
                FunctionId = fid
            }).ToList());
        
        config.NewConfig<Role, RoleUpdateDto>()
            .Map(dest => dest.FunctionIds, src => src.RoleFunctions.Select(rf => rf.FunctionId).ToList());
    }
}