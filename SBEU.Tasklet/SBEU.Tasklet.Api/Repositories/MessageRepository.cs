using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Api.Repositories.Interfaces;

namespace SBEU.Tasklet.Api.Repositories
{
    public class MessageRepository : BaseRepository<XMessage>, IMessageRepository
    {
        public MessageRepository(ApiDbContext context) : base(context)
        {
        }
    }
}
