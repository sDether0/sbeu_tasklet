using SBEU.Tasklet.Models.Responses;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Requests
{
    public class UpdateTableRequest
    {
        [Required, NotNull]
        public string Id { get; set; }
        public string? Title { get; set; }
        public virtual List<UserDto>? Users { get; set; }
    }
}
