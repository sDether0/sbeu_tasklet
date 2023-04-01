using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.DataLayer.DataBase.Entities;

namespace SBEU.Tasklet.Api.Repositories.Interfaces
{
    public interface ITaskRepository : IRepository<XTask>
    {
        Task<XTask> UpdateAsync(XTask entity, XIdentityUser user);
        Task<XTask> CreateAsync(XTask entity, XIdentityUser user, XHistory history);
    }
}
