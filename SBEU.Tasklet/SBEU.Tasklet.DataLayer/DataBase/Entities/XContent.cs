using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.Models.Enums;

namespace SBEU.Tasklet.DataLayer.DataBase.Entities
{
    public class XContent : DeletableEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public XContentType ContentType { get; set; }

    }
}
