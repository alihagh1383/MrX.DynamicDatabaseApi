﻿using Microsoft.AspNetCore.Authorization;

namespace MrX.DynamicDatabaseApi.Api.Handler;

public class RoleAuthorization
    : AuthorizationHandler<RuleAuthorizationRequirementInput>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        RuleAuthorizationRequirementInput requirement)
    {
        context.Succeed(requirement);
        // context.Fail(new AuthorizationFailureReason(this, "Dari Gooh mikhory"));
        return Task.CompletedTask;
    }
}

public class RuleAuthorizationRequirementInput : IAuthorizationRequirement
{
}