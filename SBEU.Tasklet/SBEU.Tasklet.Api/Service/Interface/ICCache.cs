using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.Models.Responses;

namespace SBEU.Tasklet.Api.Service.Interface
{
    public interface ICCache
    {
        Task<UserDto?> GetUser(string userId, ApiDbContext context);
    }
}
