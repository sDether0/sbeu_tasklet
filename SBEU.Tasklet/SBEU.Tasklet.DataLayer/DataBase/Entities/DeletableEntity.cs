using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class DeletableEntity : XEntity, IDeletable
    {
        public bool IsDeleted { get; set; }
    }
}
