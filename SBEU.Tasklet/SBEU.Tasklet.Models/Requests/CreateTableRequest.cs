using Microsoft.AspNetCore.Identity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.Models.Responses;
using System.ComponentModel.DataAnnotations;

namespace SBEU.Tasklet.Models.Requests
{
    public class CreateTableRequest
    {
        [Required]
        public string Title { get; set; }
        public virtual List<UserDto>? Users { get; set; }
    }
}
