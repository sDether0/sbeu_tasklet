using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBEU.Tasklet.Models.Responses
{
    public class ChatDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool Private { get; set; }
        
    }

    public class FullChatDto : ChatDto
    {
        public virtual List<UserDto> Users { get; set; }
    }
}
