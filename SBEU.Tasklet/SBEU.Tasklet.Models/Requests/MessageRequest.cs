﻿using Microsoft.AspNetCore.Identity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Requests
{
    public class MessageRequest
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string ChatId { get; set; }
        public string? TableId { get; set; }
        public string? TaskId { get; set; }
    }
}
