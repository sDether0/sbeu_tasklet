using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces
{
    public interface IIdEntity : IEntity
    {
        public string Id { get; set; }
    }
}
