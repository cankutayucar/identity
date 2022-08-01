using Microsoft.AspNetCore.Authorization;

namespace CankutayUcarIdentity.UI.Requiretments
{
    public class ExpireDateExchangeRequiretment : IAuthorizationRequirement
    {
    }

    public class ExpireDateExchangeHandler : AuthorizationHandler<ExpireDateExchangeRequiretment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExpireDateExchangeRequiretment requirement)
        {
            if (context.User != null & context.User.Identity != null)
            {
                var claim = context.User.Claims
                    .FirstOrDefault(c => c.Type == "ExpireDateExchange" && c.Value != null);
                if (claim != null)
                {
                    if (DateTime.Now < Convert.ToDateTime(claim.Value))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
