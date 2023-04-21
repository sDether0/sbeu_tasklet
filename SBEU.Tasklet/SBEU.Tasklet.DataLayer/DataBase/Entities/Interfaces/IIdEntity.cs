using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces
{
    public interface IIdEntity : IEntity
    {
        [Key]
        public string Id { get; set; }
    }
}
