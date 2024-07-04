using System.Collections.Generic;
using System.Threading.Tasks;
using SORS.Data.Models;
using Microsoft.AspNetCore.Identity;
namespace SORS.Services{
    public interface IUsersService{
        Task<IEnumerable<IdentityUser>> GetUsersAsync();
        Task AddUsersAsync(IdentityUser report);
    }
}
