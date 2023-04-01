using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class XTable : DeletableEntity
    {
        public XTable()
        {
            //Tasks ??= new HashSet<XTask>();
            //Users ??= new HashSet<XIdentityUser>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<XTask> Tasks { get; set; }
        public virtual ICollection<XIdentityUser> Users { get; set; }
    }
}
