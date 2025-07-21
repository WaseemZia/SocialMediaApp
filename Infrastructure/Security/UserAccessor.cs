
using System.Security.Claims;
using Application.interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure
{
    public class UserAccessor : IUserAccessor
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) {
        _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<User> GetUserAsync()
        {
            return await _dbContext.Users.FindAsync(GetUserId())
                ?? throw new UnauthorizedAccessException("No User is Logged in");
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new Exception("No User Found ");
        }

        public async Task<User> GetUserWithPhotosAsync()
        {
            var userId = GetUserId();
            return await _dbContext.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x=>x.Id==userId)
              ?? throw new UnauthorizedAccessException("No Photo Found");
        }
    }
}
