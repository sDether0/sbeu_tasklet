using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Requests
{
    public class AddUserToTableRequest
    {
        [Required, NotNull, StringLength(36)]
        public string UserId { get; set; }
        [Required, NotNull, StringLength(36)]
        public string TableId { get; set; }
    }
}
