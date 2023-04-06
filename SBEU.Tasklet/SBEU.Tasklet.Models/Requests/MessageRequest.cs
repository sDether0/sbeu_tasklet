using Microsoft.AspNetCore.Identity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Requests
{
    public class MessageRequest
    {
        [Required,NotNull]
        public string Text { get; set; }
        [Required,NotNull, StringLength(36)]
        public string ChatId { get; set; }
        [StringLength(36)]
        public string? TableId { get; set; }
        [StringLength(36)]
        public string? TaskId { get; set; }
    }
}
