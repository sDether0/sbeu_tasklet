using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class Chat : DeletableEntity
    {
        public string Id { get; set; }
        public string? Title { get; set; }
        public bool Private { get; set; }
        public virtual ICollection<XIdentityUser> Users { get; set; }
    }
}
