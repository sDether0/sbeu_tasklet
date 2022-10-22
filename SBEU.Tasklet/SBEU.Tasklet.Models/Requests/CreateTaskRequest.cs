using Microsoft.AspNetCore.Identity;
using SBEU.Tasklet.Models.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.Models.Responses;
using System.ComponentModel.DataAnnotations;

namespace SBEU.Tasklet.Models.Requests
{
    public class CreateTaskRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public List<string>? Links { get; set; }
        [Required]
        public TaskDuration Duration { get; set; }
        public TaskProgress? Status { get; set; }
        [Required]
        public bool Hidden { get; set; }
        [Required]
        public string TableId { get; set; }
        [Required]
        public string ExecutorId { get; set; }
    }
}
