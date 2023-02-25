using Microsoft.AspNetCore.Identity;
using SBEU.Tasklet.Models.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.Models.Responses;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SBEU.Tasklet.Models.Requests
{
    public class CreateTaskRequest
    {
        [Required, NotNull]
        public required string Title { get; set; }
        [Required, NotNull]
        public required string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public List<string>? Links { get; set; }
        [Required, NotNull]
        public required uint Duration { get; set; }
        public TaskProgress? Status { get; set; }
        [Required, NotNull]
        public required bool Hidden { get; set; }
        [Required, NotNull]
        public required string TableId { get; set; }
        [Required, NotNull]
        public required string ExecutorId { get; set; }
        [Required, NotNull]
        public required uint Price { get; set; }
    }
}
