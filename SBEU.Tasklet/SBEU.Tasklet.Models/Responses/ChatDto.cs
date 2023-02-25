using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class ChatDto
    {
        [NotNull]
        public string Id { get; set; }
        public string Title { get; set; }
        [NotNull]
        public bool Private { get; set; }       
        public string LastMessage { get; set; }
        
    }

    public class FullChatDto : ChatDto
    {
        public virtual List<UserDto> Users { get; set; }
    }
}
