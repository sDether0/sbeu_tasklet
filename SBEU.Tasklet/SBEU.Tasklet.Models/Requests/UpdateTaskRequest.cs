using SBEU.Tasklet.Models.Enums;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Requests
{
    public class UpdateTaskRequest
    {
        [Required, NotNull]
        public string Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<string>? Links { get; set; }
        public TaskDuration? Duration { get; set; }
        public TaskProgress? Status { get; set; }
        public string? ExecutorId { get; set; }
    }
}
