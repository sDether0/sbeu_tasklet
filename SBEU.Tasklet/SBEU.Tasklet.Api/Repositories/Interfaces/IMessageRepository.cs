using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.DataLayer.DataBase.Entities;

namespace SBEU.Tasklet.Api.Repositories.Interfaces
{
    public interface IMessageRepository : IRepository<XMessage>
    {
        Task<XMessage> CreateAsync(XMessage mess, XIdentityUser user);
    }
}
