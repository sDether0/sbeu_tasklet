using Microsoft.AspNetCore.Identity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class TableDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        //public virtual List<TaskDto> Tasks { get; set; }
        public virtual List<UserDto> Users { get; set; }
    }
}
