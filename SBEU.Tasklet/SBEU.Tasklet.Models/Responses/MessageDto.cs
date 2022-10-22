using Microsoft.AspNetCore.Identity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class MessageDto
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public virtual UserDto From { get; set; }
        public string ChatId { get; set; }
        public string? TableId { get; set; }
        public string? TaskId { get; set; }
    }
}
