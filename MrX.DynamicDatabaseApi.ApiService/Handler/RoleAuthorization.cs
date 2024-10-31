using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace MrX.DynamicDatabaseApi.Api.Handler;

public class RoleAuthorization
    : AuthorizationHandler<RuleAuthorizationRequirementInput>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RuleAuthorizationRequirementInput requirement)
    {
        // if (context.User.Identity.IsAuthenticated)
        var h = context.Resource as HttpContext;
        var r = JsonConvert.DeserializeObject<List<string>>(context.User.Claims?.Where(p => p.Type == ClaimTypes.Role).FirstOrDefault()?.Value ?? "");
        context.Succeed(requirement);
        var rus = global::Static.RouteRule.Where(p => h?.GetEndpoint()?.DisplayName?.StartsWith(p.Value.Key) ?? false);
        foreach (var ru in rus)
            if (!r?.Any(p => p == ru.Key) ?? true)
                context.Succeed(requirement);
            else
                context.Fail();
        // context.Fail(new AuthorizationFailureReason(this, "Dari Gooh mikhory"));
        return Task.CompletedTask;
    }
}

public class RuleAuthorizationRequirementInput : IAuthorizationRequirement
{
}