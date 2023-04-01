using SBEU.Tasklet.Models.Enums;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class HistoryDto
    {
        public ulong Id { get; set; }
        //public virtual TaskDto Task { get; set; }
        public DateTime UpdateDate { get; set; }
        public UserDto Updater { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskProgress? Status { get; set; }
        public UserDto? Executor { get; set; }
        
    }
}
