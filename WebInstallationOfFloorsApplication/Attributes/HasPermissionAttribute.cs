using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace WebInstallationOfFloorsApplication;

public class HasPermissionAttribute: Attribute, IAsyncAuthorizationFilter {
    private readonly string[] _permissions;

    public HasPermissionAttribute(params string[] permissions){
        _permissions = permissions;
    }
    
    public async System.Threading.Tasks.Task OnAuthorizationAsync(AuthorizationFilterContext context) {
        var user = context.HttpContext.User;
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null) {
            context.Result = new ForbidResult();
            return;
        }
        
        if (!int.TryParse(userIdClaim.Value, out var userId)) {
            context.Result = new ForbidResult();
            return;
        }

        var accountService = context.HttpContext.RequestServices.GetService<AccountService>();

        if (accountService == null) {
            context.Result = new StatusCodeResult(500);
            return;
        }
        
        var userPermissions = await accountService.GetUserPermissionsAsync(userId, context.HttpContext.RequestAborted);
        Console.WriteLine("Role in attributs");
        foreach (var userP in userPermissions) {
            Console.WriteLine(userP.ToString());
        }

        if (!_permissions.Any(p => userPermissions.Contains(p))) {
            context.Result = new ForbidResult();
        }
    }
}