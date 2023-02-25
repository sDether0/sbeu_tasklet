using Microsoft.AspNetCore.Identity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class MessageDto
    {
        [NotNull]
        public long Id { get; set; }
        [NotNull]
        public string Text { get; set; }
        [NotNull]
        public DateTime Time { get; set; }
        [NotNull]
        public virtual UserDto From { get; set; }
        [NotNull]
        public string ChatId { get; set; }
        public string? TableId { get; set; }
        public string? TaskId { get; set; }
        [NotNull]
        public bool Self { get; set; }
    }
}
