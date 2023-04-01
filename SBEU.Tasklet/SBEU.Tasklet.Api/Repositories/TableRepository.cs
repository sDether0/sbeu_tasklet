using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.Api.Repositories.Interfaces;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;

namespace SBEU.Tasklet.Api.Repositories
{
    public class TableRepository : BaseRepository<XTable>, ITableRepository
    {
        public TableRepository(ApiDbContext context) : base(context)
        {
        }
    }
}
