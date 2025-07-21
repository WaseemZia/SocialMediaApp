using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;

using Persistence;
namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }
    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsHostRequirementHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return;
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.GetRouteValue("id") is not string activityId) return;
            var attendee= await _dbContext.ActivityAttendees.
                //AsNoTracking().
                SingleOrDefaultAsync(x=>x.UserId == userId && x.ActivityId==activityId);
            if (attendee == null) return;
            if (attendee.IsHost) context.Succeed(requirement);



        }
    }
}
