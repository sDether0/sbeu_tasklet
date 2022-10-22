using Microsoft.AspNetCore.Identity;

using SBEU.Tasklet.Models.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class TaskDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public TaskDuration Duration { get; set; }
        public TaskProgress Status { get; set; }
        public List<string> Links { get; set; }
        public bool Hidden { get; set; }
        public string Note { get; set; }
        public UserDto Author { get; set; }
        public bool IsAuthor { get; set; }
        public UserDto Executor { get; set; }
        public bool IsExecutor { get; set; }
    }
}
