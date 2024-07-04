using SORS.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SORS.Data;
using SORS.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace SORS.Services
{
    public class UserService : IUsersService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IdentityUser>> GetUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task AddUsersAsync(IdentityUser user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
