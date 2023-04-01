using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces;

namespace SBEU.Tasklet.Api.Repositories.Interfaces
{
    public interface IRepository<Entity> where Entity : IDeletable
    {
        Task<Entity> GetByIdAsync(string id);
        Task<IQueryable<Entity>> GetAllAsync();
        Task<Entity> CreateAsync(Entity entity);
        Task<Entity> UpdateAsync(Entity entity);
        Task DeleteAsync(string id);
    }
}
